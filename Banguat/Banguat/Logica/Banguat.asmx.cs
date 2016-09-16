using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Banguat.Datos;
using System.Globalization;

namespace Banguat
{
    /// <summary>
    /// Summary description for Banguat
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Banguat : System.Web.Services.WebService
    {
        
        [WebMethod]
        public string ConsultaTasaDeCambio(String Fecha)
        {
            try
            {
                using (BanguatEntities be = new BanguatEntities()) { 
                    TasaCambio tc = be.TasaCambio.FirstOrDefault(us => us.fecha == Fecha);
                    return "<ConsultaTasaDeCambio><Exito> 1 </Exito><TipoCambioVentaQ> " + tc.tipoCambioVenta + " </TipoCambioVentaQ><TipoCambioCompraQ> " + tc.tipoCambioCompra + " </TipoCambioCompraQ></ConsultaTasaDeCambio> ";
                }
            }
            catch(Exception e)
            {
                return "<ConsultaTasaDeCambio><Exito> 0 </Exito><Descripcion>" +  e.Message + "</Descripcion></ConsultaTasaDeCambio> ";
            }
            
        }

        private String consultaTasaDeCambioVenta(String Fecha) {
            try
            {
                using (BanguatEntities be = new BanguatEntities())
                {
                    TasaCambio tc = be.TasaCambio.FirstOrDefault(us => us.fecha == Fecha);
                    return tc.tipoCambioVenta.ToString();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            
        }

        [WebMethod]
        public string ConsultaMontosMaximosMinimos(String Usuario, String Contrasenia, String Fecha)
        {
            try
            {
                using (BanguatEntities be = new BanguatEntities())
                {
                    Usuario u = be.Usuario.FirstOrDefault(us => us.nombre == Usuario);

                    if (u.password == Contrasenia)
                    {
                        Monto tc = be.Monto.FirstOrDefault(us => us.fecha == Fecha);
                        return "<ConsultaMontosMaximosMinimos><Exito> 1 </Exito <MaximoUSD>" + tc.montoMax + "</MaximoUSD><MinimoUSD>" + tc.montoMin + "</MinimoUSD></ConsultaMontosMaximosMinimos> ";
                    }
                    else
                    {
                        return "<ConsultaMontosMaximosMinimos><Exito> 0 </Exito><Descripcion>Usuario No Registrado</Descripcion></ConsultaMontosMaximosMinimos> ";
                    }
                }
            }
            catch (Exception e)
            {
                return "<ConsultaMontosMaximosMinimos><Exito> 0 </Exito><Descripcion>" + e.Message + "</Descripcion></ConsultaMontosMaximosMinimos> ";
            }

        }

        [WebMethod]
        public string ConsultaComisionPorRemesa()
        {
            try
            {
                using (BanguatEntities be = new BanguatEntities())
                {
                    String date = DateTime.Now.ToString("M/dd/yyyy");
                    String tasaCambio = consultaTasaDeCambioVenta(date);
                    decimal comision = 1 * decimal.Parse(tasaCambio);
                    return "<ConsultaComisionPorRemesa><Exito> 1 </Exito><ComisionPorRemesaQ>" + comision.ToString() + "</ComisionPorRemesaQ></ConsultaComisionPorRemesa> ";
                }
            }
            catch (Exception e) {
                return "<ConsultaComisionPorRemesa><Exito> 0 </Exito><Descripcion>" + e.Message+ "</Descripcion></ConsultaComisionPorRemesa> ";
            }

        }


        }
}
