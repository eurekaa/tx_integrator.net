using System;
using System.Text;
using System.Configuration;
using System.Data;
using System.IO;

namespace Ultrapulito.Jarvix.Helper {

    static class Dojo {

        public static void CreateClass(TableType tableType, string catalog, string schema, string table, DataTable columns) {

            string nameSpace = ConfigurationManager.AppSettings["DOJO_MODEL_NAMESPACE"];
            nameSpace += (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_SCHEMA_SUBDIRECTORY"])) ? "." + schema : "";

            string outputPath = ConfigurationManager.AppSettings["DOJO_MODEL_PATH"];
            outputPath += (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_SCHEMA_SUBDIRECTORY"])) ? schema + "\\" : "";

            StringBuilder output = new StringBuilder();

            // requirements
            if (tableType == TableType.VIEW) {
                output.Append("dojo.require(\"" + ConfigurationManager.AppSettings["DOJO_MODEL_BASEVIEW"] + "\");" + Environment.NewLine);
            } else {
                output.Append("dojo.require(\"" + ConfigurationManager.AppSettings["DOJO_MODEL_BASETABLE"] + "\");" + Environment.NewLine);
            }
            output.Append(Environment.NewLine);

            // namespace
            output.Append("dojo.provide(\"" + nameSpace + "." + table + "\");" + Environment.NewLine);

            // classe          
            if (tableType == TableType.VIEW) {
                output.Append("dojo.declare(\"" + nameSpace + "." + table + "\", " + ConfigurationManager.AppSettings["DOJO_MODEL_BASEVIEW"] + ", {" + Environment.NewLine);
            } else {
                output.Append("dojo.declare(\"" + nameSpace + "." + table + "\", " + ConfigurationManager.AppSettings["DOJO_MODEL_BASETABLE"] + ", {" + Environment.NewLine);
            }
            output.Append(Environment.NewLine);

            // controller
            output.AppendFormat("\t");
            string controller = ConfigurationManager.AppSettings["CONTROLLER_PATH"];
            controller += (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_SCHEMA_SUBDIRECTORY"])) ? schema + ".ashx" : ConfigurationManager.AppSettings["CONTROLLER_DEFAULT"];
            output.Append("Controller: \"" + controller +"\"," + Environment.NewLine);
            output.Append(Environment.NewLine);

            // columns            
            string nome_campo = "";
            string tipo_campo = "";
            output.AppendFormat("\t");
            output.Append("Data: {" + Environment.NewLine);
            for (int i = 0; i < columns.Rows.Count; i++) {
                nome_campo = columns.Rows[i]["COLUMN_NAME"].ToString();
                tipo_campo = columns.Rows[i]["DATA_TYPE"].ToString();
                output.AppendFormat("\t\t");
                output.Append(nome_campo + ": null");
                if (i < columns.Rows.Count - 1) {
                    output.Append("," + Environment.NewLine);
                } else {
                    output.Append(Environment.NewLine);
                }
            }
            output.AppendFormat("\t");
            output.Append("}" + Environment.NewLine);
            output.Append(Environment.NewLine);

            // fine classe              
            output.Append("});" + Environment.NewLine);
            output.Append(Environment.NewLine);

            // creo la cartella
            if (!Directory.Exists(outputPath)) {
                Directory.CreateDirectory(outputPath);
            }

            // creo il file
            StreamWriter sw = new StreamWriter(outputPath + table + ".js", false);
            sw.WriteLine(output);
            sw.Close();

        }

    }
}
