using System.IO;
using Microsoft.Extensions.Logging;

namespace Svn2GitNetX
{
    public class FileSystem : IFileSystem
    {
        // ---------------- Fields ----------------

        private readonly ILogger log;

        private readonly Options options;

        // ---------------- Constructor ----------------

        public FileSystem( Options options, ILogger logger )
        {
            this.log = logger;
            this.options = options;
        }

        // ---------------- Functions ----------------

        public void DeleteDirectoryIfItExists( string directoryPath )
        {
            if( Directory.Exists( directoryPath ) )
            {
                if( options.IsVerbose )
                {
                    this.log.LogInformation( $"Deleting Directory '{directoryPath}'" );
                }
                Directory.Delete( directoryPath, true );
            }
        }

        public void DeleteFileIfItExists( string filePath )
        {
            if( File.Exists( filePath ) )
            {
                if( options.IsVerbose )
                {
                    this.log.LogInformation( $"Deleting File '{filePath}'" );
                }
                File.Delete( filePath );
            }
        }
    }
}
