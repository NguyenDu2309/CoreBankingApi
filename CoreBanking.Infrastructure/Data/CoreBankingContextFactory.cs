using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreBanking.Infrastructure.Data;
public class CoreBankingDbContextFactory : IDesignTimeDbContextFactory<CoreBankingDbContext>
{
    public CoreBankingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CoreBankingDbContext>();
        object value = optionsBuilder.UseNpgsql("Host=localhost;Database=CoreBankingDb;Username=postgres;Password=yourpassword");
        return new CoreBankingDbContext(optionsBuilder.Options);
    }
}