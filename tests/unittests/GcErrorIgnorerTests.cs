using System.IO;
using Moq;
using Xunit;

namespace Svn2GitNetX.Tests
{
    public class GcErrorIgnorerTests
    {
        // ---------------- Tests ----------------

        [Fact]
        public void DeleteGcLogIfEnabledWhileEnabledTest()
        {
            // Prepare
            Options options = new Options
            {
                IgnoreGcErrors = true
            };

            string expectedPath = Path.Combine( ".", ".git", "gc.log" );

            Mock<IFileSystem> mockFileSystem = new Mock<IFileSystem>( MockBehavior.Strict );
            mockFileSystem.Setup( m => m.DeleteFileIfItExists( expectedPath ) );

            GcErrorIgnorer uut = new GcErrorIgnorer( null, options, mockFileSystem.Object );

            // Act
            uut.DeleteGcLogIfEnabled();

            // Assert
            mockFileSystem.Verify( m => m.DeleteFileIfItExists( expectedPath ) );
        }

        [Fact]
        public void DeleteGcLogIfEnabledWhileDisabledTest()
        {
            // Prepare
            Options options = new Options
            {
                IgnoreGcErrors = false
            };

            Mock<IFileSystem> mockFileSystem = new Mock<IFileSystem>( MockBehavior.Strict );
            mockFileSystem.Setup( m => m.DeleteFileIfItExists( It.IsAny<string>() ) );

            GcErrorIgnorer uut = new GcErrorIgnorer( null, options, mockFileSystem.Object );

            // Act
            uut.DeleteGcLogIfEnabled();

            // Assert
            mockFileSystem.VerifyNoOtherCalls(); // Nothing should be called.
        }
    }
}
