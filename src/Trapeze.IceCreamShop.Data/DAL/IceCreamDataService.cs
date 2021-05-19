namespace Trapeze.IceCreamShop.Data.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Trapeze.IceCreamShop.Data.Entities;
    using Trapeze.IceCreamShop.Enums;

    public class IceCreamDataService : IIceCreamDataService
    {
        private readonly IceCreamDbContext _context;

        public IceCreamDataService(IceCreamDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<bool> InsertIntoIceCreamInformation(IceCreamInformation iceCreamInformation)
        {
            try
            {
                await _context.IceCreams.AddAsync(iceCreamInformation).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
