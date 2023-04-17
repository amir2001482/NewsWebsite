using NewsWebsite.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NewsWebsite.XUnitTest.NewsWebsite.Common.Attributes
{
    public class UrlValidateAttributeTest
    {
        private UrlValidateAttribute _urlValidation = new UrlValidateAttribute("/", @"\", " ");
        [Fact]
        public void IsValidTest1()
        {
            Assert.True(_urlValidation.IsValid("خبرورزشی"));
        }

        [Theory]
        [InlineData("خبر ورزشی")]
        [InlineData("خبر/ورزشی")]
        [InlineData(@"خبر\ورزشی")]
        [InlineData("خبرورزشی")]
        public void IsValidTest2(string name)
        {
            Assert.True(_urlValidation.IsValid(name));
        }
    }
}
