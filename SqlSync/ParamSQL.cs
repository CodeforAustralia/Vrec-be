using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace eCertificate.Utilities.SqlSync
{
    public static class KMotor
    {
        public enum typeMotorsDB
        {
            SQLSERVER    
        }
    }

    public class ParamSQL
    {

        private string name = "";
        private string m_value = "";
        private object o_value = null;
        private DataTable dt_value = null;
        private int m_size = 0;
        private string m_valueRet = "";
        private SqlDbType dataType;

        private byte[] imageData = null;


        private ParameterDirection m_direction = ParameterDirection.Input;

        public ParamSQL()
        {

        }

        public ParamSQL(string Name, byte[] _imageData)
        {
            this.name = "";
            this.m_value = "";
            this.o_value = null;
            this.dt_value = null;
            this.m_size = 0;
            this.m_valueRet = "";
            this.imageData = null;
            this.name = Name;
            this.m_value = "";
            this.imageData = _imageData;
            this.dataType = SqlDbType.Image;
            this.m_direction = ParameterDirection.Input;
            return;
        }


        public ParamSQL(string Name, string Value)
        {
            name = Name;
            m_value = Value;
            dataType = SqlDbType.VarChar;
            m_direction = ParameterDirection.Input;

        }

        public ParamSQL(string Name, string Value, System.Data.SqlDbType DataType)
        {
            name = Name;
            m_value = Value;
            dataType = DataType;
            m_direction = ParameterDirection.Input;
        }

        public ParamSQL(string Name, string Value, System.Data.SqlDbType DataType, ParameterDirection m_Direction)
        {
            name = Name;
            m_value = Value;
            dataType = DataType;
            m_direction = m_Direction;
        }

        public string Name { get { return name; } set { name = value; } }
        public object Value
        {
            get
            {
                if (imageData != null)
                {
                    return imageData;
                }
                return m_value;

            }
            set { m_value = Convert.ToString(value); }
        }
        public object ValueObject { get { return o_value; } set { o_value = value; } }
        public System.Data.SqlDbType DataType { get { return dataType; } set { dataType = value; } }
        public ParameterDirection Direction { get { return m_direction; } set { m_direction = value; } }
        public string ValueRet { get { return m_valueRet; } set { m_valueRet = value; } }
        public int Size { get { return m_size; } set { m_size = value; } }


        public DataTable ValueTable { get { return dt_value; } set { dt_value = value; } }



    }
}
