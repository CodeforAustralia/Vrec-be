using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Rwc
{
    public class RwcVO
    {
        public string id { get; set; }
        public string vin { get; set; }
        public string reg { get; set; }
        public TimeSpan date { get; set; }
       

        public RwcVO()
        {
        }
        public RwcVO(DataRow dr)
        {
        }
        public RwcVO(string id,string vin)
        {
            this.id = id;
            this.vin = vin;
        }
    }
}
