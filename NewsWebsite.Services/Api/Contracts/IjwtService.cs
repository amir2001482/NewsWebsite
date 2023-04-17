using NewsWebsite.Entities.identity;
using System.Threading.Tasks;

namespace NewsWebsite.Services.Api.Contract
{
    public interface IjwtService
    {
        Task<string> GenerateTokenAsync(User User);
    }
}
