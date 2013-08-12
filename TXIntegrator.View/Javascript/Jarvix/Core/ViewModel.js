dojo.require("dojox.json.ref");
dojo.require("Jarvix.Core.BaseModel");

dojo.provide("Jarvix.Core.ViewModel");
dojo.declare("Jarvix.Core.ViewModel", Jarvix.Core.BaseModel, {


    Select: function (id) {
        var objectInstance = this;
        var parameters = {
            "action": "Select",
            "class": this.declaredClass,
            "data": dojox.json.ref.toJson(this.Data),
            "id": id
        };
        dojo.xhrPost({
            sync: true,
            url: objectInstance.Controller,
            content: parameters,
            load: function (response) {
                var obj = dojox.json.ref.fromJson(response);
                if (!obj.IsError) {
                    dojo.safeMixin(objectInstance.Data, obj);
                } else {
                    var error = new Jarvix.Core.ServerError();
                    dojo.safeMixin(error.Data, obj);
                    error.Show();
                }
            },
            error: function (error) {
                alert(error.message);
            }
        });
    },


    Search: function (filter) {
        var objectInstance = this;
        //var modelList = [];
        var output = "{}";
        var searchdata = (filter != undefined) ? dojox.json.ref.toJson(filter) : "{}";        
        var parameters = {
            "action": "Search",
            "class": this.declaredClass,
            "data": dojox.json.ref.toJson(this.Data),
            "searchdata": searchdata
        };
        dojo.xhrPost({
            sync: true,
            url: objectInstance.Controller,
            content: parameters,
            load: function (response) {
                var obj = dojox.json.ref.fromJson(response);
                if (!obj.IsError) {
                    output = response;
                    //var modelInstance = null;
                    //dojo.forEach(obj, function (item, index) {
                    //    eval("modelInstance = new " + objectInstance.declaredClass + "(item);");
                    //    modelList.push(modelInstance);
                    //});
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
        //return modelList;
        return output;
    }


});