using System;
using System.Data;

namespace eCertificate.Utilities.SqlSync
{
	
    /// <summary>
	/// Interfaz con los métodos que se utilizan en las clases de conexión.
	/// </summary>

	public interface SqlInterface
	{


        void Open(string strCnx, string strEngineType);
        void Close();
        void Dispose();

		#region Métodos de Consulta.

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

        #region Métodos de Inserción

        //int Insert(string strSql);

        # endregion

        #region Métodos de Ejecución de sentencias Sql.

        void BeginTrans();
		

		void CommitTrans();
		int ExecuteSql(string strSql) ;
        int ExecuteSql(string strSql, ref System.Collections.ArrayList arlParams) ;
		int ExecuteSql(string [] strSql) ;
		
		#endregion

		void Destroy();

	}
}
