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

        // ---------------- Constructor ----------------

        public GcErrorIgnorer( ILogger logger, Options options ) :
            base( options, null, null, logger )
        {
        }

        // ---------------- Properties ----------------

        public bool IgnoreGcErrors => this.Options.IgnoreGcErrors;

        // ---------------- Functions ----------------

        public void DeleteGcLog()
        {
            string filePath = Path.Combine( this.GitDirectory, "gc.log" );
            if( File.Exists( filePath ) )
            {
                Log( "Ignore GC Errors flagged, deleting gc log file" );
                File.Delete( filePath );
            }
        }
    }
}
