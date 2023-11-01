
namespace Nexify.Service.Services
{
    public class DiscountService
    {
        public string DiscountCounter(string discount, string price)
        {
            var discountPrice = decimal.Parse(price) * (decimal.Parse(discount) / 100);
            var priceWithDiscount = decimal.Parse(price) - discountPrice;

            return priceWithDiscount.ToString();
        }
    }
}
