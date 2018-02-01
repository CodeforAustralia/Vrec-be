using System;
using System.Data;

namespace eCertificate.Utilities.SqlSync
{
	
    /// <summary>
	/// Interfaz con los m�todos que se utilizan en las clases de conexi�n.
	/// </summary>

	public interface SqlInterface
	{


        void Open(string strCnx, string strEngineType);
        void Close();
        void Dispose();

		#region M�todos de Consulta.

        DataSet GetDataSet(string strQuery, ref System.Collections.ArrayList arlParams);
        DataTable GetDataTable(string strQuery, ref System.Collections.ArrayList arlParams);
        DataRow GetDataRow(string strQuery, ref System.Collections.ArrayList arlParams);

		bool Exists(string strTabla,  string strCond);
		bool Exists(string strSql);

		string GetValue(string strCampo,string strTabla,  string strCond);
		string GetValue(string strSql);
		string GetValue(string strSql,int col);
		
		void Delete(string strTabla,  string strCond);

		#endregion

        #region M�todos de Inserci�n

        //int Insert(string strSql);

        # endregion

        #region M�todos de Ejecuci�n de sentencias Sql.

        void BeginTrans();
		

		void CommitTrans();
		int ExecuteSql(string strSql) ;
        int ExecuteSql(string strSql, ref System.Collections.ArrayList arlParams) ;
		int ExecuteSql(string [] strSql) ;
		
		#endregion

		void Destroy();

	}
}
