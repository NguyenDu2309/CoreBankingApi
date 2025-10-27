using CoreBanking.Infrastructure.Data;

namespace CoreBanking.API.Services;

public class CoreBankingServices(CoreBankingDbContext DbContext , ILogger<CoreBankingServices> logger)
{
    public CoreBankingDbContext DbContext { get; } = DbContext;
    public ILogger<CoreBankingServices> Logger => logger;
}
