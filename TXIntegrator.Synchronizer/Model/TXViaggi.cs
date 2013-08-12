using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using log4net;
using Ultrapulito.Jarvix.Core;
using Volcano.TXIntegrator.Model;
using Volcano.TXIntegrator.Synchronizer.Model.TXTango;


namespace Volcano.TXIntegrator.Synchronizer.Model {

    public class TXViaggi : Viaggi {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        

        #region costruttori

        public TXViaggi(int? id_viaggio) {
            base.Select(id_viaggio);
        }

        #endregion



        #region metodi privati


        /// <summary>Crea e riempie l'oggetto "viaggio" da mandare a TXTango.</summary>
        /// <returns>PlanningInsert</returns>
        private PlanningInsert TXCreateObject() {
            // preparo il viaggio
            TripInsert trip = new TripInsert();
            trip.TripId = ConfigurationManager.AppSettings["TXTANGO_ID_PREFIX"] + this.Id;
            trip.CustomNr = (int)this.Progressivo;
            trip.Comment = this.Note;
            trip.ExecutionDate = DateTime.Now;
            trip.DriverDisplay = this.KeyViaggio;
            trip.References = new Reference() { InternalReference = this.UtenteCompetenza };
            trip.StartTripAct = new Activity() { ID = Convert.ToInt32(ConfigurationManager.AppSettings["TXTANGO_ACTIVITY_TRIP_START"]) };
            trip.StopTripAct = new Activity() { ID = Convert.ToInt32(ConfigurationManager.AppSettings["TXTANGO_ACTIVITY_TRIP_STOP"]) };
            trip.Overwrite = true;

            // preparo il planning
            PlanningInsert planning = new PlanningInsert();
            planning.Vehicle = new IdentifierVehicle() { IdentifierVehicleType = enumIdentifierVehicleType.TRANSICS_ID, Id = this.CodiceMezzo };
            planning.Driver = new Identifier() { IdentifierType = enumIdentifierType.TRANSICS_ID, Id = this.CodiceAutista };
            planning.Trips = new TripInsert[] { trip };
            planning.TransferDate = null; // è posibile pianificare la data di trasmissione a TXTango

            return planning;
        }

        #endregion



        #region metodi pubblici


        /// <summary>Interroga TXTango e ritorna un evento contenente lo stato del viaggio.
        /// Si occupa inoltre di salvare le date di inizio e fine viaggio quanto occorrono i rispettivi eventi.</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                
        /// <returns>Eventi</returns>
        public Eventi TXGetStatus(TXTango.Login login) {            

            // richiedo a TXTango lo stato del viaggio
            PlanningSelection planningSelection = new PlanningSelection();
            PlanningItemSelection itemSelection = new PlanningItemSelection();
            itemSelection.PlanningSelectionType = enumPlanningItemSelectionType.TRIP;
            itemSelection.ID = ConfigurationManager.AppSettings["TXTANGO_ID_PREFIX"] + this.Id;
            planningSelection.Item = itemSelection;
            ServiceSoapClient service = new ServiceSoapClient();
            GetPlanningResult result = service.Get_Planning(login, planningSelection);

            // creo e ritorno l'evento relativo allo stato di viaggio
            Eventi evento = new Eventi();
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_GET_STATUS"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_FROM_TXTANGO"];
            evento.XmlRequest = Serializer.SerializeObject(planningSelection, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);

            if (result.Errors.Length > 0) {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Note = "Vedi XmlResponse per i dettagli sugli errori.";

            } else {

                // se il viaggio è partito registro la data di inizio viaggio
                if (result.ItemSelection.Trips[0].Status.ToString() == ConfigurationManager.AppSettings["TXTANGO_STATO_BUSY"]) {
                    this.DataInizio = result.ItemSelection.Trips[0].StartDate.Value;
                    this.Update();
                }

                // se il viaggio è terminato registro la data di fine viaggio
                if (result.ItemSelection.Trips[0].Status.ToString() == ConfigurationManager.AppSettings["TXTANGO_STATO_FINISHED"]) {
                    this.DataFine = result.ItemSelection.Trips[0].EndDate.Value;
                    this.Update();
                }

                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
                evento.Stato = result.ItemSelection.Trips[0].Status.ToString();
                if (evento.Stato == ConfigurationManager.AppSettings["TXTANGO_STATO_FINISHED"]) {
                    evento.Data = this.DataFine;
                } else if (this.DataInizio != DateTime.MinValue) {
                    evento.Data = this.DataInizio;
                }

            }

            return evento;
        }


        /// <summary>Interroga TXTango riguardo ai kilometri percorsi, aggiorna nel database la tabella "Viaggi" e ritorna un evento contenente lo stato del viaggio impostato a "CLOSED".</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                
        /// <returns>Eventi</returns>
        public Eventi TXGetDistanceReport(TXTango.Login login) {            

            // se non sono presenti le date di inizio e fine viaggio non posso ricavare correttamente le distanze di viaggio             
            if (this.DataInizio == DateTime.MinValue || this.DataFine == DateTime.MinValue) {
                throw new Exception("Impossibile ricavare le distanze poichè non sono presenti le date di inizio o fine viaggio. IdViaggio: " + this.Id + " KeyViaggio: " + this.KeyViaggio);
            }

            ServiceSoapClient service = new ServiceSoapClient();

            // chiamata per le distanze percorse
            DistanceReportSelection reportSelection = new DistanceReportSelection();
            reportSelection.DateTimeRangeSelection = new DateTimeRangeSelection() { StartDate = (DateTime)this.DataInizio, EndDate = this.DataFine };
            reportSelection.Vehicles = new IdentifierVehicle[1];
            reportSelection.Vehicles[0] = new IdentifierVehicle() { IdentifierVehicleType = enumIdentifierVehicleType.TRANSICS_ID, Id = this.CodiceMezzo };
            reportSelection.Drivers = new Identifier[1];
            reportSelection.Drivers[0] = new Identifier() { IdentifierType = enumIdentifierType.TRANSICS_ID, Id = this.CodiceAutista };
            reportSelection.SummeryLevel = SummaryLevel.OnlyTotal;
            GetDistanceReport result = service.Get_DistanceReportSummary(login, reportSelection);

            Eventi evento = new Eventi();
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_GET_DISTANCES"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_FROM_TXTANGO"];
            evento.XmlRequest = Serializer.SerializeObject(reportSelection, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);
            evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_CLOSED"];

            if (result.Errors.Length == 0) {
                if (result.DistanceReportItems.Length > 0) {
                    this.KmInizio = result.DistanceReportItems[0].KmMin;
                    this.KmFine = result.DistanceReportItems[0].kmMax;
                    this.KmViaggio = (result.DistanceReportItems[0].Distance != null) ? result.DistanceReportItems[0].Distance.Value : 0;
                    this.Update();
                } else {
                    evento.Note = "Nessuna informazione pervenuta da TXTango.";
                }
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
            } else {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Note = "Vedi Xml Response per i dettagli sugli errori.";
            }

            return evento;
        }


        /// <summary>Interroga TXTango riguardo ai consumi (+ velocità media e ore di guida), aggiorna nel database la tabella "Viaggi" e ritorna un evento contenente lo stato del viaggio impostato a "CLOSED".</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                
        /// <returns>Eventi</returns>
        public Eventi TXGetConsumptionReport(TXTango.Login login) {           

            // se non sono presenti le date di inizio e fine viaggio non posso ricavare correttamente i consumi del viaggio             
            if (this.DataInizio == DateTime.MinValue || this.DataFine == DateTime.MinValue) {
                throw new Exception("Impossibile ricavare i consumi poichè non sono presenti le date di inizio o fine viaggio. IdViaggio: " + this.Id + " KeyViaggio: " + this.KeyViaggio);
            }

            ServiceSoapClient service = new ServiceSoapClient();

            // chiamata per i consumi
            ConsumptionReportSelection reportSelection = new ConsumptionReportSelection();
            reportSelection.DateTimeRangeSelection = new DateTimeRangeSelection() { StartDate = (DateTime)this.DataInizio, EndDate = this.DataFine };
            reportSelection.Drivers = new Identifier[1];
            reportSelection.Drivers[0] = new Identifier() { IdentifierType = enumIdentifierType.TRANSICS_ID, Id = this.CodiceAutista };
            reportSelection.SummaryLevel = SummaryLevel.OnlyTotal;
            GetConsumptionReportResult result = service.Get_ConsumptionReport(login, reportSelection);

            Eventi evento = new Eventi();
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_GET_CONSUMPTIONS"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_FROM_TXTANGO"];
            evento.XmlRequest = Serializer.SerializeObject(reportSelection, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);
            evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_CLOSED"];

            if (result.Errors.Length == 0) {
                if (result.ConsumptionReportItems.Length > 0) {
                    this.ConsumoLt = (result.ConsumptionReportItems[0].Consumption_Total != null) ? result.ConsumptionReportItems[0].Consumption_Total.Value : 0;
                    this.VelocitaMedia = (result.ConsumptionReportItems[0].Speed_Avg != null) ? result.ConsumptionReportItems[0].Speed_Avg.Value : 0;
                    this.OreGuida = (result.ConsumptionReportItems[0].Duration_Driving != null) ? result.ConsumptionReportItems[0].Duration_Driving.Value : 0;
                    this.Update();
                } else {
                    evento.Note = "Nessuna informazione pervenuta da TXTango.";
                }
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
            } else {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Note = "Vedi Xml Response per i dettagli sugli errori.";
            }

            return evento;
        }


        /// <summary>Interroga TXTango riguardo ai costi del viaggio, inserisce nel database le note spese, e ritorna un evento contenente lo stato del viaggio impostato a "CLOSED".</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                
        /// <returns>Eventi</returns>
        public Eventi TXGetCostReport(TXTango.Login login) {            

            Eventi evento = null;

            // se non sono presenti le date di inizio e fine viaggio non posso ricavare correttamente i costi di viaggio 
            // (si rischia di registrare nuovamente le spese di un viaggio antecedente).
            if (this.DataInizio == DateTime.MinValue || this.DataFine == DateTime.MinValue) {
                throw new Exception("Impossibile ricavare i costi poichè non sono presenti le date di inizio o fine viaggio. IdViaggio: " + this.Id + " KeyViaggio: " + this.KeyViaggio);
            }

            CostCompensationReportSelection reportSelection = new CostCompensationReportSelection();
            reportSelection.Vehicles = new IdentifierVehicle[1];
            reportSelection.Vehicles[0] = new IdentifierVehicle() { IdentifierVehicleType = enumIdentifierVehicleType.TRANSICS_ID, Id = this.CodiceMezzo };
            reportSelection.Drivers = new Identifier[1];
            reportSelection.Drivers[0] = new Identifier() { IdentifierType = enumIdentifierType.TRANSICS_ID, Id = this.CodiceAutista };
            reportSelection.DateTimeRangeSelection = new DateTimeRangeSelection() { StartDate = (DateTime)this.DataInizio, EndDate = this.DataFine };

            ServiceSoapClient service = new ServiceSoapClient();
            GetCostCompensationReportResult result = service.Get_CostAndCompensationReport(login, reportSelection);

            evento = new Eventi();
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_GET_COSTS"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_FROM_TXTANGO"];
            evento.XmlRequest = Serializer.SerializeObject(reportSelection, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);

            if (result.Errors.Length > 0) {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_FINISHED"];
                evento.Note = "Vedi XmlResponse per i dettagli sugli errori.";
            } else {
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
                evento.Note = "Registrate nr. " + result.CostConsumptionReportItems.Length + " note spese.";

                // inserisco le note spese nel database
                NoteSpese notaSpesa = null;

                for (int i = 0; i < result.CostConsumptionReportItems.Length; i++) {
                    notaSpesa = new NoteSpese();
                    notaSpesa.IdViaggio = this.Id;
                    notaSpesa.Data = result.CostConsumptionReportItems[i].Date;
                    notaSpesa.Tipo = result.CostConsumptionReportItems[i].CostAndCompensationType.ToString();
                    notaSpesa.Codice = result.CostConsumptionReportItems[i].CostCompensationCode.Description;
                    notaSpesa.Descrizione = "";
                    if (result.CostConsumptionReportItems[i].Position != null) {
                        notaSpesa.Indirizzo = (result.CostConsumptionReportItems[i].Position.AddressInfo!=null) ? result.CostConsumptionReportItems[i].Position.AddressInfo : "";
                        notaSpesa.GeoCoordinate = result.CostConsumptionReportItems[i].Position.Latitude + "," + result.CostConsumptionReportItems[i].Position.Longitude;
                    }
                    notaSpesa.Prezzo = result.CostConsumptionReportItems[i].Price.Value;
                    notaSpesa.Valuta = result.CostConsumptionReportItems[i].CurrencyCode;
                    notaSpesa.Insert();
                }

                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_CLOSED"];

            }

            return evento;
        }


        /// <summary>Interroga TXTango riguardo ai rifornimenti del viaggio, inserisce nel database la note spese, e ritorna un evento contenente lo stato del viaggio impostato a "CLOSED".</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                
        /// <returns>Eventi</returns>
        public Eventi TXGetRefuelReport(TXTango.Login login) {

            Eventi evento = null;

            // se non sono presenti le date di inizio e fine viaggio non posso ricavare correttamente i rifornimenti  
            // (si rischia di registrare nuovamente le spese di un viaggio antecedente).
            if (this.DataInizio == DateTime.MinValue || this.DataFine == DateTime.MinValue) {
                throw new Exception("Impossibile ricavare i rifornimenti poichè non sono presenti le date di inizio o fine viaggio. IdViaggio: " + this.Id + " KeyViaggio: " + this.KeyViaggio);
            }

            RefuelReportSelection reportSelection = new RefuelReportSelection();
            reportSelection.Vehicles = new IdentifierVehicle[1];
            reportSelection.Vehicles[0] = new IdentifierVehicle() { IdentifierVehicleType = enumIdentifierVehicleType.TRANSICS_ID, Id = this.CodiceMezzo };
            reportSelection.DateTimeRangeSelection = new DateTimeRangeSelection() { StartDate = (DateTime)this.DataInizio, EndDate = this.DataFine };

            ServiceSoapClient service = new ServiceSoapClient();
            GetRefuelReport result = service.Get_RefuelReport(login, reportSelection);

            evento = new Eventi();
            evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_CLOSED"];
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_GET_REFUELS"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_FROM_TXTANGO"];
            evento.XmlRequest = Serializer.SerializeObject(reportSelection, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);

            if (result.Errors.Length > 0) {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Note = "Vedi XmlResponse per i dettagli sugli errori.";

            } else {

                // per ogni rifornimento creo una nota spese 
                NoteSpese notaSpesa = null;
                TariffeCarburante tariffaCarburante = null;                
                DateTime dataRifornimento = DateTime.MinValue;
                string tipoCarburante = "";
                double litri = 0;
                string distributore = "";
                string nazione = "";                
                int costiCalcolati = 0;

                for (int i = 0; i < result.FuelReportItems.Length; i++) {
                    try {                        
                        // dati del rifornimento
                        nazione = result.FuelReportItems[i].CountryCode;
                        litri = result.FuelReportItems[i].Quantity;
                        if (result.FuelReportItems[i].BeginDate != null) {
                            dataRifornimento = result.FuelReportItems[i].BeginDate.Value;
                        }
                        if (result.FuelReportItems[i].AddressInfo != null) {
                            distributore = result.FuelReportItems[i].AddressInfo.Name.ToUpper();                            
                        }
                        if(result.FuelReportItems[i].FuelType != null){
                            tipoCarburante = result.FuelReportItems[i].FuelType.Name.ToUpper();
                        }
                        //tipoCarburante = (tipoCarburante != "") ? tipoCarburante : "DIESEL";

                        // inizializzo la nota spesa
                        notaSpesa = new NoteSpese();
                        notaSpesa.IdViaggio = this.Id;
                        notaSpesa.Codice = "Refuel";
                        notaSpesa.Tipo = "COST";
                        notaSpesa.Data = dataRifornimento;
                        notaSpesa.Indirizzo = nazione;
                        notaSpesa.Descrizione = "Rifornimento Distributore: " + distributore;
                        notaSpesa.Note = "Carburante: " + tipoCarburante + " - Litraggio: " + litri;

                        // cerco la tariffa carburante    
                        tariffaCarburante = TXTariffeCarburante.GetTariffa(dataRifornimento, nazione, distributore, tipoCarburante);
                        if (tariffaCarburante != null) {
                            litri = result.FuelReportItems[i].Quantity;                            
                            notaSpesa.Prezzo = (litri * tariffaCarburante.PrezzoLt);
                            notaSpesa.Valuta = tariffaCarburante.Valuta;                            
                            costiCalcolati++;
                        } else {
                            // nessuna tariffa trovata                            
                            notaSpesa.Note = "Nessuna tariffa da applicare. " + notaSpesa.Note;                                                                                 
                        }

                        notaSpesa.Insert();

                    } catch (Exception ex) {
                        log.Error(ex.Message, ex);
                    }
                }

                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
                evento.Note = "Rifornimenti effettuati: " + result.FuelReportItems.Length + " Costi Automaticamente Calcolati: " + costiCalcolati + ".";

            }

            return evento;
        }


        /// <summary>Interroga TXTango, inserisce una nota spesa per ogni transito a pagamento attraversato sul percorso, e ritorna un evento contenente lo stato del viaggio impostato a "CLOSED".</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                
        /// <returns>Eventi</returns>
        public Eventi TXGetTransitsReport(TXTango.Login login) {            

            // se non sono presenti le date di inizio e fine viaggio non posso ricavare correttamente i transiti attraversati
            // (si rischia di registrare nuovamente le spese di un viaggio antecedente).
            if (this.DataInizio == DateTime.MinValue || this.DataFine == DateTime.MinValue) {
                throw new Exception("Impossibile ricavare i transiti poichè non sono presenti le date di inizio o fine viaggio. IdViaggio: " + this.Id + " KeyViaggio: " + this.KeyViaggio);
            }

            // preparo l'evento da ritornare
            Eventi evento = new Eventi();
            evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_CLOSED"];
            evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
            evento.SyncData = DateTime.Now;
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_FROM_TXTANGO"];
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_GET_TRANSITS"];

            // ricavo le tariffe dei transiti
            List<TXTariffeTransiti> tariffeTransiti = TXTariffeTransiti.GetTariffeTransiti();

            // per ogni tariffa controllo se il viaggio ha attraversato il transito nel suo percorso e aggiunto la relativa nota spesa
            int transitiCalcolati = 0;
            int erroriGeolocalizzazione = 0;
            PositionAreaSelection areaSelection = null;
            ServiceSoapClient service = new ServiceSoapClient();
            GetPositionAreaResult result = null;
            NoteSpese notaSpesa = null;
            for (int i = 0; i < tariffeTransiti.Count; i++) {

                // controllo che il tansito sia geolocalizzato                
                try {
                    if (tariffeTransiti[i].GeoCoordinate == "") {
                        tariffeTransiti[i].GeoLocalizzaTransito();
                    }
                } catch (GeocodingException ex) {
                    erroriGeolocalizzazione++;

                    // loggo l'errore
                    log.Error(ex.Message, ex);

                    // invio una mail al responsabile del viaggio per avvertirlo che il transito non è geocodificabile
                    Boolean notificationsEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["ENABLE_EMAIL_NOTIFICATIONS"]);
                    if (notificationsEnabled && this.MailUtenteCompetenza.Trim() != "") {
                        try {
                            List<string> addresses = new List<string>();
                            addresses.Add(this.MailUtenteCompetenza);
                            string subject = "Errore geolocalizzazione transito.";
                            string body = "<strong>Attenzione:<br/>Si è verificato un errore nel tentativo di geolocalizzare il transito in oggetto.<br/>"
                                      + "Si consiglia di verificare e correggere i dati di localizzazione.</strong><br/><br/>"
                                      + "DATI TRANSITO:<br/>"
                                      + "Nome: " + tariffeTransiti[i].Nome + "<br/>"
                                      + "Indirizzo: " + tariffeTransiti[i].Indirizzo + "<br/>"
                                      + "Cap: " + tariffeTransiti[i].Cap + "<br/>"
                                      + "Citta: " + tariffeTransiti[i].Citta + "<br/>"
                                      + "Provincia: " + tariffeTransiti[i].Provincia + "<br/>"
                                      + "Nazione: " + tariffeTransiti[i].Nazione + "<br/>"                                      
                                      + "Prezzo: " + tariffeTransiti[i].Prezzo + "<br/>"
                                      + "Valuta: " + tariffeTransiti[i].Valuta + "<br/>";
                            Mailer.SendMail(addresses, subject, body);
                        } catch (Exception e) {
                            log.Error(e.Message, e);
                        }
                    }
                }

                // chiedo a TXTango se il viaggio ha attraversato il transito
                if (tariffeTransiti[i].GeoCoordinate != null && tariffeTransiti[i].GeoCoordinate != "") {                    
                    areaSelection = new PositionAreaSelection();
                    areaSelection.DateTimeRange = new DateTimeRangeSelection() { StartDate = (DateTime)this.DataInizio, EndDate = this.DataFine };
                    areaSelection.Driver = new Identifier() { IdentifierType = enumIdentifierType.TRANSICS_ID, Id = this.CodiceAutista };
                    areaSelection.Vehicle = new IdentifierVehicle() { IdentifierVehicleType = enumIdentifierVehicleType.TRANSICS_ID, Id = this.CodiceMezzo };
                    double latitude = Convert.ToDouble(tariffeTransiti[i].GeoCoordinate.Split(',')[0].Replace('.', ','));
                    double longitude = Convert.ToDouble(tariffeTransiti[i].GeoCoordinate.Split(',')[1].Replace('.', ','));
                    areaSelection.Position = new Position() { Latitude = latitude, Longitude = longitude };
                    areaSelection.KmsAround = 2; // tolleranza sul geopunto                
                    result = service.Get_Position_Area(login, areaSelection);

                    // aggiungo all'evento gli xml di richiesta e risposta
                    evento.XmlRequest += Serializer.SerializeObject(areaSelection, SerializationType.XML);
                    evento.XmlResponse += Serializer.SerializeObject(result, SerializationType.XML);

                    // se il viaggio ha attraversato il transito aggiungo la nota spesa
                    if (result.Errors.Length == 0 && result.DateTime != null) {
                        notaSpesa = new NoteSpese();
                        notaSpesa.IdViaggio = this.Id;
                        notaSpesa.Indirizzo = tariffeTransiti[i].Indirizzo + ", " + tariffeTransiti[i].Cap + " " + tariffeTransiti[i].Citta + ", " + tariffeTransiti[i].Provincia + " " + tariffeTransiti[i].Nazione;
                        notaSpesa.Tipo = "COST";
                        notaSpesa.Codice = "Transit";
                        notaSpesa.Descrizione = "";
                        notaSpesa.Data = result.DateTime.Value;
                        notaSpesa.GeoCoordinate = tariffeTransiti[i].GeoCoordinate;
                        notaSpesa.Prezzo = tariffeTransiti[i].Prezzo;
                        notaSpesa.Valuta = tariffeTransiti[i].Valuta;
                        notaSpesa.Insert();
                        log.Debug("transito rilevato: " + tariffeTransiti[i].Indirizzo + ", " + tariffeTransiti[i].Cap + " " + tariffeTransiti[i].Citta + ", " + tariffeTransiti[i].Provincia + " " + tariffeTransiti[i].Nazione);
                        transitiCalcolati++;
                    } else {
                        if (result.Errors.Length > 0) {
                            log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                        }
                        if (result.DateTime == null) {
                            log.Debug("transito NON rilevato: " + tariffeTransiti[i].Indirizzo + ", " + tariffeTransiti[i].Cap + " " + tariffeTransiti[i].Citta + ", " + tariffeTransiti[i].Provincia + " " + tariffeTransiti[i].Nazione);
                        }
                    }
                }
            }

            // note evento
            evento.Note = "Individuati " + transitiCalcolati + " transiti a pagamento sul percorso del viaggio. ";
            if (erroriGeolocalizzazione != 0) {
                evento.Note += "Attenzione: " + erroriGeolocalizzazione + " transiti non sono geolocalizzabili, non è stato pertanto possibile verificarli ed eventualmente aggiungerli alla nota spese.";
            }

            return evento;
        }


        /// <summary>Invia il viaggio a TXTango e ritorna l'evento relativo all'inserimento.</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                        
        /// <returns>Eventi</returns>
        public Eventi TXInsert(TXTango.Login login) {

            PlanningInsert planning = this.TXCreateObject();
            ServiceSoapClient service = new ServiceSoapClient();            
            PlanningResultInsert result = service.Insert_Planning(login, planning);

            Eventi evento = new Eventi();
            evento.Data = DateTime.Now;            
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_INSERT"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_TO_TXTANGO"];
            if (result.Errors.Length > 0) {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"];
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Note = "Vedi XmlResponse per i dettagli sugli errori";
            } else {
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_DELIVERED"];
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
            }
            evento.XmlRequest = Serializer.SerializeObject(planning, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);

            return evento;
        }


        /// <summary>Invia il viaggio a TXTango e ritorna l'evento relativo all'aggiornamento.</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                        
        /// <returns>Eventi</returns>        
        public Eventi TXUpdate(TXTango.Login login) {

            PlanningInsert planning = this.TXCreateObject();
            ServiceSoapClient service = new ServiceSoapClient();
            PlanningResultInsert result = service.Update_Planning(login, planning);

            Eventi evento = new Eventi();
            evento.SyncData = DateTime.Now;
            evento.SyncTask = ConfigurationManager.AppSettings["TXTANGO_TASK_INSERT"];
            evento.SyncTipo = ConfigurationManager.AppSettings["TXTEMP_TO_TXTANGO"];
            if (result.Errors.Length > 0) {
                log.Error("Errore TXTango: " + result.Errors[0].ErrorCode.ToString());
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_NOT_DELIVERED"];
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_ERRORE"];
                evento.Note = "Vedi XmlResponse per i dettagli sugli errori";
            } else {
                evento.Stato = ConfigurationManager.AppSettings["TXTANGO_STATO_DELIVERED"];
                evento.SyncStato = ConfigurationManager.AppSettings["TXTEMP_STATO_SINCRONIZZATO"];
            }
            evento.XmlRequest = Serializer.SerializeObject(planning, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);

            return evento;
        }


        /// <summary>Cancella il viaggio da TXTango e ritorna l'evento relativo all'eliminazione.</summary>
        /// <param name="login">L'oggetto login da inviare a TXTango per l'autenticazione della richiesta.</param>                                        
        /// <returns>Eventi</returns>
        public Eventi TXDelete(TXTango.Login login) {

            PlanningItemSelection planning = new PlanningItemSelection();
            planning.PlanningSelectionType = enumPlanningItemSelectionType.TRIP;
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
                // setto a "CANCELED anche tutte le spedizioni)
                this.CancellaSpedizioni();
            }
            evento.XmlRequest = Serializer.SerializeObject(planning, SerializationType.XML);
            evento.XmlResponse = Serializer.SerializeObject(result, SerializationType.XML);

            return evento;
        }


        /// <summary>Setta a "CANCELED" tutte le spedizioni del viaggio, in modo che non vengano più sincronizzate 
        /// una volta che il viaggio è stato eliminato.</summary>        
        /// <returns>void</returns>
        public void CancellaSpedizioni() {
            Dao dao = new Dao();
            string sql = "";
            sql = "UPDATE Pianificazioni SET Stato = '" + ConfigurationManager.AppSettings["TXTANGO_STATO_CANCELED"] + "' ";
            sql += "WHERE IdViaggio = @IdViaggio";

            Hashtable parameters = new Hashtable();
            parameters.Add("@IdViaggio", this.Id);
            dao.ExecuteNonQuery(sql, parameters);
        }

        #endregion


    }
}
