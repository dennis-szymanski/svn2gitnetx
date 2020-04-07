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
using Xunit;

namespace Svn2GitNetX.Tests
{
    public class ArgumentParsingTests
    {
        // ---------------- Tests ----------------

        [Fact]
        public void VerboseTest()
        {
            // Prepare
            string[] args = new string[]
            {
                "-v"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.True( opt.IsVerbose );
        }

        /// <summary>
        /// Ensures if we don't specify a user name method,
        /// we default to the CLI argument.
        /// </summary>
        [Fact]
        public void DefaultUserNamePasswordTest()
        {
            const string expectedName = "myself";
            const string expectedPassword = "password123";

            string[] args = new string[]
            {
                $"--username={expectedName}",
                $"--password={expectedPassword}"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.Equal( expectedName, opt.UserName );
            Assert.Equal( expectedName, opt.GetUserName() );

            Assert.Equal( expectedPassword, opt.Password );
            Assert.Equal( expectedPassword, opt.GetPassword() );
        }

        [Fact]
        public void UserNameThroughArgsTest()
        {
            const string expectedName = "myself";
            const string expectedPassword = "password123";

            string[] args = new string[]
            {
                $"--username={expectedName}",
                "--username-method=args",
                $"--password={expectedPassword}",
                "--password-method=args"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.Equal( expectedName, opt.UserName );
            Assert.Equal( expectedName, opt.GetUserName() );

            Assert.Equal( expectedPassword, opt.Password );
            Assert.Equal( expectedPassword, opt.GetPassword() );
        }

        [Fact]
        public void UserNamePasswordThroughEnvVarTest()
        {
            // Preparte
            const string envVarUserName = "SVN2GITNETX_USERNAME_ENV_VAR";
            const string expectedName = "someone";

            const string envVarPassword = "SVN2GITNETX_PASSWD_ENV_VAR";
            const string expectedPassword = "password4567";

            string[] args = new string[]
            {
                $"--username={envVarUserName}",
                "--username-method=env_var",
                $"--password={envVarPassword}",
                "--password-method=env_var"
            };

            try
            {
                // Act
                Environment.SetEnvironmentVariable( envVarUserName, expectedName );
                Environment.SetEnvironmentVariable( envVarPassword, expectedPassword );
                Options opt = ParseArgs( args );

                // Assert

                // UserName/password should be the environment variable name, but the
                // actual function call should return the real username/password.
                Assert.Equal( envVarUserName, opt.UserName );
                Assert.Equal( expectedName, opt.GetUserName() );

                Assert.Equal( envVarPassword, opt.Password );
                Assert.Equal( expectedPassword, opt.GetPassword() );
            }
            finally
            {
                Environment.SetEnvironmentVariable( envVarUserName, string.Empty );
                Environment.SetEnvironmentVariable( envVarPassword, string.Empty );
            }
        }

        [Fact]
        public void SingleExcludeTest()
        {
            string[] args = new string[]
            {
                "--exclude=hello"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.NotNull( opt.Exclude );
            Assert.Single( opt.Exclude );
            Assert.Equal( "hello", opt.Exclude.First() );
        }

        [Fact]
        public void SingleExcludeAndTagTest()
        {
            string[] args = new string[]
            {
                "--exclude",
                "hello",
                "--tags",
                "world"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.NotNull( opt.Exclude );
            Assert.Single( opt.Exclude );
            Assert.Equal( "hello", opt.Exclude.First() );

            Assert.NotNull( opt.Tags );
            Assert.Single( opt.Tags );
            Assert.Equal( "world", opt.Tags.First() );
        }

        [Fact]
        public void MultipleExcludeTest()
        {
            string[] args = new string[]
            {
                "--exclude",
                "hello",
                "world"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.NotNull( opt.Exclude );
            Assert.Equal( 2, opt.Exclude.Count() );
            Assert.Equal( "hello", opt.Exclude.First() );
            Assert.Equal( "world", opt.Exclude.Last() );
        }

        [Fact]
        public void MultipleExcludeAndTagTest()
        {
            string[] args = new string[]
            {
                "--exclude",
                "hello",
                "world",
                "--tags",
                "how",
                "are you"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.NotNull( opt.Exclude );
            Assert.Equal( 2, opt.Exclude.Count() );
            Assert.Equal( "hello", opt.Exclude.First() );
            Assert.Equal( "world", opt.Exclude.Last() );

            Assert.NotNull( opt.Tags );
            Assert.Equal( 2, opt.Tags.Count() );
            Assert.Equal( "how", opt.Tags.First() );
            Assert.Equal( "are you", opt.Tags.Last() );
        }

// This test does not work.  It seems like its a limitation
// with the CommandLineParser Library
// Just need to specify the exclude and tag arguments once.
#if FALSE
        [Fact]
        public void MultipleExcludeAndTagOutOfOrderTest()
        {
            string[] args = new string[]
            {
                "--exclude",
                "hello",
                "--tags",
                "how",
                "--exclude",
                "world",
                "--tags",
                "are you"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.NotNull( opt.Exclude );
            Assert.Equal( 2, opt.Exclude.Count() );
            Assert.Equal( "hello", opt.Exclude.First() );
            Assert.Equal( "world", opt.Exclude.Last() );

            Assert.NotNull( opt.Tags );
            Assert.Equal( 2, opt.Tags.Count() );
            Assert.Equal( "how", opt.Tags.First() );
            Assert.Equal( "are you", opt.Tags.Last() );
        }
#endif

        // ---------------- Test Helpers ----------------

        private static Options ParseArgs( string[] args )
        {
            Options opt = null;
            Parser.Default.ParseArguments<Options>( args )
                .WithParsed( options => { opt = options; } );

            return opt;
        }
    }
}