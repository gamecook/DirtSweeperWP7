using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace MineSweeper.DirtSweeper.Serialization
{
    public class DataContractJSONSerializationUtil
    {
        public static void Serialize(Stream streamObject, object objForSerialization)
        {
            if (objForSerialization == null || streamObject == null)
                return;

            var ser = new DataContractJsonSerializer(objForSerialization.GetType());
            ser.WriteObject(streamObject, objForSerialization);
        }

        public static object Deserialize(Stream streamObject, Type serializedObjectType)
        {
            if (serializedObjectType == null || streamObject == null)
                return null;

            var ser = new DataContractJsonSerializer(serializedObjectType);
            return ser.ReadObject(streamObject);
        }
    }
}