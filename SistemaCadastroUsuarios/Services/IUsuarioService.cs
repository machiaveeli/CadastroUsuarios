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
        void Adicionar(Usuario usuario);
        List<Usuario> ListarTodos();
    }
}
