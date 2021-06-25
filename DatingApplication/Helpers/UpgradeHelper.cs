using DatingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApplication.Helpers
{
    public static class UpgradeHelper
    {
        private static OperationResult ValidatePaymentInput(UpgradeViewModel paymentDetails) //validates the payment input
        {
            if(paymentDetails.PaymentMethod == "1") //if payment is by card
            {
                if(paymentDetails.CardNumber.Length < 16) //validate card number length
                {
                    return new OperationResult { Success = false, Message = "Ο αριθμός της κάρτας δεν έχει σωστή μορφή" };
                }

                foreach(var c in paymentDetails.CardNumber)  //validate card number content (only numbers allowed)
                {
                    var pars = int.TryParse(c.ToString(), out var res);
                    if (!pars)
                    {
                        return new OperationResult { Success = false, Message = "Ο αριθμός της κάρτας δεν έχει σωστή μορφή" };
                    }
                }

                if(int.Parse(paymentDetails.CardExpiryYear) < DateTime.Now.Year)  //validate card expiration year
                {
                    return new OperationResult { Success = false, Message = "Η ημερομηνία λήξης της κάρτας δεν είναι έγκυρη." };
                }
                else if(int.Parse(paymentDetails.CardExpiryYear) == DateTime.Now.Year)
                {
                    if(int.Parse(paymentDetails.CardExpiryMonth) <= DateTime.Now.Month) //validate card expiration month
                    {
                        return new OperationResult { Success = false, Message = "Η ημερομηνία λήξης της κάρτας δεν είναι έγκυρη." };
                    }
                }

                if(paymentDetails.CardCVC.Length < 3) //validate card cvc length
                {
                    return new OperationResult { Success = false, Message = "Ο κωδικός ασφαλείας της κάρτας δεν έχει σωστή μορφή" };
                }

                foreach (var c in paymentDetails.CardCVC) //validate card cvc content (only numbers allowed)
                {
                    var pars = int.TryParse(c.ToString(), out var res);
                    if (!pars)
                    {
                        return new OperationResult { Success = false, Message = "Ο κωδικός ασφαλείας της κάρτας δεν έχει σωστή μορφή" };
                    }
                }
            }

            if(paymentDetails.PaymentMethod == "2") //if payment is by paypal
            {
                return EmailHelper.IsValid(paymentDetails.PaypalEmail); //validate email
            }

            return new OperationResult();
        }

        public static OperationResult UpgradeUser(UpgradeViewModel paymentDetails) //records and saves the payment and upgrades the user to premium
        {
            try
            {
                var result = ValidatePaymentInput(paymentDetails); //validate payment input by user
                if (!result.Success)
                {
                    return result;
                }
               
                using (var db = new DatingEntities())
                {
                    var userId = CommonHelpers.GetLoggedUserInfo().Id;

                    var meansOfPaymentId = paymentDetails.PaymentMethod == "1" ? paymentDetails.CardNumber : paymentDetails.PaypalEmail;

                    //record payment
                    db.payments.Add(new payments { user_id = userId, payment_method = int.Parse(paymentDetails.PaymentMethod), means_of_payment_id = meansOfPaymentId, amount = 15, payment_date = DateTime.Now });

                    var user = db.users.Where(u => u.id == userId).FirstOrDefault();
                    user.category = 2; //update user category
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    CommonHelpers.UpdateUserInfo(userId);

                }
            }
            catch(Exception ex)
            {
                return new OperationResult { Success = false, Message = "Σφάλμα κατά την αναβάθμιση σε Premium" };
            }

            return new OperationResult();
        }
    }
}