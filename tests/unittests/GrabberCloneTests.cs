using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Xunit;

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, options.Password ), Times.Once() );
        }

        [Fact]
        public void CloneUserNameAndPasswordAreEmptyStrings()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options()
            {
                UserName = "userName",
                UserNameMethod = CredentialsMethod.empty_string,
                Password = "password",
                PasswordMethod = CredentialsMethod.empty_string,
                IncludeMetaData = false,
                NoMinimizeUrl = true,
                RootIsTrunk = true
            };

            string expectedArguments = $"svn init --prefix=svn/ --username=\"\" --no-metadata --no-minimize-url --trunk=\"/\" {_testSvnUrl}";

            mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

            mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                .Returns( 0 );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, string.Empty ), Times.Once() );
        }

        [Fact]
        public void CloneWhenRootIsTrunkWithAllAndUserNamePasswordAreEnvParametersTest()
        {
            const string envVarName = "svn2gitnetxtest_user";
            const string userName = "MyUser";

            const string envVarPassword = "svn2gitnetxtest_passwd";
            const string password = "Password123";
            try
            {
                Environment.SetEnvironmentVariable( envVarName, userName );
                Environment.SetEnvironmentVariable( envVarPassword, password );
             
                // Prepare
                var mock = new Mock<ICommandRunner>();
                Options options = new Options()
                {
                    UserName = envVarName,
                    UserNameMethod = CredentialsMethod.env_var,
                    Password = envVarPassword,
                    PasswordMethod = CredentialsMethod.env_var,
                    IncludeMetaData = false,
                    NoMinimizeUrl = true,
                    RootIsTrunk = true
                };

                string expectedArguments = $"svn init --prefix=svn/ --username=\"{userName}\" --no-metadata --no-minimize-url --trunk=\"/\" {_testSvnUrl}";

                mock.Setup( f => f.Run( "git", It.IsAny<string>() ) ).Returns( 0 );

                mock.Setup( f => f.RunGitSvnInteractiveCommand( It.IsAny<string>(), It.IsAny<string>() ) )
                    .Returns( 0 );

                IGrabber grabber = CreateGrabber( options, mock.Object );

                // Act
                grabber.Clone();

                // Assert
                mock.Verify( f => f.RunGitSvnInteractiveCommand( expectedArguments, password ), Times.Once() );
            }
            finally
            {
                Environment.SetEnvironmentVariable( envVarName, string.Empty );
                Environment.SetEnvironmentVariable( envVarPassword, string.Empty );
            }
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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object );

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

            IGrabber grabber = CreateGrabber( options, mock.Object, "config" );

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

            mock.Setup(
                f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            ).Returns( 0 );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify(
                f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan ),
                Times.Once()
            );
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

            mock.Setup(
                f => f.Run( "git", It.IsAny<string>() )
            ).Returns( 0 );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify(
                f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan ),
                Times.Once()
            );
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

            mock.Setup(
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            ).Returns( 0 );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify(
                f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan ),
                Times.Once()
            );
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

            mock.Setup( 
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            ).Returns( 0 );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify(
                f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan ),
                Times.Once()
            );
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

            mock.Setup(
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            ).Returns( 0 );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify(
                f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan ),
                Times.Once()
            );
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

            mock.Setup(
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            ).Returns( 0 );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            mock.Verify(
                f => f.Run( "git", expectedArguments, It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan ),
                Times.Once()
            );
        }

        [Fact]
        public void FetchWithUnlimitedRetriesTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options
            {
                // -1 means unlimited attempts.
                FetchAttempts = -1
            };

            int timesCalled = 0;
            const int maxAttempts = 3;

            mock.Setup( 
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            )
            .Returns( 
                delegate( string cmd, string arguments, Action<string> onStdout, Action<string> onStdErr, string workDir, TimeSpan timeout )
                {
                    // Only make progress on 1 revision.
                    onStdout( "r1 = somehash (ref/somewhere/svn)" );

                    // After 3 attempts, return 0
                    // to emulate the fetch finally working.
                    ++timesCalled;
                    if( timesCalled <= maxAttempts )
                    {
                        return 128;
                    }
                    else
                    {
                        return 0;   
                    }
                }
            );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert
            Assert.Equal( maxAttempts + 1, timesCalled );
        }

        [Fact]
        public void FetchWithLimitedRetriesTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options
            {
                FetchAttempts = 3
            };

            int timesCalled = 0;

            mock.Setup( 
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            )
            .Returns( 
                delegate( string cmd, string arguments, Action<string> onStdout, Action<string> onStdErr, string workDir, TimeSpan timeout )
                {
                    // Only make progress on 1 revision.
                    // This will increment our attempts since no progress has been made.
                    onStdout( "r127 = somehash (ref/somewhere/svn)" );

                    ++timesCalled;
                    return 128;
                }
            );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            Exception ex = Record.Exception( () => grabber.Clone() );

            // Assert
            Assert.IsType<MigrateException>( ex );

            // Times it will be called:
            // 1 - to go from rev -1 to rev 127 (we treat this as making progress)
            // 2 - to go from rev 127 to rev 127 (First Failure)
            // 3 - to go from rev 127 to rev 127 (Attempt 1)
            // 4 - to go from rev 127 to rev 127 (Attempt 2)
            // 5 - to go from rev 127 to rev 127 (Attempt 3, break out)
            Assert.Equal( options.FetchAttempts + 2, timesCalled );
        }

        [Fact]
        public void FetchWithLimitedRetriesButWithProgressMadeTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options
            {
                FetchAttempts = 1
            };

            List<string> responses = new List<string>
            {
                "r127 = somehash (ref/somewhere/svn)",
                "r128 = somehash (ref/somewhere/svn)",
                "r129 = somehash (ref/somewhere/svn)"
            };

            int timesCalled = 0;

            mock.Setup( 
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            )
            .Returns( 
                delegate( string cmd, string arguments, Action<string> onStdout, Action<string> onStdErr, string workDir, TimeSpan timeout )
                {
                    onStdout( responses[timesCalled] );

                    ++timesCalled;

                    if( timesCalled >= responses.Count )
                    {
                        return 0;
                    }
                    else
                    {
                        return 128;
                    }
                }
            );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert

            // Times it will be called:
            // 1 - to go from rev -1 to rev 127 (we treat this as making progress)
            // 2 - to go from rev 127 to rev 128 (Made Progress)
            // 3 - to go from rev 128 to rev 129 (Made Progress, returns 0, breaks out)
            Assert.Equal( responses.Count, timesCalled );
        }

        [Fact]
        public void FetchWithLimitedRetriesButWithProgressMadeAfter1FailureTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options
            {
                FetchAttempts = 1
            };

            List<Tuple<string, int>> responses = new List<Tuple<string, int>>
            {
                new Tuple<string, int>( "r126 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r127 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r128 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r128 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r129 = somehash (ref/somewhere/svn)", 0 )
            };

            int timesCalled = 0;

            mock.Setup( 
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            )
            .Returns( 
                delegate( string cmd, string arguments, Action<string> onStdout, Action<string> onStdErr, string workDir, TimeSpan timeout )
                {
                    onStdout( responses[timesCalled].Item1 );

                    return responses[timesCalled++].Item2;
                }
            );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert

            // Times it will be called:
            // 1 - to go from rev -1 to rev 126 (we treat this as making progress)
            // 2 - to go from rev 126 to rev 127 (Made progress even though returned non-zero)
            // 2 - to go from rev 127 to rev 128 (Made progress even though returned non-zero)
            // 3 - to go from rev 128 to rev 128 (First Failure, did not make progress, increment attempt.)
            // 4 - to go from rev 128 to rev 129 (Attempt 1, returns 0, breaks out)
            Assert.Equal( responses.Count, timesCalled );
        }

        [Fact]
        public void FetchWithLimitedRetriesAnMultipleFailures()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options
            {
                FetchAttempts = 1
            };

            List<Tuple<string, int>> responses = new List<Tuple<string, int>>
            {
                new Tuple<string, int>( "r126 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r126 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r128 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r128 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r129 = somehash (ref/somewhere/svn)", 0 )
            };

            int timesCalled = 0;

            mock.Setup( 
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            )
            .Returns( 
                delegate( string cmd, string arguments, Action<string> onStdout, Action<string> onStdErr, string workDir, TimeSpan timeout )
                {
                    onStdout( responses[timesCalled].Item1 );

                    return responses[timesCalled++].Item2;
                }
            );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            grabber.Clone();

            // Assert

            // Times it will be called:
            // 1 - to go from rev -1 to rev 126 (we treat this as making progress)
            // 2 - to go from rev 126 to rev 126 (First Failure)
            // 3 - to go from rev 126 to rev 128 (Made progress even though returned non-zero.  Attempts reset)
            // 4 - to go from rev 128 to rev 128 (First Failure for this rev, did not make progress, increment attempt.)
            // 5 - to go from rev 128 to rev 129 (Returns 0, breaks out)
            Assert.Equal( responses.Count, timesCalled );
        }

        [Fact]
        public void FetchWithLimitedRetriesButWithNoProgressMadeAfter1FailureTest()
        {
            // Prepare
            var mock = new Mock<ICommandRunner>();
            Options options = new Options
            {
                FetchAttempts = 1
            };

            List<Tuple<string, int>> responses = new List<Tuple<string, int>>
            {
                new Tuple<string, int>( "r126 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r127 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r128 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r128 = somehash (ref/somewhere/svn)", 128 ),
                new Tuple<string, int>( "r128 = somehash (ref/somewhere/svn)", 128 )
            };

            int timesCalled = 0;

            mock.Setup( 
                f => f.Run( "git", It.IsAny<string>(), It.IsAny<Action<string>>(), null, null, Timeout.InfiniteTimeSpan )
            )
            .Returns( 
                delegate( string cmd, string arguments, Action<string> onStdout, Action<string> onStdErr, string workDir, TimeSpan timeout )
                {
                    onStdout( responses[timesCalled].Item1 );

                    return responses[timesCalled++].Item2;
                }
            );

            IGrabber grabber = CreateGrabber( options, mock.Object );

            // Act
            Exception ex = Record.Exception( () => grabber.Clone() );

            // Assert
            Assert.IsType<MigrateException>( ex );

            // Times it will be called:
            // 1 - to go from rev -1 to rev 126 (we treat this as making progress)
            // 2 - to go from rev 126 to rev 127 (Made progress even though returned non-zero)
            // 3 - to go from rev 127 to rev 128 (Made progress even though returned non-zero)
            // 4 - to go from rev 128 to rev 128 (First Failure, did not make progress, increment attempt.)
            // 5 - to go from rev 128 to rev 128 (Attempt 1, no success.  Should throw exception).
            Assert.Equal( responses.Count, timesCalled );
        }

        // ---------------- Test Helpers ----------------

        private IGrabber CreateGrabber( Options options, ICommandRunner runner, string gitConfigCommandArgs = "" )
        {
            Mock<IGcErrorIgnorer> ignorer = new Mock<IGcErrorIgnorer>( MockBehavior.Strict );
            ignorer.Setup( m => m.IgnoreGcErrors ).Returns( options.IgnoreGcErrors );

            Mock<ILockBreaker> lockBreaker = new Mock<ILockBreaker>( MockBehavior.Strict );
            lockBreaker.Setup( m => m.ShouldBreakLocks ).Returns( options.BreakLocks );

            return new Grabber(
                _testSvnUrl,
                options,
                runner,
                gitConfigCommandArgs,
                null,
                null,
                ignorer.Object,
                lockBreaker.Object
            );
        }
    }
}