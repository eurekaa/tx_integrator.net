using System;
using System.Collections.Generic;
using System.Configuration;
using log4net;
using Volcano.TXIntegrator.Model;
using Ultrapulito.Jarvix.Core;
using Volcano.TXIntegrator.Synchronizer.Model.TXTango;

namespace Volcano.TXIntegrator.Synchronizer {

    class TXSync {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args) {

            DateTime dataInizio = DateTime.MinValue;
            DateTime dataFine = DateTime.MinValue;
            int TXTangoViaggiSincronizzati = 0;
            int TXTangoSpedizioniSincronizzate = 0;
            int txtempViaggiSincronizzati = 0;
            int txtempSpedizioniSincronizzate = 0;
            int txtempViaggiReport = 0;

            try {

                // attivo e configuro log4net
                log4net.Config.XmlConfigurator.Configure();

                // registro la data di inizio sincronizzazione
                dataInizio = DateTime.Now;

                if (TXSync.CheckLicence()) {

                    // preparo l'oggetto per l'autenticazione su TXTango
                    Login login = TXTango.GetLogin();

                    // sincronizzo TXTango
                    TXTango.Login = login;
                    TXTangoViaggiSincronizzati = TXTango.SyncViaggi();
                    TXTangoSpedizioniSincronizzate = TXTango.SyncSpedizioni();

                    // sincronizzo TXTemp
                    TXTemp.Login = login;
                    txtempViaggiSincronizzati = TXTemp.SyncViaggi();
                    txtempSpedizioniSincronizzate = TXTemp.SyncSpedizioni();
                    txtempViaggiReport = TXTemp.ReportViaggi();

                } else {
                    log.Error("Licenza non valida");
                }

            } catch (Exception ex) {
                log.Error(ex.Message, ex);

            } finally {

                // registro un report di sincronizzazione
                dataFine = DateTime.Now;
                TimeSpan durata = dataFine.Subtract(dataInizio);
                log.Info("Report Sincronizzazione. Data Inizio: " + dataInizio.ToString() + " Data Fine: " + dataFine.ToString() + " Durata: " + Math.Round(durata.TotalMinutes, 2) + " minuti. Viaggi inviati a TXTango: " + TXTangoViaggiSincronizzati + " Spedizioni inviate a TXTango: " + TXTangoSpedizioniSincronizzate + " Viaggi aggiornati da TXTango: " + txtempViaggiSincronizzati + " Spedizioni aggiornate da TXTango: " + txtempSpedizioniSincronizzate + " Report di Viaggio: " + txtempViaggiReport);
            }
        }



        public static Boolean CheckLicence() {
            Boolean isValid = false;
            DateTime deadtime = DateTime.ParseExact(ConfigurationManager.AppSettings["DEADTIME"].ToString(), "yyyyMMdd", null);
            DateTime today = DateTime.Now.Date;
            isValid = (today < deadtime);
            return isValid;
        }


    }

}

