dojo.require("Jarvix.Core.TableModel");

dojo.provide("TXIntegrator.Model.NoteSpese");
dojo.declare("TXIntegrator.Model.NoteSpese", Jarvix.Core.TableModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		Id: null,
		IdViaggio: null,
		Data: null,
		Tipo: null,
		Codice: null,
		Descrizione: null,
		Indirizzo: null,
		GeoCoordinate: null,
		Prezzo: null,
		Valuta: null,
		Note: null
	}

});


