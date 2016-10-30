# DelimitedStringParser

## Overview
An utility class that converts a delimited string into C# objects. It parser supports all simple types, nullable, and list/set. Map is not supported yet.
It requires custom attributes on the class definition.

## Why reinventing the wheels?
There are nice libaries for reading CSV files (E.g. [A Fast CSV Reader](http://www.codeproject.com/Articles/9258/A-Fast-CSV-Reader), [File Helpers](http://www.filehelpers.net/)). If your purpose is to read/write CSV like files, please go with those libraries.
This library is created to support the following special requirements:
* You can only access one record as string each time, not entire file.
* Need to support versioned data. See example below.
* Support multi level of objects.

## Examples:
Below C# class definition is used to parse delimited string "Field0:Field1:Field2".
```C#
[Delimiter(':')]
public class MyClass1
{
    [FieldId(0)]
    public int32 Field0 { get; set; }
    [FieldId(1)]
    public string Field1 { get; set; }
    [FieldId(2)]
    public bool Field2 { get; set; }
}
```

To use the parser to parse the string `"3:abc:false"` into the C# object.
```C#
MyClass1 myClass1 = DelimitedStringParser<MyClass1>.Parse("3:abc:false");
```

Below C# class definition is used to parse a versioned data. V1|Field0|Field1, V2|Field1|Field2.
```C#
[IsVersionedData(true)]
[Delimiter('|')]
public class MyClass2
{
    [FieldId(0)]
    public int Field0 { get; set; }
    [FieldId(1, 1)]
    public string Field1 { get; set; }
    [FieldId(1, 2)]
    [FieldId(2, 1)]
    public bool Field2 { get; set; }
}
```
