using CodingTracker.Data;

namespace CodingTracker.Services
{
    public interface ICodingSessionService
    {
        Task DeleteSession(int ID);
        Task<List<CodingSession>> FilterSessionsByDate(DateTime startDate, DateTime endDate, bool orderDesc = true);
        Task<List<CodingSession>> GetAllSessions();
        Task<CodingSession?> GetSessionByID(int ID);
        Task<List<CodingReport>> GroupByDate(DateTime startTime, DateTime endTime, DateGrouping dateGrouping, ReportGrouping reportGrouping);
        Task InsertSession(CodingSession session);
        Task UpdateSession(CodingSession session);
    }
}