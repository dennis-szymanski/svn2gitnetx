using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Linq;
using Xunit;
using Moq;

namespace Svn2GitNetX.Tests
{
    public class GrabberCloneTests
    {
        private string _testSvnUrl = "svn://testurl";

        [Fact]
        public void CloneWhenRootIsTrunkWithAllParametersTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                UserName = "userName",
                Password = "password",
                IncludeMetaData = false,
                NoMinimizeUrl = true,
                RootIsTrunk = true
            };

            string expectedArguments = $"svn init --prefix=svn/ --username=\"userName\" --no-metadata --no-minimize-url --trunk=\"/\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsTrunkWithoutUserNameAndPasswordTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                IncludeMetaData = false,
                NoMinimizeUrl = true,
                RootIsTrunk = true
            };

            string expectedArguments = $"svn init --prefix=svn/ --no-metadata --no-minimize-url --trunk=\"/\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsTrunkHasMetaDataTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                IncludeMetaData = true,
                NoMinimizeUrl = true,
                RootIsTrunk = true
            };

            string expectedArguments = $"svn init --prefix=svn/ --no-minimize-url --trunk=\"/\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsTrunkHasMinimizeUrlTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                IncludeMetaData = true,
                NoMinimizeUrl = true,
                RootIsTrunk = true
            };

            string expectedArguments = $"svn init --prefix=svn/ --no-minimize-url --trunk=\"/\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsNotTrunkWithoutBranchesAndTagsTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = true,
                RootIsTrunk = false
            };

            string expectedArguments = $"svn init --prefix=svn/ {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsNotTrunkHasSubPathToTrunkTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = true,
                RootIsTrunk = false
            };

            string expectedArguments = $"svn init --prefix=svn/ --trunk=\"subpath\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) )
                .Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsNotTrunkHasSubPathToTrunkAndTagsTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = false,
                RootIsTrunk = false,
                Tags = new List<string>()
                {
                    "tag1",
                    "tag2"
                }
            };

            string expectedArguments = $"svn init --prefix=svn/ --trunk=\"subpath\" --tags=\"tag1\" --tags=\"tag2\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) )
                .Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsNotTrunkHasSubPathToTrunkAndDefaultTagTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = false,
                RootIsTrunk = false
            };

            string expectedArguments = $"svn init --prefix=svn/ --trunk=\"subpath\" --tags=\"tags\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) )
                .Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsNotTrunkHasSubPathToTrunkAndDefaultBranchTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = false,
                NoTags = true,
                RootIsTrunk = false
            };

            string expectedArguments = $"svn init --prefix=svn/ --trunk=\"subpath\" --branches=\"branches\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) )
                .Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsNotTrunkHasSubPathToTrunkAndBranchesTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = false,
                NoTags = true,
                RootIsTrunk = false,
                Branches = new List<string>()
                {
                    "branch1",
                    "branch2"
                }
            };

            string expectedArguments = $"svn init --prefix=svn/ --trunk=\"subpath\" --branches=\"branch1\" --branches=\"branch2\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) )
                .Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsNotTrunkHasSubPathToTrunkGitCommandExecutionFailTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = true,
                RootIsTrunk = false,
            };

            string expectedExceptionMessage = string.Format( ExceptionHelper.ExceptionMessage.FAIL_TO_EXECUTE_COMMAND, $"git svn init --prefix=svn/ --trunk=\"subpath\" {_testSvnUrl}" );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( -1 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            Exception ex = Record.Exception( () => grabber.Clone() );

            // Assert
            Assert.IsType<MigrateException>( ex );
            Assert.Equal( expectedExceptionMessage, ex.Message );
        }

        [Fact]
        public void CloneWhenAuthorsIsNotEmptyTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = true,
                RootIsTrunk = false,
                Authors = "author1"
            };

            string expectedArguments = "config svn.authorsfile author1";

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "config", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.Run( "git", expectedArguments ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRevisionIsNotEmptyTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = true,
                RootIsTrunk = false,
                Revision = "123:456"
            };

            string expectedArguments = "svn fetch -r 123:456";

            mock.Setup( f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null ) ).Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null ), Times.Once() );
        }

        [Fact]
        public void CloneWhenExcludeIsNotEmptyRootIsTrunkTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = true,
                RootIsTrunk = true,
                Exclude = new List<string>()
                {
                    "ex1",
                    "ex2"
                }
            };

            string ignorePathsRegEx = @"^(?:)(?:ex1|ex2)";
            string expectedArguments = $"svn fetch --ignore-paths=\"{ignorePathsRegEx}\"";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null ), Times.Once() );
        }

        [Fact]
        public void CloneWhenExcludeIsNotEmptyRevisionIsNotEmptyRootIsTrunkTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = true,
                RootIsTrunk = true,
                Exclude = new List<string>()
                {
                    "ex1",
                    "ex2"
                },
                Revision = "123:456"
            };

            string ignorePathsRegEx = @"^(?:)(?:ex1|ex2)";
            string expectedArguments = $"svn fetch -r 123:456 --ignore-paths=\"{ignorePathsRegEx}\"";

            mock.Setup( f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null ) ).Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null ), Times.Once() );
        }

        [Fact]
        public void CloneWhenExcludeIsNotEmptyRootIsNotTrunkTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = true,
                RootIsTrunk = false,
                Exclude = new List<string>()
                {
                    "ex1",
                    "ex2"
                }
            };

            string ignorePathsRegEx = @"^(?:subpath[\/])(?:ex1|ex2)";
            string expectedArguments = $"svn fetch --ignore-paths=\"{ignorePathsRegEx}\"";

            mock.Setup( f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null ) ).Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null ), Times.Once() );
        }

        [Fact]
        public void CloneWhenExcludeIsNotEmptyRootIsNotTrunkHasTagsTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = true,
                NoTags = false,
                RootIsTrunk = false,
                Exclude = new List<string>()
                {
                    "ex1",
                    "ex2"
                },
                Tags = new List<string>()
                {
                    "tag1",
                    "tag2"
                }
            };

            string ignorePathsRegEx = @"^(?:subpath[\/]|tag1[\/][^\/]+[\/]|tag2[\/][^\/]+[\/])(?:ex1|ex2)";
            string expectedArguments = $"svn fetch --ignore-paths=\"{ignorePathsRegEx}\"";

            mock.Setup( f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null ) ).Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null ), Times.Once() );
        }

        [Fact]
        public void CloneWhenExcludeIsNotEmptyRootIsNotTrunkHasTagsHasBranchesTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                SubpathToTrunk = "subpath",
                IncludeMetaData = true,
                NoBranches = false,
                NoTags = false,
                RootIsTrunk = false,
                Exclude = new List<string>()
                {
                    "ex1",
                    "ex2"
                },
                Tags = new List<string>()
                {
                    "tag1",
                    "tag2"
                },
                Branches = new List<string>()
                {
                    "branch1",
                    "branch2"
                }
            };

            string ignorePathsRegEx = @"^(?:subpath[\/]|tag1[\/][^\/]+[\/]|tag2[\/][^\/]+[\/]|branch1[\/][^\/]+[\/]|branch2[\/][^\/]+[\/])(?:ex1|ex2)";
            string expectedArguments = $"svn fetch --ignore-paths=\"{ignorePathsRegEx}\"";

            mock.Setup( f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null ) ).Returns( 0 );

            IGrabber grabber = new Grabber( _testSvnUrl, options, mock.Object, "", null, null );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null ), Times.Once() );
        }
    }
}