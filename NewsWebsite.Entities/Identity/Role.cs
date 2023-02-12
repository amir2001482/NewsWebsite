using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Entities.Identity
{
    public class Role : IdentityRole<string>
    {
        public Role()
        {

        }

        public Role(string name) : base(name)
        {

        }

        public Role(string name, string description) : base(name)
        {
            Description = description;
        }

        public string Description { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<RoleClaim> Claims { get; set; }
    }
}
