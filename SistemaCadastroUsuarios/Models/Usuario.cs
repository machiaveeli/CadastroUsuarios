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

        /// <summary>
        /// Propriedade "Helper" que traduz o UserRoleId para o DataGrid.
        /// O 'Binding="{Binding IsAdmin}"' vai usar isto.
        /// </summary>
        public bool IsAdmin
        {
            get { return UserRoleId == 2; }
        }

        public Usuario() { }

        public Usuario(string nome, DateTime dataNascimento, string cpf, string email, string senha, int userRoleId)
            :base(nome, dataNascimento, cpf)
        { 
            Email = email;
            Senha = senha;
            UserRoleId = userRoleId; ;
        }
    }
}
