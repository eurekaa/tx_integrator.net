dojo.require("Jarvix.Core.TableModel");

dojo.provide("TXIntegrator.Model.Log");
dojo.declare("TXIntegrator.Model.Log", Jarvix.Core.TableModel, {

	Controller: "/TXIntegrator/Controller/TXIntegrator.ashx",

	Data: {
		Id: null,
		logDate: null,
		logLevel: null,
		logger: null,
		logMessage: null,
		logInfo: null
	}

});


