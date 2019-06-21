# Make your classes Flexible and Extendable with built-in JSON Schema validation protection.

Having a dilemma what to choose *Flexible and Extendable JSON* or *Strongly Typed Class*? Take advantage of both. 
Extendable Data implementation which will help you extend your strongly typed classes with additional data which could be validated against configured JSON Schema.

[![Build Status](https://travis-ci.org/vudodov/json.svg?branch=master)](https://travis-ci.org/vudodov/json) [![Build status](https://ci.appveyor.com/api/projects/status/bi163kqk0vgqd692?svg=true)](https://ci.appveyor.com/project/vudodov/json) ![Nuget](https://img.shields.io/nuget/v/v.udodov.json.svg)


## Getting Started

Get yourself familiar with [JSON Schema](https://json-schema.org/).
Special thanks to [Newtonsoft](https://newtonsoft.com).

### Usage

```c#
class FlexibleEntity : V.Udodov.Json.Entity
{
  ...
}

...

JSchema flexibleJsonSchema = JSchema.Parse(@"{
    'type': 'object',
    'properties': {
      'shoe_size': { 
        'type': 'number', 
        'minimum': 5, 
        'maximum': 12, 
        'multipleOf': 1.0 
      }
    }
  }");

FlexibleEntity entity = new FlexibleEntity 
{ 
    ExtensionDataJsonSchema = flexibleJsonSchema 
};

entity["shoe_size"] = 20; // throws a JsonEntityValidationException
entity["shoe_size"] = 8.5;
Console.WriteLine(entity["shoe_size"]); // prints "8.5"

Console.WriteLine(entity.JsonSchema); // will return full JSON Schema of an object. Extensible and static parts

```

Built in protection prevents JSON Schema taking over control of your class properties definition. 
And will result in exception if JSON Schema will try to override property of the class.
Meaning the Json Schema must contain only declaration for extension data. 
There should be no collisions in class and extensible data declarations.

```c#
class FlexibleEntity : V.Udodov.Json.Entity
{
  public string Name { get; set; }
  ...
}

...

Schema flexibleJsonSchema = JSchema.Parse(@"{
    'type': 'object',
    'properties': {
      'name': {
        'type': 'string', 
        'minLength': 3
      }
    }
  }");

// Code below throws JsonSchemaValidationException because both class and JSON Schema declarations
// have name property.
FlexibleEntity entity = new FlexibleEntity 
{ 
    ExtensionDataJsonSchema = flexibleJsonSchema 
};

```

No schema- no problems. Use your object as extensible flexible entity.
```c#
class FlexibleEntity : V.Udodov.Json.Entity
{
  public string Name { get; set; }
  ...
}

...

FlexibleEntity entity = new FlexibleEntity
{
  entity["pants_size"] = 49;
}
Console.WriteLine(entity["pants_size"]); // prints "49"

entity["middle_name"] = "peanut";
Console.WriteLine(entity["middle_name"]); // prints "peanut"
```

Easily get JSON string from your class instance
```c#
class FlexibleEntity : V.Udodov.Json.Entity
{
  public string Name { get; set; }
  ...
}

...

FlexibleEntity entity = new FlexibleEntity
{
  Name = "Peter Parker",
  ["pants_size"] = 49
};

Console.WriteLine(entity);
/*
prints

{
  name: "Peter Parker",
  pants_size: 49
}
*/

```