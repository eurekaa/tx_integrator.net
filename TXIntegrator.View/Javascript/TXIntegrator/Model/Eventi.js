dojo.require("Jarvix.Core.TableModel");

dojo.provide("TXIntegrator.Model.Eventi");
dojo.declare("TXIntegrator.Model.Eventi", Jarvix.Core.TableModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		Id: null,
		IdPianificazione: null,
		Stato: null,
		Data: null,
		SyncData: null,
		SyncTipo: null,
		SyncTask: null,
		SyncStato: null,
		XmlRequest: null,
		XmlResponse: null,
		Note: null
	}

});


