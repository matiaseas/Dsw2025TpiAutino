using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Customer : EntityBase
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public Customer() { }

        public Customer(string name, string email)
        {
            Name = name;
            Email = email;
        }

        // Propiedades de navegación
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
