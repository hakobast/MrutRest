using Newtonsoft.Json;

namespace Mrut.Converter{
    public class NewtonsoftJsonConverter : AbstractObjectConverter{

        private readonly JsonSerializerSettings settings;

        public NewtonsoftJsonConverter() {
            settings = new JsonSerializerSettings();
        }

        public NewtonsoftJsonConverter(JsonSerializerSettings settings) {
            this.settings = settings;
        }

        public override T ToObject<T>(string data) {
            return JsonConvert.DeserializeObject<T>(data, settings);
        }

        public override string ToString(object obj) {
            return JsonConvert.SerializeObject(obj, Formatting.None, settings);
        }
    }
}