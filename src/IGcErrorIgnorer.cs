namespace Svn2GitNetX
{
    public interface IGcErrorIgnorer
    {
        // ---------------- Properties ----------------

        bool IgnoreGcErrors { get; }

        // ---------------- Functions----------------

        /// <summary>
        /// Deletes the GC log, if <see cref="Options.IgnoreGcErrors"/> is set to true.
        /// Otherwise, this is a no-op.
        /// </summary>
        void DeleteGcLog();
    }

    public static class IGcErrorIgnorerExtensions
    {
        public static void DeleteGcLogIfEnabled( this IGcErrorIgnorer ignorer )
        {
            if( ignorer.IgnoreGcErrors )
            {
                ignorer.DeleteGcLog();
            }
        }
    }
}
