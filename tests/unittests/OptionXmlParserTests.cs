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
        public void ParseTest1()
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
        <RemoteGitUrl>ssh://git@github.com/xforever1313/Chaskis.git</RemoteGitUrl>
        <PushWhenDone>false</PushWhenDone>
        <IgnorePaths>
            <IgnorePath>places</IgnorePath>
        </IgnorePaths>
        <FetchTimeout>-1</FetchTimeout>
    </options>
</svn2gitnetx>
";
            // Act
            Options actualOptions = new Options();
            OptionXmlParser.ParseOptionFromString( actualOptions, xml );

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
            Assert.Equal( "ssh://git@github.com/xforever1313/Chaskis.git", actualOptions.RemoteGitUrl );
            Assert.False( actualOptions.PushWhenDone );
            Assert.Equal( new List<string> { "places" }, actualOptions.IgnorePaths );
            Assert.Equal( -1, actualOptions.FetchTimeout );
        }

        [Fact]
        public void ParseTest2()
        {
            // Prepare
            const string xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svn2gitnetx>
    <options>
        <IsVerbose>true</IsVerbose>
        <IncludeMetaData>true</IncludeMetaData>
        <NoMinimizeUrl>true</NoMinimizeUrl>
        <RootIsTrunk>true</RootIsTrunk>
        <SubpathToTrunk>/somewhere/trunk</SubpathToTrunk>
        <NoTrunk>true</NoTrunk>
        <Branches>
            <Branch>branch1</Branch>
            <Branch>branch2</Branch>
        </Branches>
        <NoBranches>true</NoBranches>
        <Tags>
            <Tag>tag1</Tag>
            <Tag>tag2</Tag>
        </Tags>
        <NoTags>true</NoTags>
        <Excludes>
            <Exclude>doc1</Exclude>
            <Exclude>doc2</Exclude>
        </Excludes>
        <Revision>100</Revision>
        <UserName>user1234</UserName>
        <UserNameMethod>env_var</UserNameMethod>
        <Password>password1234</Password>
        <PasswordMethod>env_var</PasswordMethod>
        <AuthorsFile>./authors2.txt</AuthorsFile>
        <BreakLocks>true</BreakLocks>
        <FetchAttempts>3</FetchAttempts>
        <IgnoreGcErrors>true</IgnoreGcErrors>
        <StaleSvnBranchPurgeOption>delete_local</StaleSvnBranchPurgeOption>
        <RemoteGitUrl>ssh://git@github.com/xforever1313/svn2gitnetx.git</RemoteGitUrl>
        <PushWhenDone>true</PushWhenDone>
        <IgnorePaths>
            <IgnorePath>\/doc</IgnorePath>
        </IgnorePaths>
        <FetchTimeout>-10</FetchTimeout>
    </options>
</svn2gitnetx>
";
            // Act
            Options actualOptions = new Options();
            OptionXmlParser.ParseOptionFromString( actualOptions, xml );

            // Assert
            Assert.True( actualOptions.IsVerbose );
            Assert.True( actualOptions.IncludeMetaData );
            Assert.True( actualOptions.NoMinimizeUrl );
            Assert.True( actualOptions.RootIsTrunk );
            Assert.Equal( "/somewhere/trunk", actualOptions.SubpathToTrunk );
            Assert.True( actualOptions.NoTrunk );
            Assert.Equal( new List<string> { "branch1", "branch2" }, actualOptions.Branches );
            Assert.True( actualOptions.NoBranches );
            Assert.Equal( new List<string> { "tag1", "tag2" }, actualOptions.Tags );
            Assert.True( actualOptions.NoTags );
            Assert.Equal( new List<string> { "doc1", "doc2" }, actualOptions.Exclude );
            Assert.Equal( "100", actualOptions.Revision );
            Assert.Equal( "user1234", actualOptions.UserName );
            Assert.Equal( CredentialsMethod.env_var, actualOptions.UserNameMethod );
            Assert.Equal( "password1234", actualOptions.Password );
            Assert.Equal( CredentialsMethod.env_var, actualOptions.PasswordMethod );
            Assert.Equal( "./authors2.txt", actualOptions.Authors );
            Assert.True( actualOptions.BreakLocks );
            Assert.Equal( 3, actualOptions.FetchAttempts );
            Assert.True( actualOptions.IgnoreGcErrors );
            Assert.Equal( StaleSvnBranchPurgeOptions.delete_local, actualOptions.StaleSvnBranchPurgeOption );
            Assert.Equal( "ssh://git@github.com/xforever1313/svn2gitnetx.git", actualOptions.RemoteGitUrl );
            Assert.True( actualOptions.PushWhenDone );
            Assert.Equal( new List<string> { @"\/doc" }, actualOptions.IgnorePaths );
            Assert.Equal( -1, actualOptions.FetchTimeout ); // Should be fixed up to -1.
        }

        [Fact]
        public void DefaultSettingsTest()
        {
            // Prepare
            const string xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svn2gitnetx>
    <options>
    </options>
</svn2gitnetx>
";
            // Act
            Options actualOptions = new Options();
            OptionXmlParser.ParseOptionFromString( actualOptions, xml );

            // Assert
            Assert.False( actualOptions.IsVerbose );
            Assert.False( actualOptions.IncludeMetaData );
            Assert.False( actualOptions.NoMinimizeUrl );
            Assert.False( actualOptions.RootIsTrunk );
            Assert.Null( actualOptions.SubpathToTrunk );
            Assert.False( actualOptions.NoTrunk );
            Assert.Null( actualOptions.Branches );
            Assert.False( actualOptions.NoBranches );
            Assert.Null( actualOptions.Tags );
            Assert.False( actualOptions.NoTags );
            Assert.Null( actualOptions.Exclude );
            Assert.Null( actualOptions.Revision );
            Assert.Null( actualOptions.UserName );
            Assert.Equal( CredentialsMethod.args, actualOptions.UserNameMethod );
            Assert.Null( actualOptions.Password );
            Assert.Equal( CredentialsMethod.args, actualOptions.PasswordMethod );
            Assert.Null( actualOptions.Authors );
            Assert.False( actualOptions.BreakLocks );
            Assert.Equal( 0, actualOptions.FetchAttempts );
            Assert.False( actualOptions.IgnoreGcErrors );
            Assert.Equal( StaleSvnBranchPurgeOptions.nothing, actualOptions.StaleSvnBranchPurgeOption );
            Assert.Null( actualOptions.RemoteGitUrl );
            Assert.False( actualOptions.PushWhenDone );
            Assert.Null( actualOptions.IgnorePaths );
            Assert.Equal( -1, actualOptions.FetchTimeout );
        }

        [Fact]
        public void DefaultSettingsWithEmptyListsTest()
        {
            // Prepare
            const string xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svn2gitnetx>
    <options>
        <Branches>
        </Branches>
        <Tags>
        </Tags>
        <Excludes>
        </Excludes>
        <IgnorePaths>
        </IgnorePaths>
    </options>
</svn2gitnetx>
";
            // Act
            Options actualOptions = new Options();
            OptionXmlParser.ParseOptionFromString( actualOptions, xml );

            // Assert
            Assert.False( actualOptions.IsVerbose );
            Assert.False( actualOptions.IncludeMetaData );
            Assert.False( actualOptions.NoMinimizeUrl );
            Assert.False( actualOptions.RootIsTrunk );
            Assert.Null( actualOptions.SubpathToTrunk );
            Assert.False( actualOptions.NoTrunk );
            Assert.Null( actualOptions.Branches );
            Assert.False( actualOptions.NoBranches );
            Assert.Null( actualOptions.Tags );
            Assert.False( actualOptions.NoTags );
            Assert.Null( actualOptions.Exclude );
            Assert.Null( actualOptions.Revision );
            Assert.Null( actualOptions.UserName );
            Assert.Equal( CredentialsMethod.args, actualOptions.UserNameMethod );
            Assert.Null( actualOptions.Password );
            Assert.Equal( CredentialsMethod.args, actualOptions.PasswordMethod );
            Assert.Null( actualOptions.Authors );
            Assert.False( actualOptions.BreakLocks );
            Assert.Equal( 0, actualOptions.FetchAttempts );
            Assert.False( actualOptions.IgnoreGcErrors );
            Assert.Equal( StaleSvnBranchPurgeOptions.nothing, actualOptions.StaleSvnBranchPurgeOption );
            Assert.Null( actualOptions.RemoteGitUrl );
            Assert.False( actualOptions.PushWhenDone );
            Assert.Null( actualOptions.IgnorePaths );
            Assert.Equal( -1, actualOptions.FetchTimeout );
        }
    }
}
