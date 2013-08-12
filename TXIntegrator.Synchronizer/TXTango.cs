using System;
using System.Collections.Generic;
using System.Configuration;
using log4net;
using Volcano.TXIntegrator.Model;
using Volcano.TXIntegrator.Synchronizer.Model.TXTango;
using Volcano.TXIntegrator.Synchronizer.Model;

namespace Volcano.TXIntegrator.Synchronizer {
    static class TXTango {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Login Login = null;


        /// <summary>Crea e ritorna l'oggetto Login necessario per interrogare TXTango.</summary>        
        /// <returns>Login</returns>
        public static Login GetLogin() {
            Login login = new Login();
            login.Dispatcher = ConfigurationManager.AppSettings["TXTANGO_LOGIN_DISPATCHER"];
            login.Password = ConfigurationManager.AppSettings["TXTANGO_LOGIN_PASSWORD"];
            login.SystemNr = Convert.ToInt32(ConfigurationManager.AppSettings["TXTANGO_LOGIN_SYSTEMNR"]);
            login.Integrator = ConfigurationManager.AppSettings["TXTANGO_LOGIN_INTEGRATOR"];
            login.Language = ConfigurationManager.AppSettings["TXTANGO_LOGIN_LANGUAGE"];

            return login;
        }


        /// <summary>Seleziona su TXTemp i viaggi da sincronizzare su TXTango, li Inserisce|Modifica|Elimina su TXTango e registra l' evento su TXTemp.</summary>        
        /// <returns>void</returns>
        public static int SyncViaggi() {

            int viaggiSincronizzati = 0;
            Eventi evento = null;
            TXViaggi viaggio = null;

            // estraggo i viaggi da mandare a TXTango
            List<TXPianificazioni> viaggiPianificati = TXPianificazioni.TXTango_GetPianificazioniToSync(TipoPianificazione.VIAGGIO);
            // sincronizzo i viaggi su TXTango                
            for (int i = 0; i < viaggiPianificati.Count; i++) {
                viaggio = new TXViaggi(viaggiPianificati[i].IdViaggio);   
                
                if (viaggiPianificati[i].SyncTask == ConfigurationManager.AppSettings["TXTANGO_TASK_INSERT"]) {
                    evento = viaggio.TXInsert(Login);
                } else if (viaggiPianificati[i].SyncTask == ConfigurationManager.AppSettings["TXTANGO_TASK_UPDATE"] && viaggiPianificati[i].Stato != ConfigurationManager.AppSettings["TXTANGO_STATO_BUSY"]) {                                        
                    evento = viaggio.TXUpdate(Login);
                } else if (viaggiPianificati[i].SyncTask == ConfigurationManager.AppSettings["TXTANGO_TASK_DELETE"] && viaggiPianificati[i].Stato != ConfigurationManager.AppSettings["TXTANGO_STATO_BUSY"]) {                                        
                    evento = viaggio.TXDelete(Login);                    
                }

                // registro l'evento
                if (evento != null) {
                    evento.IdPianificazione = viaggiPianificati[i].Id;
                    evento.Insert();
                    // registro lo stato della pianificazione
                    viaggiPianificati[i].Stato = evento.Stato;
                    viaggiPianificati[i].SyncStato = evento.SyncStato;
                    viaggiPianificati[i].SyncTask = null;
                    viaggiPianificati[i].SyncData = evento.SyncData;
                    viaggiPianificati[i].Update();

                    viaggiSincronizzati++;
                }
            }

            return viaggiSincronizzati;
        }


        /// <summary>Seleziona su TXTemp le spedizioni da sincronizzare su TXTango, le Inserice|Modifica|Elimina su TXTango e registra l' evento su TXTemp.</summary>        
        /// <returns>void</returns>
        public static int SyncSpedizioni() {

            int spedizioniSincronizzate = 0;
            Eventi evento = null;
            TXSpedizioni spedizione = null;

            // estraggo le spedizioni da mandare a TXTango
            List<TXPianificazioni> spedizioniPianificate = TXPianificazioni.TXTango_GetPianificazioniToSync(TipoPianificazione.SPEDIZIONE);
            // sincronizzo le spedizioni su TXTango                
            for (int i = 0; i < spedizioniPianificate.Count; i++) {
                spedizione = new TXSpedizioni(spedizioniPianificate[i].IdViaggio, spedizioniPianificate[i].IdSpedizione);
                if (spedizioniPianificate[i].SyncTask == ConfigurationManager.AppSettings["TXTANGO_TASK_INSERT"]) {
                    evento = spedizione.TXInsert(Login);
                } else if (spedizioniPianificate[i].SyncTask == ConfigurationManager.AppSettings["TXTANGO_TASK_UPDATE"] && spedizioniPianificate[i].Stato != ConfigurationManager.AppSettings["TXTANGO_STATO_BUSY"]) {
                    evento = spedizione.TXUpdate(Login);
                } else if (spedizioniPianificate[i].SyncTask == ConfigurationManager.AppSettings["TXTANGO_TASK_DELETE"] && spedizioniPianificate[i].Stato != ConfigurationManager.AppSettings["TXTANGO_STATO_BUSY"]) {
                    evento = spedizione.TXDelete(Login);
                }

                // registro l'evento
                if (evento != null) {
                    evento.IdPianificazione = spedizioniPianificate[i].Id;
                    evento.Insert();
                    // registro lo stato della pianificazione
                    spedizioniPianificate[i].Stato = evento.Stato;
                    spedizioniPianificate[i].SyncStato = evento.SyncStato;
                    spedizioniPianificate[i].SyncTask = null;
                    spedizioniPianificate[i].SyncData = evento.SyncData;
                    spedizioniPianificate[i].Update();

                    spedizioniSincronizzate++;
                }
            }

            return spedizioniSincronizzate;
        }


    }
}
