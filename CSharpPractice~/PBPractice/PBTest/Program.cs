using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace PBTest
{
    [ProtoContract]
    class Person
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public PersonAddress Address { get; set; }

        public override string ToString() {
            return $"Id:{Id} Name:{Name} City:{Address.City} Street:{Address.Street}";
        }
    }
    
    [ProtoContract]
    public class PersonAddress
    {
        [ProtoMember(1)]
        public string City { get; set; }
        [ProtoMember(2)]
        public string Street { get; set; }
    }

    class Program
    {
        static void Main(string[] args) {
            var person = new Person() {
                Id = 123,
                Name = "Saroce",
                Address = new PersonAddress() {
                    City = "Shenzhen",
                    Street = "Xili",
                }
            };

            // Obsolete 
            // var data = TestNormalSerialize(person);
            // var newPerson = TestNormalDeserialize(data);
            // Console.WriteLine(newPerson);

            byte[] bytes = null;
            // 序列化
            using (var ms = new MemoryStream()) {
                Serializer.Serialize(ms, person);
                bytes = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, bytes, 0, (int)ms.Length);
            }
            // 反序列化
            using (var ms = new MemoryStream(bytes)) {
                var newPerson = Serializer.Deserialize<Person>(ms);
                Console.WriteLine(newPerson);
            }
        }

        private static byte[] TestNormalSerialize(Person person) {
            using (var ms = new MemoryStream()) {
                var bf = new BinaryFormatter();
                try {
                    bf.Serialize(ms, person);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms.ToArray();
                }
                catch (SerializationException e) {
                    Console.WriteLine("Serialize failed.");
                }
            }
            return null;
        }

        private static Person TestNormalDeserialize(byte[] bytes) {
            using (var ms = new MemoryStream(bytes)) {
                var bf = new BinaryFormatter();
                try {
                    return bf.Deserialize(ms) as Person;
                }
                catch (SerializationException e) {
                    Console.WriteLine("Deserialize failed.");
                }
            }
            return null;
        }
    }
}