using eCertificateRest;
using eCertificateRest.Errors;
using eCertificateRest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Web;
namespace eCertificateRest
{
    [GlobalErrorHandlerBehaviour(typeof(GlobalErrorHandler))]
    public class Inspector : IDispatchMessageInspector
    {

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref Message request,
            IClientChannel channel, InstanceContext instanceContext)
        {
            try
            {
                var Request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;

                if (Request.Method == "OPTIONS")
                    throw new Error("Petición automatica de ANGULAR", true); 

                
                 if(Request.Headers["Authorization"] == null)
                        throw new Error("Datos incorrectos en la petición ( Debe especificar el Token)!!", "Inspector.AfterReceiveRequest", "",1,new Exception());  

                    //VALIDE TOKENS
                    if (Request.Headers["Authorization"].ToString() != ""  )
                    {
                        var token = Request.Headers["Authorization"].ToString();

                        if (!Tokenizer.IsValidToken(token))
                        {                           
                            throw new Error("Token Invalido", "TOKEN","",1);  
                        }
                        else
                        {
                            string user = Tokenizer.GetTokenPart(token, 1);
                            OperationContext context = OperationContext.Current;
                            MessageProperties messageProperties = context.IncomingMessageProperties;
                            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                            //Mongo.SaveRequest(messageProperties.Via.PathAndQuery, user, Request.Method, endpointProperty.Address);
                        }
                    }
                    
                
            }
            catch(Exception ex)
            {
                Error er = ex as Error;
                if (er!=null &&  er.tipo!="OPTIONS")
                {
                    if (er.tipo == "ERROR")
                    {
                        throw new Error(er.Message,"", "",1, ex);
                    }
                    else
                    {
                        throw new Error(er.Message, "TOKEN", "",1);  
                    }
                }
                else if (er.tipo=="OPTIONS")
                {
                    throw new Error(er.Message, true); 
                }
                else {

                    throw new Error(er.Message, true); 
                }
            }
            finally
            {
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,DELETE");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Max-Age", "3600");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Allow", "GET,HEAD,POST,PUT,DELETE,TRACE,OPTIONS,PATCH");

              
            }


            return instanceContext;
        }

        public void BeforeSendReply(ref  Message reply,
            object correlationState)
        {

            try
            {


            }
            catch( Exception exc){

                string hola = "";

            }


            
        }

        #endregion
    }
}