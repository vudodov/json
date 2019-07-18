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
        public static string MergeLeft(this Type mergeIn, string jsonSchema, Uri id = null)
            => Merge(mergeIn, jsonSchema, id, true);
        
        /// <summary>
        /// Generates JSON Schema from the type and merges it into JSON Schema provided in <paramref name="jsonSchema"/>.
        /// Optionally resulting JSON Schema Id might be defined. If not Id will be generated based from the <paramref name="mergeIn"/> full name.  
        /// </summary>
        public static string MergeRight(this Type mergeIn, string jsonSchema, Uri id = null)
            => Merge(mergeIn, jsonSchema, id, false);
        
        private static string Merge(Type mergeIn, string jsonSchema, Uri id = null, bool left = true)
        {
            var schema = new JSchemaGenerator
            {
                SchemaIdGenerationHandling =
                    id == null ? SchemaIdGenerationHandling.FullTypeName : SchemaIdGenerationHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }.Generate(mergeIn);

            if(!string.IsNullOrWhiteSpace(jsonSchema))
                return left 
                    ? Merge(schema, JSchema.Parse(jsonSchema)).ToString()
                    : Merge(JSchema.Parse(jsonSchema), schema).ToString();

            return schema.ToString();
        }

        
        private static JSchema Merge(JSchema left, JSchema right)
        {
            right.Properties.ToList().ForEach(left.Properties.Add);
            right.ExtensionData.ToList().ForEach(left.ExtensionData.Add);
            right.Required.ToList().ForEach(left.Required.Add);
            left.Id = left.Id ?? right.Id;
            left.SchemaVersion = left.SchemaVersion ?? right.SchemaVersion;

            return left;
        }
    }
}