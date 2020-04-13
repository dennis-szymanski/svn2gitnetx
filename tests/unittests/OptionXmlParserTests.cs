using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Svn2GitNetX.Tests
{
    public class OptionXmlParserTests
    {
        // ---------------- Tests ----------------

        [Fact]
        public void ParseTest()
        {
            // Prepare
            const string xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svn2gitnetx>
    <options>
        <IsVerbose>false</IsVerbose>
        <IncludeMetaData>false</IncludeMetaData>
        <NoMinimizeUrl>false</NoMinimizeUrl>
        <RootIsTrunk>false</RootIsTrunk>
        <SubpathToTrunk>trunk</SubpathToTrunk>
        <NoTrunk>false</NoTrunk>
        <Branches>
            <Branch>branches</Branch>
        </Branches>
        <NoBranches>false</NoBranches>
        <Tags>
            <Tag>tags</Tag>
        </Tags>
        <NoTags>false</NoTags>
        <Excludes>
            <Exclude>doc</Exclude>
        </Excludes>
        <Revision>50</Revision>
        <UserName>user123</UserName>
        <UserNameMethod>args</UserNameMethod>
        <Password>password123</Password>
        <PasswordMethod>args</PasswordMethod>
        <AuthorsFile>./authors.txt</AuthorsFile>
        <BreakLocks>false</BreakLocks>
        <FetchAttempts>0</FetchAttempts>
        <IgnoreGcErrors>false</IgnoreGcErrors>
        <StaleSvnBranchPurgeOption>nothing</StaleSvnBranchPurgeOption>
    </options>
</svn2gitnetx>
";
            // Act
            Options actualOptions = OptionXmlParser.ParseOptionFromString( xml );

            // Assert
            Assert.False( actualOptions.IsVerbose );
            Assert.False( actualOptions.IncludeMetaData );
            Assert.False( actualOptions.NoMinimizeUrl );
            Assert.False( actualOptions.RootIsTrunk );
            Assert.Equal( "trunk", actualOptions.SubpathToTrunk );
            Assert.False( actualOptions.NoTrunk );
            Assert.Equal( new List<string> { "branches" }, actualOptions.Branches );
            Assert.False( actualOptions.NoBranches );
            Assert.Equal( new List<string> { "tags" }, actualOptions.Tags );
            Assert.False( actualOptions.NoTags );
            Assert.Equal( new List<string> { "doc" }, actualOptions.Exclude );
            Assert.Equal( "50", actualOptions.Revision );
            Assert.Equal( "user123", actualOptions.UserName );
            Assert.Equal( CredentialsMethod.args, actualOptions.UserNameMethod );
            Assert.Equal( "password123", actualOptions.Password );
            Assert.Equal( CredentialsMethod.args, actualOptions.PasswordMethod );
            Assert.Equal( "./authors.txt", actualOptions.Authors );
            Assert.False( actualOptions.BreakLocks );
            Assert.Equal( 0, actualOptions.FetchAttempts );
            Assert.False( actualOptions.IgnoreGcErrors );
            Assert.Equal( StaleSvnBranchPurgeOptions.nothing, actualOptions.StaleSvnBranchPurgeOption );
        }
    }
}
