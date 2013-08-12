using System;
using System.Collections.Generic;
using System.Configuration;
using log4net;
using Ultrapulito.Jarvix.Core;
using Volcano.TXIntegrator.Model;
using Volcano.TXIntegrator.Synchronizer.Model.TXTango;
using Volcano.TXIntegrator.Synchronizer.Model;


namespace Volcano.TXIntegrator.Synchronizer {

    static class TXTemp {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Login Login = null;


        /// <summary>Seleziona su TXTemp i viaggi attivi (non conclusi), interrogga TXTango, aggiorna i viaggi e registra l'evento su TXTemp.</summary>        
        /// <returns>void</returns>
        public static int SyncViaggi() {

            int viaggiSincronizzati = 0;

            List<TXPianificazioni> viaggiPianificati = TXPianificazioni.TXTemp_GetPianificazioniToSync(TipoPianificazione.VIAGGIO);
            TXViaggi viaggio = null;
            for (int i = 0; i < viaggiPianificati.Count; i++) {
                viaggio = new TXViaggi(viaggiPianificati[i].IdViaggio);
                Eventi evento = viaggio.TXGetStatus(Login);

                // se lo stato della pianificazione è cambiato inserisco l'evento e aggiorno la pianificazione
                if (viaggiPianificati[i].Stato != evento.Stato) {
                    evento.IdPianificazione = viaggiPianificati[i].Id;
                    evento.Insert();
                    viaggiPianificati[i].Stato = evento.Stato;
                    viaggiPianificati[i].SyncData = evento.SyncData;
                    viaggiPianificati[i].Update();
                }

                viaggiSincronizzati++;
            }

            return viaggiSincronizzati;
        }



        public static int ReportViaggi() {

            int reportViaggi = 0;

            List<TXPianificazioni> viaggiPianificati = TXPianificazioni.TXTemp_GetPianificazioniToReport();
            TXViaggi viaggio = null;
            Eventi evento = null;

            for (int i = 0; i < viaggiPianificati.Count; i++) {
                viaggio = new TXViaggi(viaggiPianificati[i].IdViaggio);                

                // report delle distanze percorse
                evento = viaggio.TXGetDistanceReport(Login);
                evento.IdPianificazione = viaggiPianificati[i].Id;
                evento.Insert();

                // report dei consumi
                evento = viaggio.TXGetConsumptionReport(Login);
                evento.IdPianificazione = viaggiPianificati[i].Id;
                evento.Insert();

                // report dei costi
                evento = viaggio.TXGetCostReport(Login);
                evento.IdPianificazione = viaggiPianificati[i].Id;
                evento.Insert();

                // report dei rifornimenti                    
                evento = viaggio.TXGetRefuelReport(Login);
                evento.IdPianificazione = viaggiPianificati[i].Id;
                evento.Insert();

                // report dei transiti attraversati                
                evento = viaggio.TXGetTransitsReport(Login);
                evento.IdPianificazione = viaggiPianificati[i].Id;
                evento.Insert();                

                // chiudo la pianificazione (stato = closed)                        
                viaggiPianificati[i].Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_CLOSED"];
                viaggiPianificati[i].SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"]; ;
                viaggiPianificati[i].SyncData = DateTime.Now;
                viaggiPianificati[i].Update();

                reportViaggi++;
            }

            return reportViaggi;

        }


        /// <summary>Seleziona su TXTemp le spedizioni aperte (non concluse) su TXTango, interrogga TXTango, aggiorna le spedizioni e registra l'evento su TXTemp.
        /// Se abilitata invia una notifica via mail al responsabile del viaggio quando la spedizione è terminata.</summary>        
        /// <returns>void</returns>
        public static int SyncSpedizioni() {

            int spedizioniSincronizzate = 0;

            List<TXPianificazioni> spedizioniPianificate = TXPianificazioni.TXTemp_GetPianificazioniToSync(TipoPianificazione.SPEDIZIONE);
            TXSpedizioni spedizione = null;
            for (int i = 0; i < spedizioniPianificate.Count; i++) {
                spedizione = new TXSpedizioni(spedizioniPianificate[i].IdViaggio, spedizioniPianificate[i].IdSpedizione);
                Eventi evento = spedizione.TXGetStatus(Login);

                // se lo stato è cambiato registro l'evento e aggiorno la spedizione
                if (spedizioniPianificate[i].Stato != evento.Stato) {
                    evento.IdPianificazione = spedizioniPianificate[i].Id;
                    evento.Insert();
                    spedizioniPianificate[i].Stato = evento.Stato;
                    spedizioniPianificate[i].SyncData = evento.SyncData;
                    spedizioniPianificate[i].Update();
                }

                // se la spedizione è conclusa invio una notifica
                if (spedizioniPianificate[i].Stato == ConfigurationManager.AppSettings["TXTANGO_STATO_FINISHED"]) {
                    Boolean notificationsEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["ENABLE_EMAIL_NOTIFICATIONS"]);
                    TXViaggi viaggio = new TXViaggi(spedizione.IdViaggio);
                    if (notificationsEnabled && viaggio.MailUtenteCompetenza.Trim() != "") {
                        try {
                            List<string> addresses = new List<string>();
                            addresses.Add(viaggio.MailUtenteCompetenza);
                            string subject = "Spedizione nr. " + spedizione.KeySpedizione + " conclusa.";
                            string body = "<strong>DATI SPEDIZIONE:</strong><br/><br/>";
                            body += "Viaggio nr: " + viaggio.KeyViaggio + "<br/>";
                            body += "Spedizione nr: " + spedizione.KeySpedizione + "<br/>";
                            body += "Mittente: " + spedizione.MittenteRagSoc + "<br/>";
                            body += "Destinatario: " + spedizione.DestinazioneRagSoc + "<br/>";
                            body += "Destinazione: " + spedizione.DestinazioneIndirizzo.Trim() + ", " + spedizione.DestinazioneCAP.Trim() + " " + spedizione.DestinazioneLocalita.Trim() + " (" + spedizione.DestinazioneProvincia.Trim() + "), " + spedizione.DestinazioneNazione.Trim() + "<br/>";
                            if (spedizione.Tipo == ConfigurationManager.AppSettings["TXTEMP_SPEDIZIONE_RITIRO"]) {
                                body += "Tipo: ORDINE DI RITIRO<br/>";
                            } else {
                                body += "Tipo: CONSEGNA<br/>";
                            }
                            body += "Colli: " + spedizione.Colli + "<br/>";
                            body += "Peso: " + spedizione.Peso + "<br/>";
                            body += "Volume: " + spedizione.Volume + "<br/><br/>";
                            body += "Stato: CONCLUSA<br/>";
                            body += "Data Evento: " + evento.Data.Value.ToString("dd/MM/yyyy HH:mm:ss") + "<br/>";

                            Mailer.SendMail(addresses, subject, body);

                        } catch (Exception ex) {
                            log.Error(ex.Message, ex);
                        }
                    }
                }

                spedizioniSincronizzate++;
            }

            return spedizioniSincronizzate;
        }

    }
}
