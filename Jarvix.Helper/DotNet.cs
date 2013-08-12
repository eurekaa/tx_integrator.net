using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;

namespace Ultrapulito.Jarvix.Helper {

    static class DotNet {

        public static void CreateClass(TableType tableType, string catalog, string schema, string table, DataTable columns) {

            string nameSpace = ConfigurationManager.AppSettings["DOTNET_MODEL_NAMESPACE"];
            nameSpace += (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_SCHEMA_SUBDIRECTORY"])) ? "." + schema : "";

            string outputPath = ConfigurationManager.AppSettings["DOTNET_MODEL_PATH"];
            outputPath += (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_SCHEMA_SUBDIRECTORY"])) ? schema + "\\" : "";

            // marshalling fra tipi di dati (db > C#) - uso tipi nullable!
            Hashtable Marshalling = new Hashtable();
            Marshalling.Add("int", "int?");
            Marshalling.Add("bigint", "int?");
            Marshalling.Add("smallint", "int?");
            Marshalling.Add("numeric", "double?");
            Marshalling.Add("decimal", "double?");
            Marshalling.Add("money", "double?");
            Marshalling.Add("float", "double?");
            Marshalling.Add("tinyint", "bool?");
            Marshalling.Add("char", "string");
            Marshalling.Add("nchar", "string");
            Marshalling.Add("varchar", "string");
            Marshalling.Add("nvarchar", "string");
            Marshalling.Add("text", "string");
            Marshalling.Add("ntext", "string");
            Marshalling.Add("xml", "string");
            Marshalling.Add("geography", "string");
            Marshalling.Add("date", "DateTime?");
            Marshalling.Add("time", "DateTime?");
            Marshalling.Add("datetime", "DateTime?");
            Marshalling.Add("datetime2", "DateTime?");
            Marshalling.Add("datetimeoffset", "DateTime?");

            StringBuilder output = new StringBuilder();

            // usings            
            output.Append("using System;" + Environment.NewLine);
            output.Append("using Ultrapulito.Jarvix.Core;" + Environment.NewLine);
            output.Append(Environment.NewLine);

            // namespace
            output.Append("namespace " + nameSpace + "{" + Environment.NewLine);
            output.Append(Environment.NewLine);

            // classe            
            output.AppendFormat("\t");
            output.Append("public partial class " + table + " : " + ((tableType == TableType.VIEW) ? ConfigurationManager.AppSettings["DOTNET_MODEL_BASEVIEW"] : ConfigurationManager.AppSettings["DOTNET_MODEL_BASETABLE"]) + " {" + Environment.NewLine);
            output.Append(Environment.NewLine);

            // stored proedures
            output.AppendFormat("\t\t");
            output.Append("#region STORED PROCEDURES" + Environment.NewLine);
            output.AppendFormat("\t\t");
            output.Append("public override string SP_READ { get { return \"" + schema + "." + ConfigurationManager.AppSettings["SP_PREFIX"] + table + ConfigurationManager.AppSettings["SP_READ_SUFFIX"] + "\"; } }" + Environment.NewLine);
            if (tableType == TableType.TABLE) {
                output.AppendFormat("\t\t");
                output.Append("public override string SP_WRITE { get { return \"" + schema + "." + ConfigurationManager.AppSettings["SP_PREFIX"] + table + ConfigurationManager.AppSettings["SP_WRITE_SUFFIX"] + "\"; } }" + Environment.NewLine);
            }
            output.AppendFormat("\t\t");
            output.Append("#endregion" + Environment.NewLine);
            output.Append(Environment.NewLine);
            output.Append(Environment.NewLine);


            // columns            
            string nome_campo = "";
            string tipo_campo = "";
            output.AppendFormat("\t\t");
            output.Append("#region CAMPI DATABASE" + Environment.NewLine);
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                tipo_campo = Marshalling[columns.Rows[i]["DATA_TYPE"].ToString()].ToString();
                output.AppendFormat("\t\t");
                output.Append("public " + tipo_campo + " " + nome_campo + " { get; set; }" + Environment.NewLine);
            }
            output.AppendFormat("\t\t");
            output.Append("#endregion" + Environment.NewLine);
            output.Append(Environment.NewLine);
            output.Append(Environment.NewLine);

            // costruttore           
            output.AppendFormat("\t\t");
            output.Append("#region COSTRUTTORI" + Environment.NewLine);
            output.AppendFormat("\t\t");
            output.Append("public " + table + "(){" + Environment.NewLine);
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                tipo_campo = Marshalling[columns.Rows[i]["DATA_TYPE"].ToString()].ToString();
                output.AppendFormat("\t\t\t");
                output.Append("this." + nome_campo + " = null;" + Environment.NewLine);
            }
            output.AppendFormat("\t\t");
            output.Append("}" + Environment.NewLine);
            output.AppendFormat("\t\t");
            output.Append("#endregion" + Environment.NewLine);
            output.Append(Environment.NewLine);

            // fine classe  
            output.AppendFormat("\t");
            output.Append("}" + Environment.NewLine);
            output.Append(Environment.NewLine);

            // fine namespace
            output.Append("}" + Environment.NewLine);

            // creo la cartella
            if (!Directory.Exists(outputPath)) {
                Directory.CreateDirectory(outputPath);
            }

            // creo il file
            StreamWriter sw = new StreamWriter(outputPath + table + ".cs", false);
            sw.WriteLine(output);
            sw.Close();

        }

    }


}
