using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class Spedizioni : TableModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__Spedizioni_Rx"; } }
		public override string SP_WRITE { get { return "TX.__Spedizioni_Wx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? Id { get; set; }
		public string KeySpedizione { get; set; }
		public int? Ordinamento { get; set; }
		public string Tipo { get; set; }
		public string Societa { get; set; }
		public string Filiale { get; set; }
		public double? Anno { get; set; }
		public double? Progressivo { get; set; }
		public string Servizio { get; set; }
		public string Reparto { get; set; }
		public string MittenteRagSoc { get; set; }
		public string DestinazioneRagSoc { get; set; }
		public string DestinazioneIndirizzo { get; set; }
		public string DestinazioneCAP { get; set; }
		public string DestinazioneLocalita { get; set; }
		public string DestinazioneProvincia { get; set; }
		public string DestinazioneNazione { get; set; }
		public double? Colli { get; set; }
		public double? Peso { get; set; }
		public double? Volume { get; set; }
		public string Note { get; set; }
		public string DestinazioneGeoLoc { get; set; }
		#endregion


		#region COSTRUTTORI
		public Spedizioni(){
			this.Id = null;
			this.KeySpedizione = null;
			this.Ordinamento = null;
			this.Tipo = null;
			this.Societa = null;
			this.Filiale = null;
			this.Anno = null;
			this.Progressivo = null;
			this.Servizio = null;
			this.Reparto = null;
			this.MittenteRagSoc = null;
			this.DestinazioneRagSoc = null;
			this.DestinazioneIndirizzo = null;
			this.DestinazioneCAP = null;
			this.DestinazioneLocalita = null;
			this.DestinazioneProvincia = null;
			this.DestinazioneNazione = null;
			this.Colli = null;
			this.Peso = null;
			this.Volume = null;
			this.Note = null;
			this.DestinazioneGeoLoc = null;
		}
		#endregion

	}

}

