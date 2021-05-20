// <copyright file="IceCreamStoreController.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Ice Cream. All rights reserved.
// </copyright>
namespace Trapeze.IceCreamShop.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Trapeze.IceCreamShop.Data.BAL;
    using Trapeze.IceCreamShop.Data.Entities;

    /// <summary>
    /// An API controller for performing actions on the ice cream store.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class IceCreamStoreController : ControllerBase
    {
        private readonly IIceCreamBusinessService _iIceCreamBusinessService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IceCreamStoreController"/> class.
        /// </summary>
        public IceCreamStoreController(IIceCreamBusinessService iIceCreamBusinessService)
        {
            _iIceCreamBusinessService = iIceCreamBusinessService;
        }

        [HttpPost]
        public async Task<JsonResult> Purchase(IceCreamInformation model)
        {
            try
            {
                PurchaseInformation purchaseModel = await _iIceCreamBusinessService.PurchaseIceCream(model).ConfigureAwait(true);

                if (purchaseModel.State == Enums.PurchaseStates.Success)
                {
                    return new JsonResult(new { Refund = purchaseModel.Refund, nameOfBuyer= Util.Utils.GetUserName(HttpContext) })
                    {
                        StatusCode = StatusCodes.Status201Created
                    };
                }
                else
                {
                    return new JsonResult(new { ErrorMessage = _iIceCreamBusinessService.GetErrorMessageByPurchaseState(purchaseModel) })
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
