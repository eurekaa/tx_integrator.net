dojo.require("Jarvix.Core.TableModel");

dojo.provide("TXIntegrator.Model.Spedizioni");
dojo.declare("TXIntegrator.Model.Spedizioni", Jarvix.Core.TableModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		Id: null,
		KeySpedizione: null,
		Ordinamento: null,
		Tipo: null,
		Societa: null,
		Filiale: null,
		Anno: null,
		Progressivo: null,
		Servizio: null,
		Reparto: null,
		MittenteRagSoc: null,
		DestinazioneRagSoc: null,
		DestinazioneIndirizzo: null,
		DestinazioneCAP: null,
		DestinazioneLocalita: null,
		DestinazioneProvincia: null,
		DestinazioneNazione: null,
		Colli: null,
		Peso: null,
		Volume: null,
		Note: null,
		DestinazioneGeoLoc: null
	}

});


