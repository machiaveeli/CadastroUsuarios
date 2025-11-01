using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCadastroUsuarios.Models
{
    public class Usuario : Pessoa
    {
        public string Email {  get; set; }
        public string Senha { get; set; }
        public int UserRoleId { get; set; }

        public Usuario(string nome, DateTime dataNascimento, string cpf, string email, string senha, int userRoleId)
            :base(nome, dataNascimento, cpf)
        { 
            Email = email;
            Senha = senha;
            UserRoleId = userRoleId; ;
        }
    }
}
