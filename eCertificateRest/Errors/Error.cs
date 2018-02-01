using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eCertificateRest.Errors
{
    public class Error:Exception
    {

        public int codigo { get; set; }
        public string componente { get; set; }
        public string tipo { get; set; }//ERROR, ADVERTENCIA
        public string user { get; set; }

        public Error(string message, string componente, string user, int codigo)   
            : base(message)
        {
            this.componente = componente;
            this.tipo = "ADVERTENCIA";
            this.user = user;
            this.codigo = codigo;
        }

        public Error(string message, string componente,string user,int codigo, Exception inner)
            : base(message, inner)
        {
            this.componente = componente;
            this.tipo = "ERROR";
            this.user = user;
            this.codigo = codigo;
        }

        public Error(string message, bool options)
            : base(message)
        {            
            this.tipo = "OPTIONS";            
        }



    }
}