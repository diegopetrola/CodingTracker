
using CodingTracker.Data;
using static CodingTracker.InputValidator;

namespace CodingTracker.Services;

public class CodingSessionService
{
    CodingSessionData data;

    public CodingSessionService(CodingSessionData data)
    {
        this.data = data;
    }
    public async Task InsertSession(CodingSession session)
    {
        ValidateSession(session);
        try
        {
            await data.InsertSession(session);
        }
        catch (Exception e)
        {
            throw new Exception("Database error: ", e);
        }
    }
    public async Task<List<CodingSession>> GetAllSessions()
    {
        return await data.GetAllSessions();
    }
    public async Task<List<CodingSession>> FilterSessionsByDate(DateTime startDate, DateTime endDate, bool orderDesc = true)
    {
        if (startDate > endDate) throw new ArgumentException("End time must be after Start time.");
        return await data.FilterByDate(startDate, endDate, orderDesc);
    }
    public async Task DeleteSession(int ID)
    {
        var s = (await GetSessionByID(ID)) ?? throw new Exception("Session not found, nothing was deleted.");
        try
        {
            await data.DeleteSession(s);
        }
        catch (Exception e)
        {
            throw new Exception($"Database error: {e.Message}");
        }
    }
    public async Task<CodingSession?> GetSessionByID(int ID)
    {
        return await data.GetSessionByID(ID);
    }
    public async Task UpdateSession(CodingSession session)
    {
        ValidateSession(session);
        await data.UpdateSession(session);
    }
    public async Task<List<CodingReport>> GroupByDate(DateTime startTime, DateTime endTime,
        DateGrouping dateGrouping, ReportGrouping reportGrouping)
    {
        return await data.GroupByDate(startTime, endTime, dateGrouping, reportGrouping);
    }
}