using System;

namespace Svn2GitNetX
{
    public interface ICommandRunner
    {
        int Run( string cmd, string arguments );

        int Run( string cmd, string arguments, out string standardOutput );

        int Run( string cmd, string arguments, out string standardOutput, out string standardError );

        int Run( string cmd, string arguments, out string standardOutput, out string standardError, string workingDirectory );

        int RunGitSvnInteractiveCommand( string arguments, string password );
    }
}