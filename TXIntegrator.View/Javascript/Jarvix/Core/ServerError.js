dojo.provide("Jarvix.Core.ServerError");
dojo.declare("Jarvix.Core.ServerError", null, {

    Data: {
        IsError: null,
        Message: null,
        Source: null,
        StackTrace: null
    },

    Show: function () {
        alert(this.Data.Message);
    }

});


