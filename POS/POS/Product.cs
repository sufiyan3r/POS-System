using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS
{
    internal class Product : IComparable
    {
        private decimal Price;
        private string Product_Name;
        private int Stock;
        private int ID;

        public Product(int ID, string Product_Name, int Stock, decimal Price)
        {
            this.Price = Price;
            this.Product_Name = Product_Name;
            this.Stock = Stock;
            this.ID = ID;
        }

        public decimal price
        {
            get { return Price; }
            set { Price = value; }
        }

        public string Name
        {
            get { return Product_Name; }
            set { Product_Name = value; }
        }
        public int id
        {
            get { return ID; }
            set { ID = value; }
        }

        public int stock
        {
            get { return Stock; }
            set { Stock = value; }
        }

        public int CompareTo(Object obj)
        {
            Product other = (Product)obj;
            return ID.CompareTo(other.ID);
        }

    }
}