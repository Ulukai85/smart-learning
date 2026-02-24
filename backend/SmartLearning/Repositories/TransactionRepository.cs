using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface ITransactionRepository
{
    Task AddXpTransactionAsync(XpTransaction transaction);
}

public class TransactionRepository(AppDbContext dbContext) : ITransactionRepository
{
    public async Task AddXpTransactionAsync(XpTransaction transaction)
    {
        await dbContext.XpTransactions.AddAsync(transaction);
    }
}