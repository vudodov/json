# Flexible objects with JSON and JSON Schema

Flexible Data implementation which will help you extend your strongly typed classes with additional data and validate it against configured JSON Schema.

## Getting Started

Get yourself familiar with JSON Schema before using it. [JSON Schema official documentation](https://json-schema.org/).
This library also heavily use [Newtonsoft](https://newtonsoft.com)

### Usage

```
class FlexibleEntity : V.Udodov.Json.Entity
{
  ...
}

JSchema flexibleJsonSchema = JSchema.Parse(@"{
    'type': 'object',
    'properties': {
      'role': {'type': 'string', 'enum': ['accountant']}
    }
  }");

FlexibleEntity entity = new FlexibleEntity(flexibleJsonSchema);

entity["role"] = "not accountant"; // throws a JsonValidationException
entity["role"] = "accountant";
Console.WriteLine(entity["role"]); // prints "accountant"

Console.WriteLine(entity.JsonSchema); // will return full JSON Schema of an object. Extensible and static parts

```

The Json Schema provided in constructor shold contain only declaration for extension data. 
There should be no collisions in class declaraion and extensible data

```
class FlexibleEntity : V.Udodov.Json.Entity
{
  public string Name { get; set; }
  ...
}

Schema flexibleJsonSchema = JSchema.Parse(@"{
    'type': 'object',
    'properties': {
      'name': {'type': 'string', 'minLength': 3}
    }
  }");

// Code below throws ArgumentException because both class and JSON Schema declarations
// have declaration for name property.
FlexibleEntity entity = new FlexibleEntity(flexibleJsonSchema);

```
