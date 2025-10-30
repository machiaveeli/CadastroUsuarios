using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCadastroUsuarios.Models
{
    public class Usuario : Pessoa
    {
        public string Login {  get; set; }
        public string Senha { get; set; }

        public Usuario(string nome, DateTime dataNascimento, string cpf, string login, string senha)
            :base(nome, dataNascimento, cpf)
        { 
            Login = login;
            Senha = senha;
        }
    }
}
