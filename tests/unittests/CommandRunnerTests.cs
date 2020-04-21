using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Svn2GitNetX.Tests
{
    public class CommandRunnerTests
    {
        [Fact]
        public void SearchForPassword()
        {
            // Prepare
            const string stdError = "Password for ";

            using( MemoryStream strReader = new MemoryStream( Encoding.ASCII.GetBytes( stdError ) ) )
            {
                using( StreamReader reader = new StreamReader( strReader ) )
                {
                    // Act
                    OutputMessageType type = CommandRunner.ReadAndDisplayInteractiveCommandProcessOutput(
                        reader
                    );

                    // Assert
                    Assert.Equal( OutputMessageType.RequestInputPassword, type );
                }
            }
        }

        [Fact]
        public void SearchForRequestAcceptCertificateFullOptions()
        {
            // Prepare
            const string stdError = "(R)eject, accept (t)emporarily or accept (p)ermanently?";

            using( MemoryStream strReader = new MemoryStream( Encoding.ASCII.GetBytes( stdError ) ) )
            {
                using( StreamReader reader = new StreamReader( strReader ) )
                {
                    // Act
                    OutputMessageType type = CommandRunner.ReadAndDisplayInteractiveCommandProcessOutput(
                        reader
                    );

                    // Assert
                    Assert.Equal( OutputMessageType.RequestAcceptCertificateFullOptions, type );
                }
            }
        }

        [Fact]
        public void SearchForRequestAcceptCertificateNoPermanentOption()
        {
            // Prepare
            const string stdError = "(R)eject or accept (t)emporarily?";

            using( MemoryStream strReader = new MemoryStream( Encoding.ASCII.GetBytes( stdError ) ) )
            {
                using( StreamReader reader = new StreamReader( strReader ) )
                {
                    // Act
                    OutputMessageType type = CommandRunner.ReadAndDisplayInteractiveCommandProcessOutput(
                        reader
                    );

                    // Assert
                    Assert.Equal( OutputMessageType.RequestAcceptCertificateNoPermanentOption, type );
                }
            }
        }
    }
}
