using DatingApplication.Helpers;
using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Controllers
{
    public class UpgradeController : BaseController
    {
        [HttpGet]
        public ActionResult Index() //renders the upgrade page
        {
            if(CommonHelpers.GetLoggedUserInfo().Category == 2) //simple user only, if user is already premium the redirect
            {
                return Redirect("~/Home");
            }

            return View("UpgradePage", new UpgradeViewModel());
        }

        [HttpPost]
        public ActionResult Upgrade(UpgradeViewModel paymentDetails) //receive the input from upgrade page
        {
            var result = UpgradeHelper.UpgradeUser(paymentDetails); //validate input and save, upgrading user to premium
            if (result.Success)
            {
                if(paymentDetails.PaymentMethod == "1")
                {
                    return Redirect("/Upgrade/SuccessfulUpgrade");
                }
                else
                {
                    var returnUrl = ConfigurationManager.AppSettings["PaypalUrlReturn"];
                    return Redirect("https://www.sandbox.paypal.com/cgi-bin/websrc?cmd=_xclick&amount=15&business=business@newbeginnings.com&item_name=Subscription&return="+returnUrl);
                }
            }
            else
            {
                ViewBag.Error = result.Message;
            }

            return View("UpgradePage", paymentDetails);
        }

        [HttpGet]
        public ActionResult SuccessfulUpgrade()
        {
            return View();
        }


    }
}