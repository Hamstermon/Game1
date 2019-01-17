using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Item
    {
        int itemID;
        public int ItemID
        {
            set { itemID = value; }
            get { return itemID; }
        }
        string name;
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        int quantity;
        public int Quantity
        {
            set { quantity = value; }
            get { return quantity; }
        }
    }
    public class Event
    {
        string name;
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        int v;
        public int Value
        {
            set { v = value; }
            get { return v; }
        }
    }
}
