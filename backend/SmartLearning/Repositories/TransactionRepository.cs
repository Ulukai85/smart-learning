using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface ITransactionRepository
{
    Task<XpTransaction> SaveXpTransaction(XpTransaction transaction);
}

public class TransactionRepository(AppDbContext dbContext) : ITransactionRepository
{
    public async Task<XpTransaction> SaveXpTransaction(XpTransaction transaction)
    {
        await dbContext.XpTransactions.AddAsync(transaction);
        await dbContext.SaveChangesAsync();

        return transaction;
    }
}