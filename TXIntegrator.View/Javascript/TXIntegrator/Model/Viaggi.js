dojo.require("Jarvix.Core.TableModel");

dojo.provide("TXIntegrator.Model.Viaggi");
dojo.declare("TXIntegrator.Model.Viaggi", Jarvix.Core.TableModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		Id: null,
		KeyViaggio: null,
		Societa: null,
		Filiale: null,
		Anno: null,
		Progressivo: null,
		DataViaggio: null,
		Reparto: null,
		Servizio: null,
		CodiceMezzo: null,
		CodiceAutista: null,
		DestinazioneViaggio: null,
		Note: null,
		UtenteCompetenza: null,
		MailUtenteCompetenza: null,
		DataInizio: null,
		DataFine: null,
		KmInizio: null,
		KmFine: null,
		KmViaggio: null,
		ConsumoLt: null,
		VelocitaMedia: null,
		OreGuida: null,
		LuogoPartenza: null,
		LuogoArrivo: null
	}

});


