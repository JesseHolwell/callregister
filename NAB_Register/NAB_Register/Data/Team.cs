using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAB_Register.Data
{
    class Team
    {
        public int? ID { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }

        public Team(int? id, int number, string name, string email, bool? active)
        {
            ID = id;
            Number = number;
            Name = name;
            Email = email;
            IsActive = active;

        }

        public override string ToString()
        {
            return Number.ToString() + " - " + Name;
        }
    }
}
