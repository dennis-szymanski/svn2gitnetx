using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Svn2GitNet
{
    public class CommandRunner : ICommandRunner
    {
        private ILogger _logger;
        private bool _isVerbose;

        public CommandRunner(ILogger logger, bool isVerbose)
        {
            _logger = logger;
            _isVerbose = isVerbose;
        }

        public int Run(string cmd, string arguments)
        {
            return Run(cmd, arguments, null, null, null);
        }

        public int Run(string cmd, string arguments, out string standardOutput)
        {
            string standardError;

            return Run(cmd, arguments, out standardOutput, out standardError, null);
        }

        public int Run(string cmd, string arguments, out string standardOutput, out string standardError)
        {
            return Run(cmd, arguments, out standardOutput, out standardError, null);
        }

        public int Run(string cmd, string arguments, out string standardOutput, out string standardError, string workingDirectory)
        {
            StringBuilder stdout = new StringBuilder();
            StringBuilder stderr = new StringBuilder();

            Action<string> onStdOut = delegate( string s )
            {
                stdout.AppendLine( s );
            };

            Action<string> onStdErr = delegate( string s )
            {
                stderr.AppendLine( s );
            };

            int exitCode = Run( cmd, arguments, onStdOut, onStdErr, workingDirectory);
            standardOutput = stdout.ToString();
            standardError = stderr.ToString();

            return exitCode;
        }

        public int Run(string cmd, string arguments, Action<string> onStandardOutput, Action<string> onStandardError, string workingDirectory)
        {
            Log($"Running command: {cmd} {arguments.ToString()}");
            Process commandProcess = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = cmd,
                    Arguments = arguments
                }
            };

            if (!string.IsNullOrWhiteSpace(workingDirectory))
            {
                commandProcess.StartInfo.WorkingDirectory = workingDirectory;
            }

            commandProcess.OutputDataReceived += (s, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                {
                    return;
                }

                Console.WriteLine(e.Data);
                onStandardOutput?.Invoke(e.Data);
            };

            commandProcess.ErrorDataReceived += (s, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                {
                    return;
                }

                Console.Error.WriteLine(e.Data);
                onStandardError?.Invoke(e.Data);
            };

            int exitCode = -1;
            try
            {
                commandProcess.Start();
                commandProcess.BeginOutputReadLine();
                commandProcess.BeginErrorReadLine();
                commandProcess.WaitForExit();
            }
            catch (Win32Exception)
            {
                throw new MigrateException($"Command {cmd} does not exit. Did you install it or add it to the Environment path?");
            }
            finally
            {
                exitCode = commandProcess.ExitCode;
                commandProcess.Close();
            }

            return exitCode;
        }

        public int RunGitSvnInteractiveCommand(string arguments, string password)
        {
            Process commandProcess = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    FileName = "git",
                    Arguments = arguments
                }
            };

            int exitCode = -1;
            try
            {
                commandProcess.Start();

                OutputMessageType messageType = OutputMessageType.None;
                do
                {
                    messageType = ReadAndDisplayCommandProcessOutput(commandProcess);
                    if (messageType == OutputMessageType.RequestInputPassword)
                    {
                        if (string.IsNullOrEmpty(password))
                        {
                            while (true)
                            {
                                var key = System.Console.ReadKey(true);
                                if (key.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }

                                password += key.KeyChar;
                            }
                        }

                        commandProcess.StandardInput.WriteLine(password);
                    }
                    else if (messageType == OutputMessageType.RequestAcceptCertificateFullOptions)
                    {
                        Console.WriteLine("p");
                        commandProcess.StandardInput.WriteLine("p");
                    }
                    else if (messageType == OutputMessageType.RequestAcceptCertificateNoPermanentOption)
                    {
                        Console.WriteLine("t");
                        commandProcess.StandardInput.WriteLine("t");
                    }

                    commandProcess.StandardInput.Flush();
                } while (messageType != OutputMessageType.None);

                commandProcess.WaitForExit();
            }
            catch (Win32Exception)
            {
                throw new MigrateException($"Command git does not exit. Did you install it or add it to the Environment path?");
            }
            finally
            {
                exitCode = commandProcess.ExitCode;
                commandProcess.Close();
            }

            return exitCode;
        }

        private OutputMessageType ReadAndDisplayCommandProcessOutput(Process commandProcess)
        {
            int lastChr = 0;

            string output = "";
            OutputMessageType messageType = OutputMessageType.None;

            while ((messageType == OutputMessageType.None || commandProcess.StandardError.Peek() != -1)
                    && (lastChr = commandProcess.StandardError.Read()) > 0)
            {
                string outputChr = null;
                outputChr += commandProcess.StandardError.CurrentEncoding.GetString(new byte[] { (byte)lastChr });
                output += outputChr;

                if (messageType == OutputMessageType.None)
                {
                    if (output.Contains("Password for"))
                    {
                        messageType = OutputMessageType.RequestInputPassword;
                    }
                    else if (output.Contains("(R)eject, accept (t)emporarily or accept (p)ermanently?"))
                    {
                        messageType = OutputMessageType.RequestAcceptCertificateFullOptions;
                    }
                    else if (output.Contains("(R)eject or accept (t)emporarily?"))
                    {
                        messageType = OutputMessageType.RequestAcceptCertificateNoPermanentOption;
                    }
                }

                Console.Write(outputChr);
            }

            return messageType;
        }

        private void Log(string message)
        {
            if (_logger != null && _isVerbose)
            {
                _logger.LogInformation(message);
            }
        }
    }
}