using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore; 

namespace coer91
{
    public class Transaction<T>(T context) : ITransaction<T> where T : DbContext
    {
        private readonly T _context = context;
        private IDbContextTransaction _transaction;

        public bool HasTransaction => _transaction is not null; 


        public async Task BeginTransaction()
            => _transaction ??= await _context.Database.BeginTransactionAsync(); 


        public async Task CommitTransaction()
        {
            if (_transaction is not null) 
            { 
                await _transaction.CommitAsync();
                _transaction = null;
            }
        }


        public async Task RollbackTransaction()
        {
            if(_transaction is not null)
            {
                await _transaction.RollbackAsync();
                _transaction = null;
            }
        }


        public void ClearTracker()
           => _context.ChangeTracker.Clear();
    } 
}