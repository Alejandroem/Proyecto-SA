using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using EmpresaEmisora.Datos;
using System.Xml.Linq;

namespace EmpresaEmisora
{
    /// <summary>
    /// Summary description for EmpresaEmisora
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class EmpresaEmisora : System.Web.Services.WebService
    {

        [WebMethod]
        public string RegistrarCliente(
            String Usuario, 
            String Contrasenia, 
            String NombreCompleto,
            String DocumentoIdentificacion,
            String CorreoElectronico, 
            int NoTelefono, 
            String NoCuenta, 
            String TipoCuenta)
        {
            try
            {
                EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities();
                Usuario b = ee.Usuario.FirstOrDefault(us => us.nombre == Usuario);
                if (b.pass == Contrasenia)
                {
                    Cliente c = new Cliente();
                    c.nombre = NombreCompleto;
                    c.documento_identificacion = DocumentoIdentificacion;
                    c.correo_electronico = CorreoElectronico;
                    c.no_telefono = NoTelefono;
                    c.no_cuenta = NoCuenta;
                    c.tipo_cuenta = TipoCuenta;

                    ee.Cliente.Add(c);
                    ee.SaveChanges();
                    return @"<RegistrarCliente>
                                <Exito>1</Exito>
                                <IdCliente>" + c.idCliente.ToString() + @"</IdCliente>
                             </RegistrarCliente>";
                }

            }
            catch (Exception e)
            {
                return @"<RegistrarCliente>
                            <Exito>0</Exito>
                            <Descripcion>" + e.Message + @"</Descripcion>
                         </RegistrarCliente>";
            }


            return ":v";

        }


        [WebMethod]
        public String RegistrarRemesa(
            String Usuario,
            String Contrasenia,
            int idClienteEnvia,
            String NombreReceptor,
            String CorreoReceptor,
            String DpiReceptor,
            Decimal MontoUS,
            String EstadoRemesa)
        {
            try
            {
                using (EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities())
                {
                    Usuario b = ee.Usuario.FirstOrDefault(us => us.nombre == Usuario);
                    if (b.pass == Contrasenia)
                    {
                        Cliente c = ee.Cliente.FirstOrDefault(client => client.idCliente == idClienteEnvia);

                        Banguat.Banguat banguat = new Banguat.Banguat();
                        //Fecha actual del sistema
                        String date = DateTime.Now.ToString("M/dd/yyyy");
                        String tasacambioxml = banguat.ConsultaTasaDeCambio(date);
                        String tasadecambio = "";

                        XElement root = XElement.Parse(tasacambioxml);
                        IEnumerable<XElement> tests =
                            from el in root.Elements("TipoCambioVentaQ")
                            select el;
                        foreach (XElement el in tests)
                            tasadecambio = el.Value;

                        Decimal tasacambiod = decimal.Parse(tasadecambio);


                        Remesa r = new Remesa();
                        r.Nombre_Receptor = NombreReceptor;
                        r.Correo_Receptor = CorreoReceptor;
                        r.DPI_Receptor = DpiReceptor;
                        r.MontoUSD = MontoUS;
                        r.MontoQ = MontoUS * tasacambiod;
                        r.Estado_Remesa = "EnProceso";
                        r.Cliente = c;
                        r.fechaEnvio = date;
                        
                        ee.Remesa.Add(r);
                        ee.SaveChanges();
                        r.CodigoSeguridad =( r.idRemesa * 100).ToString();
                        ee.SaveChanges();

                        return @"<RegistrarRemesa>
                                    <Exito>1</Exito>
                                    <IdRemesa>"+r.idRemesa+@"</IdRemesa>
                                    <EstadoRemesa>EnProceso</EstadoRemesa>
                                 </RegistrarRemesa>";
                    }
                }
            }
            catch (Exception e)
            {
                return @"<RegistrarRemesa>
                            <Exito>0</Exito>
                            <Descripcion>"+e.Message+@"</Descripcion>
                         </RegistrarRemesa>";
            }

            return "";
        }

        [WebMethod]
        public String CancelarRemesa(String Usuario, String Contrasenia, int IdRemesa) {
            try {
                EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities();
                Usuario u = ee.Usuario.FirstOrDefault(usuario => usuario.nombre == Usuario);
                if (u.pass == Contrasenia)
                {
                    Remesa r = ee.Remesa.FirstOrDefault(remesa => remesa.idRemesa == IdRemesa);
                    if (r.Estado_Remesa.Equals("EN PROCESO"))
                    {
                        ee.Remesa.Remove(r);
                        ee.SaveChanges();
                        return "<CancelarRemesa><Exito>1</Exito><IdRemesa>" + IdRemesa + "</IdRemesa></CancelarRemesa> ";
                    }
                    else {
                        return "<CancelarRemesa><Exito>0</Exito><Descripcion>La remesa no se encuentra en proceso</Descripcion></CancelarRemesa>";
                    }
                    
                }
                else {
                    return "<CancelarRemesa><Exito>0</Exito><Descripcion>Usuario y contrasenia no coinciden</Descripcion></CancelarRemesa>";
                }
               
            } catch(Exception e)
            {
                return "<CancelarRemesa><Exito>0</Exito><Descripcion>"+e.Message+"</Descripcion></CancelarRemesa>";
            }
            
        }

        [WebMethod]
        public String ConsultaComisionPorRemesa() {
            return "<ConsultaComisionPorRemesa><Exito>1</Exito><ComisionUS>1.00</ComisionUS></ConsultaComisionPorRemesa>";
        }

        [WebMethod]
        public String CodigoSeguridadIdRemesa(String Usuario, String Contrasenia, int IdRemesa, String CodigoSeguridad) {
            try
            {

                EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities();
                Usuario u = ee.Usuario.FirstOrDefault(usr => usr.nombre == Usuario);
                if (u.pass == Contrasenia)
                {
                    Remesa r = ee.Remesa.FirstOrDefault(rem => rem.CodigoSeguridad == CodigoSeguridad);
                    if (r == null) {
                        return "<CodigoSeguridadIdRemesa><Exito>0</Exito><Descripcion>Remesa no encontrada.</Descripcion></CodigoSeguridadIdRemesa>";
                    }
                    if (r.CodigoSeguridad == CodigoSeguridad) {
                         return "<CodigoSeguridadIdRemesa><Exito>1</Exito><NombreReceptor>" + r.Nombre_Receptor + "</NombreReceptor><DpiReceptor>" + r.DPI_Receptor + "</DpiReceptor><EstadoRemesa>" + r.Estado_Remesa + "</EstadoRemesa></CodigoSeguridadIdRemesa>";
                    } else {
                        return "<CodigoSeguridadIdRemesa><Exito>0</Exito><Descripcion>codigos de seguridad no coinciden</Descripcion></CodigoSeguridadIdRemesa>";
                    }
                    
                }
                else
                {
                    return "<CodigoSeguridadIdRemesa><Exito>0</Exito><Descripcion>Usuario y contraseña no coinciden</Descripcion></CodigoSeguridadIdRemesa>";
                }
            }
            catch (Exception e) {
                return "<CodigoSeguridadIdRemesa><Exito>0</Exito><Descripcion>" + e.Message + "</Descripcion></CodigoSeguridadIdRemesa>";
            }
            
            
        }

        [WebMethod]
        public String ListarRemesas(String Usuario, String Contrasenia, int IdRemesa, String NombreCompletoEmisor, String NombreCompletoReceptor, String MontoQ, String MontoUS, String FechaEnvio, String FechaEntrega) {
            try
            {
                EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities();
                Usuario u = ee.Usuario.FirstOrDefault(usr => usr.nombre == Usuario);
                if (u.pass == Contrasenia)
                {
                    Decimal dMontoUS;
                    Decimal dMontoQ;
                    try {
                        dMontoUS = Decimal.Parse(MontoUS);
                    }catch(Exception e)
                    {
                        dMontoUS = (Decimal)0.0;
                    }
                    try
                    {
                        dMontoQ = Decimal.Parse(MontoQ);
                    }
                    catch (Exception e)
                    {
                        dMontoQ = (Decimal)0.0;
                    }

                    var query = from a in ee.Remesa
                                where a.Cliente.nombre == NombreCompletoEmisor
                                || a.Nombre_Receptor == NombreCompletoReceptor
                                || a.MontoUSD == dMontoUS
                                || a.MontoQ == dMontoQ
                                || a.fechaEnvio == FechaEnvio
                                || a.fechaEntrega == FechaEntrega
                                select a;
                    if(query.Count() >0)
                    {
                        query = query.Where(a => a.idRemesa == IdRemesa);
                    }
                    else
                    {
                        query = from a in ee.Remesa
                                where a.idRemesa == IdRemesa
                                select a;
                    }

                    if (query.Count() < 1)
                        return ("<ListarRemesas><Exito>0</Exito><Descripcion>No existe ningun registro en la base de datos</Descripcion></ListarRemesas>");

                    String respuesta = "<ListarRemesas><Exito>1</Exito>";
                    foreach (var valor in query)
                        { respuesta += "<Remesa idRemesa=\""+valor.idRemesa.ToString()+"\"><NombreCompletoEmisor>"+valor.Cliente.nombre+"</NombreCompletoEmisor><NombreCompletoReceptor>"+valor.Nombre_Receptor+"</NombreCompletoReceptor><MontoQ>"+valor.MontoQ.ToString()+"</MontoQ><MontoUS>"+valor.MontoUSD.ToString()+"</MontoUS></Remesa>"; }
                    respuesta += "</ListarRemesas>";
                    return respuesta;
                }
                else
                {
                    return "<ListarRemesas><Exito>0</Exito><Descripcion>usuario y contraseña no coinciden</Descripcion></ListarRemesas>";
                }
                
            }
            catch (Exception e) {
                return "<ListarRemesas><Exito>0</Exito><Descripcion>" + e.Message+ "</Descripcion></ListarRemesas>";
            }
            
        }
        [WebMethod]
        public String ConsultaRemesaEspecifica(String Usuario, String Contrasenia, int IdRemesa, String NombreCompletoEmisor, String NombreCompletoReceptor, String MontoQ, String MontoUS, String FechaEnvio, String FechaEntrega)
        {
            try
            {
                EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities();
                Usuario u = ee.Usuario.FirstOrDefault(usr => usr.nombre == Usuario);
                if (u.pass == Contrasenia)
                {
                    Decimal dMontoUS;
                    Decimal dMontoQ;
                    try
                    {
                        dMontoUS = Decimal.Parse(MontoUS);
                    }
                    catch (Exception e)
                    {
                        dMontoUS = (Decimal)0.0;
                    }
                    try
                    {
                        dMontoQ = Decimal.Parse(MontoQ);
                    }
                    catch (Exception e)
                    {
                        dMontoQ = (Decimal)0.0;
                    }
                

                    var query = from a in ee.Remesa
                                where a.Cliente.nombre == NombreCompletoEmisor
                                || a.Nombre_Receptor == NombreCompletoReceptor
                                || a.MontoUSD == dMontoUS
                                || a.MontoQ == dMontoQ
                                || a.fechaEnvio == FechaEnvio
                                || a.fechaEntrega == FechaEntrega
                                select a;
                    if (query.Count() > 0)
                    {
                        query = query.Where(a => a.idRemesa == IdRemesa);
                    }
                    else
                    {
                        query = from a in ee.Remesa
                                where a.idRemesa == IdRemesa
                                select a;
                    }

                    if (query.Count() < 1)
                        return ("<ListarRemesas><Exito>0</Exito><Descripcion>No existe ningun registro en la base de datos</Descripcion></ListarRemesas>");

                    String respuesta = "<ListarRemesas><Exito>1</Exito>";
                    foreach (var valor in query)
                    { respuesta += "<Remesa idRemesa=\"" + valor.idRemesa.ToString() + "\"><NombreCompletoEmisor>" + valor.Cliente.nombre + "</NombreCompletoEmisor><NombreCompletoReceptor>" + valor.Nombre_Receptor + "</NombreCompletoReceptor><MontoQ>" + valor.MontoQ+ "</MontoQ><MontoUS>" + valor.MontoUSD + "</MontoUS></Remesa>"; }
                    respuesta += "</ListarRemesas>";
                    return respuesta;
                }
                else
                {
                    return "<ListarRemesas><Exito>0</Exito><Descripcion>usuario y contraseña no coinciden</Descripcion></ListarRemesas>";
                }

            }
            catch (Exception e)
            {
                return "<ConsultaComisionPorRemesa><Exito>0</Exito><Descripcion>" + e.Message + "</Descripcion></ConsultaComisionPorRemesa>";
            }

        }
        /*
        [WebMethod]
        public String ObtenerRemesasListas(String Usuario, String Contrasenia, int IdBancoReceptor)
        {
            try
            {
                EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities();
                Usuario u = ee.Usuario.FirstOrDefault(usr => usr.nombre == Usuario);
                if (u.pass == Contrasenia)
                {

                }
                else
                {
                    return "<ConsultaComisionPorRemesa><Exito>0</Exito><Descripcion>usuario y contraseña no coinciden</Descripcion></ConsultaComisionPorRemesa>";
                }
            }
            catch (Exception e)
            {
                return "<ConsultaComisionPorRemesa><Exito>0</Exito><Descripcion>" + e.Message + "</Descripcion></ConsultaComisionPorRemesa>";
            }

    */
        [WebMethod]
        public String CambioEstadoRemesa(String Usuario, String Contrasenia, int IdBancoReceptor, int IdRemesa,String NuevoEstadoRemesa)
        {
            try
            {
                if (!(NuevoEstadoRemesa.Equals("EN_PROCESO")
                    || NuevoEstadoRemesa.Equals("ENVIADA")
                    || NuevoEstadoRemesa.Equals("RECIBIDA")
                    || NuevoEstadoRemesa.Equals("COBRADA")))
                    return @"< ObtenerRemesasListas >
                                <Exito>0</Exito>
                                <Descripcion>El estado ingresado es incorrecto</Descripcion>
                            </ ObtenerRemesasListas >";

                EmpresaEmisoraEntities ee = new EmpresaEmisoraEntities();
                Usuario u = ee.Usuario.FirstOrDefault(usr => usr.nombre == Usuario);
                if (u.pass == Contrasenia)
                {
                    Remesa r = ee.Remesa.FirstOrDefault(re => re.idRemesa == IdRemesa);
                    r.Estado_Remesa = NuevoEstadoRemesa;
                    ee.SaveChanges();
                    return @"<ObtenerRemesasListas>
                                <Exito>1</Exito>
                                <IdRemesa>"+r.idRemesa+@"<IdRemesa>
                                <MontoUS>"+r.MontoUSD+@"</MontoUS>
                                <Estado>"+r.Estado_Remesa+@"</Estado>
                            </ ObtenerRemesasListas >";

                }
                else
                {
                    return "<ObtenerRemesasListas><Exito>0</Exito><Descripcion>usuario y contraseña no coinciden</Descripcion></ObtenerRemesasListas>";
                }
            }
            catch (Exception e)
            {
                return "<ObtenerRemesasListas><Exito>0</Exito><Descripcion>" + e.Message + "</Descripcion></ObtenerRemesasListas>";
            }


        }













        ////////////////////
        }
}
