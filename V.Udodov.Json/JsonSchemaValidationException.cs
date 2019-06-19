using Newtonsoft.Json;

namespace V.Udodov.Json
{
    /// <summary>
    /// The exception is throw when JSON Schema is invalid.
    /// </summary>
    public class JsonSchemaValidationException : JsonException
    {
        public JsonSchemaValidationException(string message) : base(message)
        {
        }
    }
}