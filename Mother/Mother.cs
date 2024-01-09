using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mother.Models;

namespace Mother
{
    public class Mother
    {
        private readonly Dictionary<Type, Func<BinaryReader, object>> readMethods;
        private readonly Dictionary<Type, Action<BinaryWriter, object>> writeMethods;

        public Mother()
        {
            readMethods = new Dictionary<Type, Func<BinaryReader, object>>
            {
                { typeof(string), reader => reader.ReadString() },
                { typeof(byte), reader => reader.ReadByte() },
                { typeof(bool), reader => reader.ReadBoolean() },
                { typeof(decimal), reader => reader.ReadDecimal() },
                { typeof(char), reader => reader.ReadChar() },
                { typeof(double), reader => reader.ReadDouble() },
                { typeof(Half), reader => reader.ReadHalf() },
                { typeof(Int16), reader => reader.ReadInt16() },
                { typeof(Int32), reader => reader.ReadInt32() },
                { typeof(Int64), reader => reader.ReadInt64() },
                { typeof(Single), reader => reader.ReadSingle() },
            };

            writeMethods = new Dictionary<Type, Action<BinaryWriter, object>>
            {
                { typeof(string), (writer, value) => writer.Write((string)value) },
                { typeof(byte), (writer, value) => writer.Write((byte)value) },
                { typeof(bool), (writer, value) => writer.Write((bool)value) },
                { typeof(decimal), (writer, value) => writer.Write((decimal)value) },
                { typeof(char), (writer, value) => writer.Write((char)value) },
                { typeof(double), (writer, value) => writer.Write((double)value) },
                { typeof(Half), (writer, value) => writer.Write((Half)value) },
                { typeof(Int16), (writer, value) => writer.Write((Int16)value) },
                { typeof(Int32), (writer, value) => writer.Write((Int32)value) },
                { typeof(Int64), (writer, value) => writer.Write((Int64)value) },
                { typeof(Single), (writer, value) => writer.Write((Single)value) },
            };
        }

        public List<PropertyModel> Retrive<T>(T item)
        {
            List<PropertyModel> result = new List<PropertyModel>();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                result.Add(
                    new PropertyModel()
                    {
                        Type = property.PropertyType,
                        Name = property.Name,
                        Value = property.GetValue(item)
                    }
                );
            }

            return result;
        }

        public void Write<T>(T item, string path)
        {
            List<PropertyModel> properties = Retrive<T>(item);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    foreach (PropertyModel property in properties)
                    {
                        if (writeMethods.TryGetValue(property.Type, out var writeMethod))
                        {
                            writeMethod(writer, property.Value);
                        }
                        else
                        {
                            throw new ArgumentException($"Unsupported type: {property.Type}");
                        }
                    }
                }
            }
        }

        public T Read<T>(string path) where T : new()
        {
            T result = new T();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    foreach (PropertyInfo property in properties)
                    {
                        if (readMethods.TryGetValue(property.PropertyType, out var readMethod))
                        {
                            object value = readMethod(reader);
                            property.SetValue(result, value);
                        }
                        else
                        {
                            throw new ArgumentException($"Unsupported type: {property.PropertyType}");
                        }
                    }
                }
            }

            return result;
        }

        private static object ReadEnum(BinaryReader reader)
        {
            string enumTypeName = reader.ReadString();
            Type enumType = Type.GetType(enumTypeName);
            int enumValue = reader.ReadInt32();
            Enum enumResult = (Enum)Enum.ToObject(enumType, enumValue);

            return enumResult;
        }

        private static void WriteEnum(BinaryWriter writer, Enum value)
        {
            writer.Write(value.GetType().AssemblyQualifiedName);
            writer.Write(Convert.ToInt32(value));
        }

    }
}
