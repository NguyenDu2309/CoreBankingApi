
using CoreBanking.API.Models;
using CoreBanking.API.Services;
using CoreBanking.Infrastructure.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreBanking.API.Apis;

public static class CoreBankingApi
{
    public static IEndpointRouteBuilder MapCoreBankingApi(this IEndpointRouteBuilder builder)
    {
        var vApi = builder.NewVersionedApi("CoreBanking");
        var v1 = vApi.MapGroup("api/v{version:apiVersion}/corebanking").HasApiVersion(1, 0);

        v1.MapGet("/customers", GetCustomers);
        v1.MapPost("/customers", CreateCustomer);

        v1.MapGet("/accounts", GetAccounts);
        v1.MapPost("/accounts", CreateAccount);
        v1.MapPut("/accounts/{id:guid}/deposit", Deposit);
        v1.MapPut("/accounts/{id:guid}/withdraw", Withdraw);
        v1.MapPut("/accounts/{id:guid}/transfer", Transfer);


        return builder;
    }

    private static async Task Transfer(Guid id)
    {
        throw new NotImplementedException();
    }

    private static async Task Withdraw(Guid id)
    {
        throw new NotImplementedException();
    }

    private static async Task Deposit(Guid id)
    {
        throw new NotImplementedException();
    }

    private static async Task<Results<Ok<Account>, BadRequest>> CreateAccount(
        [AsParameters] CoreBankingServices services,
        Account account
        )
    {
        if (account.CustomerId == Guid.Empty)
        {
            services.Logger.LogError("Customer ID cannot be empty");
            return TypedResults.BadRequest();
        }

        account.Id = Guid.CreateVersion7();
        account.Balance = 0;
        account.Number = GenerateAccountNumber();

        services.DbContext.Accounts.Add(account);
        await services.DbContext.SaveChangesAsync();

        services.Logger.LogInformation("Account created");

        return TypedResults.Ok(account);
    }

    private static string GenerateAccountNumber()
    {
        return DateTime.UtcNow.Ticks.ToString();
    }

    private static async Task<Ok<PaginationResponse<Account>>> GetAccounts(
        [AsParameters] CoreBankingServices services,
        [AsParameters] PaginationRequest pagination,
        Guid? customerId = null
        )
    {
        IQueryable<Account> accounts = services.DbContext.Accounts;
        if (customerId.HasValue)
        {
            accounts = accounts.Where(c => c.CustomerId == customerId.Value);
        }

        return TypedResults.Ok(new PaginationResponse<Account>(
            pagination.PageIndex,
            pagination.PageSize,
            await accounts.CountAsync(),
            await accounts
            .OrderBy(c => c.Number)
            .Skip(pagination.PageIndex * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync()
        ));
    }

  

    private static async Task<Results<Ok<Customer>, BadRequest>> CreateCustomer(
        [AsParameters] CoreBankingServices services,
        Customer customer
        )
    {
        if (string.IsNullOrEmpty(customer.Name))
        {
            services.Logger.LogError("User name cannot be empty");
            return TypedResults.BadRequest();
        }
        customer.Address ??= "";
        if (customer.Id == Guid.Empty)
        {
            customer.Id = Guid.CreateVersion7();
        }

        services.DbContext.Customers.Add(customer);
        await services.DbContext.SaveChangesAsync();

        services.Logger.LogInformation("Customer created");

        return TypedResults.Ok(customer);
    }

    private static async Task<Ok<PaginationResponse<Customer>>> GetCustomers(
        [AsParameters] CoreBankingServices services,
        [AsParameters] PaginationRequest pagination
        )
    {
        return TypedResults.Ok(new PaginationResponse<Customer>(
            pagination.PageIndex,
            pagination.PageSize,
            await services.DbContext.Customers.LongCountAsync(),
            await services.DbContext.Customers
            .OrderBy(c => c.Name)
            .Skip(pagination.PageIndex * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync()
        ));
    }
}
