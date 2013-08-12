using System;
using Ultrapulito.Jarvix.Core;
using System.Collections;
using System.Data;
using Volcano.TXIntegrator.Model;

namespace Volcano.TXIntegrator.Synchronizer.Model{

    public class TXTariffeCarburante : TariffeCarburante {

        #region metodi statici
        public static TariffeCarburante GetTariffa(DateTime dataDa, string nazione, string distributore, string tipoCarburante) {
            TariffeCarburante tariffa = null;
            Dao dao = new Dao();
            Hashtable parameters = new Hashtable();
            parameters.Add("@DataDa", dataDa);
            parameters.Add("@Nazione", nazione);
            parameters.Add("@Distributore", distributore);            
            parameters.Add("@TipoCarburante", tipoCarburante);
            DataSet data = dao.ExecuteStoredProcedure("TX.TariffeCarburante_GetTariffa", parameters);
            if (data.Tables[0].Rows.Count > 0) {
                tariffa = new TariffeCarburante();
                tariffa.Id = Convert.ToInt32(data.Tables[0].Rows[0]["Id"]);
                tariffa.Distributore = Convert.ToString(data.Tables[0].Rows[0]["Distributore"]);                                
                tariffa.Nazione = Convert.ToString(data.Tables[0].Rows[0]["Nazione"]);
                tariffa.DataDa = Convert.ToDateTime(data.Tables[0].Rows[0]["DataDa"]);
                tariffa.PrezzoLt = Convert.ToDouble(data.Tables[0].Rows[0]["PrezzoLt"]);
                tariffa.Valuta = Convert.ToString(data.Tables[0].Rows[0]["Valuta"]);
                tariffa.TipoCarburante = Convert.ToString(data.Tables[0].Rows[0]["TipoCarburante"]);
            }
            return tariffa;
        }
        #endregion

	}

}

