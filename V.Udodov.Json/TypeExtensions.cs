using System;
using System.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace V.Udodov.Json
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Generates JSON Schema from the type and merges it with JSON Schema provided in <paramref name="jsonSchema"/>.
        /// Optionally resulting JSON Schema Id might be defined. If not Id will be generated based from the <paramref name="mergeIn"/> full name.  
        /// </summary>
        public static string Merge(this Type mergeIn, string jsonSchema, Uri id = null)
        {
            var schema = new JSchemaGenerator
            {
                SchemaIdGenerationHandling =
                    id == null ? SchemaIdGenerationHandling.FullTypeName : SchemaIdGenerationHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }.Generate(mergeIn);

            if(!string.IsNullOrWhiteSpace(jsonSchema))
                JSchema.Parse(jsonSchema).Properties.ToList().ForEach(schema.Properties.Add);

            return schema.ToString();
        }
    }
}