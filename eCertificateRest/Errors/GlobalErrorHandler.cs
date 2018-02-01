using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using eCertificateRest.Errors;
using eCertificateRest.Utils;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace eCertificateRest
{
    public class GlobalErrorHandler:IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            Error err = error as Error;

            try
            {
                if (err != null)
                {

                    string message = err.InnerException != null ?err.InnerException.Message.ToString(): err.Message.ToString();
                    string stacktrace= err.InnerException!=null?err.InnerException.StackTrace.ToString():"" ;

                   // Mongo.SaveError(err.componente, message, stacktrace, err.user, err.tipo);
                }
            }
            catch { }

          
            //GUARDAR LOG EN MONGO!!
            return true;
        }
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            

            Error err = error as Error;
            if (err == null)
            {
               err = new Error(error.Message.ToString(),"","",0,error);
               err.tipo = "ERROR";
            }            

            //JSON
            ErrorVO erVo = new ErrorVO();
                erVo.Message = err.Message;
                erVo.ErrorCode = err.codigo;


         

            fault = Message.CreateMessage(version, "", erVo, new DataContractJsonSerializer(erVo.GetType()));
            var wbf = new WebBodyFormatMessageProperty(WebContentFormat.Json);
            fault.Properties.Add(WebBodyFormatMessageProperty.Name, wbf);

            // return custom error code.


            //ASSIGNAMOS EL CODIGO DEL ERROR
            
            if (err.tipo!="OPTIONS")
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            else
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
            }
        


        }

    


       
    }
}