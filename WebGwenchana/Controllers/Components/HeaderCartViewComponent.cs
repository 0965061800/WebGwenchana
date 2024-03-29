﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGwenchana.Extension;
using WebGwenchana.ModelViews;

namespace WebGwenchana.Controllers.Components
{
	public class HeaderCartViewComponent: ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
			return View(cart);
		}
	}
}
