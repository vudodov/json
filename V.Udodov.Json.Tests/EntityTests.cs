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
            public EntityMock(JSchema jSchema) : base(jSchema)
            {
            }

            public string Name { get; set; }
        }

        [Fact]
        public void WhenSettingEntityFlexibleDataItShouldSetItIfDataIsValid()
        {
            var schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'role': {'type': 'string', 'enum': ['accountant']}
              }
            }");

            var entityMock = new EntityMock(schema) {["role"] = "accountant"};

            entityMock["role"].Should().Be("accountant");
        }

        [Fact]
        public void WhenSettingEntityFlexibleDataItShouldThrowIfDataIsInvalid()
        {
            var schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'role': {'type': 'string', 'enum': ['accountant']}
              }
            }");

            var entityMock = new EntityMock(schema);
            Action action = () => entityMock["role"] = "not accountant";

            action
                .Should().Throw<JsonValidationException>()
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

            Action action = () => { new EntityMock(schema); };

            action
                .Should().Throw<ArgumentException>()
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

            var entityMock = new EntityMock(schema);

            entityMock.JsonSchema.ToString().Should().Be(JSchema.Parse(@"{
            '$id': 'Pageup.Json.Test.EntityTest+EntityMock',
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
    }
}