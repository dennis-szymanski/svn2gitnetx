using System.Collections.Generic;
using System.IO;
using Moq;
using Xunit;

namespace Svn2GitNetX.Tests
{
    public class LockBreakerTests
    {
        // ---------------- Fields ----------------

        private static readonly string lockDir = Path.Combine(
            ".",
            ".git",
            "svn",
            "refs",
            "remotes",
            "svn"
        );

        // ---------------- Tests ----------------

        /// <summary>
        /// Ensures if an index file exists, we delete it.
        /// </summary>
        [Fact]
        public void BreakLocksFileExistsTest()
        {
            // Prepare

            List<string> remotes = new List<string>
            {
                "remote1",
                "remote2"
            };

            Mock<IFileSystem> mockFs = new Mock<IFileSystem>( MockBehavior.Strict );
            mockFs.Setup( m => m.DirectoryExists( lockDir ) ).Returns( true );
            mockFs.Setup( m => m.GetChildDirectories( lockDir ) ).Returns( remotes );
            mockFs.Setup( m => m.DeleteFileIfItExists( Path.Combine( remotes[0], "index.lock") ) );
            mockFs.Setup( m => m.DeleteFileIfItExists( Path.Combine( remotes[1], "index.lock" ) ) );

            Options options = new Options
            {
                BreakLocks = true
            };

            LockBreaker uut = new LockBreaker(
                null,
                options,
                mockFs.Object
            );

            // Act
            uut.BreakLocksIfEnabled();

            // Assert
            mockFs.VerifyAll();
        }

        [Fact]
        public void BreakLocksNoRemotesTest()
        {
            // Prepare

            List<string> remotes = new List<string>();

            Mock<IFileSystem> mockFs = new Mock<IFileSystem>( MockBehavior.Strict );
            mockFs.Setup( m => m.DirectoryExists( lockDir ) ).Returns( true );
            mockFs.Setup( m => m.GetChildDirectories( lockDir ) ).Returns( remotes );

            // No other calls should happen since there is no child directories specified.

            Options options = new Options
            {
                BreakLocks = true
            };

            LockBreaker uut = new LockBreaker(
                null,
                options,
                mockFs.Object
            );

            // Act
            uut.BreakLocksIfEnabled();

            // Assert
            mockFs.VerifyAll();
        }

        [Fact]
        public void BreakLocksNoSvnFolderTest()
        {
            // Prepare

            List<string> remotes = new List<string>();

            Mock<IFileSystem> mockFs = new Mock<IFileSystem>( MockBehavior.Strict );
            mockFs.Setup( m => m.DirectoryExists( lockDir ) ).Returns( false );

            // No other calls should happen since there is no remote SVN folder.

            Options options = new Options
            {
                BreakLocks = true
            };

            LockBreaker uut = new LockBreaker(
                null,
                options,
                mockFs.Object
            );

            // Act
            uut.BreakLocksIfEnabled();

            // Assert
            mockFs.VerifyAll();
        }

        /// <summary>
        /// Ensures if break locks is disabled, nothing happens.
        /// </summary>
        [Fact]
        public void BreakLocksDisabledTest()
        {
            // Prepare

            List<string> remotes = new List<string>();

            Mock<IFileSystem> mockFs = new Mock<IFileSystem>( MockBehavior.Strict );

            // No other calls should happen since this is disabled.

            Options options = new Options
            {
                BreakLocks = false
            };

            LockBreaker uut = new LockBreaker(
                null,
                options,
                mockFs.Object
            );

            // Act
            uut.BreakLocksIfEnabled();

            // Assert
            mockFs.VerifyAll();
        }
    }
}
