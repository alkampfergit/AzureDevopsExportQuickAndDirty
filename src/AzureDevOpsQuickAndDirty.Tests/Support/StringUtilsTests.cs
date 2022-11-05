using AzureDevopsExportQuickAndDirty.Support;
using Xunit;

namespace AzureDevOpsQuickAndDirty.Tests.Support
{
    public class StringUtilsTests
    {
        [Theory]
        [InlineData("te/st", "test")]
        [InlineData("te\\st", "test")]
        [InlineData("te\\st?file", "testfile")]
        public void Test1(string start, string expected)
        {
            Assert.Equal(expected, start.SanitizeForFileSystem());
        }
    }
}