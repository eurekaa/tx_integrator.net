dojo.require("Jarvix.Core.ViewModel");

dojo.provide("TXIntegrator.Model.ViewViaggiPianificati");
dojo.declare("TXIntegrator.Model.ViewViaggiPianificati", Jarvix.Core.ViewModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		IdPianificazione: null,
		IdViaggio: null,
		KeyViaggio: null,
		DataViaggio: null,
		Stato: null,
		CodiceMezzo: null,
		CodiceAutista: null,
		DestinazioneViaggio: null,
		DataInizio: null,
		DataFine: null,
		KmInizio: null,
		KmFine: null,
		KmViaggio: null,
		ConsumoLt: null,
		VelocitaMedia: null,
		OreGuida: null
	}

});


