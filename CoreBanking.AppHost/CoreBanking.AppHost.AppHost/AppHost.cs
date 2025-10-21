var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CoreBanking_API>("corebanking-api");

builder.Build().Run();
