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
        public void DefaultUserNameTest()
        {
            const string expectedName = "myself";

            string[] args = new string[]
            {
                $"--username={expectedName}"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.Equal( expectedName, opt.UserName );
            Assert.Equal( expectedName, opt.GetUserName() );
        }

        [Fact]
        public void UserNameThroughArgsTest()
        {
            const string expectedName = "myself";

            string[] args = new string[]
            {
                $"--username={expectedName}",
                "--username-method=args"
            };

            // Act
            Options opt = ParseArgs( args );

            // Assert
            Assert.Equal( expectedName, opt.UserName );
            Assert.Equal( expectedName, opt.GetUserName() );
        }

        [Fact]
        public void UserNameThroughEnvVarTest()
        {
            // Preparte
            const string envVarName = "SVN2GITNETX_USERNAME_ENV_VAR";
            const string expectedName = "someone";

            string[] args = new string[]
            {
                $"--username={envVarName}",
                "--username-method=env_var"
            };

            try
            {
                // Act
                Environment.SetEnvironmentVariable( envVarName, expectedName );
                Options opt = ParseArgs( args );

                // Assert

                // UserName should be the environment variable name, but the
                // actual function call should return the real user name.
                Assert.Equal( envVarName, opt.UserName );
                Assert.Equal( expectedName, opt.GetUserName() );
            }
            finally
            {
                Environment.SetEnvironmentVariable( envVarName, string.Empty );
            }
        }

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