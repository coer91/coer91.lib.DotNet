using Oracle.ManagedDataAccess.Client;
using System.Data; 

namespace coer91.NET.ORM
{
    public interface IProcedure
    {
        public IProcedure Scheme(string scheme);
        public IProcedure Package(string packageName);
        public IProcedure Procedure(string procedureName);
        public IProcedure Input(string parameterName, OracleDbType type, object value = null);  
        public IProcedure Input(string parameterName, SqlDbType type, object value = null);
        public IProcedure Input(string parameterName, string udtTypeName, IEnumerable<string> list); 
        public IProcedure Output(string parameterName, SqlDbType type);
        public IProcedure Output(string parameterName, OracleDbType type);
        public Task<ResponseProcedure> Exec(int timeout = 30);
    }
} 