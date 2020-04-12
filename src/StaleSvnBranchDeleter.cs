using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Svn2GitNetX
{
    /// <summary>
    /// If the option is enabled, after the fixing stage,
    /// this class will query SVN at the head revision at the branches
    /// directories and delete the local repo's branchs that are not in SVN.
    /// 
    /// This becomes a no-op if --no-branches is enabled, or if --delete-stale-svn-branches is not specified.
    /// </summary>
    public class StaleSvnBranchDeleter : Worker, IStaleSvnBranchDeleter
    {
        // ---------------- Fields ----------------

        private readonly string svnUrl;

        private readonly MetaInfo metaInfo;

        private readonly IFileSystem fileSystem;

        // ---------------- Constructor ----------------

        public StaleSvnBranchDeleter(
            string svnUrl,
            Options options,
            ICommandRunner cmdRunner,
            IMessageDisplayer msgDisplayer,
            ILogger logger,
            MetaInfo metaInfo,
            IFileSystem fileSystem
        ) : base( options, cmdRunner, msgDisplayer, logger )
        {
            this.svnUrl = svnUrl;
            this.metaInfo = metaInfo;
            this.fileSystem = fileSystem;
        }

        // ---------------- Functions ----------------

        /// <summary>
        /// Calls out to SVN and fetches the branches at the HEAD revision.
        /// </summary>
        public IEnumerable<string> QueryHeadSvnBranches()
        {
            List<string> branches = new List<string>();

            if(
                this.Options.NoBranches ||
                ( this.Options.Branches == null ) ||
                ( this.Options.Branches.Any() == false )
            ) 
            {
                Log( "No Branches specified, skipping checking SVN branches" );
                return branches;
            }

            EnsureSvnInstalled();

            foreach( string branchDir in this.Options.Branches )
            {
                QuerySvnBranches( branchDir, branches );
            }

            if( this.Options.IsVerbose )
            {
                Log( $"Found {branches.Count} SVN branches:" );
                foreach( string branch in branches )
                {
                    Log( "- " + branch );
                }
            }

            return branches;
        }

        public IEnumerable<string> GetGitBranchesToPurge( IEnumerable<string> headSvnBranches )
        {
            List<string> existingSvnBranches = new List<string>( headSvnBranches.Count() );

            // First, GIT does not like whitespace, SVN is okay with it.  Also,
            // GIT does not like a '.' in front, SVN is okay with it.
            // GIT replaces the first '.' with a "%2E" and each space with a "%20".
            //
            // So, we will modify all existing SVN branches to do the same so they stay in sync with git.
            foreach( string branch in new List<string>( headSvnBranches ) )
            {
                string newBranchName = branch.Replace( " ", "%20" );
                if( newBranchName.StartsWith( '.' ) )
                {
                    newBranchName = "%2E" + newBranchName.Substring( 1 );
                }
                existingSvnBranches.Add( newBranchName );
            }

            List<string> localBranchesToPurge = new List<string>();
            foreach( string localBranch in this.metaInfo.LocalBranches )
            {
                // Ignore our master branch so we don't napalm that.
                if( localBranch == "master" )
                {
                    continue;
                }
                // If the existing SVN branches do not contain our local branch,
                // the SVN branch got deleted.  Purge!
                else if( existingSvnBranches.Contains( localBranch ) == false )
                {
                    localBranchesToPurge.Add( localBranch );
                }
            }

            return localBranchesToPurge;
        }

        public IEnumerable<string> PurgeGitBranches( IEnumerable<string> branchesToPurge )
        {
            List<string> deletedBranches = new List<string>();

            foreach( string branch in branchesToPurge )
            {
                // Use -D to force the delete.  We want to delete branches whether or not
                // they were merged or not.
                int exitCode = CommandRunner.Run( "git", $"branch -D -r \"{branch}\"" );

                if( exitCode == 0 )
                {
                    string dirToDelete = Path.Combine( this.GitDirectory, "svn", "refs", "remotes", branch );
                    try
                    {
                        this.fileSystem.DeleteDirectoryIfItExists( dirToDelete );

                        // TODO: This is just ****guess*** at what would happen,
                        // but if git branch -D is run, but the svn ref
                        // is still there, the branch is still going to be pulled down the next rebase.
                        // So, only say the branch was deleted if both removed from GIT and SVN.
                        //
                        // If this is NOT what happens, we should consider a different approach.
                        deletedBranches.Add( branch );
                    }
                    catch( Exception e )
                    {
                        this.MessageDisplayer?.Show( "Unable to delete '" + dirToDelete + "'" );
                        LogWarning( e.Message );
                    }
                }
                else
                {
                    this.MessageDisplayer?.Show( $"Unable to delete branch '{branch}', got exit code: {exitCode}" );
                }
            }

            return deletedBranches;
        }

        private void EnsureSvnInstalled()
        {
            int exitCode = this.CommandRunner.Run( "svn", "--version" );
            if( exitCode != 0 )
            {
                throw new PlatformNotSupportedException( "Platform does not have SVN installed.  This action is not supported" );
            }
        }

        private void QuerySvnBranches( string branchPath, IList<string> branches )
        {
            void AddBranch( string branchName )
            {
                // '/' is a folder.  Otherwise it is a file.
                if( branchName.EndsWith( '/' ) )
                {
                    branches.Add( branchName.TrimEnd( '/' ) );
                }
            }

            StringBuilder arguments = new StringBuilder();
            arguments.Append( $"ls {svnUrl}/{branchPath}" );

            string userName = this.Options.GetUserName();
            if( string.IsNullOrWhiteSpace( userName ) == false )
            {
                arguments.Append( $" --username={userName}");
            }

            int exitCode = this.CommandRunner.Run( "svn", arguments.ToString(), AddBranch, null, null );
            if( exitCode != 0 )
            {
                throw new ApplicationException( "Could not query SVN branch at " + branchPath );
            }
        }
    }
}
