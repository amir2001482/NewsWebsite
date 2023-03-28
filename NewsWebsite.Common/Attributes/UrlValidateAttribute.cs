using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace NewsWebsite.Common.Attributes
{
    public class UrlValidateAttribute : ValidationAttribute
    {
        public IEnumerable<string> Characters { get;}
        public UrlValidateAttribute(params string[] characters)
        {
            Characters = new List<string>(characters);
        }

        public override bool IsValid(object value)
        {
            if(value!=null)
                return !Characters.Any(value.ToString().Trim().Contains);
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"{name} نباید شامل کاراکترهای غیر مجاز فضای خالی و ({String.Join(",", Characters)}) باشد.";
        }
    }
}

