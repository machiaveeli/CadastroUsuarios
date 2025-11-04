using SistemaCadastroUsuarios.Models;
using SistemaCadastroUsuarios.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SistemaCadastroUsuarios.Controllers
{
    public class UsuarioController
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public bool CriarUsuario(string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao)
        {
            return _usuarioService.AdicionarUsuario(nome, dataNasci, cpf, email, senha, idPermissao);
        }

        public bool AtualizarUsuario(int id, string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao)
        {
            return _usuarioService.AtualizarUsuario(id, nome, dataNasci, cpf, email, senha, idPermissao);
        }

        public List<Usuario> ListarTodosUsuarios()
        {
            return _usuarioService.ListarTodosUsuarios();
        }
    }
}
