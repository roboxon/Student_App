using System.Threading.Tasks;

namespace Student_App.Services.Interfaces
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint) where T : class;
        Task<T?> PostAsync<T>(string endpoint, object data) where T : class;
    }
} 