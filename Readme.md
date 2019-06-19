# Make your classes Flexible and Extendable with built-in JSON Schema validation protection.

Having a dilemma what to choose *Flexible and Extendable JSON* or *Strongly Typed Class*? Take advantage of both. 
Extendable Data implementation which will help you extend your strongly typed classes with additional data which could be validated against configured JSON Schema.

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

FlexibleEntity entity = new FlexibleEntity { ExtensionDataJsonSchema = flexibleJsonSchema };

entity["role"] = "not accountant"; // throws a JsonValidationException
entity["role"] = "accountant";
Console.WriteLine(entity["role"]); // prints "accountant"

Console.WriteLine(entity.JsonSchema); // will return full JSON Schema of an object. Extensible and static parts

```
Built in protection prevents JSON Schema taking over control of your class properties definition. 
And will result in exception if JSON Schema will try to override property of the class.
Meaning the Json Schema must contain only declaration for extension data. 
There should be no collisions in class and extensible data declarations.

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
FlexibleEntity entity = new FlexibleEntity{ ExtensionDataJsonSchema = flexibleJsonSchema };

```
