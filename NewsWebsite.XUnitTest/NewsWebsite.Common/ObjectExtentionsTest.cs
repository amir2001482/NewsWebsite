using NewsWebsite.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NewsWebsite.XUnitTest.NewsWebsite.Common
{
    public class ObjectExtentionsTest
    {
        [Fact]
        public void CheckArgumentIsNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => ObjectExtensions.CheckArgumentIsNull(null, "test"));
        }
    }
}
