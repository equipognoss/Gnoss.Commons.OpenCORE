using Es.Riam.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Es.Riam.Web.Util
{
    public class UtilMatomo
    {
        private string mOAuth;
        private string mUrlMatomo;

        public UtilMatomo(string pOAuth, string pUrlMatomo)
        {
            mOAuth = pOAuth;
            mUrlMatomo = pUrlMatomo;
        }

        #region User

        public List<MatomoUserModel> GetUsers()
        {
            string url = $"{mUrlMatomo}?module=API&method=UsersManager.getUsers&format=json";
            Dictionary<string, string> contentRequest = new Dictionary<string, string>() { ["token_auth"] = mOAuth };

            try
            {
                List<MatomoUserModel> response = JsonConvert.DeserializeObject<List<MatomoUserModel>>(UtilWeb.HacerPeticionPost(url, contentRequest));

                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MatomoUserModel GetUser(string pUserLogin)
        {
            string pUrl = $"{mUrlMatomo}?module=API&method=UsersManager.getUser&format=json";
            Dictionary<string, string> contentRequest = new Dictionary<string, string>() { ["token_auth"] = mOAuth, ["userLogin"] = pUserLogin };
            try
            {
                MatomoUserModel response = JsonConvert.DeserializeObject<MatomoUserModel>(UtilWeb.HacerPeticionPost(pUrl, contentRequest));

                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool AddUser(string pUserLogin, string pPassword, string pEmail, string pInicitalIDSite = "1")
        {
            string url = $"{mUrlMatomo}?module=API&method=UsersManager.addUser&format=json";

            Dictionary<string, string> contentRequest = new Dictionary<string, string>() { ["token_auth"] = mOAuth, ["userLogin"] = pUserLogin, ["password"] = pPassword, ["email"] = pEmail, ["initialIdSite"] = pInicitalIDSite };

            StandarMatomoResponseModel response = JsonConvert.DeserializeObject<StandarMatomoResponseModel>(UtilWeb.HacerPeticionPost(url, contentRequest));

            return response.result.Equals("success");
        }

        public bool SetSuperUserAccess(string pUserLogin, bool pSupperUsserAccess, string pPassword)
        {
            string url = $"{mUrlMatomo}?module=API&method=UsersManager.setSuperUserAccess&format=json";

            Dictionary<string, string> contentRequest = new Dictionary<string, string>() { ["token_auth"] = mOAuth, ["userLogin"] = pUserLogin, ["passwordConfirmation"] = pPassword, ["hasSuperUserAccess"] = pSupperUsserAccess.ToString() };

            StandarMatomoResponseModel response = JsonConvert.DeserializeObject<StandarMatomoResponseModel>(UtilWeb.HacerPeticionPost(url, contentRequest));

            return response.result.Equals("success");
        }

        public bool SetUserPassword(string pUserLogin, string pNewPassword, string pOldPassword)
        {
            string url = $"{mUrlMatomo}?module=API&method=UsersManager.updateUser&format=json";

            Dictionary<string, string> contentRequest = new Dictionary<string, string>() { ["token_auth"] = mOAuth, ["userLogin"] = pUserLogin, ["password"] = pNewPassword, ["passwordConfirmation"] = pOldPassword };

            StandarMatomoResponseModel response = JsonConvert.DeserializeObject<StandarMatomoResponseModel>(UtilWeb.HacerPeticionPost(url, contentRequest));

            return response.result.Equals("success");
        }

        #endregion

        #region Classes

        public class MatomoUserModel
        {
            public string login { get; set; }
            public string email { get; set; }
            public int superuser_access { get; set; }
            public DateTime? date_registered { get; set; }
            public string? invited_by { get; set; }
            public DateTime? invite_expired_at { get; set; }
            public DateTime? invite_accept_at { get; set; }
            public string invite_status { get; set; }
            public bool uses_2fa { get; set; }
        }

        private class SetPasswordModel
        {
            public string token_auth { get; set; }
            public string userLogin { get; set; }
            public string password { get; set; }
            public string passwordConfirmation { get; set; }
        }

        private class StandarMatomoResponseModel
        {
            public string result { get; set; }
            public string message { get; set; }
        }

        #endregion

    }
}