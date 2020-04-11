using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Linq;
using CommandLine;
using Moq;
using Xunit;

namespace Svn2GitNetX.Tests
{
    public class StaleSvnBranchDeleterTests
    {
        // ---------------- Fields ----------------

        private const string svnUrl = "https://shendrick.net/svn";

        // ---------------- Tests ----------------

        [Fact]
        public void QueryHeadSvnBranchesWithoutUserNameTest()
        {
            // Prepare

            Options options = new Options
            {
                Branches = new List<string>{ "branches" }
            };

            Mock<ICommandRunner> cmdRunner = new Mock<ICommandRunner>( MockBehavior.Strict );
            SetupMockForVersionQuery( cmdRunner );

            StaleSvnBranchDeleter uut = new StaleSvnBranchDeleter(
                svnUrl,
                options,
                cmdRunner.Object,
                null,
                null,
                null,
                null
            );

            string expectedArgs = $"ls {svnUrl}/{options.Branches.First()}";

            List<string> expectedBranches = new List<string>
            {
                "branch1",
                "My Branch",
                ".MyAwesomeBranch"
            };

            List<string> stdOut = new List<string>();
            foreach( string expectedBranch in expectedBranches )
            {
                stdOut.Add( expectedBranch + "/" );
            }
            stdOut.Add( "some_file" );

            SetupMockForBranchQuery( cmdRunner, expectedArgs, stdOut );

            // Act
            List<string> actualBranches = uut.QueryHeadSvnBranches().ToList();

            // Assert
            Assert.Equal( expectedBranches, actualBranches );
        }

        [Fact]
        public void QueryHeadSvnBranchesWithUserNameTest()
        {
            // Prepare
            const string userName = "someuser";

            Options options = new Options
            {
                Branches = new List<string>{ "branches" },
                UserName = userName
            };

            Mock<ICommandRunner> cmdRunner = new Mock<ICommandRunner>( MockBehavior.Strict );
            SetupMockForVersionQuery( cmdRunner );

            StaleSvnBranchDeleter uut = new StaleSvnBranchDeleter(
                svnUrl,
                options,
                cmdRunner.Object,
                null,
                null,
                null,
                null
            );

            string expectedArgs = $"ls {svnUrl}/{options.Branches.First()} --username={userName}";

            List<string> expectedBranches = new List<string>
            {
                "branch1",
                "My Branch",
                ".MyAwesomeBranch"
            };

            List<string> stdOut = new List<string>();
            foreach( string expectedBranch in expectedBranches )
            {
                stdOut.Add( expectedBranch + "/" );
            }
            stdOut.Add( "some_file" );

            SetupMockForBranchQuery( cmdRunner, expectedArgs, stdOut );

            // Act
            List<string> actualBranches = uut.QueryHeadSvnBranches().ToList();

            // Assert
            Assert.Equal( expectedBranches, actualBranches );
        }

        /// <summary>
        /// If the SVN branches match the git branches exactly,
        /// nothing should be purged.
        /// </summary>
        [Fact]
        public void GetGitBranchesToPurgeWithSameBranchesTest()
        {
            // Prepare
            List<string> svnBranches = new List<string>
            {
                ".something",
                "with a space",
                "something.else",
                "normal"
            };

            List<string> gitBranches = new List<string>
            {
                "%2Esomething", // Git replaces the first '.' with %2E.
                "with%20a%20space", // All spaces become %20
                "something.else", // '.' in the middle is left alone
                "normal" // normal branch name
            };

            MetaInfo info = new MetaInfo { LocalBranches = gitBranches };

            Options options = new Options
            {
                Branches = new List<string> { "branches" },
            };

            StaleSvnBranchDeleter uut = new StaleSvnBranchDeleter(
                svnUrl,
                options,
                null,
                null,
                null,
                info,
                null
            );

            // Act
            IEnumerable<string> branchesToPurge = uut.GetGitBranchesToPurge( svnBranches );

            // Assert
            Assert.Empty( branchesToPurge );
        }

        /// <summary>
        /// If there are no SVN branches, purge all local branches.
        /// </summary>
        [Fact]
        public void PurgeAllGitBranchesTest()
        {
            // Prepare
            List<string> svnBranches = new List<string>();

            List<string> gitBranches = new List<string>
            {
                "%2Esomething", // Git replaces the first '.' with %2E.
                "with%20a%20space", // All spaces become %20
                "something.else", // '.' in the middle is left alone
                "normal" // normal branch name
            };

            MetaInfo info = new MetaInfo { LocalBranches = gitBranches };

            Options options = new Options
            {
                Branches = new List<string> { "branches" },
            };

            StaleSvnBranchDeleter uut = new StaleSvnBranchDeleter(
                svnUrl,
                options,
                null,
                null,
                null,
                info,
                null
            );

            // Act
            IEnumerable<string> branchesToPurge = uut.GetGitBranchesToPurge( svnBranches );

            // Assert
            Assert.Equal( gitBranches, branchesToPurge ); // <- All branches should be purged.
        }

        /// <summary>
        /// If there are no SVN branches, purge all local branches.
        /// </summary>
        [Fact]
        public void PurgeSomeGitBranchesTest()
        {
            // Prepare
            List<string> svnBranches = new List<string>
            {
                "normal"
            };

            List<string> gitBranches = new List<string>
            {
                "%2Esomething", // Git replaces the first '.' with %2E.
                "with%20a%20space", // All spaces become %20
                "something.else", // '.' in the middle is left alone
                "normal" // normal branch name
            };

            List<string> expectedBranchesToPurge = new List<string>
            {
                "%2Esomething", // Git replaces the first '.' with %2E.
                "with%20a%20space", // All spaces become %20
                "something.else", // '.' in the middle is left alone
            };

            MetaInfo info = new MetaInfo { LocalBranches = gitBranches };

            Options options = new Options
            {
                Branches = new List<string> { "branches" },
            };

            StaleSvnBranchDeleter uut = new StaleSvnBranchDeleter(
                svnUrl,
                options,
                null,
                null,
                null,
                info,
                null
            );

            // Act
            IEnumerable<string> branchesToPurge = uut.GetGitBranchesToPurge( svnBranches );

            // Assert
            Assert.Equal( expectedBranchesToPurge, branchesToPurge ); // <- All branches should be purged except normal.
        }

        // ---------------- Test Helpers ----------------

        private void SetupMockForVersionQuery( Mock<ICommandRunner> mock, int exitCode = 0 )
        {
            mock.Setup(
                m => m.Run( "svn", "--version" )
            ).Returns( exitCode );
        }

        private void SetupMockForBranchQuery(
            Mock<ICommandRunner> mock,
            string expectedArgs,
            IEnumerable<string> stdOutLines,
            int exitCode = 0
        )
        {
            mock.Setup(
                m => m.Run(
                    "svn",
                    expectedArgs,
                    It.IsAny<Action<string>>(),
                    It.IsAny<Action<string>>(),
                    It.IsAny<string>()
                )
            ).Returns(
                delegate(
                    string command,
                    string args,
                    Action<string> onStdOut,
                    Action<string> onStdErr,
                    string workingDir
                )
                {
                    foreach( string stdOut in stdOutLines )
                    {
                        onStdOut?.Invoke( stdOut );
                    }
                    return exitCode;
                }
            );
        }
    }
}
