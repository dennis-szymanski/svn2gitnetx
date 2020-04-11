using System.IO;
using Microsoft.Extensions.Logging;

namespace Svn2GitNetX
{
    // ---------------- Constructor ----------------

    public class Worker
    {
        public Worker( 
            Options options,
            ICommandRunner commandRunner,
            IMessageDisplayer messageDisplayer,
            ILogger logger
        )
        {
            this.Options = options;
            this.CommandRunner = commandRunner;
            this.MessageDisplayer = messageDisplayer;
            this.Logger = logger;
        }

        // ---------------- Properties ----------------

        protected Options Options { get; set; }

        protected ICommandRunner CommandRunner { get; set; }

        protected IMessageDisplayer MessageDisplayer { get; set; }

        protected ILogger Logger { get; set; }

        protected string WorkingDirectory => "."; // Todo: Maybe an option?

        protected string GitDirectory => Path.Combine( WorkingDirectory, ".git" );

        // ---------------- Functions ----------------

        protected void ShowMessageIfPossible( string message )
        {
            MessageDisplayer?.Show( message );
        }

        protected void Log( string message )
        {   
            if( Options.IsVerbose )
            {
                Logger?.LogInformation( message );
            }
        }

        protected void LogWarning( string message )
        {
            if( Options.IsVerbose )
            {
                Logger?.LogWarning( message );
            }
        }

        protected void LogError( string message )
        {
            if( Options.IsVerbose )
            {
                Logger?.LogError( message );
            }
        }

        protected string RunCommandIgnoreExitCode( CommandInfo cmdInfo )
        {
            return RunCommandIgnoreExitCode( cmdInfo.Command, cmdInfo.Arguments );
        }

        protected string RunCommandIgnoreExitCode( string cmd, string arguments )
        {
            string standardOutput;
            CommandRunner.Run( cmd, arguments, out standardOutput );

            return standardOutput;
        }

        protected int RunCommand( CommandInfo cmdInfo )
        {
            return CommandRunner.Run( cmdInfo.Command, cmdInfo.Arguments );
        }

        protected int RunCommand( CommandInfo cmdInfo, out string standardOutput )
        {
            return CommandRunner.Run( cmdInfo.Command, cmdInfo.Arguments, out standardOutput );
        }

        protected int RunCommand( CommandInfo cmdInfo, out string standardOutput, out string standardError )
        {
            return CommandRunner.Run( cmdInfo.Command, cmdInfo.Arguments, out standardOutput, out standardError );
        }
    }
}
