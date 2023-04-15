using NewsWebsite.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NewsWebsite.XUnitTest.NewsWebsite.Common
{
    public class StringExtentionsTest
    {
        [Theory]
        [InlineData("Amir" , true , true)]
        [InlineData("Am ir" , true , true)]
        [InlineData("Am ir" , false , true)]
        public void HasValueTest(string value , bool ignoreWhaiteSpace , bool res)
        {
            Assert.Equal(res, StringExtensions.HasValue(value, ignoreWhaiteSpace));
        }
        [Fact]
        public void CombineWithTest()
        {
            string[] testArray = { "Hello" , "World" };
            Assert.Equal("Hello$World", StringExtensions.CombineWith(testArray, '$'));
        }

        [Theory]
        [InlineData("2" , "۲")]
        [InlineData("6" , "۶")]
        public void En2FaTest(string englishNumber , string persioanNumber)
        {
            Assert.Equal(persioanNumber, StringExtensions.En2Fa(englishNumber));
        }
    }
}
