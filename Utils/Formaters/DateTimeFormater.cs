using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Utils.StringExtensions;

namespace Utils.Formaters
{
    /// <summary>
    ///  返回前端的时候，格式化时间
    /// </summary>
    public class DateTimeFormater : JsonConverter
    {
        private readonly string _dateFormat;

        public DateTimeFormater(string dateFormat)
        {
            _dateFormat = dateFormat ?? throw new ArgumentNullException(nameof(dateFormat));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime?) || objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var dateString = reader.Value.ToString();
            if (dateString.IsNullOrEmpty())
            {
                return null;
            }
            if (DateTime.TryParseExact(dateString, _dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            return reader.Value; // 如果解析失败，返回 null
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dateTime)
            {
                writer.WriteValue(dateTime.ToString(_dateFormat, CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
