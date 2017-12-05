using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcMusicStore.Models
{
    public class ShoppingCartViewModel
    {
        /**
         * This method will give us a list of all the items in the cart
         */
        public List<Cart> CartItems { get; set; }

        public decimal CartTotal { get; set; }
    }
}