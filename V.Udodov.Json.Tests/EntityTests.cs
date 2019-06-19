using System;
using FluentAssertions;
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
            var schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'role': {'type': 'string', 'enum': ['accountant']}
              }
            }");

            var entityMock = new EntityMock
            {
                ExtensionDataJsonSchema = schema,
                ["role"] = "accountant"
            };

            entityMock["role"].Should().Be("accountant");
        }

        [Fact]
        public void WhenSettingEntityFlexibleDataWithSchemaItShouldThrowIfDataIsInvalid()
        {
            var schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'role': {'type': 'string', 'enum': ['accountant']}
              }
            }");

            var entityMock = new EntityMock
            {
                ExtensionDataJsonSchema = schema
            };
            Action action = () => entityMock["role"] = "not accountant";

            action
                .Should().Throw<JsonEntityValidationException>()
                .And.Errors
                .Should().HaveCount(1);
        }

        [Fact]
        public void WhenCreatingAnEntityItShouldThrowIfSchemaContainsClassProperties()
        {
            var schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'name': {'type': 'string', 'minLength': 3}
              }
            }");

            Action action = () => { new EntityMock {ExtensionDataJsonSchema = schema}; };

            action
                .Should().Throw<JsonSchemaValidationException>()
                .And.Message.Should().Contain("Collisions: name");
        }

        [Fact]
        public void WhenGettingSchemaItShouldReturnFullSchema()
        {
            var schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'role': {'type': 'string', 'enum': ['accountant']}
              }
            }");

            var entityMock = new EntityMock {ExtensionDataJsonSchema = schema};

            entityMock.JsonSchema.ToString().Should().Be(JSchema.Parse(@"{
            '$id': 'V.Udodov.Json.Tests.EntityTests+EntityMock',
            'type': 'object',
            'properties': {
                'name': {
                    'type': [
                        'string',
                        'null'
                            ]
                },
                'role': {
                    'type': 'string',
                    'enum': [
                        'accountant'
                        ]
                }
            },
            'required': [
                'name'
                ]
            }").ToString());
        }

        [Fact]
        public void WhenSettingEntityFlexibleDataWithoutSchemaItShouldSetIt()
        {
            var entityMock = new EntityMock
            {
                ["role"] = "accountant"
            };

            entityMock["role"].Should().Be("accountant");
        }
    }
}