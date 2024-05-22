using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Models
{
    public class Cart
    {
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public class ShoppingCartItem
        {
            public int ProductId { set; get; }
            public string ProductName { set; get; }
            public string ProductImage { set; get; }
            public int Quantity { set; get; }
            public decimal Price { set; get; }
            public decimal PriceSale { set; get; }
            public decimal TotalAmount { set; get; }
        }
    }

}
