using System.Data;

namespace CodingTracker.Data
{
    public interface ICodingSessionData
    {
        IDbConnection CreateConnection();
        Task CreateDatabase(bool seedData = true);
        Task DeleteSession(CodingSession session);
        Task<List<CodingSession>> FilterByDate(DateTime startDate, DateTime endDate, bool orderDesc = true);
        Task<List<CodingSession>> GetAllSessions();
        Task<CodingSession?> GetSessionByID(int ID);
        Task<List<CodingReport>> GroupByDate(DateTime startTime, DateTime endTime, DateGrouping dateGrouping, ReportGrouping reportGrouping);
        Task InsertSession(CodingSession session);
        Task SeedData();
        Task UpdateSession(CodingSession session);
    }
}