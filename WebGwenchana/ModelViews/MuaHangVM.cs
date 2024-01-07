using System.ComponentModel.DataAnnotations;

namespace WebGwenchana.ModelViews
{
	public class MuaHangVM
	{
		public int CustomerID { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
		public string FullName { get; set; }
		public string Email { get; set; }
		[Required(ErrorMessage = "Nhập số ĐT đi nè!")]
		public string Phone { get; set; }
		[Required(ErrorMessage = "Không nhập địa chỉ giao hàng thì sao giao!")]
		public string Address { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
		public int TinhThanh { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
		public int QuanHuyen { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
		public int PhuongXa { get; set; }
		public int PaymentID { get; set; }
	}
}
