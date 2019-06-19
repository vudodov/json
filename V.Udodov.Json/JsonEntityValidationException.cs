using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace V.Udodov.Json
{
    /// <summary>
    /// The exception is throw when extension data is invalid against configured JSON Schema
    /// </summary>
    public class JsonEntityValidationException : JsonException
    {
        public JsonEntityValidationException(string message, IEnumerable<ValidationError> errors): base(message)
        {
            Errors = errors;
        }
        
        public IEnumerable<ValidationError> Errors { get; }

        public override string ToString()
        {
            return $"{base.ToString()} \n" +
                   $"{Errors.Select(error => $"{error}; \n")}";
        }
    }
}