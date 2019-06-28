using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace V.Udodov.Json
{
    public class Entity
    {
        [JsonIgnore] private JSchema _extensionDataJsonSchema;
        [JsonExtensionData] private readonly IDictionary<string, JToken> _data = new Dictionary<string, JToken>();

        /// <summary>
        /// Get Full JSON Schema including class properties and configured flexible data schema.
        /// </summary>
        [JsonIgnore]
        public string JsonSchema
        {
            get
            {
                var schema = new JSchemaGenerator
                {
                    SchemaIdGenerationHandling = SchemaIdGenerationHandling.FullTypeName,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }.Generate(GetType());

                _extensionDataJsonSchema.Properties.ToList().ForEach(schema.Properties.Add);

                return schema.ToString();
            }
        }

        /// <summary>
        /// A setup point for JSON Schema for extension data.
        /// </summary>
        /// <exception cref="JsonSchemaException"></exception>
        [JsonIgnore]
        public string ExtensionDataJsonSchema
        {
            set
            {
                var schema = JSchema.Parse(value);

                var hasCollisions = GetType().GetProperties().Select(p => p.Name.ToLowerInvariant())
                    .Any(propName =>
                        schema.Properties.Select(jp => jp.Key.ToLowerInvariant()).Contains(propName));

                if (hasCollisions)
                {
                    var collisions = GetType().GetProperties().Select(p => p.Name.ToLowerInvariant())
                        .Intersect(schema.Properties.Select(jp => jp.Key.ToLowerInvariant()));

                    throw new JsonSchemaValidationException(
                        "JSON Schema for extension data can't contain declarations for properties which exist in the class declaration. " +
                        $"Collisions: {string.Join(", ", collisions)}");
                }

                _extensionDataJsonSchema = schema;
            }
        }

        /// <summary>
        /// JSON representation of an object
        /// </summary>
        public override string ToString() => JsonConvert.SerializeObject(this,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });

        /// <summary>
        /// Sets and validates extension data property <param name="key" /> against configured JSON Schema.
        /// Or Gets requested property by <param name="key" />
        /// </summary>
        public object this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        private object Get(string key)
        {
            if (TryGetValue(key, out object result)) return result;

            throw new KeyNotFoundException($"Key {key} was not found.");
        }

        private void Set(string key, object value)
        {
            var token = JToken.FromObject(value);

            if (_extensionDataJsonSchema != null)
            {
                var obj = JObject.FromObject(_data);
                obj.Add(key, token);

                if (!obj.IsValid(_extensionDataJsonSchema, out IList<ValidationError> errors))
                    throw new JsonEntityValidationException(
                        $"Validation for value {token} failed against JSON schema {_extensionDataJsonSchema}.",
                        errors);
            }

            _data.Add(key, token);
        }

        public bool TryGetValue(string key, out object value)
        {
            object JTokenToObject(JToken source)
            {
                switch (source.Type)
                {
                    case JTokenType.Object:
                        return ((JObject) source).Properties()
                            .ToDictionary(prop => prop.Name, prop => JTokenToObject(prop.Value));
                    case JTokenType.Array:
                        return source.Values().Select(JTokenToObject).ToList();
                    default:
                        return source.ToObject<object>();
                }
            }

            if (_data.TryGetValue(key, out JToken token))
            {
                value = JTokenToObject(token);
                return true;
            }

            value = null;
            return false;
        }
    }
}