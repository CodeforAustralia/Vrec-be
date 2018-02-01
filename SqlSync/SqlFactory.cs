using System;
using System.Data;


namespace eCertificate.Utilities.SqlSync
{
	/// <summary>
	/// Define que tipo de motor de datos se va ha utilizar.
	/// </summary>
	public class SqlFactory
	{
		public SqlFactory(){
			
		}
        
        /// <summary>
        /// Obtiene la clase de conexión
        /// </summary>
        /// <param name="strTipoCnx">Tipo de motor al cual se va a conectar.</param>
        /// <returns>Clase de conexión.</returns>
        public SqlInterface getSQLClass(string strTipoCnx, KMotor.typeMotorsDB TypeMotor)
		{
            if (strTipoCnx == "SQL" || strTipoCnx == "SQLSERVER")
			{
				return new CSQL();
			}
           
			
			return null;

		}

		
	}
}
