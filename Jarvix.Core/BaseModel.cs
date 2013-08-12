using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using log4net;


namespace Ultrapulito.Jarvix.Core {

    [Serializable]
    public class BaseModel {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #region COSTRUTTORI
        public BaseModel() { }
        #endregion


        #region METODI PUBBLICI

        /// <summary>
        /// Copia i dati dal DataRow all'oggetto.                        
        /// </summary>
        /// <param type="DataRow" name="row">DataRow contente i dati del record su database.</param>        
        /// <returns>void</returns>
        public void FillObject(DataRow row) {
            Type type = this.GetType();
            PropertyInfo property = null;
            MethodInfo setter = null;
            Type propertyType = null;
            dynamic propertyValue = null;
            DataTable table = row.Table;
            foreach (DataColumn col in table.Columns) {
                property = type.GetProperty(col.ColumnName);
                setter = property.GetSetMethod();
                propertyType = property.PropertyType;
                propertyValue = (row[col.ColumnName] != DBNull.Value) ? row[col.ColumnName] : null;
                switch (propertyType.ToString()) { 
                    case "System.Nullable`1[System.Int32]":
                        propertyValue = Marshaller.ChangeType<int?>(propertyValue);
                        break;                        
                    case "System.Nullable`1[System.Double]":
                        propertyValue = Marshaller.ChangeType<double?>(propertyValue);
                        break;
                    case "System.Nullable`1[System.Boolean]":
                        propertyValue = Marshaller.ChangeType<bool?>(propertyValue);
                        break;  
                    case "System.Nullable`1[System.DateTime]":
                        propertyValue = Marshaller.ChangeType<DateTime?>(propertyValue);
                        break;
                    default:
                        propertyValue = Marshaller.ChangeType<string>(propertyValue);
                        if (propertyValue != null) {
                            string value = (string)propertyValue;
                            propertyValue = value.Trim();
                        }
                        break;
                }
                
                object[] args = { propertyValue };
                setter.Invoke(this, args);
            }
        }


        /// <summary>
        /// Converte tutte le proprietà dell'oggetto in un Hashtable(nome,valore).
        /// Attenzione: Vengono convertiti solo i tipi semplici, i tipi che estendono "BaseModel" e le liste vengono ignorati.
        /// </summary>
        /// <returns>Hashtable</returns>
        public Hashtable GetProperties() {
            Hashtable parameters = new Hashtable();
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            dynamic propertyValue = "";
            Type propertyType = null;
            String propertyName = "";
            MethodInfo getter = null;
            foreach (PropertyInfo property in properties) {
                propertyType = property.PropertyType;                
                if (propertyType.BaseType != typeof(BaseModel) || propertyType.IsGenericType) { // creo solo parametri con tipi semplici
                    propertyName = property.Name;
                    if (propertyName != "SP_READ" && propertyName != "SP_WRITE") {
                        getter = property.GetGetMethod();
                        propertyValue = getter.Invoke(this, null);

                        switch (propertyType.ToString()) {
                            case "System.Nullable`1[System.Int32]":
                                propertyValue = Marshaller.ChangeType<int?>(propertyValue);
                                break;
                            case "System.Nullable`1[System.Double]":
                                propertyValue = Marshaller.ChangeType<double?>(propertyValue);
                                break;
                            case "System.Nullable`1[System.Boolean]":
                                propertyValue = Marshaller.ChangeType<bool?>(propertyValue);
                                break;
                            case "System.Nullable`1[System.DateTime]":
                                propertyValue = Marshaller.ChangeType<DateTime?>(propertyValue);
                                break;
                            default:
                                propertyValue = Marshaller.ChangeType<string>(propertyValue);
                                break;
                        }                

                        parameters.Add(propertyName, propertyValue);
                    }
                }
            }
            return parameters;
        }


        public List<SqlParameter> CreateSqlParameters() {
            List<SqlParameter> parameters = new List<SqlParameter>();            
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();            
            String propertyName = "";
            Type propertyType = null;
            dynamic propertyValue = "";
            MethodInfo getter = null;
            SqlParameter parameter = null;
            foreach (PropertyInfo property in properties) {
                propertyType = property.PropertyType;
                if (!(propertyType.BaseType == typeof(BaseModel) || propertyType.IsGenericType)) { // creo solo parametri con tipi semplici
                    propertyName = property.Name;
                    if (propertyName != "SP_READ" && propertyName != "SP_WRITE") {
                        getter = property.GetGetMethod();
                        propertyValue = Convert.ChangeType(getter.Invoke(this, null), propertyType);
                        parameter = new SqlParameter();

                        if (propertyType.ToString() == "System.Int32") {
                            parameter.SqlDbType = SqlDbType.Int;
                        } else if (propertyType.ToString() == "System.String") {
                            parameter.SqlDbType = SqlDbType.VarChar;
                        } else if (propertyType.ToString() == "System.DateTime") {
                            parameter.SqlDbType = SqlDbType.DateTime2;
                        } else if (propertyType.ToString() == "System.Boolean") {
                            parameter.SqlDbType = SqlDbType.TinyInt;
                        } else if (propertyType.ToString() == "System.Double") {
                            parameter.SqlDbType = SqlDbType.Decimal;
                        }                                                

                        parameter.ParameterName = propertyName;
                        parameter.Value = propertyValue;
                        parameters.Add(parameter);                        
                    }
                }
            }

            return parameters;
        }


        /// <summary>Serializza l'oggetto</summary>
        /// <param name="serializationType">Il tipo di serializzazione da applicare.</param>
        /// <param name="compressionType">Il tipo di compressione da applicare (default = nothing).</param>
        /// <returns>string</returns>
        public string Serialize(SerializationType serializationType, CompressionType compressionType = CompressionType.NOTHING) {
            return Serializer.SerializeObject(this, serializationType, compressionType);
        }


        /// <summary>Deserializza l'oggetto, ricostruendo l'istanza.</summary>
        /// <param name="data">La stringa contenente l'oggetto serializzato.</param>
        /// <param name="serializationType">Il tipo di deserializzazione da applicare.</param>
        /// <param name="compressionType">Il tipo di decompressione da applicare (default = nothing).</param>
        /// <returns>dynamic</returns>
        public dynamic Unserialize(string data, SerializationType serializationType, CompressionType compressionType = CompressionType.NOTHING) {
            return Serializer.UnserializeObject(data, this.GetType(), serializationType, compressionType);
        }


        /// <summary>Clona l'oggetto</summary>
        /// <returns>dynamic</returns>
        public dynamic Copy() {
            return Serializer.Copy(this);
        }


        /// <summary>Serializza l'oggetto in formato binario e lo comprime con GZIP per il salvataggio su sessione.</summary>
        /// <returns>string</returns>
        public string SessionWrite() {
            return Serializer.SerializeObject(this, SerializationType.BINARY, CompressionType.GZIP);
        }


        /// <summary>Deserialliza l'oggetto da sessione e ricostruisce l'istanza.</summary>
        /// Per poter essere riletto e ricostruito l'oggetto deve essere salvato in sessione con la funzione SessionWrite.
        /// <param name="sessionData">La stringa contenente l'oggetto serializzato proveniente dalla sessione.</param>        
        /// <returns>dynamic</returns>
        public dynamic SessionRead(string sessionData) {
            return Serializer.UnserializeObject(sessionData, this.GetType(), SerializationType.BINARY, CompressionType.GZIP);
        }

        #endregion


    }
}
