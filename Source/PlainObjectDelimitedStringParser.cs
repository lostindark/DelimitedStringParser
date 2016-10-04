
namespace DelimitedStringParser
{
    /// <summary>
    /// A parser to parse delimited string into a plain C# object.
    /// </summary>
    /// <typeparam name="T">The target object type.</typeparam>
    /// <example>
    /// Below C# class definition is used to parse delimited string "Field0:Field1:Field2".
    /// <code>
    /// [Delimiter(":")]
    /// public class MyClass1
    /// {
    ///     [FieldId(0)]
    ///     public int32 Field0 { get; set; }
    ///     [FieldId(1)]
    ///     public string Field1 { get; set; }
    ///     [FieldId(2)]
    ///     public bool Field2 { get; set; }
    /// }
    /// </code>
    /// To use the parser to parse the string "3:abc:false" into the C# object.
    /// <code>
    /// MyClass1 myClass1 = PlainObjectDelimitedStringParser&lt;MyClass1&gt;.Parse("3:abc:false");
    /// </code>
    /// Below C# class definition is used to parse a versioned data. V1|Field0|Field1, V2|Field1|Field2.
    /// <code>
    /// [IsVersionedData(true)]
    /// [Delimiter("|")]
    /// public class MyClass2
    /// {
    ///     [FieldId(0)]
    ///     public int Field0 { get; set; }
    ///     [FieldId(1, 1)]
    ///     public string Field1 { get; set; }
    ///     [FieldId(1, 2)]
    ///     [FieldId(2, 1)]
    ///     public bool Field2 { get; set; }
    /// }
    /// </code>
    /// </example>
    public class PlainObjectDelimitedStringParser<T> : DelimitedStringParser<T, PlainObjectMetadataReader<T>>
        where T : new()
    {
    }
}
