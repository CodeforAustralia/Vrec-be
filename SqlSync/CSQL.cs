using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Collections;

namespace eCertificate.Utilities.SqlSync
{
    /// <summary>
    /// Realiza transacciones SQL como Consultas, Inserciones, Actualizaciones y Eliminaciones
    /// </summary>
    internal class CSQL : SqlInterface
    {
        #region Attributes

        /// <summary>
        /// Objeto de transacción de la clase
        /// </summary>
        private SqlTransaction sqlTransaction;

        /// <summary>
        /// Indicador si la transacción está iniciada o no.
        /// </summary>
        private bool TransBegun = false;

        /// <summary>
        /// Cadena de Conexion
        /// </summary>
        private string strConnectString;

        /// <summary>
        /// Objeto de conexion de la clase
        /// </summary>
        private SqlConnection sqlConn;

        /// <summary>
        /// tipo de motor
        /// </summary>
        private string strEngineType = "";

        /// <summary>
        /// booleano indicador de dispose de la clase
        /// </summary>
        private bool blnDisposed;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructora de la Clase
        /// </summary>
        /// 
        public CSQL()
        {

        }

        #endregion

        /// <summary>
        /// Abre una conexión con la base de datos. 
        /// Abre una conexión con la base de datos. 
        /// </summary>
        /// <param name="strCnx">Recibe string de conexión</param>
        /// <param name="strEngineType">Tipo de motor.</param>
        public void Open(string strCnx, string strEngineType)
        {
            this.strEngineType = strEngineType;
            strConnectString = strCnx;
            this.sqlConn = new SqlConnection();
            this.sqlConn.ConnectionString = strConnectString;
            this.sqlConn.Open();
            SetFormatoFecha();
            this.blnDisposed = false;


        }

        /// <summary>
        /// Cierra la conexión.
        /// </summary>
        public void Close()
        {
            if (sqlConn.State == ConnectionState.Open)
                sqlConn.Close();
        }

        /// <summary>
        /// Establece el formato de la fecha utilizado
        /// </summary>
        public virtual void SetFormatoFecha()
        {
            string strSql = "select convert(datetime,'24/16/2006')";


            strSql = "SET DATEFORMAT DMY; set language spanish  ";
            string strFecha = System.DateTime.Now.ToShortDateString();
            SqlCommand sqlCmd = this.sqlConn.CreateCommand();
            sqlCmd.CommandText = strSql;
            try
            {
                //Ejecuta la Transaccion
                sqlCmd.ExecuteNonQuery();
                strSql = "select convert(datetime,'" + strFecha + "')";

                //strFecha=this.GetValue(strSql);

            }
            catch (System.Data.SqlClient.SqlException exc)
            {
                throw new Exception("Error Sql en la Sentencia " + strSql + ".  Error: " + exc.Message, exc);
            }
            catch (Exception e)
            {
                //Genera Una Excepcion con el error
                throw (new Exception(e.Message + " " + e.Source + ". Formateando la fecha: " + strFecha));
            }
            finally
            {
                sqlCmd.Dispose();
            }
        }

        #region Query Methods.

        /// <summary>
        /// Verifica, con base en una sentencia SQL si existe un registro en la base de datos
        /// </summary>
        /// <param name="strTabla">Nombre de la tabla</param>
        /// <param name="strCond">Condición en formato sql</param>
        /// <returns>True si existe, false si no.</returns>
        public bool Exists(string strTabla, string strCond)
        {
            string strSql = "";

            try
            {
                strSql = "SELECT count(*) FROM " + strTabla + "  ";
                if (strCond != "")
                    strSql += " WHERE " + strCond;
                ArrayList arrParams = new ArrayList();
                DataSet ds = this.GetDataSet(strSql, ref arrParams);

                bool existe = false;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    existe = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()) > 0;
                }

                ds.Dispose();
                return existe;
            }
            catch (System.Data.OleDb.OleDbException exc)
            {
                throw new Exception("Error SQL en la Sentencia " + strSql + ".  Error: " + exc.Message, exc);
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message + " " + e.Source));
            }

        }

        /// <summary>
        /// Verifica, con base en una sentencia SQL si existe un registro en la base de datos
        /// </summary>
        /// <param name="strSql">Sentencia SQL</param>
        /// <returns>True si existe, false si no.</returns>
        public bool Exists(string strSql)
        {
            try
            {
                ArrayList arrParams = new ArrayList();
                DataSet ds = this.GetDataSet(strSql, ref arrParams);

                bool existe = ds.Tables[0].Rows.Count > 0;
                ds.Dispose();
                return existe;
            }
            catch (System.Data.OleDb.OleDbException exc)
            {
                throw new Exception("Error SQL en la Sentencia " + strSql + ".  Error: " + exc.Message, exc);
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message + " " + e.Source));
            }

        }

        /// <summary>
        /// Obtiene el valor de un campo de una tabla en la base de datos
        /// </summary>
        /// <param name="strCampo">Nombre del campo</param>
        /// <param name="strTabla">Nombre de la tabla</param>
        /// <param name="strCond">Condición en formato SQL</param>
        /// <returns>String con el valor del campo</returns>
        public string GetValue(string strCampo, string strTabla, string strCond)
        {
            string strSql = "";

            try
            {
                strSql = "SELECT " + strCampo + " FROM " + strTabla + "  ";
                if (strCond != "")
                    strSql += " WHERE " + strCond;
                ArrayList arrParams = new ArrayList();
                DataSet ds = this.GetDataSet(strSql, ref arrParams);

                string valReturn = "0";
                if (ds.Tables[0].Rows.Count > 0)
                {
                    valReturn = ds.Tables[0].Rows[0][0].ToString();
                }
                ds.Dispose();
                return valReturn;
            }
            catch (System.Data.OleDb.OleDbException exc)
            {
                throw new Exception("Error SQL en la Sentencia " + strSql + ".  Error: " + exc.Message, exc);
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message + " " + e.Source));
            }

        }

        /// <summary>
        /// Obtiene el valor de un campo de una tabla en la base de datos
        /// </summary>
        /// <param name="strSql">Sentencia SQL</param>
        /// <returns>Valor del primer campo especificado en el query</returns>
        public string GetValue(string strSql)
        {
            return GetValue(strSql, 0);
        }

        /// <summary>
        /// Obtiene el valor de un campo de una tabla en la base de datos
        /// </summary>
        /// <param name="strSql">Sentencia SQL</param>
        /// <param name="col">Número del campo a recuperar de la sentencia</param>
        /// <returns>Valor del campo cuyo número se especificó en el parámetro col</returns>
        public string GetValue(string strSql, int col)
        {

            try
            {
                ArrayList arrParams = new ArrayList();
                DataSet ds = this.GetDataSet(strSql, ref arrParams);
                
                string valReturn = "0";
                if (ds.Tables[0].Rows.Count > 0)
                {
                    valReturn = ds.Tables[0].Rows[0][col].ToString();
                }
                ds.Dispose();
                return valReturn;
            }
            catch (System.Data.OleDb.OleDbException exc)
            {
                throw new Exception("Error SQL en la Sentencia " + strSql + ".  Error: " + exc.Message, exc);
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message + " " + e.Source));
            }

        }


        /// <summary>
        /// Borra registros de una tabla con base en la condición especificada
        /// </summary>
        /// <param name="strTabla">Nombre de la tabla de la cual se van a borrar los datos</param>
        /// <param name="strCond">Sentencia SQL con las condiciones para borrar los registros</param>
        public void Delete(string strTabla, string strCond)
        {
            string strSql = "";
            if (sqlConn.State == ConnectionState.Closed)
                sqlConn.Open();

            System.Data.SqlClient.SqlCommand sqlCmd = this.sqlConn.CreateCommand();
            sqlCmd.CommandType = CommandType.Text;

            try
            {
                strSql = "DELETE FROM " + strTabla + "  ";
                if (strCond != "")
                    strSql += " WHERE " + strCond;
                sqlCmd.CommandText = strSql;
                sqlCmd.ExecuteNonQuery();

            }
            catch (System.Data.OleDb.OleDbException exc)
            {
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw new Exception("Error SQL en la Sentencia " + strSql + ".  Error: " + exc.Message, exc);
            }
            catch (Exception e)
            {
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw (new Exception(e.Message + " " + e.Source));
            }
            finally
            {
                sqlCmd.Dispose();
                sqlConn.Close();
            }

        }

        /// <summary>
        /// Retorna un DataSet con la información obtenida de la base de datos al ejecutar una sentencia SQL
        /// </summary>
        /// <param name="strQuery">Sentencia SQL a ejecutar</param>
        /// <returns>DataSet con los datos obtenidos al ejecutar la sentencia </returns>
        public DataSet GetDataSet(string strQuery, ref System.Collections.ArrayList arlParams)
        {
            if (sqlConn.State == ConnectionState.Closed)
                sqlConn.Open();

            SqlDataAdapter odbDA = new SqlDataAdapter(strQuery, this.sqlConn);
            DataSet ds = new DataSet();
            try
            {
                if (arlParams != null)
                {
                    foreach (ParamSQL paramSQL in arlParams)
                    {
                        if (paramSQL.Name == "")
                        {
                            odbDA.SelectCommand.Parameters.AddWithValue("?", paramSQL.Value);
                        }
                        else
                        {
                            SqlParameter sqlParam = new SqlParameter(paramSQL.Name, paramSQL.DataType);
                            sqlParam.Direction = paramSQL.Direction;
                            sqlParam.Value = paramSQL.Value;
                            odbDA.SelectCommand.Parameters.Add(sqlParam);
                        }
                    }
                }
                //Itera y llena en el DataSet creando tablas por cada consulta enviada
                if (strQuery.Trim().Split(' ').Length == 1)
                {
                    odbDA.SelectCommand.CommandType = CommandType.StoredProcedure;
                }
                odbDA.SelectCommand.CommandText = strQuery;
                odbDA.SelectCommand.CommandTimeout = 3600;
                //Asocia el comando a la transacción iniciada
                if (this.TransBegun)
                    odbDA.SelectCommand.Transaction = this.sqlTransaction;

                odbDA.Fill(ds, "Result");


                return ds;
            }
            catch (System.Data.SqlClient.SqlException exc)
            {
                //Hace rollback de la transacción abierta
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw new Exception("Error al Consultar.  Error: " + exc.Message + "; Source: " + exc.Source, exc);
            }
            catch (Exception e)
            {
                //Hace rollback de la transacción abierta
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw new Exception("Error: " + e.Message, e);
            }
            finally
            {
                //IM:Eliminar y limpiar los objetos
                odbDA.Dispose();
            }



        }

        /// <summary>
        /// Retorna un DataTable con la información obtenida de la base de datos al ejecutar una sentencia SQL
        /// </summary>
        /// <param name="strQuery">>Sentencia SQL a ejecutar</param>
        /// <returns>DataTable con los datos obtenidos al ejecutar la sentencia</returns>
        public DataTable GetDataTable(string strQuery, ref System.Collections.ArrayList arlParams)
        {

            SqlCommand cmd = new SqlCommand("dtResult");

            try
            {
                DataTable dt = new DataTable();

                DataColumn dcKey = new DataColumn();

                dcKey.ColumnName = "Identifier";
                dcKey.DataType = Type.GetType("System.Int64");
                dcKey.AutoIncrement = true;
                dcKey.AutoIncrementSeed = 1;
                dcKey.ReadOnly = true;

                dt.Columns.Add(dcKey);

                UniqueConstraint uKey = new UniqueConstraint(dcKey);
                dt.Constraints.Add(uKey);


                cmd.Connection = sqlConn;


                if (arlParams != null)
                {
                    foreach (ParamSQL paramSQL in arlParams)
                    {
                        if (paramSQL.Name == "")
                        {
                            cmd.Parameters.AddWithValue("?", paramSQL.Value);
                        }
                        else
                        {
                            SqlParameter sqlParam = new SqlParameter(paramSQL.Name, paramSQL.DataType);
                            sqlParam.Direction = paramSQL.Direction;
                            sqlParam.Value = paramSQL.Value;
                            cmd.Parameters.Add(sqlParam);
                        }
                    }
                }
                //Itera y llena en el DataSet creando tablas por cada consulta enviada
                if (strQuery.Trim().Split(' ').Length == 1)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                cmd.CommandText = strQuery;
                cmd.CommandTimeout = 3600;
                //Asocia el comando a la transacción iniciada
                if (this.TransBegun)
                    cmd.Transaction = this.sqlTransaction;

                dt.Load(cmd.ExecuteReader());



                return dt;

            }
            catch (System.Data.SqlClient.SqlException exc)
            {
                //Hace rollback de la transacción abierta
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw new Exception("Error al Consultar.  Error: " + exc.Message + "; Source: " + exc.Source, exc);
            }
            catch (Exception e)
            {
                //Hace rollback de la transacción abierta
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw new Exception("Error: " + e.Message, e);
            }
            finally
            {
                //IM:Eliminar y limpiar los objetos
                cmd.Dispose();
            }


        }

        /// <summary>
        /// Retorna un DataTable con la información obtenida de la base de datos al ejecutar una sentencia SQL
        /// </summary>
        /// <param name="strQuery">>Sentencia SQL a ejecutar</param>
        /// <returns>DataTable con los datos obtenidos al ejecutar la sentencia</returns>
        public DataTable GetDataTable1(string strQuery, ref System.Collections.ArrayList arlParams)
        {
            DataSet ds = this.GetDataSet(strQuery, ref arlParams);
            DataTable dt = null;
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        /// <summary>
        /// Retorna un DataRow con la primera fila obtenida de la base de datos al ejecutar una sentencia SQL
        /// </summary>
        /// <param name="strQuery">>Sentencia SQL a ejecutar</param>
        /// <returns>DataRow con los datos obtenidos al ejecutar la sentencia</returns>
        public DataRow GetDataRow(string strQuery, ref System.Collections.ArrayList arlParams)
        {

            DataTable dt = this.GetDataTable(strQuery, ref arlParams);
            DataRow dr = null;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                    dr = dt.Rows[0];
            }
            return dr;
        }

        #endregion

        #region Execute Sql

        /// <summary>
        /// Inicia una transacción en el motor de base de datos
        /// </summary>
        public void BeginTrans()
        {
            if (sqlConn.State == ConnectionState.Closed)
                sqlConn.Open();
            //sqlTransaction = this.sqlConn.BeginTransaction();
            sqlTransaction = this.sqlConn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            TransBegun = true;
        }

        /// <summary>
        /// Deja en firme las operaciones realizadas en la base de datos durante una transacción
        /// </summary>
        public void CommitTrans()
        {
            if (sqlConn.State == ConnectionState.Closed)
                sqlConn.Open();
            sqlTransaction.Commit();
            TransBegun = false;
        }

        /// <summary>
        /// Método que ejecuta Funciones de Inserción, actualización y eliminación
        /// a través de una transacción y retorna el número de filas afectadas por la sentencia ejecutada.
        /// </summary>
        /// <param name="strSql">String con la sentencia SQL a ejecutar.</param>
        /// <returns>Int con el número de filas afectadas</returns>
        public virtual int ExecuteSql(string strSql)
        {
            if (sqlConn.State == ConnectionState.Closed)
                sqlConn.Open();

            SqlCommand sqlCmd = this.sqlConn.CreateCommand();
            int intAffectedRows = 0;


            //Asigna Texto de Ejecucion Sql
            sqlCmd.CommandText = strSql;

            if (this.TransBegun)
                sqlCmd.Transaction = sqlTransaction;
            try
            {
                //Ejecuta la Transaccion
                intAffectedRows = sqlCmd.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException exc)
            {
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw new Exception("Error Sql en la Sentencia " + strSql + ".  Error: " + exc.Message, exc);
            }
            catch (Exception e)
            {
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                    intAffectedRows = 0;
                }
                //Genera Una Excepcion con el error
                throw (new Exception(e.Message + " " + e.Source));
            }
            finally
            {
                sqlCmd.Dispose();
            }
            return intAffectedRows;
        }

        public virtual int ExecuteSql(string strSql, ref System.Collections.ArrayList arlParamsOut)
        {
            if (sqlConn.State == ConnectionState.Closed)
                sqlConn.Open();

            SqlCommand sqlCmd = this.sqlConn.CreateCommand();
            int intAffectedRows = 0;

            if (strSql.Trim().Split(' ').Length == 1)
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
            }
            //Asigna Texto de Ejecucion Sql
            sqlCmd.CommandText = strSql;


            System.Collections.ArrayList arlParams = arlParamsOut;
            foreach (ParamSQL paramSQL in arlParams)
            {

                SqlParameter sqlParam = new SqlParameter(paramSQL.Name, paramSQL.DataType);
                sqlParam.Direction = paramSQL.Direction;
                sqlParam.Value = paramSQL.Value;
                sqlCmd.Parameters.Add(sqlParam);

            }
            if (this.TransBegun)
                sqlCmd.Transaction = sqlTransaction;
            try
            {
                //Ejecuta la Transaccion
                sqlCmd.CommandTimeout = 3600;
                intAffectedRows = sqlCmd.ExecuteNonQuery();
                int i = -1;
                foreach (ParamSQL paramSQL in arlParams)
                {
                    i++;
                    if (paramSQL.Direction != ParameterDirection.Input)
                    {
                        paramSQL.ValueRet = sqlCmd.Parameters[i].Value.ToString();

                    }
                }

            }
            catch (System.Data.SqlClient.SqlException exc)
            {
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw new Exception("Error Sql en la Sentencia " + strSql + ".  Error: " + exc.Message, exc);
            }
            catch (Exception e)
            {
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                    intAffectedRows = 0;
                }
                //Genera Una Excepcion con el error
                throw (new Exception(e.Message + " " + e.Source));
            }
            finally
            {
                sqlCmd.Dispose();
            }

            arlParamsOut = arlParams;
            return intAffectedRows;
        }

        /// <summary>
        /// Método que ejecuta Funciones de Inserción, actualización y eliminación
        /// a través de una transacción y retorna el número de filas afectadas por la sentencia ejecutada.
        /// </summary>
        /// <param name="strSql">Arreglo de Strings con las sentencias SQL a ejecutar.</param>
        /// <returns>Int con el número de filas afectadas</returns>
        public int ExecuteSql(string[] strSql)
        {
            //Declaracion de Variables
            SqlCommand sqlCmd;
            int intAffectedRows = 0;
            string strSentenciaSql = string.Empty;
            sqlCmd = new SqlCommand();
            sqlCmd.Connection = this.sqlConn;
            if (this.TransBegun)
                sqlCmd.Transaction = sqlTransaction;
            try
            {
                //Itera a través de las sentencias Sql.
                for (int i = 0; i < strSql.Length; i++)
                {
                    if (strSql[i] != string.Empty)
                    {
                        strSentenciaSql = strSql[i];
                        sqlCmd.CommandText = strSentenciaSql;
                        int intResult = sqlCmd.ExecuteNonQuery();
                        if (intResult != -1) //La instrucción es Update, Insert, Delete
                            intAffectedRows += intResult;
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException exc)
            {

                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                    intAffectedRows = 0;
                }
                string strMensajeError = string.Empty;
                for (int i = 0; i < exc.Errors.Count; i++)
                {
                    strMensajeError += "Mensaje: " + exc.Errors[i].Message + "\n" +
                    "Native: " + exc.Errors[i].Number.ToString() + "\n" +
                    "Source: " + exc.Errors[i].Source + "\n" +
                    "Sql: " + exc.Errors[i].State + "\n";
                }
                throw new Exception("Error al ejecutar: " + strMensajeError, exc);
            }
            catch (Exception e)
            {
                //PG: Deshacer los cambios si falla la transacción
                if (this.TransBegun)
                {
                    sqlTransaction.Rollback();
                    this.TransBegun = false;
                }
                throw new Exception("Error en la Transaccion: " + e.Message + "Fuente: " + e.Source, e);
            }
            finally
            {
                sqlCmd.Dispose();
            }

            return intAffectedRows;
        }


        #endregion

        #region Garbage Collectors

        /// <summary>
        /// Hace la limpieza del objeto y cierra la conexión
        /// </summary>
        public void Dispose()
        {
            if (!this.blnDisposed)
            {
                //this.Destroy();
                if (this.sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();

                this.blnDisposed = true;
            }
        }

        public void Destroy()
        {
            if (TransBegun)
                CommitTrans();
            if (!this.blnDisposed)
            {
                this.sqlConn.Close();
                this.sqlConn.Dispose();
            }
        }

        /// <summary>
        /// Destructora de la clase
        /// </summary>
        ~CSQL()
        {
            //this.Dispose();
            //Eso cierra la preciada conexión
        }

        #endregion
    }
}