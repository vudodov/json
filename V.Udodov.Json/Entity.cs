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
        private JSchema _extensionDataJsonSchema;

        [JsonExtensionData] private readonly IDictionary<string, JToken> _data = new Dictionary<string, JToken>();

        /// <summary>
        /// Get Full JSON Schema including class properties and configured flexible data schema.
        /// </summary>
        [JsonIgnore]
        public JSchema JsonSchema
        {
            get
            {
                var schema = new JSchemaGenerator
                {
                    SchemaIdGenerationHandling = SchemaIdGenerationHandling.FullTypeName,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }.Generate(GetType());

                _extensionDataJsonSchema.Properties.ToList().ForEach(schema.Properties.Add);

                return schema;
            }
        }

        /// <summary>
        /// A setup point for JSON Schema for extension data.
        /// </summary>
        /// <exception cref="JsonSchemaException"></exception>
        [JsonIgnore]
        public JSchema ExtensionDataJsonSchema
        {
            set
            {
                var hasCollisions = GetType().GetProperties().Select(p => p.Name.ToLowerInvariant())
                    .Any(propName =>
                        value.Properties.Select(jp => jp.Key.ToLowerInvariant()).Contains(propName));

                if (hasCollisions)
                {
                    var collisions = GetType().GetProperties().Select(p => p.Name.ToLowerInvariant())
                        .Intersect(value.Properties.Select(jp => jp.Key.ToLowerInvariant()));

                    throw new JsonSchemaException(
                        "JSON Schema for extension data can't contain declarations for properties which exist in the class declaration. " +
                        $"Collisions: {string.Join(", ", collisions)}");
                }

                _extensionDataJsonSchema = value;
            }
        }

        public object this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        private object Get(string key)
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
                return JTokenToObject(token);
            }

            throw new KeyNotFoundException($"Key {key} was not found.");
        }

        private void Set(string key, object value)
        {
            var token = JToken.FromObject(value);

            if (_extensionDataJsonSchema != null)
            {
                var obj = JObject.FromObject(this);
                obj.Add(key, token);

                if (!obj.IsValid(_extensionDataJsonSchema, out IList<ValidationError> errors))
                    throw new JsonEntityValidationException(
                        $"Validation for value {value} failed against JSON schema {_extensionDataJsonSchema}.",
                        errors);
            }

            _data.Add(key, token);
        }
    }
}