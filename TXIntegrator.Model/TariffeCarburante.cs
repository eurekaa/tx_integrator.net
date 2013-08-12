using System;
using Ultrapulito.Jarvix.Core;

namespace Volcano.TXIntegrator.Model{

	public partial class TariffeCarburante : TableModel {

		#region STORED PROCEDURES
		public override string SP_READ { get { return "TX.__TariffeCarburante_Rx"; } }
		public override string SP_WRITE { get { return "TX.__TariffeCarburante_Wx"; } }
		#endregion


		#region CAMPI DATABASE
		public int? Id { get; set; }
		public string Distributore { get; set; }
		public string Nazione { get; set; }
		public DateTime? DataDa { get; set; }
		public double? PrezzoLt { get; set; }
		public string Valuta { get; set; }
		public string TipoCarburante { get; set; }
		#endregion


		#region COSTRUTTORI
		public TariffeCarburante(){
			this.Id = null;
			this.Distributore = null;
			this.Nazione = null;
			this.DataDa = null;
			this.PrezzoLt = null;
			this.Valuta = null;
			this.TipoCarburante = null;
		}
		#endregion

	}

}

