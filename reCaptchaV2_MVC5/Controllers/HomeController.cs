using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace reCaptchaV2_MVC5.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TwoViews()
        {
            return View();
        }


        public ActionResult AjaxRecaptcha()
        {
            return View();
        }
        public PartialViewResult AjaxRCPartial()
        {
            return PartialView();
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateReCaptcha(string captchaRouteValue)
        {
            string EncodedResponse = Request.Form["g-Recaptcha-Response"];
            bool IsCaptchaValid = (ReCaptchaClass.Validate(EncodedResponse).ToLower() == "true" ? true : false);
            if (IsCaptchaValid)
            {
                var msg = "Thank You!";
                if (captchaRouteValue == "rc1") { TempData["rc1"] = msg; }
                else if (captchaRouteValue == "rc2") { TempData["rc2"] = msg; }
                else { ModelState.AddModelError("reCaptcha", msg); }

                if (Request.IsAjaxRequest()) { return PartialView("AjaxRCPArtial"); }
                else { return View("TwoViews"); }
            }
            else
            {
                var msg = "Please verify that you are a human!";
                if (captchaRouteValue == "rc1") { TempData["rc1"] = msg; }
                else if (captchaRouteValue == "rc2") { TempData["rc2"] = msg; }
                else { ModelState.AddModelError("reCaptcha", msg); }

                if (Request.IsAjaxRequest()) { return PartialView("AjaxRCPArtial"); }
                else { return View("TwoViews"); }
            }
        }
    }


    public class ReCaptchaClass
    {
        private string m_Success;

        [JsonProperty("success")]
        public string Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }


        private List<string> m_ErrorCodes;

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get { return m_ErrorCodes; }
            set { m_ErrorCodes = value; }
        }

        public static string Validate(string EncodedResponse)
        {
            var client = new System.Net.WebClient();

            //string PrivateKey = ConfigurationManager.AppSettings["RcPrivate"]; //You can put it in your web.config file.
            string PrivateKey = "6LdfGyATAAAAAJ_abXDm3amRt1kqEeLXdhgydRYv";

            var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));

            var captchaResponse = JsonConvert.DeserializeObject<ReCaptchaClass>(GoogleReply);

            return captchaResponse.Success;
        }


    }
}