using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class NoteSpese : TableModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__NoteSpese_Rx"; } }
		public override string SP_WRITE { get { return "TX.__NoteSpese_Wx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? Id { get; set; }
		public int? IdViaggio { get; set; }
		public DateTime? Data { get; set; }
		public string Tipo { get; set; }
		public string Codice { get; set; }
		public string Descrizione { get; set; }
		public string Indirizzo { get; set; }
		public string GeoCoordinate { get; set; }
		public double? Prezzo { get; set; }
		public string Valuta { get; set; }
		public string Note { get; set; }
		#endregion


		#region COSTRUTTORI
		public NoteSpese(){
			this.Id = null;
			this.IdViaggio = null;
			this.Data = null;
			this.Tipo = null;
			this.Codice = null;
			this.Descrizione = null;
			this.Indirizzo = null;
			this.GeoCoordinate = null;
			this.Prezzo = null;
			this.Valuta = null;
			this.Note = null;
		}
		#endregion

	}

}

