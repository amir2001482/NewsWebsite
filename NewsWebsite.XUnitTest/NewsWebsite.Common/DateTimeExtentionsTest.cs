using NewsWebsite.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Collections;

namespace NewsWebsite.XUnitTest.NewsWebsite.Common
{
    public class DateTimeExtentionsTest
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void IsLeapYearTest(DateTime date , bool isLeap)
        {
            Assert.Equal(isLeap, DateTimeExtensions.IsLeapYear(date));
        }
        public static IEnumerable<Object[]> TestData()
        {
            yield return new Object[] { new DateTime(2023, 2, 3), false };
            yield return new Object[] { new DateTime(2024, 12, 29), true };
        }
        [Theory]
        [InlineData("1399/12/4" , true)]
        [InlineData("58966/66/55" , false)]
        public void CheckShamsiDateTest1(string shamsi ,bool res)
        {
            Assert.Equal(res, DateTimeExtensions.CheckShamsiDate(shamsi).IsShamsi);
        }
        [Theory]
        [ClassData(typeof(DateTimeResultClassData))]
        public void CheckShamsiDateTest2(DateTimeResult resTest)
        {
            var res = DateTimeExtensions.CheckShamsiDate(resTest.SearchText);
            Assert.Equal(res.IsShamsi, resTest.IsShamsi);
            Assert.Equal(res.MiladiDate, resTest.MiladiDate);
        }
    }
    public class DateTimeResultClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                new DateTimeResult
                {
                   SearchText="1398/10/02",
                   IsShamsi=true,
                   MiladiDate= new DateTime(2019,12,23),
                }
            };

            yield return new object[] {
                 new DateTimeResult
                {
                   SearchText="kfjdj",
                   IsShamsi=false,
                   MiladiDate= null,
                }
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
