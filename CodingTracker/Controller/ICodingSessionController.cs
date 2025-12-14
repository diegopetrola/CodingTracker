
namespace CodingTracker.Controller
{
    public interface ICodingSessionController
    {
        Task AddSession();
        Task CodingReport();
        Task DeleteSession();
        Task FilterSessions();
        Task HearAJoke();
        Task LiveSession();
        Task UpdateSession();
    }
}