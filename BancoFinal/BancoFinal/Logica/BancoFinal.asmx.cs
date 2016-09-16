using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BancoFinal.Datos;
using System.Xml.Linq;

namespace BancoFinal.Logica
{
    /// <summary>
    /// Summary description for BancoFinal
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BancoFinal : System.Web.Services.WebService
    {

         string webService(String Usuario, String Contrasenia, int IdRemesa, String CodigoSeguridad)
        {



            return @"<CodigoSeguridadIdRemesa>
                        <Exito>1</Exito>
                        <NombreReceptor>Juan Caminante</NombreReceptor>
                        <DpiReceptor>1029480101<DpiReceptor>
                        <EstadoRemesa>Recibida</EstadoRemesa>	
                    </ CodigoSeguridadIdRemesa >";
        }

        

        [WebMethod]
        public string RecepcionRemesa(String Usuario, String Contrasenia, String LoteRemesas)
        {
            //LoteRemesas = "<LoteRemesas><remesa	IdRemesa=\"2331\"	MontoUS=\"10.00\"/><remesa	IdRemesa=\"2332\"	MontoUS=\"20.00\"/> <remesa	IdRemesa=\"2333\"	MontoUS=\"30.00\"/></LoteRemesas>";
            try { 
                using (BancoFinalEntities bf = new BancoFinalEntities())
                {
                    Usuario u = bf.Usuario.FirstOrDefault(us => us.nombre == Usuario);
                    if(u.pass == Contrasenia)
                    {

                        XElement root = XElement.Parse(LoteRemesas);
                        //return root.Value;
                        IEnumerable<XElement> remesas =
                            from el in root.Elements("remesa")
                            select el;
                        foreach (XElement el in remesas)
                        {
                            Remesas r = new Remesas();
                            r.idRemesa = (int) el.Attribute("IdRemesa");
                            r.montoUSD = (decimal) el.Attribute("MontoUS");
                            
                            bf.Remesas.Add(r);
                            
                        }
                        bf.SaveChanges();
                    }
                }
                return @"<RecepcionRemesa>
                            <Exito> 1 </Exito>
                        </RecepcionRemesa> ";
            }catch(Exception e)
            {
                return @"<RecepcionRemesa>
                            <Exito>0</Exito>
                            <Descripcion>"+e.Message+@"</Descripcion>
                        </RecepcionRemesa>";
            }
        }

        [WebMethod]
        public string PagoRemesa(
            String Usuario, 
            String Contrasenia, 
            String NombreEmisor,
            int IdRemesa,
            String CodigoSeguridad,
            String NombreReceptor,
            String DpiReceptor)
        {
            try
            {
                using (BancoFinalEntities bf = new BancoFinalEntities())
                {
                    Usuario u = bf.Usuario.FirstOrDefault(us => us.nombre == Usuario);
                    if (u.pass == Contrasenia)
                    {
                        //cambiar stub
                        //String respuesta = webService(Usuario, Contrasenia, IdRemesa, CodigoSeguridad);
                        EmpresaEmisora.EmpresaEmisora e = new EmpresaEmisora.EmpresaEmisora();
                        String respuesta = e.CodigoSeguridadIdRemesa(Usuario, Contrasenia, IdRemesa, CodigoSeguridad);
                        XElement root = XElement.Parse(respuesta);
                        if (int.Parse(root.Element("Exito").Value) > 0)
                        {
                            if (root.Element("EstadoRemesa").Value.Equals("RECIBIDA"))
                            {
                                
                               String remesa= e.ListarRemesas(Usuario, Contrasenia, IdRemesa, null, null, null, null, null, null);
                                


                                XElement root2 = XElement.Parse(remesa);
                                root2 = root2.Element("Remesa");
                                Decimal montousd = Decimal.Parse(root2.Element("MontoUS").Value);
                                Decimal montogtq = Decimal.Parse(root2.Element("MontoQ").Value);
                                Decimal tipoCambio = montogtq / montousd;


                                e.CambioEstadoRemesa(Usuario, Contrasenia, 14, IdRemesa, "COBRADA");
                                return @"< PagoRemesa>
                                            < Exito > 1 </ Exito >
                                            < MontoUS >"+root2.Element("MontoUS").Value+@" </ MontoUS >
                                            < TipoCambio > "+tipoCambio.ToString()+@" </ TipoCambio >
                                            < EstadoFinal > COBRADA </ EstadoFinal >
                                            </ PagoRemesa > ";
                            }

                        }
                        //return root.Value;
                        /*
                        IEnumerable<XElement> remesas =
                            from el in root.Elements("CodigoSeguridadIdRemesa")
                            select el;
                        foreach (XElement el in remesas)
                        {

                        }*/
                    }
                }
            }catch(Exception e)
            {
                return @"<	RecepcionRemesa >
                            <Exito>0</Exito>
                            <Descripcion>"+e.Message+@"</Descripcion>
                         </	RecepcionRemesa >";
            }

            return "";

        }

        [WebMethod]
        public String ListarRemesasRecibidas(
            String Usuario,
            String Contrasenia,
            int IdRemesa,
            String NombreCompletoEmisor,
            String NombreCompletoReceptor,
            String MontoQ,
            String MontoUS,
            String FechaEnvio,
            String FechaEntrega,
            String EstadoRemesa)
        {
            try
            {
                using (BancoFinalEntities bf = new BancoFinalEntities())
                {
                    Usuario u = bf.Usuario.FirstOrDefault(us => us.nombre == Usuario);
                    if (u.pass == Contrasenia)
                    {
                        EmpresaEmisora.EmpresaEmisora e = new EmpresaEmisora.EmpresaEmisora();
                        String remesa = e.ListarRemesas(Usuario, Contrasenia, IdRemesa, NombreCompletoEmisor,
                            NombreCompletoReceptor, MontoQ, MontoUS, FechaEnvio, FechaEntrega);

                        XElement root2 = XElement.Parse(remesa);
                        root2 = root2.Element("Remesa");
                        //aca hay que agregar un listado consultar con grupo 
                        return @"< ListarRemesasRecibidas>
                                    <Exito>1</Exito>
                                    <Remesa IdRemesa="+root2.Element("Remesa").Attribute("idRemesa").Value+@">
                                    <NombreCompletoEmisor> "+ root2.Element("NombreCompletoEmisor").Value + @"</NombreCompletoEmisor>
                                    <NombreCompletoReceptor>"+ root2.Element("NombreCompletoReceptor").Value + @"</NombreCompletoReceptor>
                                    <MontoQ>"+ root2.Element("MontoQ").Value + @"</MontoQ>
                                    <MontoUS>"+ root2.Element("MontoUS").Value + @"</MontoUS>
                                    <FechaEnvio>"+0+ @"</FechaEnvio>
                                    <FechaEntrega>"+ 0+ @"</FechaEntrega>
                                    <EstadoRemesa>"+ 0 + @"</EstadoRemesa>
                                    </Remesa>
                                </ ListarRemesasRecibidas>";



                    }
                }
                return "";
            }
            catch (Exception e)
            {
                return @"< ListarRemesasRecibidas >
                            <Exito>0</Exito>
                            <Descripcion>"+e.Message+@"</Descripcion>
                         </ ListarRemesasRecibidas >";
            }
            return "";
        }


    }
}
