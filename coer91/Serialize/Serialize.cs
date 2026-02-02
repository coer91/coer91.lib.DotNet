using System.Data;
using Newtonsoft.Json;
using System.Xml;

namespace coer91
{
    public static class Serialize
    {

        #region ToJson

        public static string ToJson<T>(T obj, bool nesting = false)
        {
            if (!nesting)
                obj = Clean.NoNesting(obj);

            return ToJson([obj], nesting);
        }


        public static string ToJson<T>(List<T> objectList, bool nesting = false)
        {
            if (!nesting)
                objectList = Clean.NoNesting(objectList);

            return JsonConvert.SerializeObject(objectList);
        }


        public static string ToJson<K, V>(Dictionary<K, V> dictionary)
            => JsonConvert.SerializeObject(dictionary); 


        public static string ToJson(string xml)
        {
            XmlDocument doc = new();
            doc.LoadXml(xml);
            string json = JsonConvert.SerializeXmlNode(doc);

            if (json.StartsWith("{\"Document\":{\"Nodo\":") && json.EndsWith("}}"))
            {
                json = json[20..];
                json = json[..^2];
            }

            return json;
        }

        #endregion


        #region ToXML

        public static string ToXML<T>(T obj, bool nesting = false)
        {
            if (!nesting)
                obj = Clean.NoNesting(obj);

            return ToXML([obj], nesting);
        }


        public static string ToXML<T>(List<T> objectList, bool nesting = false)
        {
            if (!nesting)
                objectList = Clean.NoNesting(objectList);

            return ToXML(JsonConvert.SerializeObject(objectList), nesting);
        }


        public static string ToXML(string json, bool nesting = false)
        {
            object obj = JsonConvert.DeserializeObject(json);

            if (Validations.IsCollection(obj))
            {
                if (json.StartsWith("[[")) json = json[1..];
                if (json.EndsWith("]]")) json = json[..^1];
                return ToXMLDocument(json);
            }

            json = ToJson(obj, nesting);
            obj = JsonConvert.DeserializeObject(json);
            return ToXML(obj, false);
        }


        private static string ToXMLDocument(string json)
        {
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(json);
            dataTable.TableName = "Nodo";

            DataSet dataSet = new("Document");
            dataSet.Tables.Add(dataTable);

            StringWriter stringWriter = new();
            dataSet.WriteXml(stringWriter);
            return stringWriter.ToString();
        }
        #endregion 
    }
}