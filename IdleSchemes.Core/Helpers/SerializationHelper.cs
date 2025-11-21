using Newtonsoft.Json;

namespace IdleSchemes.Core.Helpers {
    public static class SerializationHelper {

        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public static T Deserialize<T>(string serialized) {
            T? result = JsonConvert.DeserializeObject<T>(serialized, _settings);
            if(result is null) {
                throw new Exception("Failed to deserialize");
            }
            return result;
        }

        public static string Serialize<T>(T obj) {
            return JsonConvert.SerializeObject(obj, typeof(T), _settings);
        }
    
    }
}
