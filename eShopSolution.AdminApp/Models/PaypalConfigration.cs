using PayPal.Api;

namespace eShopSolution.AdminApp.Models
{
    public static class PaypalConfiguration
    {
        //Variables for storing the clientID and clientSecret key  
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        //Constructor  
        static PaypalConfiguration()
        {
            var config = GetConfig();
            ClientId = "AcPKUuHJ0sWCWSA6Ppol0o0OFeiB_kYNc0XUccU2bjzbaOCEEYaSGil7--2p9BG_kh8WZNrrZ_FVdv41";
            ClientSecret = "EH8mX_BTYUB5qxF2GNDflo43nUV3lrudnwUfsRip56QL5c8vqWiAQnDrPFdS7hq5t2_MVpZjc4m6a5wL";
        }
        // getting properties from the web.config  
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        private static string GetAccessToken()
        {
            // getting accesstocken from paypal  
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }
        public static APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken  
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}

