using WebGwenchana.Models;

namespace WebGwenchana.ModelViews
{
    public class CartItem
    {
        public SizePrice? sizePrice {  get; set; }
        public Product product { get; set; }
        public int amount { get; set; }
        public Size? size { get; set; }
        public double TotalMoney => (double)(amount * sizePrice.ProductPrice.Value);
    }
}
