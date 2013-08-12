dojo.require("Jarvix.Core.ViewModel");

dojo.provide("TXIntegrator.Model.ViewSpedizioniPianificate");
dojo.declare("TXIntegrator.Model.ViewSpedizioniPianificate", Jarvix.Core.ViewModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		IdPianificazione: null,
		IdViaggio: null,
		IdSpedizione: null,
		Stato: null,
		Tipo: null,
		DestinazioneRagSoc: null,
		MittenteRagSoc: null,
		KeySpedizione: null,
		DestinazioneLocalita: null
	}

});


