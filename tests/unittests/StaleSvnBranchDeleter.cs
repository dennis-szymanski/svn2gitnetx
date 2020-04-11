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
