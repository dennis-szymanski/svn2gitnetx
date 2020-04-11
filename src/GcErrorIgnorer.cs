using System.IO;
using Microsoft.Extensions.Logging;

namespace Svn2GitNetX
{
    /// <summary>
    /// This class will ignore GC errors, and remove the GC log.
    /// </summary>
    public class GcErrorIgnorer : Worker, IGcErrorIgnorer
    {
        // ---------------- Fields ----------------

        private readonly IFileSystem fileSystem;

        // ---------------- Constructor ----------------

        public GcErrorIgnorer( ILogger logger, Options options, IFileSystem fileSystem ) :
            base( options, null, null, logger )
        {
            this.fileSystem = fileSystem;
        }

        // ---------------- Properties ----------------

        public bool IgnoreGcErrors => this.Options.IgnoreGcErrors;

        // ---------------- Functions ----------------

        public void DeleteGcLog()
        {
            string filePath = Path.Combine( this.GitDirectory, "gc.log" );
            this.fileSystem.DeleteFileIfItExists( filePath );
        }
    }
}
