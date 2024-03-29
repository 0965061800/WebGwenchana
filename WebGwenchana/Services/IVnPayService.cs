﻿using WebGwenchana.ModelViews;

namespace WebGwenchana.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);

    }
}
