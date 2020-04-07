using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Svn2GitNetX
{
    public class Grabber : InteractiveWorker, IGrabber
    {
        private string _svnUrl = string.Empty;
        private MetaInfo _metaInfo = null;

        private static readonly Regex revisionRegex = new Regex(
            @"r(?<rev>\d+)\s+=\s+\S+\s+\(.+\)",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        public Grabber(
            string svnUrl,
            Options options,
            ICommandRunner commandRunner,
            string gitConfigCommandArguments,
            IMessageDisplayer messageDisplayer,
            ILogger logger
        ) :
            base( options, commandRunner, gitConfigCommandArguments, messageDisplayer, logger )
        {
            _svnUrl = svnUrl;
            _metaInfo = new MetaInfo()
            {
                RemoteBranches = new List<string>(),
                LocalBranches = new List<string>(),
                Tags = new List<string>()
            };
        }

        public void Clone()
        {
            Log( "Start cloning..." );
            
            var branches = Options.Branches == null ? new List<string>() : new List<string>( Options.Branches );
            var tags = Options.Tags == null ? new List<string>() : new List<string>( Options.Tags );

            Log( "Git Init" );
            DoGitInit( branches, tags );

            Log( "Git Fetch" );
            DoFetch( branches, tags );

            Log( "Fetch Branches" );
            FetchBranches();

            Log( "End clone." );
        }

        private void DoGitInit( IList<string> branches, IList<string> tags )
        {
            StringBuilder arguments = new StringBuilder( "svn init --prefix=svn/ " );

            if( string.IsNullOrWhiteSpace( Options.GetUserName() ) == false )
            {
                arguments.AppendFormat( "--username=\"{0}\" ", Options.GetUserName() );
            }

            if( Options.IncludeMetaData == false )
            {
                arguments.Append( "--no-metadata " );
            }

            if( Options.NoMinimizeUrl )
            {
                arguments.Append( "--no-minimize-url " );
            }

            if( Options.RootIsTrunk )
            {
                // Non-standard repository layout.
                // The repository root is effectively trunk.
                //
                // Note: There's a bug of git svn init so that we cannot assign the svn url to --trunk.
                // We assign "/" to --trunk to tell git svn init we're using root as trunk.
                arguments.Append( "--trunk=\"/\" " );
            }
            else
            {
                // Add each component to the command that was passed as an argument.
                if( string.IsNullOrWhiteSpace( Options.SubpathToTrunk ) == false )
                {
                    arguments.AppendFormat( "--trunk=\"{0}\" ", Options.SubpathToTrunk );
                }

                if( Options.NoTags == false )
                {
                    if( tags.Count == 0 )
                    {
                        // Fill default tags here so that they can be filtered later
                        tags.Add( "tags" );
                    }

                    // Process default or user-supplied tags
                    foreach( var t in tags )
                    {
                        arguments.AppendFormat( "--tags=\"{0}\" ", t );
                    }
                }

                if( Options.NoBranches == false )
                {
                    if( branches.Count == 0 )
                    {
                        // Fill default branches here so that they can be filtered later
                        branches.Add( "branches" );
                    }

                    // Process default or user-supplied branches
                    foreach( var b in branches )
                    {
                        arguments.AppendFormat( "--branches=\"{0}\" ", b );
                    }
                }
            }

            arguments.Append( _svnUrl );

            if( CommandRunner.RunGitSvnInteractiveCommand( arguments.ToString(), Options.GetPassword() ) != 0 )
            {
                string exceptionMessage = string.Format( ExceptionHelper.ExceptionMessage.FAIL_TO_EXECUTE_COMMAND, $"git {arguments.ToString()}" );
                throw new MigrateException( exceptionMessage );
            }

            // Setup author file after the init.
            if( string.IsNullOrWhiteSpace( Options.Authors ) == false )
            {
                string args = string.Format( "{0} svn.authorsfile {1}",
                            GitConfigCommandArguments, Options.Authors );
                CommandRunner.Run( "git", args );
            }
        }

        private void DoFetch( IList<string> branches, IList<string> tags )
        {
            StringBuilder arguments = new StringBuilder( "svn fetch " );
            if( string.IsNullOrWhiteSpace( Options.Revision ) == false )
            {
                var range = Options.Revision.Split( ":" );
                string start = range[0];
                string end = range.Length < 2 || string.IsNullOrWhiteSpace( range[1] ) ? "HEAD" : range[1];
                arguments.AppendFormat( "-r {0}:{1} ", start, end );
            }

            if( ( Options.Exclude != null ) && Options.Exclude.Any() )
            {
                // Add exclude paths to the command line. Some versions of git support
                // this for fetch only, later also for init.
                List<string> regex = new List<string>();
                if( Options.RootIsTrunk == false )
                {
                    if( string.IsNullOrWhiteSpace( Options.SubpathToTrunk ) == false )
                    {
                        regex.Add( Options.SubpathToTrunk + @"[\/]" );
                    }

                    if( ( Options.NoTags == false ) && tags.Count > 0 )
                    {
                        foreach( var t in tags )
                        {
                            regex.Add( t + @"[\/][^\/]+[\/]" );
                        }
                    }

                    if( ( Options.NoBranches == false ) && branches.Count > 0 )
                    {
                        foreach( var b in branches )
                        {
                            regex.Add( b + @"[\/][^\/]+[\/]" );
                        }
                    }
                }

                string regexStr = "^(?:" + string.Join( "|", regex ) + ")(?:" + string.Join( "|", Options.Exclude ) + ")";
                arguments.AppendFormat( "--ignore-paths=\"{0}\" ", regexStr );
            }

            //---- Fetch ----

            int lastRevision = -1;
            int currentRevision = lastRevision;
            void ParseRevision( string s )
            {
                Match match = revisionRegex.Match( s );
                if( match.Success )
                {
                    int.TryParse( match.Groups["rev"].Value, out currentRevision );
                }
            }

            bool success = false;
            int currentAttempt = 0;
            do
            {
                int exitCode = CommandRunner.Run( "git", arguments.ToString().Trim(), ParseRevision, null, null );
                if( exitCode == 0 )
                {
                    Log( "Fetch Successful!" );
                    success = true;
                    break;
                }
                else if( lastRevision != currentRevision )
                {
                    Log( "Made Progress, will not increment attempt" );
                    lastRevision = currentRevision;
                    // We made progress! Reset our current attempts.
                    currentAttempt = 0;
                }
                else
                {
                    ++currentAttempt;
                    Log( "No progress made, attempt #" + currentAttempt + " was a failure" );
                }

                if( Options.IgnoreGcErrors )
                {
                    // Todo: Working Directory option.
                    string filePath = Path.Combine( ".git", "gc.log" );
                    if( File.Exists( filePath ) )
                    {
                        Log( "Ignore GC Errors flagged, deleting gc log file" );
                        File.Delete( filePath );
                    }
                }
            }
            while( ( this.Options.FetchAttempts <= 0 ) || ( currentAttempt <= this.Options.FetchAttempts ) );

            if( success == false )
            {
                throw new MigrateException( $"Fail to execute command \"git {arguments.ToString()}\". Run with -v or --verbose for details." );
            }
        }

        public void FetchBranches()
        {
            Log( "Start fetching branches..." );
            _metaInfo.LocalBranches = FetchBranchesWorker( true );
            _metaInfo.RemoteBranches = FetchBranchesWorker( false );
            Log( "End fetch branches." );

            // Tags are remote branches that start with "tags/".
            Log( "Start retrieving tags..." );
            _metaInfo.Tags = _metaInfo.RemoteBranches.ToList().FindAll( r => Regex.IsMatch( r.Trim(), @"^svn\/tags\/" ) );
            if( Options.IsVerbose )
            {
                Log( $"We have {_metaInfo.Tags.Count()} tags:" );
                foreach( var t in _metaInfo.Tags )
                {
                    Log( t );
                }
            }
            Log( "End retrieve tags..." );
        }

        public void FetchRebaseBraches()
        {
            FetchBranches();

            _metaInfo.LocalBranches = _metaInfo.LocalBranches.ToList().FindAll( l => l == Options.RebaseBranch );
            _metaInfo.RemoteBranches = _metaInfo.RemoteBranches.ToList().FindAll( r => r == Options.RebaseBranch );

            if( !_metaInfo.LocalBranches.Any() )
            {
                throw new MigrateException( string.Format( ExceptionHelper.ExceptionMessage.NO_LOCAL_BRANCH_FOUND, Options.RebaseBranch ) );
            }

            if( _metaInfo.LocalBranches.Count() > 1 )
            {
                throw new MigrateException( ExceptionHelper.ExceptionMessage.TOO_MANY_MATCHING_LOCAL_BRANCHES );
            }

            if( _metaInfo.RemoteBranches.Count() > 2 )
            {
                // 1 if remote is not pushed, 2 if its pushed to remote.
                throw new MigrateException( ExceptionHelper.ExceptionMessage.TOO_MANY_MATCHING_REMOTE_BRANCHES );
            }

            if( !_metaInfo.RemoteBranches.Any() )
            {
                throw new MigrateException( string.Format( ExceptionHelper.ExceptionMessage.NO_REMOTE_BRANCH_FOUND, Options.RebaseBranch ) );
            }

            string foundLocalBranch = _metaInfo.LocalBranches.First();
            ShowMessageIfPossible( $"Local branches \"{foundLocalBranch}\" found" );

            string foundRemoteBranches = string.Join( " ", _metaInfo.RemoteBranches );
            ShowMessageIfPossible( $"Remote branches \"{foundRemoteBranches}\" found" );

            // We only rebase the specified branch. Clear tags now.
            _metaInfo.Tags = new List<string>();
        }

        public MetaInfo GetMetaInfo()
        {
            return _metaInfo;
        }

        private IEnumerable<string> FetchBranchesWorker( bool isLocal )
        {
            // Get the list of local and remote branches, taking care to ignore console color codes and ignoring the
            // '*' character used to indicate the currently selected branch.
            string parameter = isLocal ? "l" : "r";

            string args = $"branch -{parameter} --no-color";
            string branchInfo = RunCommandIgnoreExitCode( "git", args );

            IEnumerable<string> branches = new List<string>();
            if( string.IsNullOrWhiteSpace( branchInfo ) )
            {
                Log( "No branch found." );
                return branches;
            }

            string splitter = "\n";
            if( !branchInfo.Contains( splitter ) )
            {
                splitter = " ";
                Log( "Branches are not splitted by new liner. Use ' ' as splitter." );
            }

            branches = branchInfo
                       .Split( splitter, StringSplitOptions.RemoveEmptyEntries )
                       .Select( x => x.Replace( "*", "" ).Trim() );

            if( Options.IsVerbose )
            {
                Log( $"Fechted {branches.Count()} branches ({parameter}):" );
                foreach( var b in branches )
                {
                    Log( b );
                }
            }

            Log( "End of FetchBranchesWorker" );

            return branches;
        }
    }
}
