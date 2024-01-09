# Mother
🫀 Binary formatter for classes in .NET

## Supported types
- string
- byte
- bool
- decimal
- char
- double
- Half
- Int16
- Int32
- Int64
- Single

## Example
### Input
```CS
using System.Security.Cryptography;
using Mother;

namespace Tests
{
    public class Program
    {
        public class Example
        {
            public int T1 { get; set; }
            public bool T2 { get; set; }
            public string T3 { get; set; }
            public decimal T4 { get; set; }
            public char T5 { get; set; }
            public double T6 { get; set; }
            public Half T7 { get; set; }
            public Int16 T8 { get; set; }
            public Int32 T9 { get; set; }
            public Int64 T10 { get; set; }
            public Single T11 { get; set; }
        }

        public static void Main()
        {
            Mother.Mother mother = new Mother.Mother();
            Example fake = new Example()
            {
                T1 = 1,
                T2 = false,
                T3 = "this is a fake class",
                T4 = 4,
                T5 = 'c',
                T6 = 6d,
                T7 = Half.MaxValue,
                T8 = (Int16)8,
                T9 = (Int32)9,
                T10 = (Int64)10,
                T11 = Single.Epsilon
            };

            List<Mother.Models.PropertyModel> props = mother.Retrive<Example>(fake);

            foreach (Mother.Models.PropertyModel prop in props)
            {
                Console.WriteLine($"{prop.Name}:{prop.Type}:{prop.Value}");
            }

            Write<Example>(mother, "test.bin", fake);
            fake = Read<Example>(mother, "test.bin");

            Console.WriteLine(fake.T1);
            Console.WriteLine(fake.T2);
            Console.WriteLine(fake.T3);
            Console.WriteLine(fake.T4);
            Console.WriteLine(fake.T5);
            Console.WriteLine(fake.T6);
            Console.WriteLine(fake.T7);
            Console.WriteLine(fake.T8);
            Console.WriteLine(fake.T9);
            Console.WriteLine(fake.T10);
            Console.WriteLine(fake.T11);
        }

        private static void Write<T>(Mother.Mother mother, string path, T data)
        {
            mother.Write<T>(data, path);
        }

        private static T Read<T>(Mother.Mother mother, string path) where T : new()
        {
            return (mother.Read<T>(path));
        }
    }
}
```

### Output
```
T1:System.Int32:1
T2:System.Boolean:False
T3:System.String:this is a fake class
T4:System.Decimal:4
T5:System.Char:c
T6:System.Double:6
T7:System.Half:65500
T8:System.Int16:8
T9:System.Int32:9
T10:System.Int64:10
T11:System.Single:1E-45
1
False
this is a fake class
4
c
6
65500
8
9
10
1E-45
```