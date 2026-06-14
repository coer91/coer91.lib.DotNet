using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace coer91.NET.ORM
{
    public class ProcedureOracle : IProcedure
    {
        private OracleCommand _command;
        private OracleConnection _connection;
        private List<OracleParameter> _parameterOutputList = [];
        private Dictionary<string, dynamic[]> _parameterInputList = [];
        private Dictionary<int, List<Dictionary<string, string>>> _tables = [];
        private Dictionary<string, string> _outputs = [];
        private bool _useEnumerableInputs = false;
        private string _scheme = "";
        private string _package = "";
        private string _procedure = "";


        public ProcedureOracle(string connectionString)
        {
            _connection = new OracleConnection(connectionString);
            _command = _connection.CreateCommand();
        }


        public ProcedureOracle(DbContext context)
        {
            _connection = new OracleConnection(context.Database.GetDbConnection().ConnectionString);
            _command = _connection.CreateCommand();
        }


        ~ProcedureOracle()
            => Dispose();


        private void Dispose()
        {
            if (_command != null)
            {
                _command.Cancel();
                _command.Dispose();
                _command = null;
            }

            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open) _connection?.Close();
                _connection.Dispose();
                _connection = null;
            }

            _parameterInputList.Clear();
        }


        public IProcedure Scheme(string schemeName)
        {
            _scheme = schemeName;
            return this;
        }


        public IProcedure Package(string packageName)
        {
            _package = packageName;
            return this;
        }


        public IProcedure Procedure(string procedureName)
        {
            _procedure = procedureName;
            return this;
        }
        

        public IProcedure Input(string parameterName, SqlDbType type, object value = null)
            => Input(parameterName, (OracleDbType)type, value);


        public IProcedure Input(string parameterName, OracleDbType type, object value = null)
        {
            OracleParameter parameter = _command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            parameter.Direction = ParameterDirection.Input;
            parameter.OracleDbType = type;

            _parameterInputList.Add(parameterName, [parameter, null]);
            return this;
        }


        public IProcedure Input(string parameterName, string typeName, IEnumerable<string> list)
        {
            Dictionary<string, IEnumerable<string>> dictionary = new() { { typeName, list } };
            _parameterInputList.Add(parameterName, [null, dictionary]);
            _useEnumerableInputs = true;
            return this;
        }


        public IProcedure Output(string parameterName, SqlDbType type)
            => Output(parameterName, (OracleDbType)type);


        public IProcedure Output(string parameterName, OracleDbType type)
        {
            OracleParameter parameter = _command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = DBNull.Value;
            parameter.Direction = ParameterDirection.Output;
            parameter.OracleDbType = type;

            if (type == OracleDbType.Varchar2 || type == OracleDbType.NVarchar2)
                parameter.Size = 5000;

            _parameterOutputList.Add(parameter);
            return this;
        }


        private async Task AddDictionary(OracleDataReader reader)
        {
            var columns = GetColumns(reader);
            var rows = new List<Dictionary<string, string>>();

            while (await reader.ReadAsync())
            {
                var dataRow = new Dictionary<string, string>();

                foreach (string column in columns)
                    dataRow.Add(column, await reader.IsDBNullAsync(columns.IndexOf(column)) ? null : reader.GetValue(columns.IndexOf(column)).ToString());

                rows.Add(dataRow);
            }

            _tables.Add(_tables.Count + 1, rows);
        }


        private static List<string> GetColumns(OracleDataReader reader)
        {
            string column;
            List<string> columns = [];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                column = reader.GetName(i);

                if (columns.Exists(x => x.Equals(column, StringComparison.OrdinalIgnoreCase)))
                {
                    int counter = 0;

                    do
                    {
                        column += $"_{++counter}";
                        if (!columns.Exists(x => x.Equals(column, StringComparison.OrdinalIgnoreCase))) break;
                    } while (counter <= 10);
                }

                columns.Add(column);
            }

            return columns;
        }


        public async Task<ResponseProcedure> Exec(int timeout = 30)
        {
            bool Failure = false;
            string Message = "Success";

            try
            {
                if (string.IsNullOrWhiteSpace(_procedure))
                    _command.CommandText = string.Empty;

                else
                {
                    _command.CommandText += !string.IsNullOrWhiteSpace(_scheme)  ? $"{_scheme}."  : string.Empty;
                    _command.CommandText += !string.IsNullOrWhiteSpace(_package) ? $"{_package}." : string.Empty;
                    _command.CommandText += $"{_procedure}";
                }

                if (string.IsNullOrWhiteSpace(_command.CommandText))
                    return new ResponseProcedure(true, "Procedure name cannot be null or whitespace.");

                _command.CommandTimeout = timeout;
                using OracleDataReader reader = _useEnumerableInputs ? await ExecText() : await ExecStoredProcedure();

                await AddDictionary(reader);
                while (await reader.NextResultAsync())
                    await AddDictionary(reader);

                await reader.CloseAsync();
                await reader.DisposeAsync();

                foreach (OracleParameter parameter in _parameterOutputList.Where(x => x.OracleDbType != OracleDbType.RefCursor))
                {
                    string ParameterValue = parameter.Value is not null ? parameter.Value?.ToString() : string.Empty;
                    if (ParameterValue.Equals("NULL", StringComparison.OrdinalIgnoreCase)) ParameterValue = string.Empty;
                    _outputs.Add(parameter.ParameterName, ParameterValue);
                }
            }

            catch (Exception ex)
            {
                Failure = true;
                Message = ex.InnerException?.Message ?? ex.Message;
            }

            finally
            {
                Dispose();
            }

            return new ResponseProcedure(Failure, Message, _tables, _outputs);
        }


        private async Task<OracleDataReader> ExecText()
        {
            _command.CommandType = CommandType.Text;
            _command.CommandText = $"BEGIN {_command.CommandText}(";

            foreach (var input in _parameterInputList)
            {
                var parameterName = input.Key;
                var parameter = (OracleParameter)input.Value[0];
                var dictionary = (Dictionary<string, IEnumerable<string>>)input.Value[1];

                _command.CommandText += $"{parameterName} => ";
                if (parameter is not null) _command.CommandText += $"'{parameter.Value}', ";
                else if (dictionary is not null) _command.CommandText += $"{dictionary.First().Key}({string.Join(", ", dictionary.First().Value.Select(value => $"'{value}'"))}), ";
            }

            foreach (var output in _parameterOutputList)
                _command.CommandText += $"{output.ParameterName} => :{output.ParameterName}, ";

            _command.CommandText += $"); END;";
            _command.CommandText = _command.CommandText.Replace(", );", ");");

            foreach (OracleParameter parameter in _parameterOutputList)
                _command.Parameters.Add(parameter);

            await _connection.OpenAsync();
            return await _command.ExecuteReaderAsync();
        }


        private async Task<OracleDataReader> ExecStoredProcedure()
        {
            _command.CommandType = CommandType.StoredProcedure;

            foreach (OracleParameter parameter in _parameterInputList.Where(x => x.Value[0] is not null).Select(x => (OracleParameter)x.Value[0]))
                _command.Parameters.Add(parameter);

            foreach (OracleParameter parameter in _parameterOutputList)
                _command.Parameters.Add(parameter);

            await _connection.OpenAsync();
            return await _command.ExecuteReaderAsync();
        }
    }
}