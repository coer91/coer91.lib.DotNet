using Microsoft.EntityFrameworkCore;

namespace coer91.NET.ORM
{
    public static class Procedure
    {
        public static IProcedure Oracle(string connectionString)
            => new ProcedureOracle(connectionString);

        public static IProcedure Oracle(DbContext context)
            => new ProcedureOracle(context);
    }
}