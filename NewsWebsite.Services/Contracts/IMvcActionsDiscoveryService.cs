using NewsWebsite.ViewModels.DynamicAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.Services.Contracts
{
    public interface IMvcActionsDiscoveryService
    {
        ICollection<ControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName);
    }
}
