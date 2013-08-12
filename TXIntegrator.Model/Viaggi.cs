using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class Viaggi : TableModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__Viaggi_Rx"; } }
		public override string SP_WRITE { get { return "TX.__Viaggi_Wx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? Id { get; set; }
		public string KeyViaggio { get; set; }
		public string Societa { get; set; }
		public string Filiale { get; set; }
		public double? Anno { get; set; }
		public double? Progressivo { get; set; }
		public double? DataViaggio { get; set; }
		public string Reparto { get; set; }
		public string Servizio { get; set; }
		public string CodiceMezzo { get; set; }
		public string CodiceAutista { get; set; }
		public string DestinazioneViaggio { get; set; }
		public string Note { get; set; }
		public string UtenteCompetenza { get; set; }
		public string MailUtenteCompetenza { get; set; }
		public DateTime? DataInizio { get; set; }
		public DateTime? DataFine { get; set; }
		public double? KmInizio { get; set; }
		public double? KmFine { get; set; }
		public double? KmViaggio { get; set; }
		public double? ConsumoLt { get; set; }
		public double? VelocitaMedia { get; set; }
		public double? OreGuida { get; set; }
		public string LuogoPartenza { get; set; }
		public string LuogoArrivo { get; set; }
		#endregion


		#region COSTRUTTORI
		public Viaggi(){
			this.Id = null;
			this.KeyViaggio = null;
			this.Societa = null;
			this.Filiale = null;
			this.Anno = null;
			this.Progressivo = null;
			this.DataViaggio = null;
			this.Reparto = null;
			this.Servizio = null;
			this.CodiceMezzo = null;
			this.CodiceAutista = null;
			this.DestinazioneViaggio = null;
			this.Note = null;
			this.UtenteCompetenza = null;
			this.MailUtenteCompetenza = null;
			this.DataInizio = null;
			this.DataFine = null;
			this.KmInizio = null;
			this.KmFine = null;
			this.KmViaggio = null;
			this.ConsumoLt = null;
			this.VelocitaMedia = null;
			this.OreGuida = null;
			this.LuogoPartenza = null;
			this.LuogoArrivo = null;
		}
		#endregion

	}

}

