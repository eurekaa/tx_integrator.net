using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class ViewSpedizioniPianificate : ViewModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__ViewSpedizioniPianificate_Rx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? IdPianificazione { get; set; }
		public int? IdViaggio { get; set; }
		public int? IdSpedizione { get; set; }
		public string Stato { get; set; }
		public string Tipo { get; set; }
		public string DestinazioneRagSoc { get; set; }
		public string MittenteRagSoc { get; set; }
		public string KeySpedizione { get; set; }
		public string DestinazioneLocalita { get; set; }
		#endregion


		#region COSTRUTTORI
		public ViewSpedizioniPianificate(){
			this.IdPianificazione = null;
			this.IdViaggio = null;
			this.IdSpedizione = null;
			this.Stato = null;
			this.Tipo = null;
			this.DestinazioneRagSoc = null;
			this.MittenteRagSoc = null;
			this.KeySpedizione = null;
			this.DestinazioneLocalita = null;
		}
		#endregion

	}

}

