using eCertificate.Utilities.SqlSync;
using Model.Rwc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CRwc
    {
       
        protected static CRwc objControl = null;
        private static string defaultConex = "cnx";
        public CRwc()
        {

        }
        public static CRwc Instance
        {
            get
            {
                if (objControl == null)
                    objControl = new CRwc();

                return objControl;

            }
        }
        public List<RwcVO> getRwc()
        {
            SQL sql = new SQL(defaultConex);
            Model.Rwc.Rwc us = new Model.Rwc.Rwc(sql);
            try
            {
                return us.getRwc();
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message);
            }
            finally
            {
                sql.Dispose();
            }
        }
    }
}
