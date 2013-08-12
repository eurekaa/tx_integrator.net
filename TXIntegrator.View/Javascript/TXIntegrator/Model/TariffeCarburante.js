dojo.require("Jarvix.Core.TableModel");

dojo.provide("TXIntegrator.Model.TariffeCarburante");
dojo.declare("TXIntegrator.Model.TariffeCarburante", Jarvix.Core.TableModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		Id: null,
		Distributore: null,
		Nazione: null,
		DataDa: null,
		PrezzoLt: null,
		Valuta: null,
		TipoCarburante: null
	}

});


