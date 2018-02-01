using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Data;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;

namespace eCertificateRest.Utilities.SqlSync
{
	/// <summary>
	/// Clase que contiene metodos para ser utilizados por toda la aplicacion
	/// </summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ProgId("CUtilidades")]
    public class CUtilidades
    {


        


        #region Datatabler Ordering
        public static DataTable OrderDataTable(DataTable SourceTable, string OrderBy)
        {
        

            //Create a subset of the data, and process that table
            DataTable subset = SourceTable.Clone();



            // Sort the table
            DataRow[] subRows;
            subRows = SourceTable.Select("1=1", OrderBy);

            // Import our subset
            foreach (DataRow dr in subRows)
            {
                subset.ImportRow(dr);
            }


            return subset;
        }

       

      
      
        #endregion



        /// <summary>
        /// howtocompare= m:minutos, s: segundos, t:ticks, mm:milisegundos, yyyy:años, q:trimestre, M:meses, d:dias
        /// </summary>
        /// <param name="howtocompare"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [DispId(3), ComVisible(true)]
        public static double DateDiff(string howtocompare, System.DateTime startDate, System.DateTime endDate)
        {
            double diff = 0;
            try
            {
                System.TimeSpan TS = new System.TimeSpan(startDate.Ticks - endDate.Ticks);
                switch (howtocompare)
                {
                    case "m":
                        diff = Convert.ToDouble(TS.TotalMinutes);
                        break;
                    case "s":
                        diff = Convert.ToDouble(TS.TotalSeconds);
                        break;
                    case "t":
                        diff = Convert.ToDouble(TS.Ticks);
                        break;
                    case "mm":
                        diff = Convert.ToDouble(TS.TotalMilliseconds);
                        break;
                    case "yyyy":
                        diff = Convert.ToDouble(TS.TotalDays / 365);
                        break;
                    case "q":
                        diff = Convert.ToDouble((TS.TotalDays / 365) / 4);
                        break;
                    case "M":
                        diff = Convert.ToDouble((TS.TotalDays / 30));
                        break;
                    default:
                        //d 
                        diff = Convert.ToDouble(TS.TotalDays);
                        break;
                }
            }
            catch
            {
                diff = -1;
            }
            return diff;
        }


        [DispId(1), ComVisible(true)]
        public static string Encriptar(string Valor)
        {
            try
            {

                //Vetor de inicialização de criptografia
                Byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                //'Clave de criptografia
                Byte[] key = Encoding.UTF8.GetBytes("12345678");

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                Byte[] inputByteArray = Encoding.UTF8.GetBytes(Valor);
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message + " " + ex.Source));
            }
        }

        [DispId(2), ComVisible(true)]
        public static string Desencriptar(string Valor)
        {

            try
            {
                //Vetor de inicialização de criptografia
                Byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                //'Clave de criptografia
                Byte[] key = Encoding.UTF8.GetBytes("12345678");

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Byte[] inputByteArray = new byte[Valor.Length];
                //Valor=Valor;
                inputByteArray = Convert.FromBase64String(Valor);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message + " " + ex.Source));
            }
        }

        public static string getValorParametro(string parValue)
        {

            string response = parValue.ToLower();
            DateTime d = System.DateTime.Today;

            string[] parData = parValue.Split('|');
            string formatData = "yyyyMMdd";
            if (parData.Length > 1)
            {
                formatData = parData[1];
            }

            #region Convercion factores de fechas
            if (response.IndexOf("@fechasistema") >= 0)
            {
                #region fecha sistema
                int dias = 0;
                int.TryParse(response.Replace("@fechasistema", ""), out dias);
                response = DateTime.Today.AddDays(dias).ToString(formatData);
                #endregion
            }
            else if (response.IndexOf("@diacorte") >= 0)
            {
                #region dia corte

                int diaCorte = 0;
                int.TryParse(response.Replace("=", "").Replace("@diacorte", "").Trim(), out diaCorte);
                DateTime fechaHoy = DateTime.Today;
                if (diaCorte > fechaHoy.Day)
                {
                    fechaHoy = fechaHoy.AddMonths(-1);
                }
                fechaHoy = fechaHoy.AddDays(fechaHoy.Day * -1).AddDays(diaCorte);
                response = fechaHoy.ToString(formatData);
                #endregion
            }
            else if (response.IndexOf("@diainiciosemana") >= 0)
            {

                d = d.AddDays(-7);

                int diaSemana = (int)d.DayOfWeek;
                d = d.AddDays(-diaSemana);
                d = d.AddDays(1);

                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diafinsemana") >= 0)
            {

                d = d.AddDays(-7);

                int diaSemana = (int)d.DayOfWeek;
                d = d.AddDays(-diaSemana);
                d = d.AddDays(5);

                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diainiciomes") >= 0)
            {

                d.AddMonths(-1);
                int dia = d.Day;
                d = d.AddDays(-dia);
                d = d.AddDays(1);
                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diafinmes") >= 0)
            {

                d.AddDays(-d.Day);
                d = d.AddDays(-1);

                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diainiciotrimestre") >= 0)
            {

                var mes = 7;
                var año = d.Year;
                var mesAct = d.Month;
                if (mesAct <= 3)
                {
                    mes = 10;
                    año--;
                }
                else if (mesAct <= 6)
                {
                    mes = 1;
                }
                else if (mesAct <= 9)
                {
                    mes = 4;
                }


                d = Convert.ToDateTime("01/" + mes + "/" + año);

                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diafintrimestre") >= 0)
            {


                int mes = 7;
                int año = d.Year;
                int mesAct = d.Month;
                if (mesAct <= 3)
                {
                    mes = 10;
                    año--;
                }
                else if (mesAct <= 6)
                {
                    mes = 1;
                }
                else if (mesAct <= 9)
                {
                    mes = 4;
                }


                d = Convert.ToDateTime("01/" + mes + "/" + año);

                d = d.AddDays(93);

                d = d.AddDays(-d.Day);

                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diainiciosemestre") >= 0)
            {

                int mes = 7;
                int año = d.Year;
                int mesAct = d.Month;
                if (mesAct <= 6)
                {
                    mes = 1;
                    año--;
                }


                d = Convert.ToDateTime("01/" + mes + "/" + año);

                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diafinsemestre") >= 0)
            {


                int mes = 7;
                int año = d.Year;
                int mesAct = d.Month;
                if (mesAct <= 6)
                {
                    mes = 1;
                    año--;
                }


                d = Convert.ToDateTime("01/" + mes + "/" + año);

                d = d.AddDays(186);

                d = d.AddDays(-d.Day);

                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diainicioano") >= 0)
            {
                int año = d.Year - 1;
                d = Convert.ToDateTime("01/01/" + año);

                response = d.ToString(formatData);


            }
            else if (response.IndexOf("@diafinano") >= 0)
            {


                int año = d.Year - 1;
                d = Convert.ToDateTime("31/12/" + año);

                response = d.ToString(formatData);



            }
            else
            {
                response = parData[0];
            }

            #endregion
            return response;
        }


        public static byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData;
            FileInfo fileInfo;
            long imageFileLength;
            FileStream fs;
            BinaryReader br;

            imageData = null;
            fileInfo = new FileInfo(imageLocation);
            imageFileLength = fileInfo.Length;
            fs = new FileStream(imageLocation, FileMode.Open,FileAccess.Read);
            br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            fs.Close();

            return imageData;
        }


        public static void WriteImageFile(string imageLocation, byte[] imageData)
        {
            FileStream fs;
            BinaryWriter bw;

            if (File.Exists(imageLocation))
            {

                File.Delete(imageLocation);
            }
            fs = new FileStream(imageLocation, FileMode.Create);
            bw = new BinaryWriter(fs);
            bw.Write(imageData);
            fs.Close();
            
        }


        private static XmlDocument xmlDoc = null;


        /// <summary>
        /// Convierte el menu institucional a un documento Xml
        /// </summary>
        /// <param name="arlBreadCrumbs">Arreglo de páginas que contienen la información del menú institucional</param>
        /// <returns>Documento Xml con el menú institucional</returns>
        public static XmlDocument dtToXmlDocument(DataRow drHeader, DataTable dtDetail )
        {
            xmlDoc = new XmlDocument();

            //Se crea el elemento base
            XmlElement xmlElemEntidad = xmlDoc.CreateElement("header");

            foreach (DataColumn dc in drHeader.Table.Columns)
            {
                createElement(xmlElemEntidad, dc.ColumnName, drHeader[dc.ColumnName].ToString());
            }
           

            XmlElement xmlElemRows = xmlDoc.CreateElement("rows");
            int i = 0;
            foreach (DataRow dr in dtDetail.Rows)
            {
                i++;
                XmlElement xmlElemItem = xmlDoc.CreateElement("row");
                foreach (DataColumn dc in dtDetail.Columns)
                {
                    createElement(xmlElemItem, dc.ColumnName, dr[dc.ColumnName].ToString());
                }

                xmlElemRows.AppendChild(xmlElemItem);

            }


            xmlElemEntidad.AppendChild(xmlElemRows);
            //Se asocia el elemento category al nodo informado
            xmlDoc.AppendChild(xmlElemEntidad);
            return xmlDoc;
        }


        public static void createAttribute(XmlNode xmlNode, string name, string value)
        {

            //Se crea el atributo id para ese elemento
            XmlAttribute xmlAttribId = xmlDoc.CreateAttribute(name);
            xmlAttribId.Value = value;
            xmlNode.Attributes.Append(xmlAttribId);

        }

        public static void createElement(XmlNode xmlNode, string name, string value)
        {

            //Se crea el atributo id para ese elemento
            XmlElement xmlElem = xmlDoc.CreateElement(name);
            xmlElem.AppendChild(xmlDoc.CreateCDataSection(value));
            xmlNode.AppendChild(xmlElem);

        }

 

    }
}
