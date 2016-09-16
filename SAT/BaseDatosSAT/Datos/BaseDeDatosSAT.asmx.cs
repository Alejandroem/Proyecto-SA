using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SAT.Datos
{
    /// <summary>
    /// Summary description for BaseDeDatosSAT
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BaseDeDatosSAT : System.Web.Services.WebService
    {


        [WebMethod]

        public int insertarPagoRemesa(String u, String p,int idRemesa, Decimal  MontoRemesa, Decimal Impuesto)
        {
            SATEntities sa = new SATEntities();
            Usuario b = sa.Usuario.FirstOrDefault(us => us.nombre == u);

            if(b.password == p) { 

                LogPagoRemesa l = new LogPagoRemesa();
                l.idRemesa = idRemesa;
                l.montoRemesa = MontoRemesa;
                l.impuesto = Impuesto;

                sa.LogPagoRemesa.Add(l);
                sa.SaveChanges();
                return 1;
            }
            
            return -1;
        }
    }
}
