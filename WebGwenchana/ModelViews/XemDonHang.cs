using WebGwenchana.Models;

namespace WebGwenchana.ModelViews
{
	public class XemDonHang
	{
		public Order DonHang { get; set; }
		public List<OrderDetail> ChiTietDonHang { get; set; }
	}
}
