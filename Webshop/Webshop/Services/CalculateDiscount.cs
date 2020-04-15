namespace Webshop.Services
{
    public class CalculateDiscount
    {
        public static decimal NewPrice(decimal price, decimal discount)
        {
            return price - (price * discount);
        }
    }
}
