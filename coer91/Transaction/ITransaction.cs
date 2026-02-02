using Microsoft.EntityFrameworkCore; 

namespace coer91
{
    public interface ITransaction<T> where T : DbContext
    {
        Task BeginTransaction();
        Task CommitTransaction();
        Task RollbackTransaction();
        void ClearTracker();
    }
} 