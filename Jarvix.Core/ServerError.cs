
namespace Ultrapulito.Jarvix.Core {

    public class ServerError {

        #region PROPRIETA'
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        #endregion

        #region COSTRUTTORI
        public ServerError() {
            this.IsError = true;
            this.Message = "";
            this.Source = "";
            this.StackTrace = "";
        }
        #endregion

    }

}
