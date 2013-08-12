using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Data;

namespace Ultrapulito.Jarvix.Core {

    public class GeoCoder {


        /// <summary>Traduce un indirizzo in coordinate di latitudine e longitudine.</summary>
        /// <param name="address">L'indirizzo da convertire.</param>                
        /// <returns>string</returns> 
        public static string EncodeAddress(string address) {
            string coordinate = "";
            try {                
                WebRequest request = WebRequest.Create("http://maps.google.com/maps/api/geocode/xml?address=" + address + "&sensor=false");
                WebProxy proxy = new WebProxy("http://maps.google.com/maps/api/geocode/xml?address=" + address + "&sensor=false", true);
                request.Proxy = proxy;
                request.Timeout = 2000;
                WebResponse response = request.GetResponse();
                XmlTextReader xml = new XmlTextReader(response.GetResponseStream());
                DataSet data = new DataSet();
                data.ReadXml(xml);
                if (data.Tables[data.Tables.IndexOf("GeocodeResponse")].Rows[0]["status"].ToString() != "ZERO_RESULTS") {
                    DataTable location = data.Tables[data.Tables.IndexOf("location")];
                    string lat = location.Rows[0]["lat"].ToString();
                    string lng = location.Rows[0]["lng"].ToString();
                    coordinate = lat + "," + lng;
                }
                if (coordinate == "") {
                    throw new GeocodingException("Impossibile codificare l'indirizzo in geo-coordinate");
                }
            } catch (Exception ex) {
                throw new GeocodingException(ex.Message);
            }
            return coordinate;
        }


        /// <summary>Traduce coordinate di latitudine e longitudine in un indirizzo.</summary>
        /// <param name="lat">latitudine.</param>                
        /// <param name="lng">longitudine.</param>                
        /// <returns>string</returns>
        public static string DecodeAddress(string lat, string lng) {
            string address = "";
            try {
                WebRequest request = WebRequest.Create("http://maps.google.com/maps/api/geocode/xml?latlng=" + lat + "," + lng + "&language=it&sensor=false");
                WebProxy proxy = new WebProxy("http://maps.google.com/maps/api/geocode/xml?latlng=" + lat + "," + lng + "&language=it&sensor=false", true);
                request.Proxy = proxy;
                request.Timeout = 2000;
                WebResponse response = request.GetResponse();
                XmlTextReader xml = new XmlTextReader(response.GetResponseStream());
                DataSet data = new DataSet();
                data.ReadXml(xml);
                if (data != null) {
                    DataTable result = data.Tables[data.Tables.IndexOf("result")];
                    address = result.Rows[0]["formatted_address"].ToString();
                }
                if (address == "") {
                    throw new GeocodingException("Impossibile decodificare le geo-coordinate");
                }
            } catch (Exception ex) {
                throw new GeocodingException(ex.Message);
            }
            return address;
        }


        /// <summary>Traduce coordinate di latitudine e longitudine in un indirizzo.</summary>
        /// <param name="coordinate">latitudine,longitudine.</param>                        
        /// <returns>string</returns>
        public static string DecodeAddress(string coordinate) {
            string lat = coordinate.Split(',')[0];
            string lng = coordinate.Split(',')[1];
            return DecodeAddress(lat, lng);
        }

    }
}
