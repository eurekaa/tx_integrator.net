using System;
using System.Collections.Generic;
using System.Data;
using log4net;
using Ultrapulito.Jarvix.Core;
using System.Configuration;
using Volcano.TXIntegrator.Model;
using Volcano.TXIntegrator.Synchronizer.Model.TXTango;


namespace Volcano.TXIntegrator.Synchronizer.Model {

    public class TXSpedizioni : Spedizioni {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #region proprietà        
        public int? IdViaggio { get; set; }        
        #endregion



        #region costruttori
          
        public TXSpedizioni(int? id_viaggio, int? id_spedizione) {
            this.Select(id_spedizione);
            this.IdViaggio = id_viaggio;            
        }

        #endregion



        #region metodi privati

        /// <summary>Geolocalizza la spedizione e salva le coordinte sul database.</summary>        
        /// <returns>void</returns>
        private void GeoLocalizzaSpedizione() {
            string indirizzo = this.DestinazioneIndirizzo + ", " + this.DestinazioneCAP + " " + this.DestinazioneLocalita + ", " + this.DestinazioneProvincia + " " + this.DestinazioneNazione;
            string coordinate = GeoCoder.EncodeAddress(indirizzo);            
            this.DestinazioneGeoLoc = coordinate;
            Dao dao = new Dao();
            string sql = "UPDATE Spedizioni SET DestinazioneGeoLoc = '" + this.DestinazioneGeoLoc + "' WHERE Id=" + this.Id;
            dao.ExecuteNonQuery(sql);
        }


        /// <summary>Crea e riempie l'oggetto rappresentante la spedizione da mandare a TXTango.</summary>                   
        /// <returns>Insert_new_place_on_trip</returns>
        private Insert_new_place_on_trip TXCreateObject() {

            // preparo la spedizione                
            PlaceInsert place = new PlaceInsert();
            place.PlaceId = ConfigurationManager.AppSettings["TXTANGO_ID_PREFIX"] + this.Id;
            place.CustomNr = (int)this.Progressivo;
            string display = this.DestinazioneLocalita + " - " + this.DestinazioneRagSoc;
            if (display.Length >= 50) {
                display = display.Substring(0, 45) + "..";
            }
            place.DriverDisplay = display;
            place.OrderSeq = this.Ordinamento;
            place.Comment = this.DestinazioneIndirizzo + ", " + this.DestinazioneCAP + " " + this.DestinazioneLocalita + " " + this.DestinazioneProvincia + ", " + this.DestinazioneNazione + " " + this.Note;

            // inserisco la posizione (geo coordinate)
            double latitudine = double.Parse(this.DestinazioneGeoLoc.Split(',')[0].Replace(".", ","));
            double longitudine = double.Parse(this.DestinazioneGeoLoc.Split(',')[1].Replace(".", ","));
            place.Position = new Position() { Latitude = latitudine, Longitude = longitudine };

            // inserisco l'activity (carico o scarico)
            int activityId = 0;
            if (this.Tipo == ConfigurationManager.AppSettings["TXTEMP_SPEDIZIONE_RITIRO"]) {
                activityId = Convert.ToInt32(ConfigurationManager.AppSettings["TXTANGO_ACTIVITY_PLACE_LOAD"]);
            } else {
                activityId = Convert.ToInt32(ConfigurationManager.AppSettings["TXTANGO_ACTIVITY_PLACE_UNLOAD"]);
            }
            place.Activity = new ActivityPlace() { ID = activityId };

            // inserisco la spedizione nel viaggio
            Insert_new_place_on_trip insertPlace = new Insert_new_place_on_trip();
            insertPlace.TripID = ConfigurationManager.AppSettings["TXTANGO_ID_PREFIX"] + this.IdViaggio;
            insertPlace.TransferDate = null; // è possibile pianificare la data di trasmissione a TXTango
            insertPlace.Places = new PlaceInsert[] { place };

            return insertPlace;
        }


        #endregion



        #region metodi pubblici


        /// <summary>Interroga TXTango e ritorna un evento contenente lo stato della spedizione.</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                
        /// <returns>Eventi</returns>
        public Eventi TXGetStatus(TXTango.Login login) {

            PlanningSelection planningSelection = new PlanningSelection();
            PlanningItemSelection itemSelection = new PlanningItemSelection();
            itemSelection.PlanningSelectionType = enumPlanningItemSelectionType.PLACE;
            itemSelection.ID = ConfigurationManager.AppSettings["TXTANGO_ID_PREFIX"] + this.Id;
            //itemSelection.ParentID = ConfigurationManager.AppSettings["TXTANGO_ID_PREFIX"] + this.Viaggio.Id;
            planningSelection.Item = itemSelection;
            ServiceSoapClient service = new ServiceSoapClient();
            GetPlanningResult result = service.Get_Planning(login, planningSelection);

            Eventi evento = new Eventi();
            evento.XmlRequest = Serializer.SerializeObject(planningSelection, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);
            if (result.Errors.Length > 0) {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.Stato = "";
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Note = "Vedi XmlResponse per i dettagli sugli errori";
            } else {
                evento.Stato = result.ItemSelection.Places[0].Status.ToString();
                if (evento.Stato == ConfigurationManager.AppSettings["TXTANGO_STATO_FINISHED"]) {
                    evento.Data = result.ItemSelection.Places[0].EndDate.Value;
                }
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
            }
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_GET_STATUS"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_FROM_TXTANGO"];

            return evento;
        }



        /// <summary>Invia la spedizione a TXTango e ritorna l'evento relativo all'inserimento.</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                        
        /// <returns>Eventi</returns>
        public Eventi TXInsert(TXTango.Login login) {            
            Eventi evento = null;
            try {
                // geolocalizzo la spedizione                
                this.GeoLocalizzaSpedizione();
                
                // creo la spedizione nel formato TXTango e la invio al webservice
                Insert_new_place_on_trip insertPlace = this.TXCreateObject();
                ServiceSoapClient service = new ServiceSoapClient();
                PlanningResultInsertNewPlaceOnTrip result = service.Insert_New_Place_On_Trip(login, insertPlace);
                // creo l'evento e registro le informazioni necessarie
                evento = new Eventi();
                evento.Data = DateTime.Now;                
                evento.SyncData = DateTime.Now;                
                evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_INSERT"];
                evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_TO_TXTANGO"];
                evento.XmlRequest = Serializer.SerializeObject(insertPlace, SerializationType.XML);
                evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);
                if (result.Errors.Length > 0) {
                    log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                    evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"];
                    evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                    evento.Note = "Vedi XmlResponse per i dettagli sugli errori";
                } else {
                    evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_DELIVERED"];
                    evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
                }                

            // intercetto gli errori nella geocodifica
            } catch (GeocodingException ex) {
                // loggo l'errore
                log.Error(ex.Message, ex);

                // preparo l'evento di errore
                evento = new Eventi();
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"];
                evento.SyncData = DateTime.Now;
                evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_INSERT"];
                evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_TO_TXTango"];
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"];
                evento.Note = "Impossibile geocodifcare la spedizione, verificare la correttezza dei campi indirizzo.";

                // invio una mail al responsabile del viaggio
                Boolean notificationsEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["ENABLE_EMAIL_NOTIFICATIONS"]);
                TXViaggi viaggio = new TXViaggi(this.IdViaggio);
                if (notificationsEnabled && viaggio.MailUtenteCompetenza.Trim() != "") {
                    try {
                        List<string> addresses = new List<string>();
                        addresses.Add(viaggio.MailUtenteCompetenza);
                        string subject = "Spedizione nr. " + this.KeySpedizione + " non geolocalizzata.";
                        string body = "<strong>Attenzione:<br/>Si è verificato un errore nel tentativo di geolocalizzare la spedizione in oggetto.<br/>La spedizione non è stata quindi caricata sul computer di bordo.<br/>E' necessario rivedere e correggere l'indirizzo di destinazione.</strong><br/><br/>";
                        body += "<strong>DATI SPEDIZIONE:</strong><br/>";
                        body += "Viaggio nr: " + viaggio.KeyViaggio + "<br/>";
                        body += "Spedizione nr: " + this.KeySpedizione + "<br/>";
                        body += "Mittente: " + this.MittenteRagSoc + "<br/>";
                        body += "Destinatario: " + this.DestinazioneRagSoc + "<br/>";
                        body += "Destinazione: " + this.DestinazioneIndirizzo.Trim() + ", " + this.DestinazioneCAP.Trim() + " " + this.DestinazioneLocalita.Trim() + " (" + this.DestinazioneProvincia.Trim() + "), " + this.DestinazioneNazione.Trim() + "<br/>";
                        if (this.Tipo == ConfigurationManager.AppSettings["TXTEMP_SPEDIZIONE_RITIRO"]) {
                            body += "Tipo: ORDINE DI RITIRO<br/>";
                        } else {
                            body += "Tipo: CONSEGNA<br/>";
                        }
                        body += "Colli: " + this.Colli + "<br/>";
                        body += "Peso: " + this.Peso + "<br/>";
                        body += "Volume: " + this.Volume + "<br/><br/>";

                        Mailer.SendMail(addresses, subject, body);
                    } catch (Exception e) {
                        log.Error(e.Message, e);
                    }
                }
            }

            return evento;
        }


        /// <summary>Aggiorna la spedizione su TXTango e ritorna l'evento relativo all'aggiornamento.</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                        
        /// <returns>Eventi</returns>
        public Eventi TXUpdate(TXTango.Login login) {
            Eventi evento = null;
            try {
                // geolocalizzo la spedizione                
                this.GeoLocalizzaSpedizione();

                // creo la spedizione nel formato TXTango e la invio al webservice
                Insert_new_place_on_trip insertPlace = this.TXCreateObject();
                ServiceSoapClient service = new ServiceSoapClient();
                PlanningResultInsertNewPlaceOnTrip result = service.Update_Place_On_Trip(login, insertPlace);
                // creo l'evento e registro le informazioni necessarie
                evento = new Eventi();                             
                evento.SyncData = DateTime.Now;
                evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_UPDATE"];
                evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_TO_TXTango"];
                evento.XmlRequest = Serializer.SerializeObject(insertPlace, SerializationType.XML);
                evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);
                if (result.Errors.Length > 0) {
                    log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                    evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                    evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"];
                    evento.Note = "Vedi XmlResponse per i dettagli sugli errori";
                } else {
                    evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_DELIVERED"];
                    evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
                }


            // intercetto gli errori nella geocodifica
            } catch (GeocodingException ex) {
                // loggo l'errore
                log.Error(ex.Message, ex);
                
                // preparo l'evento di errore
                evento = new Eventi();
                evento.SyncData = DateTime.Now;
                evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_UPDATE"];
                evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_TO_TXTango"];
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"];
                evento.Note = "Impossibile geocodifcare la spedizione, verificare la correttezza dei campi indirizzo.";

                // invio una mail al responsabile del viaggio
                Boolean notificationsEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["ENABLE_EMAIL_NOTIFICATIONS"]);
                TXViaggi viaggio = new TXViaggi(this.IdViaggio);
                if (notificationsEnabled && viaggio.MailUtenteCompetenza.Trim() != "") {
                    try {
                        List<string> addresses = new List<string>();
                        addresses.Add(viaggio.MailUtenteCompetenza);
                        string subject = "Spedizione nr. " + this.KeySpedizione + " non geolocalizzata.";
                        string body = "<strong>Attenzione:<br/>Si è verificato un errore nel tentativo di geolocalizzare la spedizione in oggetto.<br/>La spedizione non è stata quindi caricata sul computer di bordo.<br/>E' necessario rivedere e correggere l'indirizzo di destinazione.</strong><br/><br/>";
                        body += "<strong>DATI SPEDIZIONE:</strong><br/>";
                        body += "Viaggio nr: " + viaggio.KeyViaggio + "<br/>";
                        body += "Spedizione nr: " + this.KeySpedizione + "<br/>";
                        body += "Mittente: " + this.MittenteRagSoc + "<br/>";
                        body += "Destinatario: " + this.DestinazioneRagSoc + "<br/>";
                        body += "Destinazione: " + this.DestinazioneIndirizzo.Trim() + ", " + this.DestinazioneCAP.Trim() + " " + this.DestinazioneLocalita.Trim() + " (" + this.DestinazioneProvincia.Trim() + "), " + this.DestinazioneNazione.Trim() + "<br/>";
                        if (this.Tipo == ConfigurationManager.AppSettings["TXTEMP_SPEDIZIONE_RITIRO"]) {
                            body += "Tipo: ORDINE DI RITIRO<br/>";
                        } else {
                            body += "Tipo: CONSEGNA<br/>";
                        }
                        body += "Colli: " + this.Colli + "<br/>";
                        body += "Peso: " + this.Peso + "<br/>";
                        body += "Volume: " + this.Volume + "<br/><br/>";

                        Mailer.SendMail(addresses, subject, body);

                    } catch (Exception e) {
                        log.Error(e.Message, e);
                    }
                }
            }

            return evento;
        }


        /// <summary>Cancella la spedizione su TXTango e ritorna l'evento relativo all'eliminazione.</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                        
        /// <returns>Eventi</returns>
        public Eventi TXDelete(TXTango.Login login) {
            PlanningItemSelection planning = new PlanningItemSelection();
            planning.PlanningSelectionType = enumPlanningItemSelectionType.PLACE;
            planning.ParentID = ConfigurationManager.AppSettings["TXTANGO_ID_PREFIX"] + this.IdViaggio;
            planning.ID = ConfigurationManager.AppSettings["TXTANGO_ID_PREFIX"] + this.Id;
            ServiceSoapClient service = new ServiceSoapClient();
            ExecutionResult result = service.Cancel_Planning(login, planning);
            Eventi evento = new Eventi();
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_DELETE"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_TO_TXTANGO"];
            if (result.Errors.Length > 0) {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"];
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Note = "Vedi XmlResponse per i dettagli sugli errori";
            } else {
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_CANCELED"];
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
            }
            evento.XmlRequest = Serializer.SerializeObject(planning, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);

            return evento;
        }


        #endregion


    }
}
