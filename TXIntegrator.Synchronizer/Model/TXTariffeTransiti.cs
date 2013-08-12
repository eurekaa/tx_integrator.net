using System;
using System.Collections.Generic;
using System.Data;
using log4net;
using Ultrapulito.Jarvix.Core;
using Volcano.TXIntegrator.Model;

namespace Volcano.TXIntegrator.Synchronizer.Model {

    public class TXTariffeTransiti : TariffeTransiti {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #region metodi statici

        /// <summary>Seleziona tutte le tariffe transiti e le ritorna dentro una lista di oggetti "TariffaTransito".
        /// Tenta anche di geolocalizzare i transiti, quelli che non potranno essere convertiti in coordinate non saranno inseriti nella lista.</summary>        
        /// <returns>List</returns>
        public static List<TXTariffeTransiti> GetTariffeTransiti() {
            List<TXTariffeTransiti> tariffeTransiti = new List<TXTariffeTransiti>();
            Dao dao = new Dao();
            string sql = "SELECT * FROM TariffeTransiti";
            DataSet data = dao.ExecuteQuery(sql);
            if (data.Tables.Count > 0) {
                TXTariffeTransiti tariffaTransito = null;
                for (int i = 0; i < data.Tables[0].Rows.Count; i++) {
                    tariffaTransito = new TXTariffeTransiti();
                    tariffaTransito.CreateObject(data.Tables[0].Rows[i]);
                    tariffeTransiti.Add(tariffaTransito);                                    
                }
            }
            return tariffeTransiti;
        }



        #endregion



        #region metodi privati

        /// <summary>Crea l'oggetto a partire da un DataRow.</summary>        
        /// <returns>void</returns>
        private void CreateObject(DataRow tariffa) {
            this.Id = (tariffa["Id"] != DBNull.Value) ? Convert.ToInt32(tariffa["Id"]) : 0;
            this.Nome = (tariffa["Nome"] != DBNull.Value) ? Convert.ToString(tariffa["Nome"]).Trim() : "";
            this.Indirizzo = (tariffa["Indirizzo"] != DBNull.Value) ? Convert.ToString(tariffa["Indirizzo"]).Trim() : "";
            this.Cap = (tariffa["Cap"] != DBNull.Value) ? Convert.ToString(tariffa["Cap"]).Trim() : "";
            this.Citta = (tariffa["Citta"] != DBNull.Value) ? Convert.ToString(tariffa["Citta"]).Trim() : "";
            this.Provincia = (tariffa["Provincia"] != DBNull.Value) ? Convert.ToString(tariffa["Provincia"]).Trim() : "";
            this.Nazione = (tariffa["Nazione"] != DBNull.Value) ? Convert.ToString(tariffa["Nazione"]).Trim() : "";
            this.GeoCoordinate = (tariffa["GeoCoordinate"] != DBNull.Value) ? Convert.ToString(tariffa["GeoCoordinate"]).Trim() : "";
            this.Prezzo = (tariffa["Prezzo"] != DBNull.Value) ? Convert.ToDouble(tariffa["Prezzo"]) : 0;
            this.Valuta = (tariffa["Valuta"] != DBNull.Value) ? Convert.ToString(tariffa["Valuta"]).Trim() : "";            
        }

        #endregion



        #region metodi pubblici

        /// <summary>Geolocalizza il transito e salva le coordinate sul database.</summary>        
        /// <returns>void</returns>
        public void GeoLocalizzaTransito() {
            string indirizzo = this.Indirizzo + ", " + this.Cap + " " + this.Citta + ", " + this.Provincia + " " + this.Nazione;
            string coordinate = GeoCoder.EncodeAddress(indirizzo);
            this.GeoCoordinate = coordinate;
            Dao dao = new Dao();
            string sql = "UPDATE TariffeTransiti SET GeoCoordinate = '" + this.GeoCoordinate + "' WHERE Id=" + this.Id;
            dao.ExecuteNonQuery(sql);
        }

        #endregion

    }
}
