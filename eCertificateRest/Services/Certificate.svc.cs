using eCertificateRest.Errors;
using Model;
using Model.Rwc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace eCertificateRest.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Certificate" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Certificate.svc or Certificate.svc.cs at the Solution Explorer and start debugging.
    [GlobalErrorHandlerBehaviour(typeof(GlobalErrorHandler))]
    public class Certificate : ICertificate
    {

       
        public  List<RwcVO> getRwc()
        {
            //var Request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            List<RwcVO> list = null;
            try
            {
                list = CRwc.Instance.getRwc();
            }
            catch (Exception ex)
            {
                //throw new Error("Problemas cargando el listado de terceros", "Comercial.getTercerosBusqueda", Tokenizer.GetUser(Request.Headers["Authorization"].ToString()), 0, ex);
            }
            return list;
        }
    }
}
