using Microsoft.Extensions.DependencyInjection; 
using Microsoft.EntityFrameworkCore; 

namespace coer91.NET
{
    public static class TransactionService
    {
        public static IServiceCollection AddTransactionService<T>(this IServiceCollection service) where T : DbContext
        {
            service.AddTransient<ITransaction<T>, Transaction<T>>();
            return service;
        }
    }
} 