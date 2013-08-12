using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using log4net;

namespace Ultrapulito.Jarvix.Core {    

    public class Dao {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _connectionString = string.Empty;
        private SqlConnection _connection = null;


        public Dao() {
            _connectionString = ConfigurationManager.AppSettings["ConnString"].ToString();
        }


        public Dao(string connectionString) {
            _connectionString = connectionString;
        }


        /// <summary>
        /// Distruttore: viene invocato automaticamente dal garbage collector quando l'oggetto viene rilasciato.
        /// </summary>        
        /// <returns>void</returns>
        ~Dao() {
            Disconnect();
        }


        /// <summary>
        /// Funzione per l'apertura della connessione.
        /// </summary>        
        /// <returns>void</returns>
        private void Connect() {
            _connection = new SqlConnection();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
        }


        /// <summary>
        /// Funzione per la chiusura della connessione.
        /// </summary>        
        /// <returns>void</returns>
        private void Disconnect() {
            _connection.Close();
        }


        /// <summary>
        /// Funzione generica per il lancio di una stored proedure.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet ExecuteStoredProcedure(string storedprocedure, Hashtable parameters = null) {
            SqlCommand cmd = new SqlCommand();
            DataSet data = new DataSet();
            Connect();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = _connection;
            cmd.CommandText = storedprocedure;
            if (parameters != null) {
                SqlParameter sqlParameter = null;
                String parameterName = "";
                dynamic parameterValue = null;
                foreach (DictionaryEntry parameter in parameters) {
                    parameterName = parameter.Key.ToString();
                    if (parameter.Value == null) {
                        parameterValue = DBNull.Value;
                    } else {
                        parameterValue = parameter.Value;
                    }
                    sqlParameter = new SqlParameter(parameterName, parameterValue);
                    cmd.Parameters.Add(sqlParameter);
                }
            }
            SqlDataAdapter datadapter = new SqlDataAdapter(cmd);
            datadapter.Fill(data);

            Disconnect();


            return data;
        }


        /// <summary>
        /// Funzione generica per il lancio di una stored proedure.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet ExecuteStoredProcedure(string storedprocedure, List<SqlParameter> parameters = null) {
            SqlCommand cmd = new SqlCommand();
            DataSet data = new DataSet();

            Connect();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = _connection;
            cmd.CommandText = storedprocedure;
            if (parameters != null) {
                foreach (SqlParameter parameter in parameters) {
                    cmd.Parameters.Add(parameter);
                }
            }
            SqlDataAdapter datadapter = new SqlDataAdapter(cmd);
            datadapter.Fill(data);

            Disconnect();

            return data;
        }


        /// <summary>
        /// Funzione per il lancio di comandi sql
        /// </summary>
        /// <returns>DataTable</returns>
        public int ExecuteNonQuery(string sql, Hashtable parameters = null) {
            int rows_affected = 0;
            SqlCommand cmd = new SqlCommand();
            try {
                Connect();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = _connection;
                cmd.CommandText = sql;
                if (parameters != null) {
                    CreateSqlParameters(cmd, parameters);
                }
                rows_affected = cmd.ExecuteNonQuery();
            } finally {
                Disconnect();
            }
            return rows_affected;
        }


        /// <summary>
        /// Funzione per il lancio di query sql
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet ExecuteQuery(string sql, Hashtable parameters = null) {
            DataSet data = null;
            try {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter datadapter = new SqlDataAdapter(cmd);
                data = new DataSet();
                Connect();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = _connection;
                cmd.CommandText = sql;
                if (parameters != null) {
                    CreateSqlParameters(cmd, parameters);
                }
                datadapter.Fill(data);
            } finally {
                Disconnect();
            }
            return data;
        }


        /// <summary>
        /// Funzione per la creazione dei parametri sql.
        /// </summary>
        /// <param name="cmd">SqlCommand su cui agganciare i parametri</param>
        /// <param name="parameters">Hashtable contenente i nomi e i valori dei parametri (inseriti come chiave-valore)</param>
        /// <returns>void</returns>
        private void CreateSqlParameters(SqlCommand cmd, Hashtable parameters = null) {
            if (parameters != null) {
                foreach (DictionaryEntry parameter in parameters) {
                    String parameterName = parameter.Key.ToString();
                    String parameterType = parameter.Value.GetType().ToString().Replace("System.", "");
                    String parameterValue = parameter.Value.ToString();
                    if (parameterType == "Int32") {
                        cmd.Parameters.Add(parameterName, SqlDbType.Int).Value = int.Parse(parameterValue);
                    } else if (parameterType == "String") {
                        cmd.Parameters.Add(parameterName, SqlDbType.VarChar, parameterValue.Length).Value = parameterValue;
                    } else if (parameterType == "DateTime") {
                        cmd.Parameters.Add(parameterName, SqlDbType.DateTime2).Value = Convert.ToDateTime(parameterValue);
                    } else if (parameterType == "Boolean") {
                        cmd.Parameters.Add(parameterName, SqlDbType.Bit).Value = bool.Parse(parameterValue);
                    } else {
                        cmd.Parameters.Add(parameterName, SqlDbType.Decimal).Value = Convert.ToDecimal(parameterValue);
                    }
                }
            }
        }


    }
}

