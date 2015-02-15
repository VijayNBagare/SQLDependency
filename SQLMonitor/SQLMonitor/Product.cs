using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLMonitor
{

    public partial class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Skunumber { get; set; }
        public string Vintage { get; set; }
        public string Notes { get; set; }
        public string Size { get; set; }
        public int Points { get; set; }
        public string Department { get; set; }
        public string Varietal { get; set; }
        public string Color { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Review { get; set; }
        public string Reviewer { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public int CountOfInv { get; set; }
        public int Shipping_category_id { get; set; }

    }

}
