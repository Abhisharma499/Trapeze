using System.ComponentModel.DataAnnotations;

namespace Trapeze.IceCreamShop.Data.Entities
{
    public class UserInformation
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }
    }
}
