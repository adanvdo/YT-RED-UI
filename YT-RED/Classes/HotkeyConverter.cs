
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using YTR.Logging;
using DevExpress.Utils;

namespace YTR.Classes
{
    public  class HotkeyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Shortcut));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JToken token = JToken.Load(reader);
                Shortcut? shortcut = EnumExtensions.ToEnum<Shortcut>(token.ToString());
                if (shortcut != null)
                {
                    return shortcut;
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                writer.WriteValue(value.ToString());
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
        }
    }
}
