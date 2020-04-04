using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Svn2GitNet
{
    public class CommandRunner : ICommandRunner
    {
        private readonly ILogger _logger;
        private readonly bool _isVerbose;

        private readonly CancellationToken _cancelToken;

        public CommandRunner( ILogger logger, bool isVerbose, CancellationToken cancelToken )
        {
            _logger = logger;
            _isVerbose = isVerbose;
            _cancelToken = cancelToken;
        }

        public int Run( string cmd, string arguments )
        {
            return Run( cmd, arguments, null, null, null );
        }

        public int Run( string cmd, string arguments, out string standardOutput )
        {
            string standardError;

            return Run( cmd, arguments, out standardOutput, out standardError, null );
        }

        public int Run( string cmd, string arguments, out string standardOutput, out string standardError )
        {
            return Run( cmd, arguments, out standardOutput, out standardError, null );
        }

        public int Run( string cmd, string arguments, out string standardOutput, out string standardError, string workingDirectory )
        {
            StringBuilder stdout = new StringBuilder();
            StringBuilder stderr = new StringBuilder();

            void onStdOut( string s )
            {
                stdout.Append( s );
            }

            void onStdErr( string s )
            {
                stderr.Append( s );
            }

            int exitCode = Run( cmd, arguments, onStdOut, onStdErr, workingDirectory );
            standardOutput = stdout.ToString();
            standardError = stderr.ToString();

            return exitCode;
        }

        public int Run( string cmd, string arguments, Action<string> onStandardOutput, Action<string> onStandardError, string workingDirectory )
        {
            Log( $"Running command: {cmd} {arguments.ToString()}" );

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = cmd,
                Arguments = arguments
            };

            if( !string.IsNullOrWhiteSpace( workingDirectory ) )
            {
                startInfo.WorkingDirectory = workingDirectory;
            }

            using( ManualResetEventSlim exitedEvent = new ManualResetEventSlim( false ) )
            {
                using( Process commandProcess = new Process() )
                {
                    commandProcess.StartInfo = startInfo;

                    commandProcess.OutputDataReceived += ( s, e ) =>
                    {
                        if( string.IsNullOrEmpty( e.Data ) )
                        {
                            return;
                        }

                        Console.WriteLine( e.Data );
                        onStandardOutput?.Invoke( e.Data );
                    };

                    commandProcess.ErrorDataReceived += ( s, e ) =>
                    {
                        if( string.IsNullOrEmpty( e.Data ) )
                        {
                            return;
                        }

                        Console.Error.WriteLine( e.Data );
                        onStandardError?.Invoke( e.Data );
                    };

                    commandProcess.EnableRaisingEvents = true;
                    commandProcess.Exited += ( s, e ) =>
                    {
                        Log( $"Process '{startInfo.FileName} {startInfo.Arguments}' exited" );
                        exitedEvent.Set();
                    };

                    int exitCode = -1;
                    try
                    {
                        commandProcess.Start();
                        commandProcess.BeginOutputReadLine();
                        commandProcess.BeginErrorReadLine();

                        exitedEvent.Wait( this._cancelToken );

                        commandProcess.WaitForExit();
                    }
                    catch( Win32Exception )
                    {
                        throw new MigrateException( $"Command {cmd} does not exit. Did you install it or add it to the Environment path?" );
                    }
                    catch( OperationCanceledException )
                    {
                        Log( "CTRL+C Received" );
                        commandProcess.Kill( true );
                        commandProcess.WaitForExit();
                        throw;
                    }
                    finally
                    {
                        exitCode = commandProcess.ExitCode;
                        commandProcess.Close();
                        commandProcess.Dispose();
                    }

                    return exitCode;
                }
            }
        }

        public int RunGitSvnInteractiveCommand( string arguments, string password )
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                FileName = "git",
                Arguments = arguments
            };

            using( ManualResetEventSlim exitedEvent = new ManualResetEventSlim( false ) )
            {
                using( Process commandProcess = new Process() )
                {
                    int exitCode = -1;
                    try
                    {
                        commandProcess.StartInfo = startInfo;

                        commandProcess.EnableRaisingEvents = true;
                        commandProcess.Exited += ( s, e ) =>
                        {
                            Log( $"Process '{startInfo.FileName} {startInfo.Arguments}' exited" );
                            exitedEvent.Set();
                        };

                        commandProcess.Start();

                        OutputMessageType messageType = OutputMessageType.None;
                        do
                        {
                            // Stop if we want to cancel.
                            this._cancelToken.ThrowIfCancellationRequested();

                            messageType = ReadAndDisplayCommandProcessOutput( commandProcess );
                            if( messageType == OutputMessageType.RequestInputPassword )
                            {
                                if( string.IsNullOrEmpty( password ) )
                                {
                                    while( true )
                                    {
                                        var key = System.Console.ReadKey( true );
                                        if( key.Key == ConsoleKey.Enter )
                                        {
                                            break;
                                        }

                                        password += key.KeyChar;
                                    }
                                }

                                commandProcess.StandardInput.WriteLine( password );
                            }
                            else if( messageType == OutputMessageType.RequestAcceptCertificateFullOptions )
                            {
                                Console.WriteLine( "p" );
                                commandProcess.StandardInput.WriteLine( "p" );
                            }
                            else if( messageType == OutputMessageType.RequestAcceptCertificateNoPermanentOption )
                            {
                                Console.WriteLine( "t" );
                                commandProcess.StandardInput.WriteLine( "t" );
                            }

                            commandProcess.StandardInput.Flush();
                        } while( messageType != OutputMessageType.None );

                        exitedEvent.Wait( this._cancelToken );

                        commandProcess.WaitForExit();
                    }
                    catch( Win32Exception )
                    {
                        throw new MigrateException( $"Command git does not exit. Did you install it or add it to the Environment path?" );
                    }
                    catch( OperationCanceledException )
                    {
                        Log( "CTRL+C Received" );

                        commandProcess.Kill( true );
                        commandProcess.WaitForExit();
                        throw;
                    }
                    finally
                    {
                        exitCode = commandProcess.ExitCode;
                        commandProcess.Close();
                    }

                    return exitCode;
                }
            }
        }

        private OutputMessageType ReadAndDisplayCommandProcessOutput( Process commandProcess )
        {
            int lastChr = 0;

            string output = "";
            OutputMessageType messageType = OutputMessageType.None;

            while( ( messageType == OutputMessageType.None || commandProcess.StandardError.Peek() != -1 )
                    && ( lastChr = commandProcess.StandardError.Read() ) > 0 )
            {
                string outputChr = null;
                outputChr += commandProcess.StandardError.CurrentEncoding.GetString( new byte[] { (byte)lastChr } );
                output += outputChr;

                if( messageType == OutputMessageType.None )
                {
                    if( output.Contains( "Password for" ) )
                    {
                        messageType = OutputMessageType.RequestInputPassword;
                    }
                    else if( output.Contains( "(R)eject, accept (t)emporarily or accept (p)ermanently?" ) )
                    {
                        messageType = OutputMessageType.RequestAcceptCertificateFullOptions;
                    }
                    else if( output.Contains( "(R)eject or accept (t)emporarily?" ) )
                    {
                        messageType = OutputMessageType.RequestAcceptCertificateNoPermanentOption;
                    }
                }

                Console.Write( outputChr );
            }

            return messageType;
        }

        private void Log( string message )
        {
            if( _logger != null && _isVerbose )
            {
                _logger.LogInformation( message );
            }
        }
    }
}