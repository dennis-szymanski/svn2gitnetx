using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Svn2GitNetX.Tests
{
    public class IgnorePathBuilderTests
    {
        [Fact]
        public void NullExcludesTest()
        {
            // Prepare
            Options options = new Options
            {
                Exclude = null
            };

            // Act
            string regexStr = IgnorePathBuilder.BuildIgnorePathRegex(
                options,
                new List<string>(),
                new List<string>()
            );

            // Asset
            Assert.Null( regexStr );
        }

        [Fact]
        public void EmptyExcludesTest()
        {
            // Prepare
            Options options = new Options
            {
                Exclude = new List<string>()
            };

            // Act
            string regexStr = IgnorePathBuilder.BuildIgnorePathRegex(
                options,
                new List<string>(),
                new List<string>()
            );

            // Asset
            Assert.Null( regexStr );
        }

        [Fact]
        public void OnlyIgnoresTest()
        {
            // Prepare
            Options options = new Options
            {
                IgnorePaths = new List<string>
                {
                    @"\/doc",
                    @"\/somewhere"
                }
            };

            // Act
            string regexStr = IgnorePathBuilder.BuildIgnorePathRegex(
                options,
                new List<string>(),
                new List<string>()
            );

            // Assert
            Assert.Equal(
                @"^(?:\/doc|\/somewhere)",
                regexStr
            );
        }

        [Fact]
        public void ExcludeAndIgnoreTest()
        {
            // Prepare
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
                },
                IgnorePaths = new List<string>
                {
                    @"\/doc",
                    @"\/somewhere"
                }
            };

            // Act
            string regexStr = IgnorePathBuilder.BuildIgnorePathRegex(
                options,
                new List<string>(),
                options.Tags
            );

            // Assert
            Assert.Equal(
                @"^(?:(\/doc|\/somewhere)|((subpath[\/]|tag1[\/][^\/]+[\/]|tag2[\/][^\/]+[\/])(?:ex1|ex2)))",
                regexStr
            );
        }
    }
}
