using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace EmpresaEmisora.Datos
{
    /// <summary>
    /// Summary description for BDEmpresaEmisora
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BDEmpresaEmisora : System.Web.Services.WebService
    {

        [WebMethod]
        public String insertarCliente(String usuario, String pass, String nombre, String documento_identificacion, String correo_electronico, int no_telefono, String no_cuenta, String tipo_cuenta)
        {
            try
            {
                EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities();
                Usuario b = ee.Usuario.FirstOrDefault(us => us.nombre == usuario);
                if(b.pass == pass)
                {
                    Cliente c = new Cliente();
                    c.nombre = nombre;
                    c.documento_identificacion = documento_identificacion;
                    c.correo_electronico = correo_electronico;
                    c.no_telefono = no_telefono;
                    c.no_cuenta = no_cuenta;
                    c.tipo_cuenta = tipo_cuenta;

                    ee.Cliente.Add(c);
                    ee.SaveChanges();
                    return @"<RegistrarCliente>
                                <Exito>1</Exito>
                                <IdCliente>"+c.idCliente.ToString()+@"</IdCliente>
                             </RegistrarCliente>";
                }
                
            }
            catch (Exception e) {
                return @"<RegistrarCliente>
                            <Exito>0</Exito>
                            <Descripcion>"+e.Message+@"</Descripcion>
                         </RegistrarCliente>";
            }


            return ":v";
        }




    }
}
