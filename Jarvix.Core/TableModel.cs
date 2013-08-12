using System;
using System.Collections;
using System.Data;
using System.Reflection;
using log4net;


namespace Ultrapulito.Jarvix.Core {

    [Serializable]
    public abstract class TableModel : ViewModel {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public abstract string SP_WRITE { get; }        


        public TableModel() { }


        /// <summary>
        /// Inserisce nel database un record corrispondete all'oggetto e copia nell'oggetto l'id del record appena inserito.        
        /// </summary>
        /// <returns>void</returns>
        public void Insert() {
            // ricavo il nome della stored procedure da eseguire
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty("SP_WRITE");
            MethodInfo getter = property.GetGetMethod();
            string stored_procedure = (string)getter.Invoke(this, null);
            if (stored_procedure == "") {
                throw new Exception("L'oggetto " + type.ToString() + " non specifica la stored procedure per il salvataggio");
            }
            // ricavo i parametri dell'oggetto da passare alla stored
            Hashtable parameters = this.GetProperties();
            parameters.Add("@@ACTION", "INSERT");
            // eseguo la stored
            Dao dao = new Dao();
            DataSet dati = dao.ExecuteStoredProcedure(stored_procedure, parameters);
            // salvo l'id appena inserito nell'oggetto         
            int identity = Convert.ToInt32(dati.Tables[0].Rows[0]["Id"]);
            PropertyInfo idProperty = type.GetProperty("Id");
            MethodInfo setter = idProperty.GetSetMethod();
            object[] args = new object[] { identity };
            setter.Invoke(this, args);
        }


        /// <summary>
        /// Modifica nel database il record corrispondente all'oggetto.        
        /// </summary>
        /// <returns>void</returns>
        public void Update() {
            // ricavo il nome della stored procedure da eseguire
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty("SP_WRITE");
            MethodInfo getter = property.GetGetMethod();
            string stored_procedure = (string)getter.Invoke(this, null);
            if (stored_procedure == "") {
                throw new Exception("L'oggetto " + type.ToString() + " non specifica la stored procedure per il salvataggio");
            }
            // ricavo i parametri dell'oggetto da passare alla stored
            Hashtable parameters = this.GetProperties();
            parameters.Add("@@ACTION", "UPDATE");
            // eseguo la stored
            Dao dao = new Dao();
            dao.ExecuteStoredProcedure(stored_procedure, parameters);
        }


        /// <summary>
        /// Elimina nel database il record corrispondente all'oggetto.        
        /// </summary>
        /// <returns>void</returns>
        public void Delete() {
            // ricavo il nome della stored procedure da eseguire
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty("SP_WRITE");
            MethodInfo getter = property.GetGetMethod();
            string stored_procedure = (string)getter.Invoke(this, null);
            if (stored_procedure == "") {
                throw new Exception("L'oggetto " + type.ToString() + " non specifica la stored procedure per il salvataggio");
            }
            // ricavo l'id dell'oggetto da passare alla stored
            Hashtable parameters = new Hashtable();
            PropertyInfo idProperty = type.GetProperty("Id");
            MethodInfo idGetter = idProperty.GetGetMethod();
            int id = (int)idGetter.Invoke(this, null);
            parameters.Add("Id", id);
            // specifico l'azione che la stored procedure deve eseguire
            parameters.Add("@@ACTION", "DELETE");
            // eseguo la stored
            Dao dao = new Dao();
            dao.ExecuteStoredProcedure(stored_procedure, parameters);
        }

    }
}
