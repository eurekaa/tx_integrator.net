using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class Pianificazioni : TableModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__Pianificazioni_Rx"; } }
		public override string SP_WRITE { get { return "TX.__Pianificazioni_Wx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? Id { get; set; }
		public int? IdViaggio { get; set; }
		public int? IdSpedizione { get; set; }
		public string Stato { get; set; }
		public string SyncStato { get; set; }
		public string SyncTask { get; set; }
		public DateTime? SyncData { get; set; }
		#endregion


		#region COSTRUTTORI
		public Pianificazioni(){
			this.Id = null;
			this.IdViaggio = null;
			this.IdSpedizione = null;
			this.Stato = null;
			this.SyncStato = null;
			this.SyncTask = null;
			this.SyncData = null;
		}
		#endregion

	}

}

