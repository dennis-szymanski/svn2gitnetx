using System;
using Moq;
using Xunit;

namespace Svn2GitNetX.Tests
{
    public class GitPusherTests
    {
        // ---------------- Tests ----------------

        // -------- Push All --------

        [Fact]
        public void PushAllNoUrlTest()
        {
            // Prepare
            Mock<ICommandRunner> mockCmdRunner = new Mock<ICommandRunner>( MockBehavior.Strict );

            Options options = new Options
            {
                PushWhenDone = true
            };

            GitPusher uut = new GitPusher( options, mockCmdRunner.Object, null, null );

            mockCmdRunner.Setup(
                m => m.Run( "git", "push --all" )
            ).Returns( 0 );

            // Act
            uut.PushAll();

            // Assert
            mockCmdRunner.VerifyAll();
        }

        [Fact]
        public void PushAllWithUrlTest()
        {
            // Prepare
            const string url = "ssh://git@github.com:xforever1313/svn2gitnetx.git";

            Mock<ICommandRunner> mockCmdRunner = new Mock<ICommandRunner>( MockBehavior.Strict );

            Options options = new Options
            {
                PushWhenDone = true,
                RemoteGitUrl = url
            };

            GitPusher uut = new GitPusher( options, mockCmdRunner.Object, null, null );

            mockCmdRunner.Setup(
                m => m.Run( "git", $"push --all \"{url}\"" )
            ).Returns( 0 );

            // Act
            uut.PushAll();

            // Assert
            mockCmdRunner.VerifyAll();
        }

        [Fact]
        public void PushAllFailureTest()
        {
            // Prepare
            Mock<ICommandRunner> mockCmdRunner = new Mock<ICommandRunner>( MockBehavior.Strict );

            Options options = new Options
            {
                PushWhenDone = true
            };

            GitPusher uut = new GitPusher( options, mockCmdRunner.Object, null, null );

            mockCmdRunner.Setup(
                m => m.Run( "git", "push --all" )
            ).Returns( 1 ); // Non-zero, should get an exception.

            // Act
            Assert.Throws<ApplicationException>( () => uut.PushAll() );

            // Assert
            mockCmdRunner.VerifyAll();
        }
    }
}
