using Es.Riam.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Es.Riam.Web.Util
{
    public class UtilMatomo
    {
        private readonly string mOAuth;
        private readonly string mUrlMatomo;

        public UtilMatomo(string pOAuth, string pUrlMatomo)
        {
            mOAuth = pOAuth;
            mUrlMatomo = $"https://{pUrlMatomo}";
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
                return new List<MatomoUserModel>();
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

        #region Widget
        public string GetWidget(MatomoWidgetViewModel pWidgetModel)
        {
            string url = $"{mUrlMatomo}?module=Widgetize&action=iframe&containerId=VisitOverviewWithGraph&disableLink=1&widget=1&token_auth=9338f0eb2f2523212b83305a11c5ecb8&moduleToWidgetize=CoreHome&actionToWidgetize={pWidgetModel.actionToWidgetize}&idSite=1&period={pWidgetModel.period}&date={pWidgetModel.date}&segment=pageUrl=@https://testing.gnoss.com/comunidad/testing-publica";
            string content = UtilWeb.WebRequest("GET", url);

            return content;
        }

        public WebResponse MatomoRequest(string pPage, string pQueryString)
        {
            string url = $"{mUrlMatomo}/{pPage}{pQueryString}";
            return UtilWeb.HacerPeticionGetDevolviendoWebResponse(url);
        }


        #endregion

        #region Graphic

        public string GetGraphic(MatomoGraphicsViewModel pModel)
        {
            string url = $"{mUrlMatomo}?module=API&idSite=1&format=json"; 
            Dictionary<string, string> contentRequest = new Dictionary<string, string>() { ["token_auth"] = mOAuth, ["segment"] = pModel.segment, ["method"] = pModel.method,["module"] = pModel.module, ["apiAction"] = pModel.apiAction, ["period"] = pModel.period, ["date"] = pModel.date, ["width"] = pModel.width, ["height"] = pModel.height, ["graphType"] = pModel.graphType, ["idSubtable"] =pModel.idSubtable,["force_api_session"] = pModel.force_api_session};
            return UtilWeb.HacerPeticionPost(url, contentRequest);
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

        public class MatomoGraphicsViewModel
        {
            public string method { get; set; }
            public string module { get; set; }
            public string apiAction { get; set; }
            public string period { get; set; }
            public string date { get; set; }
            public string width { get; set; }
            public string height { get; set; }
            public string graphType { get; set; }
            public string segment { get; set; }
            public string force_api_session { get; set; }
            public string idSubtable { get; set; }
        }
        public class MatomoWidgetViewModel
        {
            public string period { get; set; }
            public string date { get; set; }
            public string actionToWidgetize { get; set; }
        }


        private class StandarMatomoResponseModel
        {
            public string result { get; set; }
            public string message { get; set; }
        }

        #endregion

    }
}