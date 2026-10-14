using Microsoft.EntityFrameworkCore;

namespace GasMonitoringApi.Data;

public class DatabaseInitializer
{
    private readonly AppDbContext db;

    public DatabaseInitializer(AppDbContext db)
    {
        this.db = db;
    }

    public async Task InitializeAsync()
    {
        await db.Database.MigrateAsync();
    }
}
