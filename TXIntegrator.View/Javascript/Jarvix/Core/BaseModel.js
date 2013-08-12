dojo.require("dojox.json.ref");
dojo.require("Jarvix.Core.ServerError");

dojo.provide("Jarvix.Core.BaseModel");
dojo.declare("Jarvix.Core.BaseModel", null, {

    Data: {},
    MetaData: {}, 

    constructor: function (args) {
        if (args != undefined) {
            dojo.safeMixin(this.Data, args);
        }
    },


    Invoke: function (method, parameters) {
        var objectInstance = this;
        var output = {};
        if (parameters == undefined) {
            parameters = {};
        }
        parameters.action = "Invoke";
        parameters.class = this.declaredClass;
        parameters.method = method;
        parameters.data = dojox.json.ref.toJson(this.Data);
        dojo.xhrPost({
            sync: true,
            url: objectInstance.Controller,
            content: parameters,
            load: function (response) {
                var obj = dojox.json.ref.fromJson(response);
                if (!obj.IsError) {
                    output = obj;
                } else {
                    var error = new Jarvix.Core.ServerError();
                    dojo.safeMixin(error.Data, obj);
                    error.Show();
                }
            },
            error: function (errorMessage) {
                alert("errore: " + errorMessage);
            }
        });
        return output;
    }


});