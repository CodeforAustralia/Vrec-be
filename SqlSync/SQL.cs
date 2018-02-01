using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Runtime.InteropServices;

namespace eCertificate.Utilities.SqlSync
{
    /// <summary>
    /// Summary description for SQL.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ProgId("eCertificate.Utilities.SqlSync.SQL")]
    public class SQL
    {
        private string strConnectString = "";
        private string strConnType = "";
        private string strEngineType = "";
        public string strPrefCnx = "";

        public bool isOpen = true;



        //private bool blnDisposed=false;



        public KMotor.typeMotorsDB TypeMotor;



        private SqlInterface sqlInterface;


        public SQL(string connectionString, string provider, string engine)
        {
            this.strConnectString = connectionString;

            if (ConfigurationManager.AppSettings["dsEncript"] == "S")
            {
                string cnx = "";
                foreach (string partCnx in strConnectString.Split(';'))
                {
                    if (partCnx != "")
                    {
                        cnx += (cnx == "" ? "" : ";") + partCnx.Split('=')[0] + "=";
                        string valuePart = partCnx.Split('=')[1];
                        valuePart = valuePart.Replace("|¨|", "=").Replace("|~|", ";");
                        try
                        {
                            valuePart = eCertificateRest.Utilities.SqlSync.CUtilidades.Desencriptar(valuePart);
                        }
                        catch
                        {
                            valuePart = partCnx.Split('=')[1];
                        }

                        cnx += valuePart;
                    }
                }
                this.strConnectString = cnx;
            }
            string providerName = provider + ";" + engine;
            this.strConnType = providerName.Split(';')[0];
            this.strEngineType = providerName.Split(';')[1];
            if (this.strConnType != "ORACLE" && this.strConnType != "SQLSERVER" && this.strConnType != "MYSQL" && this.strConnType != "ODBC" && this.strConnType != "OLEDB")
                throw new Exception("Error en el parámetro de tipo de motor de base de datos o motor de base de datos no soportado");
            CreateSQL();
        }
        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="strPref">Tipo de conexión que vamos a usar, los valores pueden ser: SQL Server ->MsSql, Oracle: Oracle, MySQL -> MySql, OleDB -> OleDb, ODBC -> Odbc</param>
        public SQL(string strPref)
        {
            if (strPref.IndexOf("Provider=Microsoft.Jet.OLEDB") >= 0)
            {
                this.strConnectString = strPref;
                this.strConnType = "OLEDB";
                this.strEngineType = "OLEDB";
                CreateSQL();
            }
            else
            {
                GetSQL(strPref, "", "", "", "");
            }
        }

        public SQL(string strPref, string UserDB, string PwdDB, string NameDB, string ServerName)
        {
            GetSQL(strPref, UserDB, PwdDB, NameDB, ServerName);

        }

        public void GetSQL(string strPref, string UserDB, string PwdDB, string NameDB, string ServerName)
        {

            if (strPref != null && !strPref.Equals(""))
                strPrefCnx = strPref;
            else
                strPrefCnx = "cms";
            this.strConnectString = this.GetConnectionString(this.strPrefCnx);

            if (ConfigurationManager.AppSettings["dsEncript"] == "S")
            {

                string cnx = "";
                foreach (string partCnx in strConnectString.Split(';'))
                {
                    if (partCnx != "")
                    {
                        cnx += (cnx == "" ? "" : ";") + partCnx.Split('=')[0] + "=";
                        string valuePart = partCnx.Split('=')[1];
                        valuePart = valuePart.Replace("|¨|", "=").Replace("|~|", ";");
                        try
                        {
                            valuePart = eCertificateRest.Utilities.SqlSync.CUtilidades.Desencriptar(valuePart);
                        }
                        catch
                        {
                            valuePart = partCnx.Split('=')[1];
                        }

                        cnx += valuePart;
                    }
                }
                this.strConnectString = cnx;
            }



            string providerName = ConfigurationManager.ConnectionStrings[this.strPrefCnx].ProviderName.ToUpper();

            if (UserDB != "")
            {
                this.strConnectString = this.strConnectString.Replace("@USERNAME", UserDB);
            }
            if (PwdDB != "")
            {
                this.strConnectString = this.strConnectString.Replace("@USERPWD", PwdDB);
            }
            if (NameDB != "")
            {
                this.strConnectString = this.strConnectString.Replace("@DATABASENAME", NameDB);
            }
            if (ServerName != "")
            {
                this.strConnectString = this.strConnectString.Replace("@SERVERNAME", ServerName);
            }

            providerName = providerName + ";" + providerName;
            this.strConnType = providerName.Split(';')[0];
            this.strEngineType = providerName.Split(';')[1];
            if (this.strConnType != "ORACLE" && this.strConnType != "SQLSERVER" && this.strConnType != "MYSQL" && this.strConnType != "ODBC" && this.strConnType != "OLEDB")
                throw new Exception("Error en el parámetro de tipo de motor de base de datos o motor de base de datos no soportado");
            CreateSQL();

        }


        /// <summary>
        /// Se obtiene el string de conexión para el prefijo de conexión especificado
        /// </summary>
        /// <param name="strPref">Prefijo de conexión</param>
        /// <returns></returns>
        public string GetConnectionString(string strPref)
        {

            string strConn = ConfigurationManager.ConnectionStrings[strPref].ConnectionString;

            strConnectString = strConn;
            if (ConfigurationManager.AppSettings["dsEncript"] == "S")
            {
                string cnx = "";
                foreach (string partCnx in strConnectString.Split(';'))
                {
                    if (partCnx != "")
                    {
                        cnx += (cnx == "" ? "" : ";") + partCnx.Split('=')[0] + "=";
                        string valuePart = partCnx.Split('=')[1];
                        valuePart = valuePart.Replace("|¨|", "=").Replace("|~|", ";");
                        try
                        {
                            valuePart = eCertificateRest.Utilities.SqlSync.CUtilidades.Desencriptar(valuePart);
                        }
                        catch
                        {
                            valuePart = partCnx.Split('=')[1];
                        }

                        cnx += valuePart;
                    }
                }

                strConn = cnx;
            }

            return strConn;

        }

        /// <summary>
        /// Con base en el prefijo cargado en el constructor, obtiene cual clase se usará para la conexión y la abre
        /// </summary>
        public void CreateSQL()
        {

            TypeMotor = (KMotor.typeMotorsDB)Enum.Parse(typeof(KMotor.typeMotorsDB), this.strEngineType, true);

            SqlFactory sqlFactory = new SqlFactory();

            sqlInterface = sqlFactory.getSQLClass(this.strConnType, TypeMotor);

            try
            {
                sqlInterface.Open(this.strConnectString, this.strConnType);
                isOpen = true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                strError += ". " + ex.Source;
                isOpen = false;
            }
            finally
            {
                // sqlInterface.Close();
            }

        }

        public string ToVarchar(string strField)
        {

            string strResponse = "";

            if (this.strEngineType == "SQL")
            {
                strResponse = string.Format("convert(varchar,{0})", strField);
            }
            else if (this.strEngineType == "MYSQL")
            {
                strResponse = string.Format("convert(varchar,{0})", strField);
            }
            else if (this.strEngineType == "ORACLE")
            {
                strResponse = string.Format("To_Char({0})", strField);
            }

            return strResponse;

        }

        public string GetFunctionMotor(string strFunction)
        {

            string strResponse = "";

            if (this.strEngineType == "SQL")
            {
                if (strFunction.ToLower().Equals("substring"))
                {
                    strResponse = "SubString";
                }
                else if (strFunction.ToLower().Equals("isnull"))
                {
                    strResponse = "IsNull";
                }
                else if (strFunction.ToLower().Equals("toVarchar"))
                {
                    strResponse = "convert(varchar,";
                }
            }
            else if (this.strEngineType == "MYSQL")
            {
                if (strFunction.ToLower().Equals("substring"))
                {
                    strResponse = "SubString";
                }
                else if (strFunction.ToLower().Equals("isnull"))
                {
                    strResponse = "IsNull";
                }
                else if (strFunction.ToLower().Equals("toVarchar"))
                {
                    strResponse = "convert(varchar,";
                }
            }
            else if (this.strEngineType == "ORACLE")
            {
                if (strFunction.ToLower().Equals("substring"))
                {
                    strResponse = "SUBSTR";
                }
                else if (strFunction.ToLower().Equals("isnull"))
                {
                    strResponse = "NVL";
                }
                else if (strFunction.ToLower().Equals("toVarchar"))
                {
                    strResponse = "to_char(";
                }
            }

            return strResponse;

        }

        #region Execute Method Sql

        /// <summary>
        /// Inicia una transacción en el motor de base de datos
        /// </summary>
        public void BeginTrans()
        {

            try
            {
                sqlInterface.BeginTrans();
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);
            }
            //finally
            //{
            //    sqlInterface.Close();
            //}

        }

        /// <summary>
        /// Deja en firme las operaciones realizadas en la base de datos durante una transacción
        /// </summary>
        public void CommitTrans()
        {

            try
            {

                sqlInterface.CommitTrans();
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);
            }
            //finally
            //{
            //    sqlInterface.Close();
            //}

        }


        /// <summary>
        /// Método que ejecuta Funciones de Insercion, actualizacion y eliminacion
        /// a través de una transaccion.
        /// </summary>
        /// <param name="strSql">String con la sentencia SQL a ejecutar.</param>
        /// 
        public virtual int ExecuteSql(string strSql)
        {
            try
            {
                strSql = ClearSql(strSql);
                return sqlInterface.ExecuteSql(strSql);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);
            }
        }

        public virtual int ExecuteSql(string strSql, ref System.Collections.ArrayList arlParams)
        {
            try
            {
                strSql = ClearSql(strSql);
                return sqlInterface.ExecuteSql(strSql, ref arlParams);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                //strError+=". "+ ex.Source;
                throw new Exception(strError);
            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}

        }

        /// <summary>
        /// Limpia la cadena de Consulta Sql evitando el llamado Sql Injection. 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public string ClearSql(string strSql)
        {

            string strResult = "";
            if ((strSql.IndexOf("' ") >= 0 || strSql.IndexOf(" '") >= 0) && false)
            {
                while (strSql.IndexOf(" ' ") >= 0)
                {
                    strSql = strSql.Replace(" ' ", " '");
                    if (strSql.IndexOf(" ' ") == -1)
                        break;
                }

                string strCols = strSql;

                strCols.Trim();
                string strCar = "";
                for (int i = 0; i < strCols.Length; i++)
                {
                    strCar = strCols.Substring(i, 1);
                    if (strCar.Equals("'") && i > 0 && i < strCols.Length - 1)
                    {
                        if (",=><()' ".IndexOf(strCols.Substring(i - 1, 1)) == -1 && ",=><()' ".IndexOf(strCols.Substring(i + 1, 1)) == -1)
                        {
                            strCar = "''";
                        }

                    }
                    strResult += strCar;
                }
            }
            else
                strResult = strSql;

            strSql = strSql.Replace("\r", "");
            strSql = strSql.Replace("\n", "");

            return strResult;
        }

        /// <summary>
        /// Método que ejecuta Funciones de Insercion, actualizacion y eliminacion
        /// a través de una transaccion.
        /// </summary>
        /// <param name="strSql">Arreglo de Strings con las sentencias SQL a ejecutar.</param>
        public int ExecuteSql(string[] strSql)
        {

            try
            {
                for (int i = 0; i < strSql.Length; i++)
                {
                    strSql[i] = ClearSql(strSql[i]);
                }
                return sqlInterface.ExecuteSql(strSql);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);
            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}

        }



        #endregion

        /// <summary>
        /// Función para insertar un registro en una tabla de la base de datos
        /// </summary>
        /// <param name="sqlInsertQuery">SqlInsertQuery con los datos del query de inserción</param>
        /// <returns>Retorna un Integer con el Identity </returns>
        public long Insert(SqlInsertQuery sqlInsertQuery)
        {
            try
            {
                long intIdentity = 0;
                string strIdentity = "";
                //Se obtiene el valor del autonumérico a insertar para los motores que lo requieran
                switch (this.strEngineType)
                {
                    case "ORACLE":
                        //El nombre de la secuencia del autonumérico en ORACLE se debe crear con el siguiente estandar:
                        //seq_nombretabla_nombrecampo. Ej: seq_portal_category_category_id
                        strIdentity = this.GetValue("SELECT " + "seq_" + sqlInsertQuery.TableName + "_" + sqlInsertQuery.AutoNumField.Name + ".NEXTVAL FROM dual");
                        if (!Int64.TryParse(strIdentity, out intIdentity))
                            throw new Exception("Error obteniendo autonumérico para inserción en ORACLE. Method SQL:Insert");
                        break;
                }
                if (intIdentity > 0) //Se obtuvo el autonumérico
                    sqlInsertQuery.AutoNumField.Value = intIdentity.ToString();
                else
                {
                    //Se limpia el valor para no incluirlo en la inserción
                    if (sqlInsertQuery.AutoNumField != null)
                        sqlInsertQuery.AutoNumField.Value = "";
                }

                string strSql = sqlInsertQuery.GetSqlSentence();
                int intAffectedRows = this.ExecuteSql(strSql);
                if (intAffectedRows > 0) //El Insert se efectuó
                {
                    if (intIdentity <= 0) //Obtener identidad del registro insertado para los motores que aplica
                    {
                        switch (this.strEngineType)
                        {
                            case "SQL":
                            case "MYSQL":
                                strIdentity = this.GetValue("SELECT @@IDENTITY");
                                break;
                        }
                        if (!Int64.TryParse(strIdentity, out intIdentity))
                            throw new Exception("Error obteniendo autonumérico del registro Insertado. Method SQL:Insert");
                    }
                }
                return intIdentity;
            }
            catch
            {
                return 0;
            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}

        }

        /// <summary>
        /// Función para insertar un registro en una tabla de la base de datos
        /// </summary>
        /// <param name="sqlInsertQuery">SqlInsertQuery con los datos del query de inserción</param>
        /// <returns>Retorna un Integer con el Identity </returns>
        public int Update(SqlUpdateQuery sqlUpdateQuery)
        {
            int intAffectedRows = 0;
            try
            {

                string strSql = sqlUpdateQuery.GetSqlSentence();
                intAffectedRows = this.ExecuteSql(strSql);
                if (intAffectedRows == 0) //El Insert se efectuó
                {
                    throw new Exception("Error Method SQL:UPDATE no se afectaron registros.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Method SQL:UPDATE" + ex.Message);
            }
            return intAffectedRows;
            //finally
            //{
            //    //sqlInterface.Close();
            //}

        }

        /// <summary>
        /// Borra registros de una tabla con base en la condición especificada
        /// </summary>
        /// <param name="strTabla">Nombre de la tabla de la cual se van a borrar los datos</param>
        /// <param name="strCond">Condición en formato SQL</param>
        public void Delete(string strTabla, string strCond)
        {
            try
            {
                strCond = ClearSql(strCond);
                sqlInterface.Delete(strTabla, strCond);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);
            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}

        }


        #region Métodos de consulta

        /// <summary>
        /// Retorna un DataSet con la información obtenida de la base de datos al ejecutar una sentencia SQL
        /// </summary>
        /// <param name="strQuery">Sentencia SQL a ejecutar</param>
        /// <returns>DataSet con los datos obtenidos al ejecutar la sentencia </returns>
        public DataSet GetDataSet(string strQuery)
        {
            ArrayList arrParams = new ArrayList();
            return GetDataSet(strQuery, ref arrParams);
        }
        public DataSet GetDataSet(string strQuery, ref System.Collections.ArrayList arlParams)
        {
            try
            {
                strQuery = ClearSql(strQuery);
                return sqlInterface.GetDataSet(strQuery, ref arlParams);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);


            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}

        }

        /// <summary>
        /// Retorna un DataTable con la información obtenida de la base de datos al ejecutar una sentencia SQL
        /// </summary>
        /// <param name="strQuery">>Sentencia SQL a ejecutar</param>
        /// <returns>DataTable con los datos obtenidos al ejecutar la sentencia</returns>
        public DataTable GetDataTable(string strQuery)
        {
            ArrayList arrParams = new ArrayList();
            return GetDataTable(strQuery, ref arrParams);
        }
        public DataTable GetDataTable(string strQuery, ref System.Collections.ArrayList arlParams)
        {
            try
            {
                strQuery = ClearSql(strQuery);
                return sqlInterface.GetDataTable(strQuery, ref arlParams);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);
            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}


        }

        /// <summary>
        /// Retorna un DataRow con la primera fila obtenida de la base de datos al ejecutar una sentencia SQL
        /// </summary>
        /// <param name="strQuery">>Sentencia SQL a ejecutar</param>
        /// <returns>DataRow con los datos obtenidos al ejecutar la sentencia</returns>

        public DataRow GetDataRow(string strQuery)
        {
            ArrayList arrParams = new ArrayList();
            return GetDataRow(strQuery, ref arrParams);
        }
        public DataRow GetDataRow(string strQuery, ref ArrayList arlParams)
        {
            try
            {
                strQuery = ClearSql(strQuery);
                return sqlInterface.GetDataRow(strQuery, ref arlParams);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);


            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}


        }

        /// <summary>
        /// Verifica, con base en una sentencia SQL si existe un registro en la base de datos
        /// </summary>
        /// <param name="strTabla">Nombre de la tabla</param>
        /// <param name="strCond">Condición en formato SQL</param>
        /// <returns>True si existe, false si no.</returns>
        public bool Exists(string strTabla, string strCond)
        {
            try
            {
                strCond = ClearSql(strCond);

                return sqlInterface.Exists(strTabla, strCond);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);


            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}


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
                strSql = ClearSql(strSql);
                return sqlInterface.Exists(strSql);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);

            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}


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
            try
            {
                strCampo = ClearSql(strCampo);
                strCond = ClearSql(strCond);
                return sqlInterface.GetValue(strCampo, strTabla, strCond);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);


            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}

        }

        /// <summary>
        /// Obtiene el valor de un campo de una tabla en la base de datos
        /// </summary>
        /// <param name="strSql">Sentencia SQL</param>
        /// <returns>Valor del primer campo especificado en el query</returns>
        public string GetValue(string strSql)
        {
            try
            {
                strSql = ClearSql(strSql);
                return sqlInterface.GetValue(strSql);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);


            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}

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
                strSql = ClearSql(strSql);
                return sqlInterface.GetValue(strSql, col);
            }
            catch (Exception ex)
            {
                string strError = ex.InnerException.Message;
                strError += ". " + ex.Source;
                throw new Exception(strError);


            }
            //finally
            //{
            //    //sqlInterface.Close();
            //}


        }

        #endregion

        #region Métodos Destructores


        public void Dispose()
        {
            sqlInterface.Dispose();
            //if (!this.blnDisposed) 
            //    this.blnDisposed = true;
            //this.Destroy();
        }

        public void Destroy()
        {
            //sqlInterface.Destroy();

        }


        /// <summary>
        /// Destructora de la clase
        /// </summary>
        ~SQL()
        {
            //this.Dispose();
            //sqlInterface.Close();
            //IM:Eso cierra la preciada conexión
        }

        #endregion

        /// <summary>
        /// Limpia datos de sentencias mal intensionadas.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ClearText(string data)
        {

            if (data != null)
            {
                //Datos relevantes de limpieza de código.
                data = data.Replace("--", "");
                data = data.Replace("'", "");
                data = data.Replace(" OR ", "");
                data = data.Replace(" AND ", "");
                data = data.Replace("SELECT", "");
                data = data.Replace("UPDATE", "");
                data = data.Replace("DELETE", "");
                data = data.Replace("INSERT", "");
                data = data.Replace(" or ", "");
                data = data.Replace(" and ", "");
                data = data.Replace("select", "");
                data = data.Replace("update", "");
                data = data.Replace("delete", "");
                data = data.Replace("insert", "");
                data = data.Replace(";", "");
                data = data.Replace("LIKE", "");
                data = data.Replace("like", "");
            }
            return data;
        }
    }

    /// <summary>
    /// Permite el manejo de una sentencia SQL de Inserción
    /// </summary>
    public class SqlUpdateQuery
    {
        private string tableName;
        private string condition;
        private ArrayList arlSqlQueryFields;

        public string TableName { get { return tableName; } }

        /// <summary>
        /// Inicializa la clase con el nombre de la tabla en la cual se desea insertar
        /// </summary>
        /// <param name="strTableName">Nombre de la tabla en la que se desean insertar los datos</param>
        public SqlUpdateQuery(string strTableName, string strCondition)
        {
            this.tableName = strTableName;
            this.condition = strCondition;
            this.arlSqlQueryFields = new ArrayList();
        }

        /// <summary>
        /// Adiciona un campo a la setencia de inserción. Si el campo es autonumérico no se tiene en cuenta su valor
        /// </summary>
        /// <param name="sqlQueryField">SqlQueryField con los datos del campo a insertar</param>
        public void AddField(SqlQueryField sqlQueryField)
        {
            if (sqlQueryField != null)
            {
                sqlQueryField.Value = ClearText(sqlQueryField.Value);
                arlSqlQueryFields.Add(sqlQueryField);
            }

        }

        /// <summary>
        /// Adiciona un campo a la sentencia de inserción. Si el campo es de tipo autonumérico no se tiene en cuenta el valor
        /// </summary>
        /// <param name="strFieldName">Nombre del campo en el que se desea realizar la inserción</param>
        /// <param name="strFieldValue">Valor del campo a insertar</param>
        /// <param name="datatype">Tipo de dato del campo a Insertar</param>
        public void AddField(string strFieldName, string strFieldValue, SqlQueryDataType datatype)
        {
            SqlQueryField sqlQueryField = new SqlQueryField();
            sqlQueryField.Name = strFieldName;
            sqlQueryField.Value = ClearText(strFieldValue);
            sqlQueryField.Datatype = datatype;
            this.AddField(sqlQueryField);
        }

        /// <summary>
        /// Limpia datos de sentencias mal intensionadas.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ClearText(string data)
        {

            if (data != null)
            {
                //Datos relevantes de limpieza de código.
                data = data.Replace("--", "");
                data = data.Replace("'", "");
                data = data.Replace(" OR ", "");
                data = data.Replace(" AND ", "");
                data = data.Replace("SELECT", "");
                data = data.Replace("UPDATE", "");
                data = data.Replace("DELETE", "");
                data = data.Replace("INSERT", "");
                data = data.Replace(" or ", "");
                data = data.Replace(" and ", "");
                data = data.Replace("select", "");
                data = data.Replace("update", "");
                data = data.Replace("delete", "");
                data = data.Replace("insert", "");
                data = data.Replace(";", "");
                data = data.Replace("LIKE", "");
                data = data.Replace("like", "");
            }
            return data;
        }

        /// <summary>
        /// Obtiene la sentencia SQL para el motor de base de datos especificado
        /// </summary>
        /// <param name="strEngineType">Tipo de motor de base de datos</param>
        /// <returns>String con la sentencia Sql generada</returns>
        public string GetSqlSentence()
        {
            string strCurrentField = "";
            string strCurrentValue = "";
            string strSqlSentence = "UPDATE " + this.TableName + " SET ";
            //Se agregan los campos
            for (int i = 0; i < arlSqlQueryFields.Count; i++)
            {
                SqlQueryField sqlQueryField = (SqlQueryField)arlSqlQueryFields[i];
                switch (sqlQueryField.Datatype)
                {
                    case SqlQueryDataType.Date:
                        if (sqlQueryField.Name != null && sqlQueryField.Name != "")
                        {
                            strCurrentField = sqlQueryField.Name;
                            strCurrentValue = "'" + sqlQueryField.Value + "'";
                        }
                        break;

                    case SqlQueryDataType.String:
                        strCurrentField = sqlQueryField.Name;
                        strCurrentValue = "'" + sqlQueryField.Value + "'";
                        break;
                    case SqlQueryDataType.Number:
                        strCurrentField = sqlQueryField.Name;
                        strCurrentValue = sqlQueryField.Value;
                        break;
                }

                strSqlSentence += strCurrentField + "=" + strCurrentValue;

            }
            strSqlSentence += this.condition;
            return strSqlSentence;
        }
    }


    /// <summary>
    /// Permite el manejo de una sentencia SQL de Inserción
    /// </summary>
    public class SqlInsertQuery
    {
        private string tableName;
        private SqlQueryField autoNumField = null;
        private ArrayList arlSqlQueryFields;

        public string TableName { get { return tableName; } }
        public SqlQueryField AutoNumField { get { return autoNumField; } }

        /// <summary>
        /// Inicializa la clase con el nombre de la tabla en la cual se desea insertar
        /// </summary>
        /// <param name="strTableName">Nombre de la tabla en la que se desean insertar los datos</param>
        public SqlInsertQuery(string strTableName)
        {
            this.tableName = strTableName;
            this.arlSqlQueryFields = new ArrayList();
        }

        /// <summary>
        /// Adiciona un campo a la setencia de inserción. Si el campo es autonumérico no se tiene en cuenta su valor
        /// </summary>
        /// <param name="sqlQueryField">SqlQueryField con los datos del campo a insertar</param>
        public void AddField(SqlQueryField sqlQueryField)
        {
            if (sqlQueryField != null)
            {
                sqlQueryField.Value = ClearText(sqlQueryField.Value);
                if (sqlQueryField.Datatype == SqlQueryDataType.AutoNumeric)
                {
                    if (this.AutoNumField == null)
                    {
                        sqlQueryField.Value = "";
                        this.autoNumField = sqlQueryField;
                    }
                    else
                        throw new Exception("Solo puede existir un campo autonumérico en la inserción. Method: SqlInsertQuery.AddField");
                }
                else
                {
                    arlSqlQueryFields.Add(sqlQueryField);
                }
            }

        }

        /// <summary>
        /// Adiciona un campo a la sentencia de inserción. Si el campo es de tipo autonumérico no se tiene en cuenta el valor
        /// </summary>
        /// <param name="strFieldName">Nombre del campo en el que se desea realizar la inserción</param>
        /// <param name="strFieldValue">Valor del campo a insertar</param>
        /// <param name="datatype">Tipo de dato del campo a Insertar</param>
        public void AddField(string strFieldName, string strFieldValue, SqlQueryDataType datatype)
        {
            SqlQueryField sqlQueryField = new SqlQueryField();
            sqlQueryField.Name = strFieldName;
            sqlQueryField.Value = ClearText(strFieldValue);
            sqlQueryField.Datatype = datatype;
            this.AddField(sqlQueryField);
        }

        /// <summary>
        /// Limpia datos de sentencias mal intensionadas.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ClearText(string data)
        {

            if (data != null)
            {
                //Datos relevantes de limpieza de código.
                data = data.Replace("--", "");
                data = data.Replace("'", "");
                data = data.Replace(" OR ", "");
                data = data.Replace(" AND ", "");
                data = data.Replace("SELECT", "");
                data = data.Replace("UPDATE", "");
                data = data.Replace("DELETE", "");
                data = data.Replace("INSERT", "");
                data = data.Replace(" or ", "");
                data = data.Replace(" and ", "");
                data = data.Replace("select", "");
                data = data.Replace("update", "");
                data = data.Replace("delete", "");
                data = data.Replace("insert", "");
                data = data.Replace(";", "");
                data = data.Replace("LIKE", "");
                data = data.Replace("like", "");
            }
            return data;
        }

        /// <summary>
        /// Obtiene la sentencia SQL para el motor de base de datos especificado
        /// </summary>
        /// <param name="strEngineType">Tipo de motor de base de datos</param>
        /// <returns>String con la sentencia Sql generada</returns>
        public string GetSqlSentence()
        {
            string strFields = "";
            string strValues = "";
            string strCurrentField = "";
            string strCurrentValue = "";
            string strSqlSentence = "";
            //Se agrega el campo autonumérico
            if (this.AutoNumField != null)
            {
                if (this.AutoNumField.Value != "" && this.AutoNumField.Value != null)
                {
                    strFields += this.AutoNumField.Name;
                    strValues += this.AutoNumField.Value;
                    strFields += ",";
                    strValues += ",";
                }
            }
            //Se agregan los demás campos
            for (int i = 0; i < arlSqlQueryFields.Count; i++)
            {
                SqlQueryField sqlQueryField = (SqlQueryField)arlSqlQueryFields[i];
                switch (sqlQueryField.Datatype)
                {
                    case SqlQueryDataType.Date:
                        if (sqlQueryField.Name != null && sqlQueryField.Name != "")
                        {
                            strCurrentField = sqlQueryField.Name;
                            strCurrentValue = "'" + sqlQueryField.Value + "'";
                        }
                        break;

                    case SqlQueryDataType.String:
                        strCurrentField = sqlQueryField.Name;
                        strCurrentValue = "'" + sqlQueryField.Value + "'";
                        break;
                    case SqlQueryDataType.Number:
                        strCurrentField = sqlQueryField.Name;
                        strCurrentValue = sqlQueryField.Value;
                        break;
                }
                if (strCurrentField != null && strCurrentField != "" && strCurrentValue != null && strCurrentValue != "")
                {
                    strFields += strCurrentField;
                    if (strCurrentField.Contains("category_date"))
                    {
                        if (strCurrentValue != "" && strCurrentValue != null)
                        {
                            strCurrentValue = strCurrentValue.Remove(11).Replace("/", "").Replace("-", "");
                            strValues += strCurrentValue + "'";
                        }
                        else
                            strValues += strCurrentValue;
                    }
                    else
                        strValues += strCurrentValue;
                    strFields += ",";
                    strValues += ",";
                }
            }
            //se quita la última coma
            strFields = strFields.Remove(strFields.Length - 1);
            strValues = strValues.Remove(strValues.Length - 1);
            strSqlSentence = "INSERT INTO " + tableName + " (" + strFields + ") VALUES (" + strValues + ")";
            return strSqlSentence;
        }
    }

    /// <summary>
    /// Clase que modela un campo en el query de inserción
    /// </summary>
    public class SqlQueryField
    {
        private string name;
        private string fvalue;
        private SqlQueryDataType datatype;

        public string Name { get { return name; } set { name = value; } }
        public string Value { get { return fvalue; } set { fvalue = value; } }
        public SqlQueryDataType Datatype { get { return datatype; } set { datatype = value; } }
    }

    /// <summary>
    /// Especifica los posibles tipos de datos a procesar en un Query
    /// </summary>
    public enum SqlQueryDataType
    {
        /// <summary>
        /// Tipo de datos autonumérico
        /// </summary>
        AutoNumeric,
        /// <summary>
        /// Tipo de datos fecha
        /// </summary>
        Date,
        /// <summary>
        /// Tipo de datos numérico (int, double, byte, etc)
        /// </summary>
        Number,
        /// <summary>
        /// Tipo de datos cadena de caracteres
        /// </summary>
        String
    }
}
