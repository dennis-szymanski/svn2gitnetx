using System;
using System.Threading;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace Svn2GitNet
{
    class Program
    {
        static void Migrate(Options options, string[] args)
        {
            using( CancellationTokenSource cancelToken = new CancellationTokenSource() )
            {
                ConsoleCancelEventHandler onCtrlC = delegate( object sender, ConsoleCancelEventArgs cancelArgs )
                {
                    // Wait for the process to end gracefully if we get CTRL+C,
                    // otherwise, let it die without clean up if we get CTRL+Break.
                    if( cancelArgs.SpecialKey == ConsoleSpecialKey.ControlC )
                    {
                        cancelArgs.Cancel = true;
                        Console.WriteLine( "CTRL+C was received, cleaning up..." );
                        cancelToken.Cancel();
                    }
                };

                try
                {
                    Console.CancelKeyPress += onCtrlC;

                    ILoggerFactory loggerFactory = LoggerFactory.Create(
                        builder =>
                        {
                            builder.AddConsole();
                        }
                    );

                    ICommandRunner commandRunner = new CommandRunner(
                        loggerFactory.CreateLogger<CommandRunner>(),
                        options.IsVerbose,
                        cancelToken.Token
                    );
                    IMessageDisplayer messageDisplayer = new ConsoleMessageDisplayer();

                    Migrator migrator = new Migrator(
                        options,
                        args,
                        commandRunner,
                        messageDisplayer,
                        loggerFactory
                    );
                    migrator.Initialize();
                    migrator.Run();
                }
                catch(OperationCanceledException)
                {
                    Console.WriteLine( "CTRL+C was received, child processes have been killed." );
                }
                finally
                {
                    Console.CancelKeyPress -= onCtrlC;
                }
            }
        }

        static int Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                              .WithParsed(options => Migrate(options, args));
            }
            catch (MigrateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Type 'svn2gitnet --help' for more information");

                return -1;
            }
            catch(Exception ex)
            {
                Console.WriteLine("FATAL: Unhandled Exception: " + ex.Message);
                return -2;
            }

            return 0;
        }
    }
}
