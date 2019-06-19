using Newtonsoft.Json;

namespace V.Udodov.Json
{
    public class JsonSchemaValidationException : JsonException
    {
        public JsonSchemaValidationException(string message) : base(message)
        {
        }
    }
}