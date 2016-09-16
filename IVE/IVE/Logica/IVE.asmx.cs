using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using IVE.Datos;
using System.Xml.Linq;

namespace IVE
{
    /// <summary>
    /// Summary description for IVE
    /// </summary>
    [WebService(Namespace = "http://IVE.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class IVE : System.Web.Services.WebService
    {

        [WebMethod]
        public string RegistrarRemesa(
            String Usuario,
            String Password,
            int IdRemesa,
            Decimal MontoUS,
            String NombreCompletoEmisor,
            String CorreoEmisor,
            String DocumentoIdentificacionEmisor,
            String DireccionEmisor,
            int NoTelefonoEmisor,
            String NombreCompletoReceptor,
            String CorreoReceptor,
            String DocumentoIdentificacionReceptor,
            String DireccionReceptor,
            int NoTelefonoReceptor,
            String FechaRecepcionRemesa)
        {
            
            try
            {
                IVEEntities ive = new IVEEntities();
                Usuario b = ive.Usuario.FirstOrDefault(u => u.Usuario1 == Usuario);
                Remesa r = new Remesa();
                if (b.Password == Password)
                {
                    
                    r.ID_Remesa = IdRemesa;
                    r.Monto = MontoUS;
                    r.Nombre_Emisor = NombreCompletoEmisor;
                    r.Correo_Emisor = CorreoEmisor;
                    r.Doc_Identificacion_Emisor = DocumentoIdentificacionEmisor;
                    r.Direccion_Emisor = DireccionEmisor;
                    r.No_Telefono_Emisor = NoTelefonoEmisor;
                    r.Nombre_Receptor = NombreCompletoReceptor;
                    r.Correo_Receptor = CorreoReceptor;
                    r.Doc_Identificacion_Receptor = DocumentoIdentificacionReceptor;
                    r.Direccion_Receptor = DireccionReceptor;
                    r.No_Telefono_Receptor = NoTelefonoReceptor;
                    r.Fecha_Recepcion_Remesa = FechaRecepcionRemesa;

                    ive.Remesa.Add(r);
                    ive.SaveChanges();

                }
                return "<RegistrarRemesa><Exito>1</Exito><IdRemesa>"+r.Id+"</IdRemesa></RegistrarRemesa>";
            }
            catch (Exception e)
            {
                return "<RegistrarRemesa><Exito>0</Exito><IdRemesa>"+e.Message+"</IdRemesa></RegistrarRemesa>";
            }
        }

        [WebMethod]
        public string ConsultaRemesaRegistrada(
            String Usuario,
            String Password,
            int IdRemesa,
            Decimal MontoUS,
            String NombreCompletoEmisor,
            String CorreoEmisor,
            String DocumentoIdentificacionEmisor,
            String DireccionEmisor,
            int NoTelefonoEmisor,
            String NombreCompletoReceptor,
            String CorreoReceptor,
            String DocumentoIdentificacionReceptor,
            String DireccionReceptor,
            int NoTelefonoReceptor,
            String FechaRecepcionRemesa)
        {
            try
            {
                IVEEntities ivedb = new IVEEntities();
                Usuario b = ivedb.Usuario.FirstOrDefault(u => u.Usuario1 == Usuario);
                if (b.Password == Password)
                {
                      var query = from a in ivedb.Remesa
                                where a.ID_Remesa == IdRemesa
                                && a.Monto == MontoUS
                                && a.Nombre_Emisor == NombreCompletoEmisor
                                && a.Correo_Emisor == CorreoEmisor
                                && a.Doc_Identificacion_Emisor == DocumentoIdentificacionEmisor
                                && a.Direccion_Emisor == DireccionEmisor
                                && a.No_Telefono_Emisor == NoTelefonoEmisor
                                && a.Nombre_Receptor == NombreCompletoReceptor
                                && a.Correo_Receptor == CorreoReceptor
                                && a.Doc_Identificacion_Receptor == DocumentoIdentificacionReceptor
                                && a.Direccion_Receptor == DireccionReceptor
                                && a.No_Telefono_Receptor == NoTelefonoReceptor
                                && a.Fecha_Recepcion_Remesa == FechaRecepcionRemesa
                                select a;
                    String respuesta = @"<ConsultaRemesasRegistradas>
                                        <Exito>0</Exito>
                                        <Descripcion>La busqueda no obtuvo resultados</Descripcion>
                                    </ConsultaRemesasRegistradas>";
                    if(query.Count() > 0)
                    respuesta = @"<ConsultaRemesasRegistradas>
	                        <Exito>1</Exito>";
                    foreach (var valor in query)
                    {
                        respuesta = @"
	                        <Remesa	IdRemesa=" + "\"" + IdRemesa + "\"" + @">
	                        <MontoUS>" + valor.Monto + @"</MontoUS>
	                        <NombreCompletoEmisor>" + valor.Nombre_Emisor + @"</NombreCompletoEmisor>
	                        <CorreoEmisor>" + valor.Correo_Emisor + @"</CorreoEmisor>
	                        <DocumentoIdentificacionEmisor>" + valor.Doc_Identificacion_Emisor + @"</DocumentoIdentificacionEmisor>
	                        <NoTelefonoEmisor>" + valor.No_Telefono_Emisor + @"</NoTelefonoEmisor>
	                        <NombreCompletoReceptor>" + valor.Nombre_Receptor + @"</NombreCompletoReceptor>
	                        <CorreoReceptor>" + valor.Correo_Receptor + @"</CorreoReceptor>
	                        <DocumentoIdentificacionReceptor>" + valor.Doc_Identificacion_Receptor + @"</DocumentoIdentificacionReceptor>
	                        <NoTelefonoReceptor>" + valor.No_Telefono_Receptor + @"</NoTelefonoReceptor>
	                        </Remesa>
                        ";
                    }
                    respuesta = @"</ConsultaRemesasRegistradas>";

                    return respuesta;
                }
            }
            catch(Exception e)
            {
                return @"<ConsultaRemesasRegistradas>
                            <Exito>0</Exito>
                            <Descripcion>"+e.Message+@"</Descripcion>
                        </ConsultaRemesasRegistradas>";
            }
            return "";

        }


    }
}
