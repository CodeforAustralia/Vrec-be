using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace eCertificateRest.Utils
{
    public  static class Extras
    {
        public static string token = "";
        public static string tokenPath = "";
        private static readonly long DatetimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
        private static readonly DateTime DatetimeUtcTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public static long ToJavaScriptMilliseconds(this DateTime dt)
        {
            return (long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
        }
        public static long Datetime2Timestamp(DateTime toConvert)
        {


            return ToJavaScriptMilliseconds(toConvert);
        }
        public static DateTime Timestamp2Datetime(long unixtime)
        {


            DateTime final = DatetimeUtcTime.AddMilliseconds(unixtime);

            TimeZone zone = TimeZone.CurrentTimeZone;
            // Demonstrate ToLocalTime and ToUniversalTime.
            DateTime local = zone.ToLocalTime(final);
            //DateTime universal = zone.ToUniversalTime(final);

            return local;
        }
        public  static string  Stream2String(Stream streamdata)
        {
        StreamReader reader = new StreamReader(streamdata);
        string res = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();

        return res;
        }
          public static bool WriteFile(string imageLocation, byte[] imageData)
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
                  return true;   
          }
          public static string Base64Decode(string base64EncodedData)
          {
              var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
              return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
          }
          public static string Base64Encode(string plainText)
          {
              var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
              return System.Convert.ToBase64String(plainTextBytes);
          }
          public static void CreateTokenFolder()
          {
              string pathString = "~/Tmp/Downloads/";
              string resultFileUrl = pathString+token ;
              string resultFilePath = HostingEnvironment.MapPath(resultFileUrl);
              try
              {
                  if (Directory.Exists(resultFilePath))
                      Directory.Delete(resultFilePath);
                  else
                      Directory.CreateDirectory(resultFilePath);
              }
              catch { }
          }
          public static void DeleteTokenFolder()
          {

              //ELIMINAMOS LA CARPETA DEL TOKEN
              if (Directory.Exists(HostingEnvironment.MapPath("~/Tmp/Downloads/" + tokenPath)) && tokenPath != "")
              {
                  Directory.Delete(HostingEnvironment.MapPath("~/Tmp/Downloads/" + tokenPath));
              }
           }        
          public static string ReplaceParametersResetPasswordEmail(string correo, string nombre, string token, string url)
          {
                correo=  correo.Replace("~:~NOMBREUSUARIO~:~", nombre);
                correo = correo.Replace("~:~TOKEN~:~", token);
                correo = correo.Replace("~:~URL~:~", url);
              return correo;
          }
    }
}