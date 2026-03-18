using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface ITransactionRepository
{
    Task AddXpTransactionAsync(XpTransaction transaction);
    Task<XpData> GetXpStatistics(string userId);
}