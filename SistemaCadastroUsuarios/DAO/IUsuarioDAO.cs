using SistemaCadastroUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCadastroUsuarios.Services
{
    public interface IUsuarioDAO
    {
        void Adicionar(Usuario usuario);
        void Atualizar(Usuario aluno);
        void Excluir(int id);
        Usuario GetPorEmail(string email);
        List<Usuario> ListarTodos();
        List<Usuario> Buscar(string termo);
    }
}
