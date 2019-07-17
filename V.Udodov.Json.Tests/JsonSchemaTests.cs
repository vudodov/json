using Xunit;

namespace V.Udodov.Json.Tests
{
    public class JsonSchemaTests
    {
        private class TheMock
        {
            public string MockProp { get; set; }
        }

        [Fact]
        public void WhenGeneratingJsonSchemaFromTypeAndStringItShouldGenerate()
        {
            const string schema = @"{
              'type': 'object',
              'properties': {
                'shoe_size': { 'type': 'number', 'minimum': 5, 'maximum': 12, 'multipleOf': 1.0 }
              }
            }";

            var result = typeof(TheMock).Merge(schema);

            result.JsonEquals(@"{
                ""$id"": ""V.Udodov.Json.Tests.JsonSchemaTests+TheMock"",
                ""type"": ""object"",
                ""properties"": {
                    ""mockProp"": {
                        ""type"": [
                            ""string"",
                            ""null""
                                ]
                    },
                    ""shoe_size"": { 
                        ""type"": ""number"", 
                        ""minimum"": 5.0, 
                        ""maximum"": 12.0, 
                        ""multipleOf"": 1.0 
                    }
                },
                ""required"": [
                    ""mockProp""
                    ]
                }");
        }
        
        [Fact]
        public void WhenGeneratingJsonSchemaFromTypeItShouldGenerate()
        {
            const string schema = null;

            var result = typeof(TheMock).Merge(schema);

            result.JsonEquals(@"{
                ""$id"": ""V.Udodov.Json.Tests.JsonSchemaTests+TheMock"",
                ""type"": ""object"",
                ""properties"": {
                    ""mockProp"": {
                        ""type"": [
                            ""string"",
                            ""null""
                                ]
                    }
                },
                ""required"": [
                    ""mockProp""
                    ]
                }");
        }
    }
}