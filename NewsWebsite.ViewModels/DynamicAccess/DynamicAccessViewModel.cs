using NewsWebsite.Entities.identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.ViewModels.DynamicAccess
{
    public class DynamicAccessIndexViewModel
    {
        public string ActionIds { set; get; }
        public int UserId { set; get; }
        public User UserIncludeUserClaims { set; get; }
        public ICollection<ControllerViewModel> SecuredControllerActions { set; get; }
    }
}
