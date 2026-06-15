using System;

namespace client_firebase
{
    public static class AuthSession
    {
        public static string FirebaseIdToken { get; set; }
        public static string FirebaseLocalId { get; set; }

        public static string GoogleIdToken { get; set; }
        public static string GoogleAccessToken { get; set; }
        public static string GoogleRefreshToken { get; set; }
        public static string FacebookAccessToken { get; set; }

        public static bool IsAuthenticated => !string.IsNullOrEmpty(FirebaseIdToken) || !string.IsNullOrEmpty(GoogleIdToken) || !string.IsNullOrEmpty(FacebookAccessToken);

        public static void Clear()
        {
            FirebaseIdToken = null;
            FirebaseLocalId = null;
            GoogleIdToken = null;
            GoogleAccessToken = null;
            GoogleRefreshToken = null;
            FacebookAccessToken = null;
        }
    }
}
