using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class TariffeTransiti : TableModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__TariffeTransiti_Rx"; } }
		public override string SP_WRITE { get { return "TX.__TariffeTransiti_Wx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? Id { get; set; }
		public string Nome { get; set; }
		public string Indirizzo { get; set; }
		public string Cap { get; set; }
		public string Citta { get; set; }
		public string Provincia { get; set; }
		public string Nazione { get; set; }
		public string GeoCoordinate { get; set; }
		public double? Prezzo { get; set; }
		public string Valuta { get; set; }
		#endregion


		#region COSTRUTTORI
		public TariffeTransiti(){
			this.Id = null;
			this.Nome = null;
			this.Indirizzo = null;
			this.Cap = null;
			this.Citta = null;
			this.Provincia = null;
			this.Nazione = null;
			this.GeoCoordinate = null;
			this.Prezzo = null;
			this.Valuta = null;
		}
		#endregion

	}

}

