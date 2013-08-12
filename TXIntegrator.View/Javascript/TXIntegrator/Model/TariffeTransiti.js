dojo.require("Jarvix.Core.TableModel");

dojo.provide("TXIntegrator.Model.TariffeTransiti");
dojo.declare("TXIntegrator.Model.TariffeTransiti", Jarvix.Core.TableModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		Id: null,
		Nome: null,
		Indirizzo: null,
		Cap: null,
		Citta: null,
		Provincia: null,
		Nazione: null,
		GeoCoordinate: null,
		Prezzo: null,
		Valuta: null
	}

});


