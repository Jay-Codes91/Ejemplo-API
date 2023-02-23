using System;
using System.Collections.Generic;

namespace Empleados.Models
{
    public partial class Personal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido1 { get; set; } = null!;
        public string Apellido2 { get; set; } = null!;
        public string? Correo { get; set; }
        public string Pass { get; set; } = null!;
        public string? Token { get; set; }

        public virtual Puesto? Puesto { get; set; }
    }
}
