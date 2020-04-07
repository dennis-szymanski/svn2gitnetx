using System;
using System.Collections.Generic;
using CommandLine;

namespace Svn2GitNetX
{
    public enum CredentialsMethod
    {
        /// <summary>
        /// Get the credentials from the command line arguments
        /// (default)
        /// </summary>
        args,

        /// <summary>
        /// Get the credentials from an environment variable.
        /// </summary>
        env_var
    }

    public class Options
    {
        public Options()
        {
            this.UserNameMethod = CredentialsMethod.args;
        }

        /// <summary>
        /// Be verbose in logging -- useful for debugging issues
        /// </summary>
        /// <returns></returns>
        [Option( 'v', "verbose", Default = false, HelpText = "Be verbose in logging -- useful for debugging issues" )]
        public bool IsVerbose
        {
            get;
            set;
        }

        /// <summary>
        /// Include metadata in git logs (git-svn-id)
        /// </summary>
        /// <returns></returns>
        [Option( 'm', "metadata", Default = false, HelpText = "Include metadata in git logs (git-svn-id)" )]
        public bool IncludeMetaData
        {
            get;
            set;
        }

        /// <summary>
        /// Accept URLs as-is without attempting to connect to a higher level directory
        /// </summary>
        /// <returns></returns>
        [Option( "no-minimize-url", HelpText = "Accept URLs as-is without attempting to connect to a higher level directory" )]
        public bool NoMinimizeUrl
        {
            get;
            set;
        }

        [Option( "rootistrunk", HelpText = "Use this if the root level of the repo is equivalent to the trunk and there are no tags or branches" )]
        public bool RootIsTrunk
        {
            get;
            set;
        }

        /// <summary>
        /// Subpath to trunk from repository URL (default: trunk)
        /// </summary>
        /// <returns></returns>
        [Option( "trunk", Default = "trunk", HelpText = "Subpath to trunk from repository URL (default: trunk)" )]
        public string SubpathToTrunk
        {
            get;
            set;
        }

        [Option( "notrunk", HelpText = "Do not import anything from trunk" )]
        public bool NoTrunk
        {
            get;
            set;
        }

        [Option(
            "branches",
            HelpText = "Subpath to branches from repository URL (default: branches); can take in multiple values via '--branches banch1 branch2'"
        )]
        public IEnumerable<string> Branches
        {
            get;
            set;
        }

        [Option( "nobranches", HelpText = "Do not try to import any branches" )]
        public bool NoBranches
        {
            get;
            set;
        }

        [Option(
            "tags",
            HelpText = "Subpath to tags from repository URL (default: tags); can take in multiple values via '--tags tag1 tag2'"
        )]
        public IEnumerable<string> Tags
        {
            get;
            set;
        }


        [Option( "notags", HelpText = "Do not try to import any tags" )]
        public bool NoTags
        {
            get;
            set;
        }

        [Option(
            "exclude",
            HelpText = "Specify a Perl regular expression to filter paths when fetching; can take in multiple values via '--exclude exclude1 exclude2'"
        )]
        public IEnumerable<string> Exclude
        {
            get;
            set;
        }

        [Option( "revision", HelpText = "Start importing from SVN revision START_REV; optionally end at END_REV" )]
        public string Revision
        {
            get;
            set;
        }

        [Option(
            "username",
            HelpText = "Username for transports that needs it (http(s), svn)" 
        )]
        public string UserName
        {
            get;
            set;
        }

        [Option(
            "username-method",
            HelpText = "How to get the user name.  '" + 
                       nameof( CredentialsMethod.args ) + 
                       "' for using the value passed into the username argument.  '" +
                       nameof( CredentialsMethod.env_var ) + "' for using the value stored in the environment variable specified in the username argument.",
            Default = CredentialsMethod.args
        )]
        public CredentialsMethod UserNameMethod
        {
            get;
            set;
        }

        [Option( "password", HelpText = "Password for transports that need it (http(s), svn)" )]
        public string Password
        {
            get;
            set;
        }

        [Option( "rebase", HelpText = "Instead of cloning a new project, rebase an existing one against SVN" )]
        public bool Rebase
        {
            get;
            set;
        }

        [Option( "rebasebranch", HelpText = "Rebase specified branch" )]
        public string RebaseBranch
        {
            get;
            set;
        }

        [Option( "authors", HelpText = "Path to file containing svn-to-git authors mapping" )]
        public string Authors
        {
            get;
            set;
        }

        [Option(
            "break-locks",
            HelpText = "Breaks any index.lock files in the .git/svn/refs/remotes/svn/* directories.  Only use this if you are sure there are no git process running in this directory.",
            Default = false
        )]
        public bool BreakLocks
        {
            get;
            set;
        }

        [Option( 
            "fetch-attempts", 
            HelpText = "How many attempts to try to fetch a single revision AFTER the first failure.  Set to -1 (or less) to try forever until CTRL+C is hit.",
            Default = 0
        )]
        public int FetchAttempts
        {
            get;
            set;
        }

        [Option(
            "ignore-gc-errors",
            HelpText = "If a GC error happens during fetching, ignore it.  This also deletes the gc.log file.",
            Default = false
        )]
        public bool IgnoreGcErrors
        {
            get;
            set;
        }

        // ---------------- Functions ----------------

        public string GetUserName()
        {
            if( this.UserNameMethod == CredentialsMethod.env_var )
            {
                return Environment.GetEnvironmentVariable( this.UserName );
            }
            else
            {
                return this.UserName;
            }
        }
    }
}
