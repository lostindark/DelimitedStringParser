
namespace DelimitedStringParser.Bond
{
    /// <summary>
    /// A parser to parse delimited string into a Bond object.
    /// </summary>
    /// <typeparam name="T">The target object type.</typeparam>
    /// <example>
    /// Below Bond struct definition is used to parse delimited string "Field0:Field1:Field2".
    /// <code>
    /// [Delimiter(":")]
    /// struct MyStruct1
    /// {
    ///     0: int32 Field0;
    ///     1: string Field1;
    ///     2: bool Field2;
    /// }
    /// </code>
    /// To use the parser to parse the string "3:abc:false" into the Bond struct.
    /// <code>
    /// MyStruct1 mystruct1 = DelimitedStringParser&lt;MyStruct1&gt;.Parse("3:abc:false");
    /// </code>
    /// Below Bond struct definition is used to parse a versioned data. V1|Field0|Field1, V2|Field1|Field2.
    /// Note: Field0 doesn't need FieldId attribute (default to Bond field id).
    /// <code>
    /// [IsVersionedData("true")]
    /// [Delimiter("|")]
    /// struct MyStruct2
    /// {
    ///     0: int32 Field0;
    ///     [FieldId("v1=1")]
    ///     1: string Field1;
    ///     [FieldId("v1=2;v2=1")]
    ///     2: bool Field2;
    /// }
    /// </code>
    /// </example>
    public class BondDelimitedStringParser<T> : DelimitedStringParser<T, BondMetadataReader<T>>
        where T : new()
    {
    }
}
