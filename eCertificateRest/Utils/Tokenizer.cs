using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using eCertificateRest.Utils;

namespace eCertificateRest
{
   public static class Tokenizer
    {
        private const string _alg = "HmacSHA256";
        private const string _salt = "rz8LuOtFBXphj9WQfvFh";
        public const int _expirationMinutes = 1000;

        public static string GenerateToken(string username, string password, string empresaId, string funcionarioId,string usuarioId, long ticks)
        {
            string hash = string.Join("<", new string[] { username, empresaId, funcionarioId, usuarioId, ticks.ToString() });
            string hashLeft = "";
            string hashRight = "";

            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword(password));
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));

                hashLeft = Convert.ToBase64String(hmac.Hash);
                hashRight = string.Join("<", new string[] { username, password, empresaId, usuarioId, funcionarioId });
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("<", hashLeft, hashRight)));
        }

        public static string GenerateResetToken(string username, string email,  string usuarioId, long ticks)
        {
            string hash = string.Join(":", new string[] { username, usuarioId });
            string hashLeft = "";
            string hashRight = "";

            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword(email));
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));

                hashLeft = Convert.ToBase64String(hmac.Hash);
                hashRight = string.Join(":", new string[] { username, email, usuarioId, ticks.ToString() });
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
        }



        public static string GenerateRefreshToken(string username, string password, string ip, string empresaId, string funcionarioId, string usuarioId, long ticks)
        {
            string hash = string.Join("<", new string[] { username, ip, empresaId, funcionarioId, usuarioId, ticks.ToString() });
            string hashLeft = "";
            string hashRight = "";

            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword(password));
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));

                hashLeft = Convert.ToBase64String(hmac.Hash);
                hashRight = string.Join("<", new string[] { username, ip, password, empresaId, funcionarioId, usuarioId, ticks.ToString() });
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("<", hashLeft, hashRight)));
        }
        public static string GetHashedPassword(string password)
        {
            string key = string.Join("<", new string[] { password, _salt });

            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(_salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));

                return Convert.ToBase64String(hmac.Hash);
            }
        }
       /// <summary>
       /// METODO QUE VALIDA SI EL TOKEN ES VALIDO , USANDO EL  TIMEOUT
       /// </summary>
       /// <param name="token"></param>
       /// <param name="ip"></param>
       /// <param name="empresaId"></param>
       /// <param name="funcionarioId"></param>
       /// <returns></returns>
        public static bool IsValidToken(string token)
        {
            bool result = false;

            try
            {             
                string newToken = Regex.Replace(token, @"\""", "").Trim();               
                
                // Base64 decode the string, obtaining the token:username:timeStamp.
                string key = Encoding.UTF8.GetString(Convert.FromBase64String( newToken ));

                // Split the parts.  hashRight = string.Join("<", new string[] { username, password, empresaId, usuarioId, funcionarioId });
                string[] parts = key.Split(new string[] { "<" }, StringSplitOptions.None);
                if (parts.Length == 6)
                {
                    // Get the hash message, username, and timestamp.
                    string hash = parts[0];
                    string username = parts[1];                    
                    string password = parts[2];                  
                    string empresaId = parts[3];
                    string usuarioId  = parts[4];
                    string funcionarioId = parts[5]; 
                    // Compare the computed token with the one supplied and ensure they match.
                    //result = CAdministracion.Instance.isValidToken(newToken, Convert.ToInt32(usuarioId));                       
                   
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// METODO QUE VALIDA SI EL TOKEN PARA EL CAMBIO DE CONTRASEÑA ES VALIDO , USANDO EL  TIMEOUT
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ip"></param>
        /// <param name="empresaId"></param>
        /// <param name="funcionarioId"></param>
        /// <returns></returns>
        public static bool IsValidResetToken(string token)
        {
            bool result = false;

            try
            {
                string newToken = Regex.Replace(token, @"\""", "").Trim();

                // Base64 decode the string, obtaining the token:username:timeStamp.
                string key = Encoding.UTF8.GetString(Convert.FromBase64String(newToken));

                // Split the parts.  hashRight = string.Join("<", new string[] { username, password, empresaId, usuarioId, funcionarioId });
                string[] parts = key.Split(new string[] { ":" }, StringSplitOptions.None);
                if (parts.Length == 5)
                {
                    // Get the hash message, username, and timestamp.
                    string hash = parts[0];
                    string username = parts[1];
                    string email = parts[2];
                    string usuario = parts[3];
                    string ticks = parts[4];

                    DateTime dateTicks=Extras.Timestamp2Datetime(Convert.ToInt64(ticks));

                    if ( DateTime.Now<=dateTicks)
                    {
                        result = true;
                    }
                    


                    // Compare the computed token with the one supplied and ensure they match.
                    //result = CAdministracion.Instance.isValidToken(newToken, Convert.ToInt32(usuarioId));

                }
            }
            catch
            {
            }

            return result;
        }




        /// <summary>
        /// METODO QUE VALIDA SI EL TOKEN ES VALIDO , USANDO EL  TIMEOUT
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ip"></param>
        /// <param name="empresaId"></param>
        /// <param name="funcionarioId"></param>
        /// <returns></returns>
        public static bool IsValidRefreshToken(string refreshToken)
        {
            bool result = false;

            try
            {
                string newToken = Regex.Replace(refreshToken, @"\""", "").Trim();


                // Base64 decode the string, obtaining the token:username:timeStamp.
                string key = Encoding.UTF8.GetString(Convert.FromBase64String(newToken));

                // Split the parts.
                string[] parts = key.Split(new string[] { "<" }, StringSplitOptions.None);
                if (parts.Length == 8)
                {
                    // Get the hash message, username, and timestamp.
                    string hash = parts[0];
                    string username = parts[1];
                    string ip = parts[2];
                    string password = parts[3];
                    string empresaId = parts[4];
                    string funcionarioId = parts[5];
                    string usuarioId = parts[6];
                    long ticks = long.Parse(parts[7]);
                    DateTime timeStamp = eCertificateRest.Utils.Extras.Timestamp2Datetime(ticks);

                    // Ensure the timestamp is valid.
                    bool expired = Math.Abs((DateTime.Now - timeStamp).TotalMinutes) > _expirationMinutes;
                    /*
                    if (!expired && CAdministracion.Instance.isValidRefreshToken(newToken, Convert.ToInt32(usuarioId)))
                    {
                      

                        // Hash the message with the key to generate a token.
                        string computedToken = GenerateRefreshToken(username, password,ip, empresaId, funcionarioId, usuarioId, ticks);

                        // Compare the computed token with the one supplied and ensure they match.
                        result = (newToken == computedToken);

                    }
                     * */
                }
            }
            catch
            {
            }

            return result;
        }




       /// <summary>
       ///  METODO QUE RETORNA UNA POSICION DEL TOKEN
       /// </summary>
       /// <param name="token"></param>
       /// <param name="index"></param>
       /// <returns></returns>

        public static string GetTokenPart(string token,int index)
        {



            string newToken = Regex.Replace(token, @"\""", "").Trim();
            string key = Encoding.UTF8.GetString(Convert.FromBase64String(newToken));

            // Split the parts.
            string[] parts = key.Split(new string[] { "<" }, StringSplitOptions.None);


            try { return parts[index]; }
            catch { return ""; }
            
        }


        /// <summary>
        ///  METODO QUE RETORNA UNA POSICION DEL TOKEN
        /// </summary>
        /// <param name="token"></param>
        /// <param name="index"></param>
        /// <returns></returns>

        public static string GetResetTokenPart(string token, int index)
        {



            string newToken = Regex.Replace(token, @"\""", "").Trim();
            string key = Encoding.UTF8.GetString(Convert.FromBase64String(newToken));

            // Split the parts.
            string[] parts = key.Split(new string[] { ":" }, StringSplitOptions.None);


            try { return parts[index]; }
            catch { return ""; }

        }


        /// <summary>
        ///  METODO QUE RETORNA EL USUARIO
        /// </summary>
        /// <param name="token"></param>
        /// <param name="index"></param>
        /// <returns></returns>

        public static string GetUser(string token)
        {
            return GetTokenPart(token, 1);   
        }



        /// <summary>
        /// METODO QUE VALIDA SI EL TOKEN ES VALIDO , USANDO EL  TIMEOUT
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ip"></param>
        /// <param name="empresaId"></param>
        /// <param name="funcionarioId"></param>
        /// <returns></returns>
        public static string RefreshToken(string token)
        {

            string newToken = "";

            //if (Token.IsTokenValid(token))
            //{
            //     token = Regex.Replace(token, @"\""", "").Trim();
            //    // Base64 decode the string, obtaining the token:username:timeStamp.
            //     string key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            //     string[] parts = key.Split(new char[] { ':' });
            //     if (parts.Length == 7)
            //     {

            //         string hash = parts[0];
            //         string username = parts[1];
            //         string ip = parts[2];
            //         string password = parts[3];
            //         string empresaId = parts[4];
            //         string funcionarioId = parts[5];
            //         long ticks = Utils.Extras.Datetime2Timestamp(DateTime.Now);
            //         newToken = GenerateToken(username, password, ip, empresaId, funcionarioId, ticks);
            //     }
            //}

            return newToken;

        }



        internal static string GetUserId(string token)
        {
            return GetTokenPart(token, 4);  
        }
    }
}
