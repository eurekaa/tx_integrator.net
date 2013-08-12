using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using JsonFx.Json;
using log4net;


namespace Ultrapulito.Jarvix.Core {

    public enum SerializationType { BINARY, XML, JSON };


    public static class Serializer {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>Funzione per la serializzazione di liste di oggetti.</summary>
        /// <param name="objList">La lista di oggetti da serializzare.</param>        
        /// <param name="serializzationType">Il tipo di serializzazione che si vuole ottenere.</param>        
        /// <param name="compressionType">[Optional: default=nothing] Il tipo di compressione da impiegare.</param>        
        /// <returns>string</returns> 
        public static string SerializeObjectList(dynamic objList, SerializationType serializationType, CompressionType compressionType = CompressionType.NOTHING) {
            string data = "";
            Type objType = objList.GetType().GetGenericArguments()[0];

            if (objList.Count > 0) {

                // serializzo BINARY
                if (serializationType == SerializationType.BINARY) {
                    MemoryStream memStream = new MemoryStream();
                    IFormatter binFormatter = new BinaryFormatter();
                    binFormatter.Serialize(memStream, objList);
                    byte[] buffer = memStream.ToArray();
                    data = Convert.ToBase64String(buffer);

                    // serializzo XML
                } else if (serializationType == SerializationType.XML) {
                    MemoryStream memStream = new MemoryStream();
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<>).MakeGenericType(objType));
                    xmlSerializer.Serialize(memStream, objList);
                    byte[] buffer = memStream.ToArray();
                    data = Encoding.UTF8.GetString(buffer);

                    // serializzo JSON
                } else if (serializationType == SerializationType.JSON) {
                    data = JsonWriter.Serialize(objList);

                    // eccezione: tipo serializzazione non specificato
                } else {
                    throw new Exception("Tipo di serializzazione sconosciuto.");
                }

                // comprimo la serializzazione se richiesto               
                data = Compressor.Compress(data, compressionType);
            }

            return data;
        }


        /// <summary>Funzione per la deserializzazione di liste di oggetti.</summary>
        /// <param name="data">La stringa contenente la lista di oggetti serializzata.</param>        
        /// <param name="serializzationType">Il tipo di serializzazione usato.</param>        
        /// <param name="compressionType">[Optional: default=nothing] Il tipo di compressione usato.</param>        
        /// <returns>dynamic</returns> 
        public static dynamic UnserializeObjectList(string data, Type objType, SerializationType serializationType, CompressionType compressionType = CompressionType.NOTHING) {
            dynamic objList = null;

            // decomprimo la serializzazione se richiesto                        
            data = Compressor.Decompress(data, compressionType);

            // deserializzo da BINARY
            if (serializationType == SerializationType.BINARY) {
                IFormatter binFormatter = new BinaryFormatter();
                byte[] buffer = Convert.FromBase64String(data);
                MemoryStream memStream = new MemoryStream(buffer);
                Type listType = typeof(List<>).MakeGenericType(objType);
                objList = (IList)Activator.CreateInstance(listType);
                objList = binFormatter.Deserialize(memStream);

                // deserializzo da XML
            } else if (serializationType == SerializationType.XML) {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<>).MakeGenericType(objType));
                byte[] b = System.Text.Encoding.UTF8.GetBytes(data);
                MemoryStream memStream = new MemoryStream(b);
                Type listType = typeof(List<>).MakeGenericType(objType);
                objList = (IList)Activator.CreateInstance(listType);
                objList = xmlSerializer.Deserialize(memStream);

                // deserializzo da JSON
            } else if (serializationType == SerializationType.JSON) {
                object[] tempList = (object[])JsonReader.Deserialize(data, typeof(List<>).MakeGenericType(objType));
                Type listType = typeof(List<>).MakeGenericType(objType);
                objList = (IList)Activator.CreateInstance(listType);
                foreach (object value in tempList) {
                    objList.Add((dynamic)value);
                }

                // eccezione: tipo serializzazione non specificato
            } else {
                throw new Exception("Tipo di serializzazione sconosciuto.");
            }

            return objList;

        }


        /// <summary>Funzione per la serializzazione di oggetti.</summary>
        /// <param name="objList">L'oggetto da serializzare.</param>        
        /// <param name="serializzationType">Il tipo di serializzazione che si vuole ottenere.</param>        
        /// <param name="compressionType">[Optional: default=nothing] Il tipo di compressione da impiegare.</param>        
        /// <returns>string</returns> 
        public static string SerializeObject(Object obj, SerializationType serializationType, CompressionType compressionType = CompressionType.NOTHING) {
            string data = "";

            // serializzo BINARY
            if (serializationType == SerializationType.BINARY) {
                MemoryStream memStream = new MemoryStream();
                IFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(memStream, obj);
                byte[] buffer = memStream.ToArray();
                data = Convert.ToBase64String(buffer);

                // serializzo XML
            } else if (serializationType == SerializationType.XML) {
                MemoryStream memStream = new MemoryStream();
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(memStream, obj);
                byte[] buffer = memStream.ToArray();
                data = Encoding.UTF8.GetString(buffer);

                // serializzo JSON
            } else if (serializationType == SerializationType.JSON) {
                data = JsonWriter.Serialize(obj);

                // eccezione: tipo serializzazione non specificato
            } else {
                throw new Exception("Tipo di serializzazione sconosciuto.");
            }

            // comprimo la serializzazione se richiesto
            data = Compressor.Compress(data, compressionType);

            return data;
        }


        /// <summary>Funzione per la deserializzazione di oggetti.</summary>
        /// <param name="data">La stringa contenente l'oggetto serializzato.</param>        
        /// <param name="serializzationType">Il tipo di serializzazione usato.</param>        
        /// <param name="compressionType">[Optional: default=nothing] Il tipo di compressione usato.</param>        
        /// <returns>dynamic</returns> 
        public static dynamic UnserializeObject(string data, Type objType, SerializationType serializationType, CompressionType compressionType = CompressionType.NOTHING) {
            dynamic obj = null;

            // decomprimo la serializzazione se richiesto
            data = Compressor.Decompress(data, compressionType);

            // deserializzo da BINARY
            if (serializationType == SerializationType.BINARY) {
                IFormatter binFormatter = new BinaryFormatter();
                byte[] buffer = Convert.FromBase64String(data);
                MemoryStream memStream = new MemoryStream(buffer);
                obj = binFormatter.Deserialize(memStream);

                // deserializzo da XML
            } else if (serializationType == SerializationType.XML) {
                XmlSerializer xmlSerializer = new XmlSerializer(objType);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
                MemoryStream memStream = new MemoryStream(buffer);
                obj = xmlSerializer.Deserialize(memStream);

                // deserializzo da JSON
            } else if (serializationType == SerializationType.JSON) {                
                obj = JsonReader.Deserialize(data, objType);

                // eccezione: tipo serializzazione non specificato
            } else {
                throw new Exception("Tipo di serializzazione sconosciuto.");
            }

            return obj;
        }


        /// <summary>Funzione per la clonazione di oggetti.</summary>
        /// <param name="obj">L'oggetto da clonare.</param>                        
        /// <returns>dynamic</returns> 
        public static dynamic Copy(dynamic obj) {
            using (MemoryStream memStream = new MemoryStream()) {
                IFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(memStream, obj);
                memStream.Position = 0;

                return binFormatter.Deserialize(memStream);
            }
        }

    }
}
