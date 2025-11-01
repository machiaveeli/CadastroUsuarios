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
        public string Nome { get; set; }

        public UserRole() { }

        public UserRole(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }
    }
}
