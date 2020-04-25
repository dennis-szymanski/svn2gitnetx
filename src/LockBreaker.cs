using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Svn2GitNetX
{
    public class LockBreaker : Worker, ILockBreaker
    {
        // ---------------- Fields ----------------

        private readonly IFileSystem fileSystem;

        // ---------------- Constructor ----------------

        public LockBreaker( ILogger logger, Options options, IFileSystem fileSystem ) :
            base( options, null, null, logger )
        {
            this.fileSystem = fileSystem;
        }

        // ---------------- Properties ----------------

        public bool ShouldBreakLocks => this.Options.BreakLocks;

        // ---------------- Functions ----------------

        public void BreakLocks()
        {
            string svnIndexFolder = Path.Combine(
                this.GitDirectory,
                "svn",
                "refs",
                "remotes",
                "svn"
            );

            if( this.fileSystem.DirectoryExists( svnIndexFolder ) )
            {
                foreach( string dir in this.fileSystem.GetChildDirectories( svnIndexFolder ) )
                {
                    string lockFile = Path.Combine( dir, "index.lock" );
                    try
                    {
                        this.fileSystem.DeleteFileIfItExists( lockFile );
                    }
                    catch( Exception )
                    {
                        LogError( $"Could not delete lock at '{lockFile}', is the file in use by a different process?" );
                        throw;
                    }
                }
            }
        }
    }
}
