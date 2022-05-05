using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using DevExpress.Utils;

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
            JToken token = JToken.Load(reader);

            string keys = token.ToString();
            KeysConverter keysConverter = new KeysConverter();
            Keys dlKey = (Keys)keysConverter.ConvertFrom(keys);
            KeyShortcut keyShortcut = new KeyShortcut(dlKey);
            return keyShortcut;
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
