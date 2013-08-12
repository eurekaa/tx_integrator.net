using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Ultrapulito.Jarvix.Core;
using Volcano.TXIntegrator.Model;

namespace Volcano.TXIntegrator.Synchronizer.Model {

    public enum TipoPianificazione { VIAGGIO, SPEDIZIONE };


    public partial class TXPianificazioni : Pianificazioni {


        #region metodi statici
        
        /// <summary>Cambia lo stato delle pianificazioni in modo che possano essere elaborate senza interferenze.
        /// Ritorna le pianificazioni da mandare a TXTango (devono essere state precedentemente lockate dalla funzione TXSync_LockPianificazioni).</summary>
        /// <param name="tipoPianificazione">Il tipo di pianificazione (viaggio, spedizione, ec).</param>                        
        /// <returns>List</returns>
        public static List<TXPianificazioni> TXTango_GetPianificazioniToSync(TipoPianificazione tipoPianificazione) {

            List<TXPianificazioni> pianificazioni = null;            
            Dao dao = new Dao();
            Hashtable parameters = new Hashtable();            

            // metto lo stato "LOCKED" alle pianificazioni da inviare a TXTango e le estraggo
            parameters.Add("@TipoPianificazione", tipoPianificazione.ToString());
            parameters.Add("@OldSyncStato", ConfigurationManager.AppSettings["TXTEMP_STATO_DA_SINCRONIZZARE"]);            
            parameters.Add("@NewSyncStato", ConfigurationManager.AppSettings["TXTEMP_STATO_IN_USO"]);
            DataSet data = dao.ExecuteStoredProcedure("TX.Pianificazioni_GetTXTangoToSync", parameters);            
            if (data.Tables.Count > 0) {
                pianificazioni = new List<TXPianificazioni>();
                TXPianificazioni pianificazione = null;
                for (int i = 0; i < data.Tables[0].Rows.Count; i++) {
                    pianificazione = new TXPianificazioni(data.Tables[0].Rows[i]);
                    pianificazioni.Add(pianificazione);
                }
            }

            return pianificazioni;

        }


        /// <summary>Ritorna le pianificazioni da controllare su TXTango e aggiornare sul database (stato diverso da CLOSED [FINISHED x le spedizioni] o CANCELED).</summary>
        /// <param name="tipoPianificazione">Il tipo di pianificazione (viaggio, spedizione, ec).</param>                        
        /// <returns>List</returns>
        public static List<TXPianificazioni> TXTemp_GetPianificazioniToSync(TipoPianificazione tipoPianificazione) {

            List<TXPianificazioni> pianificazioni = null;
            Dao dao = new Dao();
            Hashtable parameters = new Hashtable();

            parameters.Add("@TipoPianificazione", tipoPianificazione.ToString());
            parameters.Add("@StatoFinished", ConfigurationManager.AppSettings["TXTANGO_STATO_FINISHED"]);
            parameters.Add("@StatoClosed", ConfigurationManager.AppSettings["TXTANGO_STATO_CLOSED"]);
            parameters.Add("@StatoCancelled", ConfigurationManager.AppSettings["TXTANGO_STATO_CANCELED"]);
            parameters.Add("@StatoNotDelivered", ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"]);
            DataSet data = dao.ExecuteStoredProcedure("TX.Pianificazioni_GetTXTempToSync", parameters);
            if (data.Tables.Count > 0) {
                pianificazioni = new List<TXPianificazioni>();
                TXPianificazioni pianificazione = null;
                for (int i = 0; i < data.Tables[0].Rows.Count; i++) {
                    pianificazione = new TXPianificazioni(data.Tables[0].Rows[i]);
                    pianificazioni.Add(pianificazione);
                }
            }

            return pianificazioni;

        }


        /// <summary>Ritorna le pianificazioni (viaggi) in stato "FINISHED" la cui data ha il ritardo stabilito (per il delay sui report).</summary>      
        /// <returns>List</returns>
        public static List<TXPianificazioni> TXTemp_GetPianificazioniToReport() {

            List<TXPianificazioni> pianificazioni = null;
            Dao dao = new Dao();
            Hashtable parameters = new Hashtable();
            
            parameters.Add("@StatoFinished", ConfigurationManager.AppSettings["TXTANGO_STATO_FINISHED"]);
            parameters.Add("@ReportDelay", ConfigurationManager.AppSettings["TXTANGO_REPORT_DELAY"]);

            DataSet data = dao.ExecuteStoredProcedure("TX.Pianificazioni_GetTXTempToReport", parameters);
            if (data.Tables.Count > 0) {
                pianificazioni = new List<TXPianificazioni>();
                TXPianificazioni pianificazione = null;
                for (int i = 0; i < data.Tables[0].Rows.Count; i++) {
                    pianificazione = new TXPianificazioni(data.Tables[0].Rows[i]);
                    pianificazioni.Add(pianificazione);
                }
            }

            return pianificazioni;
        }

        #endregion


        #region costruttori

        public TXPianificazioni(DataRow pianificazione) {
            this.CreateObject(pianificazione);
        }

        #endregion



        #region metodi privati


        /// <summary>Riempie l'oggetto a partire da un DataRow.</summary>
        /// <param name="pianificazione">DataRow contenente i dati della pianificazione</param>                        
        /// <returns>void<Pianificazione></returns>
        private void CreateObject(DataRow pianificazione) {
            this.Id = (Int32)pianificazione["Id"];
            if (pianificazione["IdViaggio"] != DBNull.Value) {
                this.IdViaggio = (int)pianificazione["IdViaggio"];
            } else {
                this.IdViaggio = null;
            }
            if (pianificazione["IdSpedizione"] != DBNull.Value) {
                this.IdSpedizione = (int)pianificazione["IdSpedizione"];
            } else { 
                 this.IdSpedizione = null;
            }
            this.Stato = (pianificazione["Stato"] != DBNull.Value) ? (string)pianificazione["Stato"] : null;
            this.SyncStato = (pianificazione["SyncStato"] != DBNull.Value) ? (string)pianificazione["SyncStato"] : null;
            this.SyncTask = (pianificazione["SyncTask"] != DBNull.Value) ? (string)pianificazione["SyncTask"] : null;
        }

        #endregion



        #region metodi pubblici

        #endregion

    }
}
