using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace eCertificateRest.Errors
{
    public class GlobalErrorHandlerBehaviourAttribute:Attribute,IServiceBehavior
    {
        private readonly Type errorHandlerType;
        public GlobalErrorHandlerBehaviourAttribute(Type errorHandlerType) {
            this.errorHandlerType = errorHandlerType;
        }
        void IServiceBehavior.Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase) { }
        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {}

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            IErrorHandler handler = (IErrorHandler)Activator.CreateInstance(this.errorHandlerType);

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher=channelDispatcherBase as ChannelDispatcher;

                if(channelDispatcher!=null){

                    channelDispatcher.ErrorHandlers.Add(handler);
                }

            }
        
        }

        
    }
}