using System.IO;
using Microsoft.Extensions.Logging;

namespace Svn2GitNetX
{
    /// <summary>
    /// This class will ignore GC errors, and remove the GC log.
    /// </summary>
    public class GcErrorIgnorer : IGcErrorIgnorer
    {
        // ---------------- Fields ----------------

        private readonly ILogger logger;

        // ---------------- Constructor ----------------

        public GcErrorIgnorer( ILogger logger, Options options )
        {
            this.logger = logger;
            this.Options = options;
        }

        // ---------------- Properties ----------------

        public Options Options { get; private set; }

        // ---------------- Functions ----------------

        public void DeleteGcLog()
        {
            // Todo: Working Directory option.
            string filePath = Path.Combine( ".git", "gc.log" );
            if( File.Exists( filePath ) )
            {
                if( this.Options.IsVerbose )
                {
                    logger?.LogInformation( "Ignore GC Errors flagged, deleting gc log file" );
                }

                File.Delete( filePath );
            }
        }
    }
}
