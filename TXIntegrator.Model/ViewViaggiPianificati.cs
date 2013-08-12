using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class ViewViaggiPianificati : ViewModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__ViewViaggiPianificati_Rx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? IdPianificazione { get; set; }
		public int? IdViaggio { get; set; }
		public string KeyViaggio { get; set; }
		public double? DataViaggio { get; set; }
		public string Stato { get; set; }
		public string CodiceMezzo { get; set; }
		public string CodiceAutista { get; set; }
		public string DestinazioneViaggio { get; set; }
		public DateTime? DataInizio { get; set; }
		public DateTime? DataFine { get; set; }
		public double? KmInizio { get; set; }
		public double? KmFine { get; set; }
		public double? KmViaggio { get; set; }
		public double? ConsumoLt { get; set; }
		public double? VelocitaMedia { get; set; }
		public double? OreGuida { get; set; }
		#endregion


		#region COSTRUTTORI
		public ViewViaggiPianificati(){
			this.IdPianificazione = null;
			this.IdViaggio = null;
			this.KeyViaggio = null;
			this.DataViaggio = null;
			this.Stato = null;
			this.CodiceMezzo = null;
			this.CodiceAutista = null;
			this.DestinazioneViaggio = null;
			this.DataInizio = null;
			this.DataFine = null;
			this.KmInizio = null;
			this.KmFine = null;
			this.KmViaggio = null;
			this.ConsumoLt = null;
			this.VelocitaMedia = null;
			this.OreGuida = null;
		}
		#endregion

	}

}

