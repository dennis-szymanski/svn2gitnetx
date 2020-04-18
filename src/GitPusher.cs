using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Svn2GitNetX
{
    public class GitPusher : Worker, IGitPusher
    {
        // ---------------- Constructor ----------------

        // ---------------- Properties ----------------

        public GitPusher(
            Options options,
            ICommandRunner cmdRunner,
            IMessageDisplayer msgDisplayer,
            ILogger logger
        ) :
            base( options, cmdRunner, msgDisplayer, logger )
        {
        }

        // ---------------- Functions ----------------

        public void PushAll()
        {
            StringBuilder args = new StringBuilder();

            args.Append( "push --all" );
            if( string.IsNullOrWhiteSpace( this.Options.RemoteGitUrl ) == false )
            {
                args.Append( $" \"{this.Options.RemoteGitUrl}\"" );
            }

            int exitCode = CommandRunner.Run( "git", args.ToString() );
            if( exitCode != 0 )
            {
                throw new ApplicationException( "Unable to push to git repo" );
            }
        }

        public void PushPrune()
        {
            StringBuilder args = new StringBuilder();

            args.Append( "push --prune" );
            if( string.IsNullOrWhiteSpace( this.Options.RemoteGitUrl ) == false )
            {
                args.Append( $" \"{this.Options.RemoteGitUrl}\"" );
            }

            int exitCode = CommandRunner.Run( "git", args.ToString() );
            if( exitCode != 0 )
            {
                throw new ApplicationException( "Unable to push to git repo.  Does your version of git support 'git push --prune'?" );
            }
        }
    }
}
