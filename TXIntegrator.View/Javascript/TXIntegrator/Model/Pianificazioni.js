dojo.require("Jarvix.Core.TableModel");

dojo.provide("TXIntegrator.Model.Pianificazioni");
dojo.declare("TXIntegrator.Model.Pianificazioni", Jarvix.Core.TableModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		Id: null,
		IdViaggio: null,
		IdSpedizione: null,
		Stato: null,
		SyncStato: null,
		SyncTask: null,
		SyncData: null
	}

});


