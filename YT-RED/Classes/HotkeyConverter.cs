using DevExpress.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using YT_RED.Logging;

namespace YT_RED.Classes
{
    public  class HotkeyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(KeyShortcut));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JToken token = JToken.Load(reader);

                string keys = token.ToString();
                if (keys.ToLower() == "(none)")
                    keys = "None";
                KeysConverter keysConverter = new KeysConverter();
                Keys dlKey = (Keys)keysConverter.ConvertFrom(keys);
                KeyShortcut keyShortcut = new KeyShortcut(dlKey);
                return keyShortcut;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
