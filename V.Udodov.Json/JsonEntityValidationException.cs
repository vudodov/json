using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace V.Udodov.Json
{
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