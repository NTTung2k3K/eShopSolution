using PayPal.Api;

namespace eShopSolution.AdminApp.Models
{
    public class Transaction
    {
        public string invoice_number { get; set; }
        public string description { get; set; }
        public Amount amount { get; set; }
        public ItemList item_list { get; set; }
    }
}
