using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCadastroUsuarios.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public bool Administrador { get; set; } = false;

        public UserRole(bool administrador)
        {
            Administrador = administrador;
        }
    }
}
