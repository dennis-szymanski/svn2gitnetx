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
    }
}
