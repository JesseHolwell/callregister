using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAB_Register.Data
{
    class Product
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public Product(int? id, string name, bool? active)
        {
            ID = id;
            Name = name;
            IsActive = active;

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
