using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatingApplication.Models
{
    public class UpgradeViewModel
    {
        public List<SelectListItem> PaymentMethods { get; set; }
        public string PaymentMethod { get; set; }
        public string CardNumber { get; set; }
        public string PaypalEmail { get; set; }
        public List<SelectListItem> CardExpiryMonths { get; set; }
        public string CardExpiryMonth { get; set; }
        public List<SelectListItem> CardExpiryYears { get; set; }
        public string CardExpiryYear { get; set; }
        public string CardCVC { get; set; }

        public UpgradeViewModel()
        {
            PaymentMethods = new List<SelectListItem>();
            using (var db = new DatingEntities())
            {
                db.payment_methods.Where(p => p.active == 1).ToList().ForEach(p => 
                {
                    PaymentMethods.Add(new SelectListItem { Value = p.id.ToString(), Text = p.description });
                });
            }

            CardExpiryMonths = new List<SelectListItem>();
            for (var i = 1; i<=12; i++)
            {
                string month = i.ToString();
                if(month.Length == 1)
                {
                    month = "0" + month;
                }

                CardExpiryMonths.Add(new SelectListItem { Value = i.ToString(), Text = month });
            }

            CardExpiryYears = new List<SelectListItem>();
            for (var i = 1; i <= 9; i++)
            {
                string year = "202" + i.ToString();
                CardExpiryYears.Add(new SelectListItem { Value = year, Text = year });
            }
        }
    }

    
}