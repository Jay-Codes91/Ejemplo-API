using System;
using System.Collections.Generic;

namespace Empleados.Models
{
    public partial class Puesto
    {
        public int Id { get; set; }
        public string Cargo { get; set; } = null!;
        public double Salario { get; set; }

        public virtual Personal IdNavigation { get; set; } = null!;
    }
}
