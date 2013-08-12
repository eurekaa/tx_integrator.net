using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using log4net;


namespace Ultrapulito.Jarvix.Core {

    [Serializable]
    public abstract class ViewModel : BaseModel {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public abstract string SP_READ { get; }


        public ViewModel() { }


        /// <summary>
        /// Seleziona un record dal database e copia i dati nell'oggetto.
        /// L'oggetto deve specificare nella proprietà "SP_SELECT" la stored procedure da usare per l'estrazione dei dati.
        /// La stored procedure deve dichiarare un parametro "@Id" (la primary key della tabella).
        /// I nomi delle colonne del DataSet devono corrispondere ai nomi delle proprietà dell'oggetto.        
        /// (Se la tabella del database ha nomi diversi usare gli alias nella store procedure per risolvere il mapping).
        /// </summary>
        /// <param type="int" name="id">Primary key del record su database.</param>        
        /// <returns>void</returns>         
        public void Select(int? id) {            
            // ricavo il nome della stored procedure da eseguire
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty("SP_READ");
            MethodInfo getter = property.GetGetMethod();
            string stored_procedure = (string)getter.Invoke(this, null);
            if (stored_procedure == "") {
                throw new Exception("L'oggetto non specifica la stored procedure di select");
            }
            // setto i parametri da passare alla stored
            Hashtable parameters = new Hashtable();
            parameters.Add("@Id", id);
            // eseguo la stored
            Dao dao = new Dao();
            DataSet dati = dao.ExecuteStoredProcedure(stored_procedure, parameters);
            if (dati.Tables[0].Rows.Count > 0) {
                DataRow row = dati.Tables[0].Rows[0];
                // riempio l'oggetto
                this.FillObject(row);
            } else {
                throw new Exception("Nessun record trovato con id: " + id + " nella tabella " + type.ToString());
            }
        }


        /// <summary>
        /// Ricerca uno o più records sul database e ritorna una lista di oggetti corrispondenti.
        /// L'oggetto deve specificare nella proprietà "SP_SEARCH" la stored procedure da usare per l'estrazione dei dati.
        /// I nomi dei @parametri della stored procedure devono corrispondere ai nomi delle proprietà dell'oggetto.
        /// I nomi delle colonne del DataSet devono corrispondere ai nomi delle proprietà dell'oggetto.
        /// (Se la tabella del database ha nomi diversi usare gli alias nella store procedure per risolvere il mapping).
        /// </summary>
        /// <param type="Hashtable" name="parmeters">Nomi->valori dei campi da usare per la ricerca.</param>        
        /// <returns>List<dynamic></returns>
        public List<dynamic> Search(Hashtable parameters) {
            List<dynamic> modelList = null;
            // ricavo il nome della stored procedure da eseguire
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty("SP_READ");
            MethodInfo getter = property.GetGetMethod();
            string stored_procedure = (string)getter.Invoke(this, null);
            if (stored_procedure == "") {
                throw new Exception("L'oggetto " + type.ToString() + " non specifica la stored procedure di ricerca");
            }
            // eseguo la stored
            Dao dao = new Dao();
            DataSet dati = dao.ExecuteStoredProcedure(stored_procedure, parameters);
            if (dati != null) {
                modelList = new List<dynamic>();
                dynamic objectInstance = null;
                for (int i = 0; i < dati.Tables[0].Rows.Count; i++) {
                    DataRow row = dati.Tables[0].Rows[i];
                    // creo e riempio l'oggetto
                    objectInstance = System.Activator.CreateInstance(type, null);
                    MethodInfo fillMethod = type.GetMethod("FillObject");
                    object[] args = new object[] { row };
                    fillMethod.Invoke(objectInstance, args);
                    modelList.Add(objectInstance);
                }
            }

            return modelList;
        }


    }
}
