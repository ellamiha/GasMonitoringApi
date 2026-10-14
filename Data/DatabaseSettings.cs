using Microsoft.Data.Sqlite;

namespace GasMonitoringApi.Data;

public static class DatabaseSettings
{
    public static string GetConnectionString(IConfiguration configuration, string contentRoot)
    {
        var connectionString = configuration.GetConnectionString("AppDbContext")
            ?? throw new InvalidOperationException("Не задана строка подключения AppDbContext.");
        var builder = new SqliteConnectionStringBuilder(connectionString);
        builder.DataSource = Path.GetFullPath(builder.DataSource, contentRoot);
        builder.ForeignKeys = true;
        Directory.CreateDirectory(Path.GetDirectoryName(builder.DataSource)!);
        return builder.ToString();
    }
}
