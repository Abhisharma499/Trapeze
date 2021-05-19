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
    using Trapeze.IceCreamShop.Data.DAL;
    using Trapeze.IceCreamShop.Data.Entities;

    /// <summary>
    /// An API controller for performing actions on the ice cream store.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class IceCreamStoreController : ControllerBase
    {

        private readonly IIceCreamBusinessService _IIceCreamBusinessService;
        /// <summary>
        /// Initializes a new instance of the <see cref="IceCreamStoreController"/> class.
        /// </summary>
        public IceCreamStoreController(IIceCreamBusinessService IIceCreamBusinessService)
        {
            _IIceCreamBusinessService = IIceCreamBusinessService;
        }

        [HttpPost]
        public async Task<JsonResult> Purchase(IceCreamInformation model)
        {
            try
            {
                PurchaseInformation purchaseModel = await _IIceCreamBusinessService.PurchaseIceCream(model).ConfigureAwait(true);

                if (purchaseModel != null && purchaseModel.IsSuccess && purchaseModel.State == Enums.PurchaseStates.PurchaseSucess)
                {
                    return new JsonResult(new { purchaseModel })
                    {
                        StatusCode = StatusCodes.Status201Created
                    };
                }
                else
                {
                    return new JsonResult(new { Error= purchaseModel.State.ToString() })
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
