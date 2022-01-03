using Jose;
using System;
using System.Text;

namespace App.Server
{
    public static class Jwt
    {
        public static string Encode(string priv_token, object data, int expired = 0)
        {
            string pub_token = null;

            try
            {
                var priv_token_bytes = Encoding.UTF8.GetBytes(priv_token);
                pub_token = JWT.Encode(data, priv_token_bytes, JwsAlgorithm.HS256);
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return pub_token;
        }

        public static string Decode(string pub_token, string priv_token)
        {
            string data = null;

            try
            {
                var priv_token_bytes = Encoding.UTF8.GetBytes(priv_token);
                data = JWT.Decode(pub_token, priv_token_bytes);
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return data;
        }
    }
}
