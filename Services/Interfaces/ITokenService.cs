using System.Threading.Tasks;

namespace Student_App.Services.Interfaces
{
    public interface ITokenService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<string> GetAccessTokenAsync();
        void Logout();
    }
} 