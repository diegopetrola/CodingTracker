using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CodingTracker.Data;

public class CodingSessionData
{
    private readonly IConfiguration _config;
    private string ConnectionString { get; set; }

    public CodingSessionData()
    {
        _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        ConnectionString = _config["ConnectionString"]
            ?? throw new Exception("Please configure a valid connection string");
    }

    public IDbConnection CreateConnection()
    {
        // For a local database I supposed there is no harm in creating a new
        // connecton for each query, it might even be good to avoid locking the db file.
        return new SqliteConnection(ConnectionString);
    }

    public async Task CreateDatabase(bool seedData = true)
    {
        using IDbConnection db = new SqliteConnection(ConnectionString);
        string createTable = """
                CREATE TABLE IF NOT EXISTS CodingSession(
                    ID INTEGER PRIMARY KEY,
                    StartTime DATETIME NOT NULL,
                    EndTime DATETIME NOT NULL
                )
            """;

        db.Execute(createTable);

        if (seedData)
            await SeedData();
    }

    public async Task SeedData()
    {
        using IDbConnection db = new SqliteConnection(ConnectionString);
        int count = db.QuerySingle<int>("SELECT COUNT (*) FROM CodingSession");

        if (count > 0)
        {
            return;
        }

        var random = new Random();
        var insertSql = @"
            INSERT INTO CodingSession (StartTime, EndTime)
            VALUES (@StartTime, @EndTime)";

        var seedData = new List<CodingSession>();

        for (int i = 0; i < 60; i++)
        {
            var baseDate = DateTime.Now.AddMonths(-1).AddDays(random.Next(0, 30));
            var startTime = baseDate.Date.AddHours(random.Next(6, 23)).AddMinutes(random.Next(0, 60));

            var durationMinutes = random.Next(15, 480);
            var endTime = startTime.AddMinutes(durationMinutes);

            seedData.Add(new CodingSession
            {
                StartTime = startTime,
                EndTime = endTime
            });
        }

        db.Execute(insertSql, seedData);
    }

    public async Task InsertSession(CodingSession session)
    {
        using var conn = CreateConnection();
        var sql = "INSERT INTO CodingSession (StartTime, EndTime) VALUES (@StartTime, @EndTime)";

        await conn.ExecuteAsync(sql, session);
    }
    public async Task<List<CodingSession>> GetAllSessions()
    {
        using var conn = CreateConnection();
        var sql = "SELECT * FROM CodingSession";
        var sessions = conn.Query<CodingSession>(sql).ToList();
        return sessions;
    }
    public async Task<CodingSession?> GetSessionByID(int ID)
    {
        var sql = @"Select * FROM CodingSession WHERE ID=@ID";
        using var conn = CreateConnection();
        var result = await conn.QueryAsync<CodingSession>(sql, new { ID });
        return result.FirstOrDefault();
    }
    public async Task DeleteSession(CodingSession session)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("DELETE FROM CodingSession WHERE ID = @ID", new { ID = session.ID });
    }
    public async Task UpdateSession(CodingSession session)
    {
        using var conn = CreateConnection();
        var sql = @"
            UPDATE CodingSession
            SET StartDate=StartDate, EndDate=@EndDate
            WHERE ID=@ID";
        await conn.ExecuteAsync(sql, session);
    }
    public async Task<List<CodingSession>> FilterByDate(DateTime startDate, DateTime endDate, bool orderDesc = true)
    {
        var orderType = orderDesc ? "DESC" : "ASC";
        using var conn = CreateConnection();
        var sql = @"SELECT * FROM CodingSession 
                    WHERE StartTime>=@startDate AND EndTime<=@endDate
                    ORDER BY EndTime " + orderType;
        var sessions = await conn.QueryAsync<CodingSession>(sql, new { startDate, endDate });
        return sessions.ToList();
    }
    public async Task<List<CodingReport>> GroupByDate(DateTime startTime, DateTime endTime,
        DateGrouping dateGrouping, ReportGrouping reportGrouping)
    {
        string reportLine = reportGrouping == ReportGrouping.Total
            ? "SUM(strftime('%s', EndTime) - strftime('%s', StartTime)) AS TotalSeconds"
            : "AVG(strftime('%s', EndTime) - strftime('%s', StartTime)) AS TotalSeconds";

        string periodLine = dateGrouping == DateGrouping.Week
            ? "strftime('%Y-%W', StartTime) AS Period,"
            : "strftime('%Y-%m', StartTime) AS Period,";

        // there is no possibility of injection here
        var sqlHeader = $"""
            SELECT
            {periodLine}
            {reportLine}
            FROM CodingSession
            """;

        var sql = sqlHeader+@"
            WHERE 
                StartTime >= @startTime AND EndTime <= @endTime
            GROUP BY 
                Period";

        using var conn = CreateConnection();
        var reportData = await conn.QueryAsync<CodingReport>(sql, new { startTime, endTime });
        foreach (var report in reportData)
        {
            Console.WriteLine($"{report.Period} - {report.Duration.TotalHours:N1}");
        }

        return reportData.ToList();
    }
}

public class CodingSession
{
    public int ID { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
}
public class CodingReport
{
    public string Period { get; set; } = "";
    public long TotalSeconds { get; set; }
    public TimeSpan Duration => TimeSpan.FromSeconds(TotalSeconds);
}
