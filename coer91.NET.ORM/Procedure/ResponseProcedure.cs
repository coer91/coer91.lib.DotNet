using Newtonsoft.Json;

namespace coer91.NET.ORM
{
    public class ResponseProcedure : ResponseProcedureBuilder
    {
        private int _index = 1;
        private readonly Dictionary<string, string> _outputs = [];
        private readonly Dictionary<int, List<Dictionary<string, string>>> _tables = [];


        public ResponseProcedure(bool failure, string message, Dictionary<int, List<Dictionary<string, string>>> tables = null, Dictionary<string, string> outputs = null)
        {
            Failure = failure;
            MessageList = message.Length > 2000 ? [$"{message[..2000]}..."] : [message];
            if (failure) HttpCode = 500;

            if (tables is not null)
                _tables = tables;

            if (outputs is not null)
                _outputs = outputs;
        }


        public ResponseProcedure(Dictionary<int, List<Dictionary<string, string>>> tables = null, Dictionary<string, string> outputs = null)
        {
            if (tables is not null)
                _tables = tables;

            if (outputs is not null)
                _outputs = outputs;
        }


        /// <summary>
        /// Return the next table
        /// </summary> 
        public List<T> GetTable<T>()
        {
            if (_tables.Count <= 0 || !_tables.ContainsKey(_index))
                return [];

            string json = JsonConvert.SerializeObject(_tables[_index++]);
            IEnumerable<T> data = JsonConvert.DeserializeObject<IEnumerable<T>>(json) ?? [];
            return [.. data];
        }


        /// <summary>
        /// Return the table by index
        /// </summary> 
        public List<T> GetTable<T>(int index)
        {
            if (_tables.Count <= 0 || !_tables.ContainsKey(index))
                return [];

            string json = JsonConvert.SerializeObject(_tables[index]);
            IEnumerable<T> data = JsonConvert.DeserializeObject<IEnumerable<T>>(json) ?? [];
            return [.. data];
        }


        /// <summary>
        /// Returns output not tables
        /// </summary> 
        public Dictionary<string, string> GetOutputs() => _outputs;


        /// <summary>
        /// Return output
        /// </summary> 
        public string GetOutput(string outputName) => _outputs.GetValueOrDefault(outputName, string.Empty);
    }
}
