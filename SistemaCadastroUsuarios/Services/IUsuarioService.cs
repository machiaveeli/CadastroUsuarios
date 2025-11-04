using SistemaCadastroUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCadastroUsuarios.Services
{
    public interface IUsuarioService
    {
        bool AdicionarUsuario(string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao);
        bool AtualizarUsuario(int id, string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao);
        bool ExcluirUsuario(int id);
        List<Usuario> ListarTodosUsuarios();
        List<Usuario> BuscarUsuarios(string termo);
        bool ValidarDados(string nome, DateTime? dataNasci, string cpf, string email, string senha, bool ehUpdate);
    }
}
