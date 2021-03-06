﻿using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace DFC.App.JobCategories.Data.Converters
{
    [ExcludeFromCodeCoverage]
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

                    var relationship = propAsObj.Properties().FirstOrDefault(x => x.Name.ToLower() == "relationship");
                    var href = propAsObj.Properties().FirstOrDefault(x => x.Name.ToLower() == "href");

                    listToReturn.Add(new Link() { LinkValue = new KeyValuePair<string, DynamicLink>(prop.Name, new DynamicLink { Href = new Uri(href.Value.ToString()), Relationship = relationship.Value.ToString() }) });
                }
            }

            return listToReturn;
        }
    }
}
