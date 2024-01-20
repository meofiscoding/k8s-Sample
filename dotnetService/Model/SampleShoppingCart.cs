using System;
namespace dotnetService.Model
{
    public class SampleShoppingCart
    {
        public string UserName { get; set; }
        public List<SampleShoppingCartItem> Items { get; set; } = new List<SampleShoppingCartItem>();


        public SampleShoppingCart()
        {
        }
        public SampleShoppingCart(string userName)
        {
            UserName = userName;
        }

        public decimal TotalPrice
        {
            get
            {
                decimal totalprice = 0;
                foreach (var item in Items)
                {
                    totalprice += item.Price * item.Quantity;
                }
                return totalprice;
            }
        }
    }
}

