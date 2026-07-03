using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace client_firebase
{
    public static class FirebaseAuthService
    {
        
        public static async Task<string> Auth(string email, string password, bool isSignUp)
        {
            
            string url = isSignUp
                ? $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={AppConfig.FirebaseApiKey}"
                : $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={AppConfig.FirebaseApiKey}";

            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject json = JObject.Parse(responseString);
                    
                    string idToken = json["idToken"]?.ToString();
                    string localId = json["localId"]?.ToString();
                    return "Success|" + idToken + "|" + localId;
                }
                else
                {
                    JObject errorJson = JObject.Parse(responseString);
                    string errorMessage = errorJson["error"]["message"].ToString();
                    return "Error|" + errorMessage;
                }
            }
        }

        public static async Task<string> SaveUserProfileAsync(string idToken, string localId, string email, string username, string birthdate)
        {
            try
            {
                
                string dbUrl = AppConfig.FirebaseDatabaseUrl;

                if (string.IsNullOrWhiteSpace(dbUrl))
                {
                    return "Error|MissingDatabaseUrl";
                }

                
                dbUrl = dbUrl.TrimEnd('/');

                string url = $"{dbUrl}/users/{localId}.json?auth={idToken}";

                var payload = new
                {
                    email = email,
                    username = username,
                    birthdate = birthdate
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PutAsync(url, content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Error|" + responseString;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error|" + ex.Message;
            }
        }

        public static async Task<string> ResetPassword(string email)
        {
            
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={AppConfig.FirebaseApiKey}";

            var payload = new
            {
                requestType = "PASSWORD_RESET",
                email = email
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return "Success";
                }
                else
                {
                    JObject errorJson = JObject.Parse(responseString);
                    string errorMessage = errorJson["error"]["message"].ToString();
                    return "Error|" + errorMessage;
                }
            }
        }

        public static async Task<string> LoginWithGoogleAsync(string googleIdToken)
        {
            
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={AppConfig.FirebaseApiKey}";

            
            var payload = new
            {
                postBody = $"id_token={googleIdToken}&providerId=google.com",
                requestUri = "http://localhost",
                returnIdpCredential = true,
                returnSecureToken = true
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    
                    JObject json = JObject.Parse(responseString);
                    string firebaseToken = json["idToken"]?.ToString();
                    string localId = json["localId"]?.ToString();
                    string userEmail = json["email"]?.ToString() ?? "Chưa cung cấp email";
                    string userName = json["displayName"]?.ToString() ?? "Người dùng Google";
                    bool isNewUser = json["isNewUser"] != null && (bool)json["isNewUser"];

                    return $"Success|{firebaseToken}|{localId}|{userEmail}|{userName}|{isNewUser}";
                }
                else
                {
                    
                    JObject errorJson = JObject.Parse(responseString);
                    string errorMessage = errorJson["error"]["message"]?.ToString() ?? responseString;
                    return $"Error|{errorMessage}";
                }
            }
        }

        public static async Task<string> GetFacebookAccessTokenAsync()
        {
            string redirectUri = "http://localhost:8080/";
            string oauthUrl = $"https://www.facebook.com/v19.0/dialog/oauth?client_id={AppConfig.FacebookAppId}&redirect_uri={redirectUri}&scope=email,public_profile";

            
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(redirectUri);
                listener.Start();

                
                Process.Start(new ProcessStartInfo { FileName = oauthUrl, UseShellExecute = true });

                
                var context = await listener.GetContextAsync();
                string code = context.Request.QueryString["code"];
                string error = context.Request.QueryString["error"];

                
                string responseString = "<html><body style='font-family:sans-serif; text-align:center; margin-top:50px;'><h1>Đăng nhập Facebook thành công!</h1><p>Bạn có thể đóng tab này và quay lại ứng dụng.</p></body></html>";
                var buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                listener.Stop();

                if (!string.IsNullOrEmpty(error)) return "Error|Đăng nhập bị hủy.";
                if (string.IsNullOrEmpty(code)) return "Error|Không lấy được mã xác thực.";

                
                string tokenUrl = $"https://graph.facebook.com/v19.0/oauth/access_token?client_id={AppConfig.FacebookAppId}&redirect_uri={redirectUri}&client_secret={AppConfig.FacebookClientSecret}&code={code}";

                using (HttpClient client = new HttpClient())
                {
                    var tokenResponse = await client.GetAsync(tokenUrl);
                    string tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                    if (tokenResponse.IsSuccessStatusCode)
                    {
                        return "Success|" + JObject.Parse(tokenJson)["access_token"].ToString();
                    }
                    return "Error|Lỗi lấy Token Facebook: " + tokenJson;
                }
            }
        }

        public static async Task<string> LoginWithFacebookAsync(string facebookAccessToken)
        {
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={AppConfig.FirebaseApiKey}";

            var payload = new
            {
                postBody = $"access_token={facebookAccessToken}&providerId=facebook.com",
                requestUri = "http://localhost",
                returnIdpCredential = true,
                returnSecureToken = true
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject json = JObject.Parse(responseString);
                    string idToken = json["idToken"]?.ToString();
                    string localId = json["localId"]?.ToString();
                    string email = json["email"]?.ToString() ?? "Chưa cung cấp email";
                    string displayName = json["displayName"]?.ToString() ?? "Người dùng Facebook";

                    
                    bool isNewUser = json["isNewUser"] != null && (bool)json["isNewUser"];

                    
                    return $"Success|{idToken}|{localId}|{email}|{displayName}|{isNewUser}";
                }
                else
                {
                    JObject errorJson = JObject.Parse(responseString);
                    string errorMessage = errorJson["error"]["message"]?.ToString() ?? responseString;
                    return $"Error|{errorMessage}";
                }
            }
        }
    }
}