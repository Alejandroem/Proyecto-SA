using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SAT.Datos;
using System.Xml;
using System.Xml.Linq;

namespace SAT
{
    /// <summary>
    /// Summary description for SAT
    /// </summary>
    [WebService(Namespace = "http://sat.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SAT : System.Web.Services.WebService
    {



        [WebMethod]
        public string PagarImpuestoPorRemesa(String Usuario, String Contrasena, int IdRemesa, Decimal MontoRemesa)
        {
            //Calculo del impuesto
            Decimal impuesto = Decimal.Parse(((double)MontoRemesa * 0.05).ToString());
            try
            {
                SATEntities1 sa = new SATEntities1();
                Usuario b = sa.Usuario.FirstOrDefault(us => us.nombre == Usuario);

                if (b.password == Contrasena)
                {

                    LogPagoRemesa l = new LogPagoRemesa();
                    l.idRemesa = IdRemesa;
                    l.montoRemesa = MontoRemesa;
                    l.impuesto = impuesto;

                    

                    String date = DateTime.Now.ToString("M/dd/yyyy");
                    

                    sa.LogPagoRemesa.Add(l);
                    sa.SaveChanges();

                    Banguat.Banguat banguat = new Banguat.Banguat();
                    String tasacambioxml = banguat.ConsultaTasaDeCambio(date);
                    String tasadecambio = "";

                    XElement root = XElement.Parse(tasacambioxml);
                    IEnumerable<XElement> tests =
                        from el in root.Elements("TipoCambioVentaQ")
                        select el;
                    foreach (XElement el in tests)
                         tasadecambio = el.Value;

                    Decimal tasacambiod = decimal.Parse(tasadecambio);


                    return @"<PagarImpuestoRemesa>
                                <Exito>1</Exito>
                                <MontoInicialUSD>" + MontoRemesa + @"</MontoInicialUSD>
                                <MontoInicialQ>" + (MontoRemesa * tasacambiod) + @"</MontoInicialQ>
                                <ImpuestoCobradoUSD>" + impuesto + @"<  /ImpuestoCobradoUSD>
                                <ImpuestoCobradoQ>" + (impuesto*tasacambiod) + @"</ImpuestoCobradoQ>
                                <CantidadSinImpuestoUSD>" + (MontoRemesa-impuesto) + @"</CantidadSinImpuestoUSD>
                                <CantidadSinImpuestoQ>" +(MontoRemesa-impuesto)*tasacambiod + @"</CantidadSinImpuestoQ>
                             </PagarImpuestoRemesa>";
                }
            }
            catch (Exception e)
            {
                return @"<PagarImpuestoRemesa>
                            <Exito>0</Exito>
                            <Descripcion" + e.Message + @"</Descripcion>
                         </PagarImpuestoRemesa>";
            }

            return "";
        }
        [WebMethod]
        public string ConsultaImpuestoRemesa(Decimal  MontoRemesa)
        {


            return ((double)MontoRemesa * 0.05).ToString();
            //String con estructura xml indicando el valor de impuesto por remesa en quetzales tomando como base el cambio actual y el nuevo monto en dolares sin el impuesto.

        }
    }
}
