using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Eidolon.Network
{
    public class Payload<T>
    {
        public Payload(string key, T data)
        {
            SetPayload(key, data);
        }

        public Dictionary<string, T> Data { get; private set; } = new Dictionary<string, T>();

        public void SetPayload(string key, T data) => Data[key] = data;

        public string GetJson()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(Data, settings);
        }
    }
}