using System;
using System.Collections;
using System.Configuration;
using System.Data;
using log4net;

namespace Ultrapulito.Jarvix.Helper {

    public enum TableType { TABLE, VIEW };

    class Helper {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args) {

            try {

                // attivo e configuro log4net
                log4net.Config.XmlConfigurator.Configure();                

                // elenco degli schema da escluere
                ArrayList excludedSchemas = new ArrayList(ConfigurationManager.AppSettings["EXCLUDED_SCHEMAS"].ToString().ToUpper().Split(','));

                DataTable tables = null;
                DataTable views = null;
                DataTable columns = null;
                string catalog = "";
                string schema = "";
                string table = "";
                string view = "";                

                // astraggo le tabelle
                tables = Database.SelectTables();
                for (int i = 0; i < tables.Rows.Count; i++) {
                    catalog = tables.Rows[i]["table_catalog"].ToString();
                    schema = tables.Rows[i]["table_schema"].ToString();
                    table = tables.Rows[i]["table_name"].ToString();
                    columns = Database.SelectColumns(table);
                    if (!excludedSchemas.Contains(schema.ToUpper())) {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_DOTNET_MODEL"])) {
                            DotNet.CreateClass(TableType.TABLE, catalog, schema, table, columns);
                        }
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_DOJO_MODEL"])) {
                            Dojo.CreateClass(TableType.TABLE, catalog, schema, table, columns);
                        }
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_STORED_PROCEDURES"])) {
                            Database.CreateSpRead(catalog, schema, table, columns);
                            Database.CreateSpWrite(catalog, schema, table, columns);
                        }
                    }
                }

                // astraggo le viste
                views = Database.SelectViews();
                for (int i = 0; i < views.Rows.Count; i++) {
                    catalog = views.Rows[i]["table_catalog"].ToString();
                    schema = views.Rows[i]["table_schema"].ToString();
                    view = views.Rows[i]["table_name"].ToString();
                    columns = Database.SelectColumns(view);
                    if (!excludedSchemas.Contains(schema.ToUpper())) {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_DOTNET_MODEL"])) {
                            DotNet.CreateClass(TableType.VIEW, catalog, schema, view, columns);
                        }
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_DOJO_MODEL"])) {
                            Dojo.CreateClass(TableType.VIEW, catalog, schema, view, columns);
                        }
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["CREATE_STORED_PROCEDURES"])) {
                            Database.CreateSpRead(catalog, schema, view, columns);                            
                        }
                    }
                }

            } catch (Exception ex) {
                log.Error(ex.Message, ex);
            }

        }
    }

}
