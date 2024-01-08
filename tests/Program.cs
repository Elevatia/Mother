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
        }

        public static void Main()
        {
            Mother.Mother mother = new Mother.Mother();
            Example fake = new Example()
            {
                T1 = 0,
                T2 = false,
                T3 = "this is a fake class"
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
