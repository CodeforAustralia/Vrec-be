using Model.Rwc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace eCertificateRest.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICertificate" in both code and config file together.
    [ServiceContract]
    public interface ICertificate
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "rwc")]
        List<RwcVO> getRwc();
    }
}
