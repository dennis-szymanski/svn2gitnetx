namespace Svn2GitNetX
{
    public interface IFileSystem
    {
        // ---------------- Functions ----------------

        /// <summary>
        /// Deletes the given file if it exists.
        /// </summary>
        void DeleteFileIfItExists( string filePath );

        /// <summary>
        /// Deletes the given directory if it exists.
        /// </summary>
        void DeleteDirectoryIfItExists( string directoryPath );
    }
}
