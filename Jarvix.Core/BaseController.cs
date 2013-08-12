using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using log4net;


namespace Ultrapulito.Jarvix.Core {

    public class BaseController : IHttpHandler, IRequiresSessionState {

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public BaseController() { }

        public void ProcessRequest(HttpContext context) {
            try {

                string action = context.Request["action"];                
                Type type = this.GetType();
                MethodInfo method = type.GetMethod(action);
                object[] args = { context };
                method.Invoke(this, args);

            } catch (Exception ex) {
                // trovo il vero errore all'interno di tutti i rimbalzi di eccezioni causati dalla reflection
                while (ex.Message == "Exception has been thrown by the target of an invocation.") {
                    ex = ex.InnerException;
                }
                // loggo l'errore
                log.Error(ex.Message, ex);
                // creo l'oggetto ServerError e lo invio al client
                ServerError error = new ServerError();
                error.Message = ex.Message;
                error.Source = ex.Source;
                error.StackTrace = ex.StackTrace;
                string output = Serializer.SerializeObject(error, SerializationType.JSON);
                context.Response.Write(output);
            }
        }


        public void Select(HttpContext context) {
            // determino il tipo di oggetto passato
            string[] typeOf = context.Request["class"].ToString().Split('.');
            Type type = GetObjectType(context, typeOf[typeOf.Length - 1]);

            // ricostruisco l'oggetto
            string data = context.Request["data"];
            object obj = Serializer.UnserializeObject(data, type, SerializationType.JSON);

            // ricavo l'id dell'oggetto da selezionare
            int id = int.Parse(context.Request["id"].ToString());

            // invoco il metodo
            MethodInfo method = type.GetMethod("Select");
            object[] args = { id };
            method.Invoke(obj, args);

            // restituisco al client l'oggetto
            string output = Serializer.SerializeObject(obj, SerializationType.JSON);
            context.Response.Write(output);
        }


        public void Search(HttpContext context) {
            // determino il tipo di oggetto passato
            string[] typeOf = context.Request["class"].ToString().Split('.');
            Type modelType = GetObjectType(context, typeOf[typeOf.Length - 1]);

            // ricostruisco l'oggetto
            string data = context.Request["data"];
            object modelInstance = Serializer.UnserializeObject(data, modelType, SerializationType.JSON);

            // ricavo il metodo
            MethodInfo method = modelType.GetMethod("Search");

            // ricavo i parametri di ricerca da passare al metodo
            string searchdata = context.Request["searchdata"].ToString();
            searchdata = searchdata.Replace("{", "").Replace("}", "");
            Hashtable searchParameters = new Hashtable();
            if (searchdata != "") {
                string[] searchDataList = searchdata.Split(',');
                string searchKey = "";
                dynamic searchValue = "";
                for (int i = 0; i < searchDataList.Length; i++) {
                    searchKey = searchDataList[i].Split(':')[0].Replace("\"", "");
                    searchValue = searchDataList[i].Split(':')[1].Replace("\"", "").Replace("[", "").Replace("]", "");
                    if (searchValue == "null") {
                        searchValue = null;
                    }
                    // converto il valore nel tipo definito nell'oggetto
                    PropertyInfo property = modelType.GetProperty(searchKey);
                    Type propertyType = property.PropertyType;

                    switch (propertyType.ToString()) {
                        case "System.Nullable`1[System.Int32]":
                            searchValue = Marshaller.ChangeType<int?>(searchValue);
                            break;
                        case "System.Nullable`1[System.Double]":
                            searchValue = Marshaller.ChangeType<double?>(searchValue);
                            break;
                        case "System.Nullable`1[System.Boolean]":
                            searchValue = Marshaller.ChangeType<bool?>(searchValue);
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            searchValue = Marshaller.ChangeType<DateTime?>(searchValue);
                            break;
                        default:
                            searchValue = Marshaller.ChangeType<string>(searchValue);
                            break;
                    }

                    searchParameters.Add(searchKey, searchValue);
                }
            }

            // invoco il metodo
            object[] args = { searchParameters };
            List<dynamic> modelList = (List<dynamic>)method.Invoke(modelInstance, args);

            // restituisco al client l'oggetto
            string output = (modelList != null && modelList.Count > 0) ? Serializer.SerializeObjectList(modelList, SerializationType.JSON) : "{}";
            context.Response.Write(output);
        }


        public void Insert(HttpContext context) {
            // determino il tipo di oggetto passato
            string[] typeOf = context.Request["class"].ToString().Split('.');
            Type type = GetObjectType(context, typeOf[typeOf.Length - 1]);

            // ricostruisco l'oggetto
            string data = context.Request["data"];
            object obj = Serializer.UnserializeObject(data, type, SerializationType.JSON);

            // invoco il metodo            
            MethodInfo method = type.GetMethod("Insert");
            object[] args = { };
            method.Invoke(obj, args);

            // restituisco al client l'oggetto (con l'id del record appena inserito)
            string output = Serializer.SerializeObject(obj, SerializationType.JSON);
            context.Response.Write(output);
        }


        public void Update(HttpContext context) {
            // determino il tipo di oggetto passato
            string[] typeOf = context.Request["class"].ToString().Split('.');
            Type type = GetObjectType(context, typeOf[typeOf.Length - 1]);

            // ricostruisco l'oggetto
            string data = context.Request["data"];
            object obj = Serializer.UnserializeObject(data, type, SerializationType.JSON);

            // invoco il metodo            
            MethodInfo method = type.GetMethod("Update");
            object[] args = { };
            method.Invoke(obj, args);

            // rimando al client l'oggetto
            string output = Serializer.SerializeObject(obj, SerializationType.JSON);
            context.Response.Write(output);
        }


        public void Delete(HttpContext context) {
            // determino il tipo di oggetto passato
            string[] typeOf = context.Request["class"].ToString().Split('.');
            Type type = GetObjectType(context, typeOf[typeOf.Length - 1]);

            // ricostruisco l'oggetto
            string data = context.Request["data"];
            object obj = Serializer.UnserializeObject(data, type, SerializationType.JSON);

            // invoco il metodo
            MethodInfo method = type.GetMethod("Delete");
            object[] args = { };
            method.Invoke(obj, args);

            // restituisco al client l'oggetto vuoto
            string output = "{}";
            context.Response.Write(output);
        }


        public void Invoke(HttpContext context) {
            // determino il tipo di oggetto passato
            string[] typeOf = context.Request["class"].ToString().Split('.');
            Type type = GetObjectType(context, typeOf[typeOf.Length - 1]);
            // ricostruisco l'oggetto
            string data = context.Request["data"];
            object obj = Serializer.UnserializeObject(data, type, SerializationType.JSON);
            // invoco il metodo      
            string methodName = context.Request["method"];
            MethodInfo method = type.GetMethod(methodName);
            object[] args = { context };
            dynamic returnObj = method.Invoke(obj, args);
            string output = "";
            // i metodi del model possono tornare un istanza o una lista di istanze della classe
            if (method.ReturnType.BaseType == typeof(BaseModel)) {
                output = Serializer.SerializeObject(returnObj, SerializationType.JSON);
            } else {
                output = Serializer.SerializeObjectList(returnObj, SerializationType.JSON);
            }
            context.Response.Write(output);
        }


        private Type GetObjectType(HttpContext context, string type) {
            Type ObjectType = null;
            if (context.Application["ModelObjects"] == null) {
                Assembly Model = Assembly.Load(new AssemblyName("TXIntegrator.Model"));
                context.Application["ModelObjects"] = Model.GetTypes();
            }
            Type[] ModelObjects = (Type[])context.Application["ModelObjects"];
            foreach (Type ModelObject in ModelObjects) {
                if (ModelObject.Name == type) {
                    ObjectType = ModelObject;
                }
            }
            return ObjectType;
        }


        public bool IsReusable {
            get {
                return false;
            }
        }

    }
}
