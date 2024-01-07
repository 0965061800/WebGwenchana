using WebGwenchana.Models;

namespace WebGwenchana.ModelViews
{
	public class ProductHomeVM
	{
		public Category category { get; set; }
		public List<Product> lsProducts { get; set; }
		public List<Product> bestSeller { get; set; }
	}
}
