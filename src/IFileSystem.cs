using System.Collections.Generic;

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

        /// <summary>
        /// Does the specified directory exist?
        /// </summary>
        bool DirectoryExists( string directoryPath );

        /// <summary>
        /// Gets a list of child directories within the given directory.
        /// </summary>
        IEnumerable<string> GetChildDirectories( string directoryPath );
    }
}
