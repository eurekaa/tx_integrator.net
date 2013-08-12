using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class Log : TableModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__Log_Rx"; } }
		public override string SP_WRITE { get { return "TX.__Log_Wx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? Id { get; set; }
		public DateTime? logDate { get; set; }
		public string logLevel { get; set; }
		public string logger { get; set; }
		public string logMessage { get; set; }
		public string logInfo { get; set; }
		#endregion


		#region COSTRUTTORI
		public Log(){
			this.Id = null;
			this.logDate = null;
			this.logLevel = null;
			this.logger = null;
			this.logMessage = null;
			this.logInfo = null;
		}
		#endregion

	}

}

