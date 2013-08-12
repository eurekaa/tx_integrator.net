dojo.require("dojox.json.ref");
dojo.require("Jarvix.Core.ViewModel");

dojo.provide("Jarvix.Core.TableModel");
dojo.declare("Jarvix.Core.TableModel", Jarvix.Core.ViewModel, {


    Insert: function () {
        var objectInstance = this;
        var parameters = {
            "action": "Insert",
            "class": this.declaredClass,
            "data": dojox.json.ref.toJson(this.Data)
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
            error: function (errorMessage) {
                alert("errore: " + errorMessage);
            }
        });
    },


    Update: function () {
        var objectInstance = this;
        var parameters = {
            "action": "Update",
            "class": this.declaredClass,
            "data": dojox.json.ref.toJson(this.Data)
        };
        dojo.xhrPost({
            sync: true,
            url: objectInstance.Controller,
            content: parameters,
            load: function (response) {
                var obj = dojox.json.ref.fromJson(response);
                if (obj.IsError) {
                    var error = new Jarvix.Core.ServerError();
                    dojo.safeMixin(error.Data, obj);
                    error.Show();
                }
            },
            error: function (errorMessage) {
                alert("errore: " + errorMessage);
            }
        });
    },


    Delete: function () {
        var objectInstance = this;
        var parameters = {
            "action": "Delete",
            "class": this.declaredClass,
            "data": dojox.json.ref.toJson(this.Data)
        };
        dojo.xhrPost({
            sync: true,
            url: objectInstance.Controller,
            content: parameters,
            load: function (response) {
                var obj = dojox.json.ref.fromJson(response);
                if (obj.IsError) {
                    var error = new Jarvix.Core.ServerError();
                    dojo.safeMixin(error.Data, obj);
                    error.Show();
                }
            },
            error: function (errorMessage) {
                alert("errore: " + errorMessage);
            }
        });
    }

});