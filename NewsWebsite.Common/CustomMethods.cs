using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Common
{
    public static class CustomMethods
    {
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}
