using System;
using System.Configuration;
using System.Data;
using Ultrapulito.Jarvix.Core;


namespace Ultrapulito.Jarvix.Helper {

    static class Database {

        public static DataTable SelectTables() {
            Dao dao = new Dao();
            String sql = "SELECT table_catalog, table_schema, table_name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' order by table_name asc";
            DataSet dati = dao.ExecuteQuery(sql);
            return dati.Tables[0];
        }


        public static DataTable SelectViews() {
            Dao dao = new Dao();
            String sql = "SELECT table_catalog, table_schema, table_name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='VIEW' order by table_name asc";
            DataSet dati = dao.ExecuteQuery(sql);
            return dati.Tables[0];
        }


        public static DataTable SelectColumns(string table) {
            Dao dao = new Dao();
            String sql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + table + "'";
            DataSet dati = dao.ExecuteQuery(sql);
            return dati.Tables[0];
        }


        public static void CreateSpRead(string catalog, string schema, string table, DataTable columns) {

            Dao dao = new Dao();

            string stored_procedure = "[" + schema + "].[" + ConfigurationManager.AppSettings["SP_PREFIX"] + table + ConfigurationManager.AppSettings["SP_READ_SUFFIX"] + "]";

            // elimino la stored proedure
            Database.DropStoredProcedure(schema, table, ConfigurationManager.AppSettings["SP_READ_SUFFIX"], ConfigurationManager.AppSettings["SP_PREFIX"]);

            // creo la stored procedure
            string sql = Environment.NewLine;
            sql += "CREATE PROCEDURE " + stored_procedure + Environment.NewLine;
            // parametri 
            string nome_campo = "";
            string tipo_campo = "";
            string char_max_length = "";
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                tipo_campo = columns.Rows[i]["DATA_TYPE"].ToString();
                if (columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value && tipo_campo != "text" && tipo_campo != "ntext" && tipo_campo != "geography") {
                    char_max_length = columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString();
                    char_max_length = (char_max_length == "-1") ? "MAX" : char_max_length;
                    sql += "@" + nome_campo + " " + tipo_campo + "(" + char_max_length + ") = NULL";
                } else {
                    sql += "@" + nome_campo + " " + tipo_campo + " = NULL";
                }
                if (i < columns.Rows.Count - 1) {
                    sql += "," + Environment.NewLine;
                } else {
                    sql += " " + Environment.NewLine;
                }
            }
            sql += Environment.NewLine + Environment.NewLine;
            // corpo
            sql += "AS BEGIN " + Environment.NewLine;
            sql += "SET NOCOUNT ON; " + Environment.NewLine + Environment.NewLine;
            sql += "SELECT * FROM " + schema + "." + table + " WHERE 1=1 " + Environment.NewLine;
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                tipo_campo = columns.Rows[i]["DATA_TYPE"].ToString();
                if (tipo_campo != "text" && tipo_campo != "ntext") {
                    sql += "AND (" + nome_campo + " = @" + nome_campo + " OR @" + nome_campo + " IS NULL) " + Environment.NewLine;
                }
            }            
            sql += Environment.NewLine;
            sql += "END" + Environment.NewLine;

            // eseguo la query
            dao.ExecuteNonQuery(sql.ToString());

        }


        public static void CreateSpWrite(string catalog, string schema, string table, DataTable columns) {

            Dao dao = new Dao();

            string stored_procedure = "[" + schema + "].[" + ConfigurationManager.AppSettings["SP_PREFIX"] + table + ConfigurationManager.AppSettings["SP_WRITE_SUFFIX"] + "]";
           
            // elimino la stored proedure
            Database.DropStoredProcedure(schema, table, ConfigurationManager.AppSettings["SP_WRITE_SUFFIX"], ConfigurationManager.AppSettings["SP_PREFIX"]);

            // creo la stored procedure
            string sql = Environment.NewLine;
            sql += "CREATE PROCEDURE " + stored_procedure + Environment.NewLine;

            // parametro che identifica il tipo di operazione da eseguire (insert|update|delete)
            sql += "@@ACTION varchar(10) = NULL," + Environment.NewLine;

            // parametri 
            string nome_campo = "";
            string tipo_campo = "";
            string char_max_length = "";
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                tipo_campo = columns.Rows[i]["DATA_TYPE"].ToString();
                if (columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value && tipo_campo != "text" && tipo_campo != "ntext" && tipo_campo != "geography") {
                    char_max_length = columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString();
                    char_max_length = (char_max_length == "-1") ? "MAX" : char_max_length;
                    sql += "@" + nome_campo + " " + tipo_campo + "(" + char_max_length + ") = NULL";
                } else {
                    sql += "@" + nome_campo + " " + tipo_campo + " = NULL";
                }
                sql += (i < columns.Rows.Count - 1) ? "," + Environment.NewLine : "" + Environment.NewLine;
            }
            sql += Environment.NewLine + Environment.NewLine;


            // corpo intestazione
            sql += "AS BEGIN " + Environment.NewLine;
            sql += "SET NOCOUNT ON; " + Environment.NewLine + Environment.NewLine;


            // corpo insert
            sql += "IF @@ACTION = 'INSERT' BEGIN" + Environment.NewLine;
            sql += "INSERT INTO [" + schema + "].[" + table + "] (";
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                if (nome_campo != "Id") {
                    sql += nome_campo;
                    sql += (i < (columns.Rows.Count - 1)) ? "," : "";
                }
            }
            sql += ") " + Environment.NewLine;
            sql += "OUTPUT INSERTED.Id " + Environment.NewLine;
            sql += "VALUES (";
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                if (nome_campo != "Id") {
                    sql += "@" + nome_campo;
                    sql += (i < (columns.Rows.Count - 1)) ? "," : "";
                }
            }
            sql += ")" + Environment.NewLine;
            sql += "END" + Environment.NewLine + Environment.NewLine;


            // corpo update
            sql += "ELSE IF @@ACTION = 'UPDATE' BEGIN" + Environment.NewLine;
            sql += "UPDATE [" + schema + "].[" + table + "] SET " + Environment.NewLine;
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                if (nome_campo != "Id") {
                    sql += nome_campo + " = @" + nome_campo;
                    sql += (i < (columns.Rows.Count - 1)) ? "," + Environment.NewLine : " " + Environment.NewLine;
                }
            }
            sql += "OUTPUT INSERTED.Id " + Environment.NewLine;
            sql += "WHERE " + schema + "." + table + ".Id = @Id " + Environment.NewLine;
            sql += "END " + Environment.NewLine + Environment.NewLine;


            // corpo delete
            sql += "ELSE IF @@ACTION = 'DELETE' BEGIN" + Environment.NewLine;
            sql += "DELETE FROM " + schema + "." + table + " " + Environment.NewLine;
            sql += "OUTPUT DELETED.Id " + Environment.NewLine;
            sql += "WHERE " + schema + "." + table + ".Id = @Id " + Environment.NewLine;
            sql += "END " + Environment.NewLine + Environment.NewLine;

            // corpo chiusura
            sql += "END " + Environment.NewLine;

            // eseguo la query
            dao.ExecuteNonQuery(sql.ToString());

        }


        private static void DropStoredProcedure(string schema, string table, string suffix, string prefix) {
            Dao dao = new Dao();
            string sql = "";
            sql = "IF EXISTS ";
            sql += "(SELECT * FROM dbo.sysobjects ";
            sql += "WHERE id = object_id(N'[" + schema + "].[" + prefix + table + suffix + "]') ";
            sql += "and OBJECTPROPERTY(id, N'IsProcedure') = 1) ";
            sql += "DROP PROCEDURE [" + schema + "].[" + prefix + table + suffix + "] ";
            dao.ExecuteNonQuery(sql);          
        }
    }
}
