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
                { typeof(int), reader => reader.ReadInt32() },
                { typeof(string), reader => reader.ReadString() },
                { typeof(byte), reader => reader.ReadByte() },
                { typeof(bool), reader => reader.ReadBoolean() },
                { typeof(decimal), reader => reader.ReadDecimal()}
            };

            writeMethods = new Dictionary<Type, Action<BinaryWriter, object>>
            {
                { typeof(int), (writer, value) => writer.Write((int)value) },
                { typeof(string), (writer, value) => writer.Write((string)value) },
                { typeof(byte), (writer, value) => writer.Write((byte)value) },
                { typeof(bool), (writer, value) => writer.Write((bool)value) },
                { typeof(decimal), (writer, value) => writer.Write((decimal)value) }
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
    }
}
