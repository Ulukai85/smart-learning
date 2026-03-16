using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface ITransactionRepository
{
    Task AddXpTransactionAsync(XpTransaction transaction);
    Task<XpData> GetXpStatistics(string userId);
}

public class TransactionRepository(AppDbContext dbContext) : ITransactionRepository
{
    public async Task AddXpTransactionAsync(XpTransaction transaction)
    {
        await dbContext.XpTransactions.AddAsync(transaction);
    }

    public async Task<XpData> GetXpStatistics(string userId)
    {
        var grouped = dbContext.XpTransactions
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalXp = g.Sum(x => x.Amount)
            });
        
        var currentUserXp = await grouped
            .Where(x => x.UserId == userId)
            .Select(x => x.TotalXp)
            .FirstOrDefaultAsync();
        
        var currentUserRank = await grouped
            .CountAsync(x => x.TotalXp > currentUserXp + 1);
        
        var topFive = await grouped
            .OrderByDescending(x => x.TotalXp)
            .Take(5)
            .Join(dbContext.AppUsers,
                xp => xp.UserId,
                user => user.Id,
                (xp, user) => new LeaderboardEntryData
                {
                    UserId = xp.UserId,
                    Username = user.Handle,
                    TotalXp = xp.TotalXp,
                })
            .ToListAsync();

        return new XpData
        {
            CurrentUserXp = currentUserXp,
            CurrentUserRank = currentUserRank,
            TopUsers = topFive
        };
    }
}