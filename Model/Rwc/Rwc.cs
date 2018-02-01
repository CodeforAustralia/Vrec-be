using eCertificate.Utilities.SqlSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Rwc
{
    public class Rwc
    {
        SQL sql = null;
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <returns></returns>
        public Rwc()
        {
        }
        /// <summary>
        /// Constructor de la clase, recargado con el conector a base de datos instanciado
        /// </summary>
        /// <returns></returns>
        public Rwc(SQL _sql)
        {
            sql = _sql;
        }
        public List<RwcVO> getRwc()
        {
            StringBuilder sb = new StringBuilder();
            System.Data.DataTable dt = null;
            List<RwcVO> list = new List<RwcVO>(); ;
            //CARGAMOS INFORMACION DEL CAMPO 

            list.Add(new RwcVO("1", "VIN1234567890"));
            list.Add(new RwcVO("2", "VIN098765432"));
            list.Add(new RwcVO("3", "VIN234567809"));
            list.Add(new RwcVO("4", "VIN234567809"));

            /*
            sb.AppendFormat("sp_getTerceros @codigo='{0}',@razonSocial='{1}',@identificacion='{2}',@email='{3}',@direccion='{4}',@telefono='{5}',@estado={6},@tipo={7},@intermediario='{8}'", codigo, razonSocial, identificacion, email, direccion, telefono, estado, tipo, intermediario);
            dt = sql.GetDataTable(sb.ToString());
            //CARGAMOS CONTRATOS
            if (dt != null)
            {
                list = new List<TerceroVO>();
                foreach (DataRow dr in dt.Rows)
                {
                    TerceroVO c = new TerceroVO(dr);
                    list.Add(c);
                }
            }
            */
            return list;
        }
    }
}
