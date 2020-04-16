namespace Webshop.Models
{
    public class OrderAndPaymentMethods
    {
        public User User { get; set; }


        // Payment methods
        public OrderViewModel OrderViewModel { get; set; }

        public CreditCardModel CreditCardModel { get; set; }

        // Add more paymentmethod-modells here here...
    }
}
