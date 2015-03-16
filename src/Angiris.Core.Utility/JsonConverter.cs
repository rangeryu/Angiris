using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Utility
{
    /// <summary>
    /// http://azure.microsoft.com/blog/2014/11/19/working-with-dates-in-azure-documentdb-4/
    /// </summary>
    public class EpochDateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(DateTime))
            {
                return true;
            }
            else
                return false;
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.None || reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.Integer)
            {
                throw new Exception(
                    String.Format("Unexpected token parsing date. Expected Integer, got {0}.",
                    reader.TokenType));
            }

            int seconds = (int)reader.Value;
            return new DateTime(1970, 1, 1).AddSeconds(seconds);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            int seconds;
            if (value is DateTime)
            {
                DateTime dt = (DateTime)value;
                if (!dt.Equals(DateTime.MinValue))
                    seconds = dt.ToEpoch();
                else
                    seconds = int.MinValue;
            }
            else
            {
                throw new Exception("Expected date object value.");
            }

            writer.WriteValue(seconds);
        }
    }

    public class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Guid))
            {
                return true;
            }
            else
                return false;
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.None || reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.String)
            {
                throw new Exception(
                    String.Format("Unexpected token parsing guid. Expected string, got {0}.",
                    reader.TokenType));
            }

            string guidStr = (string)reader.Value;
            Guid output = new Guid ();
            Guid.TryParse(guidStr, out output);            
            
            return output;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string guidStr = "";
            if (value is Guid)
            {
                guidStr = ((Guid)value).ToString();
            }
            else
            {
                throw new Exception("Expected guid object value.");
            }

            writer.WriteValue(guidStr);
        }
    }
}
