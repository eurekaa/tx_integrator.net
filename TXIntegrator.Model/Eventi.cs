using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class Eventi : TableModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__Eventi_Rx"; } }
		public override string SP_WRITE { get { return "TX.__Eventi_Wx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? Id { get; set; }
		public int? IdPianificazione { get; set; }
		public string Stato { get; set; }
		public DateTime? Data { get; set; }
		public DateTime? SyncData { get; set; }
		public string SyncTipo { get; set; }
		public string SyncTask { get; set; }
		public string SyncStato { get; set; }
		public string XmlRequest { get; set; }
		public string XmlResponse { get; set; }
		public string Note { get; set; }
		#endregion


		#region COSTRUTTORI
		public Eventi(){
			this.Id = null;
			this.IdPianificazione = null;
			this.Stato = null;
			this.Data = null;
			this.SyncData = null;
			this.SyncTipo = null;
			this.SyncTask = null;
			this.SyncStato = null;
			this.XmlRequest = null;
			this.XmlResponse = null;
			this.Note = null;
		}
		#endregion

	}

}

