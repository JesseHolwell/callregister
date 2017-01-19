using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAB_Register.Data
{
    class Request
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string Product{ get; set; }
        public bool? IsActive { get; set; }

        public Request(int? id, string name, string product, bool? active)
        {
            ID = id;
            Name = name;
            Product = product;
            IsActive = active;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
