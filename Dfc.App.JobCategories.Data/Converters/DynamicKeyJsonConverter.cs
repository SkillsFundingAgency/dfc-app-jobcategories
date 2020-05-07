using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DFC.App.JobCategories.Data.Converters
{
    public class DynamicKeyJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().IsClass;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var listToReturn = new List<Link>();

            JArray array = JArray.Load(reader);

            foreach (JObject content in array.Children<JObject>())
            {
                foreach (JProperty prop in content.Properties())
                {
                    var propAsObj = (JObject)prop.Value;
                    foreach (var subProp in propAsObj.Properties())
                    {
                        listToReturn.Add(new Link() { Value = new KeyValuePair<string, DynamicLink>(prop.Name, new DynamicLink { Href = new Uri(subProp.Value.ToString()) }) });
                    }
                }
            }

            return listToReturn;
        }
    }
}
