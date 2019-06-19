using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace V.Udodov.Json.Tests
{
    public class ParcelCheckpointTests
    {
        private class ParcelCheckpoint
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string WarehouseName { get; set; }

            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData { get; set; } = new Dictionary<string, JToken>();
        }

        [Fact]
        public void WhenAddingFlexibleDataItShouldSerializesIt()
        {
            var checkpoint = new ParcelCheckpoint
            {
                Id = 1,
                Title = "First Checkpoint",
                WarehouseName = "Main Warehouse",
                AdditionalData = {["Address"] = "1600 Pennsylvania Avenue NW"}
            };

            JsonConvert.SerializeObject(checkpoint).Should()
                .Be(
                    @"{""Id"":1,""Title"":""First Checkpoint"",""WarehouseName"":""Main Warehouse"",""Address"":""1600 Pennsylvania Avenue NW""}");
        }

        [Fact]
        public void WhenAddingFlexibleDataItShouldDeserializesIt()
        {
            var checkpoint = JsonConvert.DeserializeObject<ParcelCheckpoint>(
                @"{""Id"":1,""Title"":""First Checkpoint"",""WarehouseName"":""Main Warehouse"",""Address"":""1600 Pennsylvania Avenue NW""}");

            checkpoint.Should().BeEquivalentTo(new ParcelCheckpoint
            {
                Id = 1,
                Title = "First Checkpoint",
                WarehouseName = "Main Warehouse",
                AdditionalData = {["Address"] = "1600 Pennsylvania Avenue NW"}
            });
        }

        [Fact]
        public void WhenClassIsValidItShouldPassTheValidation()
        {
            var checkpoint = new ParcelCheckpoint
            {
                Id = 1,
                Title = "First Checkpoint",
                WarehouseName = "Main Warehouse",
                AdditionalData =
                {
                    ["location"] = JToken.FromObject(
                        JsonConvert.DeserializeObject(
                            @"{""latitude"": 48.858093,""longitude"": 2.294694}"))
                }
            };

            var schema = JSchema.Parse(@"
        {
            ""$id"": ""https://example.com/location-address-or-coord.schema.json"",
            ""$schema"": ""http://json-schema.org/draft-07/schema#"",
            ""definitions"": {
                ""geo-coordinates"": {
                    ""type"": ""object"",
                    ""title"": ""Longitude and Latitude Values"",
                    ""description"": ""A geographical coordinate."",
                    ""properties"": {
                        ""latitude"": {
                            ""type"": ""number"",
                            ""minimum"": -90,
                            ""maximum"": 90
                        },
                        ""longitude"": {
                            ""type"": ""number"",
                            ""minimum"": -180,
                            ""maximum"": 180
                        }
                    },
                    ""required"": [
                    ""latitude"",
                    ""longitude""
                        ],
                    ""additionalProperties"": false
                },
                ""address"": {
                    ""type"": ""object"",
                    ""title"": ""Address"",
                    ""description"": ""A direct address"",
                    ""properties"": {
                        ""street_address"": {
                            ""type"": ""string""
                        },
                        ""city"": {
                            ""type"": ""string""
                        },
                        ""state"": {
                            ""type"": ""string""
                        }
                    },
                    ""required"": [
                    ""street_address"",
                    ""city"",
                    ""state""
                        ],
                    ""additionalProperties"": false
                }
            },
            ""type"": ""object"",
            ""properties"": {
                ""id"": {""type"": ""number""},
                ""title"": {""type"": ""string""},
                ""warehouseName"": {""type"": ""string""},
                ""location"": {
                    ""oneOf"": [
                    {
                        ""$ref"": ""#/definitions/geo-coordinates""
                    },
                    {
                        ""$ref"": ""#/definitions/address""
                    }
                    ]
                }
            },
            ""additionalProperties"": false
            }");
            var checkpointJsonObject = JObject
                .FromObject(
                    checkpoint,
                    new JsonSerializer {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            checkpointJsonObject.IsValid(schema);
        }
    }
}