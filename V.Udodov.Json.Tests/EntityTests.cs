using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Xunit;

namespace V.Udodov.Json.Tests
{
    public class EntityTests
    {
        private class EntityMock : Entity
        {
            public string Name { get; set; }
        }

        [Fact]
        public void WhenSettingEntityFlexibleDataWithSchemaItShouldSetItIfDataIsValid()
        {
            const string schema = @"{
              'type': 'object',
              'properties': {
                'shoe_size': { 'type': 'number', 'minimum': 5, 'maximum': 12, 'multipleOf': 1.0 }
              }
            }";

            var entityMock = new EntityMock
            {
                ExtensionDataJsonSchema = schema,
                ["shoe_size"] = 10
            };

            entityMock["shoe_size"].Should().Be(10);
        }

        [Fact]
        public void WhenSettingEntityFlexibleDataWithSchemaAndDataIsInvalidItShouldThrow()
        {
            const string schema = @"{
              'type': 'object',
              'properties': {
                'shoe_size': { 'type': 'number', 'minimum': 5, 'maximum': 12, 'multipleOf': 1.0 }
              }
            }";

            var entityMock = new EntityMock
            {
                ExtensionDataJsonSchema = schema
            };
            Action action = () => entityMock["shoe_size"] = 15;

            action
                .Should().Throw<JsonEntityValidationException>()
                .And.Errors
                .Should().HaveCount(1);
        }

        [Fact]
        public void WhenUpdatingFlexibleDataItShouldUpdateWithoutErrors()
        {
            const string schema = @"{
              'type': 'object',
              'properties': {
                'shoe_size': { 'type': 'number', 'minimum': 5, 'maximum': 12, 'multipleOf': 1.0 }
              }
            }";

            var entityMock = new EntityMock
            {
                ExtensionDataJsonSchema = schema,
                ["shoe_size"] = 10
            };

            Action action = () => entityMock["shoe_size"] = 8;

            action.Should().NotThrow();

            entityMock["shoe_size"].Should().Be(8);
        }

        [Fact]
        public void WhenConfiguringAnEntityItShouldThrowIfSchemaRedefinesClassProperties()
        {
            const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type': 'string', 'minLength': 3}
              }
            }";

            Action action = () => { new EntityMock {ExtensionDataJsonSchema = schema}; };

            action
                .Should().Throw<JsonSchemaValidationException>()
                .And.Message.Should().Contain("Collisions: name");
        }

        [Fact]
        public void WhenGettingSchemaItShouldReturnFullSchema()
        {
            const string schema = @"{
              'type': 'object',
              'properties': {
                'shoe_size': { 'type': 'number', 'minimum': 5, 'maximum': 12, 'multipleOf': 1.0 }
              }
            }";

            var entityMock = new EntityMock {ExtensionDataJsonSchema = schema};

            entityMock.JsonSchema.JsonEquals(@"{
                ""$id"": ""V.Udodov.Json.Tests.EntityTests+EntityMock"",
                ""type"": ""object"",
                ""properties"": {
                    ""name"": {
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
                    ""name""
                    ]
                }");
        }

        [Fact]
        public void WhenSettingEntityFlexibleDataWithoutSchemaItShouldSetIt()
        {
            var entityMock = new EntityMock
            {
                ["shoe_size"] = 12,
                ["coffee_preference"] = "cappuccino"
            };

            entityMock["shoe_size"].Should().Be(12);
            entityMock["coffee_preference"].Should().Be("cappuccino");
        }

        [Fact]
        public void WhenGettingEntityAsAStringItShouldSerializeIt()
        {
            var entityMock = new EntityMock
            {
                Name = "Peter Parker",
                ["shoe_size"] = 12,
                ["coffee_preference"] = "cappuccino"
            };

            entityMock.ToString().JsonEquals(@"{
              ""name"": ""Peter Parker"",
              ""shoe_size"": 12,
              ""coffee_preference"": ""cappuccino""
            }");
        }

        [Fact]
        public void WhenTryingToGetWrongFlexibleDataEntityItemThroughIndexerItShouldThrow()
        {
            var entityMock = new EntityMock
            {
                ["shoe_size"] = 12
            };

            Action action = () =>
            {
                var pants = entityMock["pants_size"];
            };

            action.Should().Throw<KeyNotFoundException>();
        }
        
        [Fact]
        public void WhenTryingToGetWrongFlexibleDataEntityItemItShouldThrow()
        {
            var entityMock = new EntityMock
            {
                ["shoe_size"] = 12
            };

            entityMock.TryGetValue("pants_size", out var result)
                .Should().BeFalse();

            result.Should().BeNull();
        }
        
        [Fact]
        public void WhenTryingToGetFlexibleDataEntityItemItShouldThrow()
        {
            var entityMock = new EntityMock
            {
                ["shoe_size"] = 12
            };

            entityMock.TryGetValue("shoe_size", out var result)
                .Should().BeTrue();

            result.Should().Be(12);
        }
        
        [Fact]
        public void WhenSettingEntityFlexibleDataWithSchemaAndAutoValidationIsEnabledAndDataIsInvalidItShouldThrow()
        {
            const string schema = @"{
              'type': 'object',
              'properties': {
                'shoe_size': { 'type': 'number', 'minimum': 5, 'maximum': 12, 'multipleOf': 1.0 }
              }
            }";

            var entityMock = new EntityMock
            {
                AutoValidate = false,
                ExtensionDataJsonSchema = schema
            };
            Action action = () => entityMock["shoe_size"] = 15;
            Action validation = () => entityMock.Validate();

            action.Should().NotThrow<JsonEntityValidationException>();
            validation
                .Should().Throw<JsonEntityValidationException>()
                .And.Errors
                .Should().HaveCount(1);
        }
    }
}