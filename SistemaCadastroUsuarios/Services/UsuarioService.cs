using SistemaCadastroUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SistemaCadastroUsuarios.Services;

namespace SistemaCadastroUsuarios.Controllers
{
    public class UsuarioService :  IUsuarioService
    {
        private readonly IUsuarioDAO _usuarioDAO;

        public UsuarioService(IUsuarioDAO usuarioService)
        {
            _usuarioDAO = usuarioService;
        }

        public bool AdicionarUsuario(string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao)
        {
            bool ehUpate = false;

            ValidarDados(nome, dataNasci, cpf, email, senha, ehUpate);

            DateTime valorDataNascimento = dataNasci.Value;

            var usuario = new Usuario(nome, valorDataNascimento, cpf, email, senha, idPermissao);

            _usuarioDAO.Adicionar(usuario);

            return true;
        }

        public bool AtualizarUsuario(int id, string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao)
        {
            bool ehUpate = true;

            ValidarDados(nome, dataNasci, cpf, email, senha, ehUpate);

            DateTime valorDataNascimento = dataNasci.Value;

            var usuario = new Usuario
            {
                Id = id,
                Nome = nome,
                DataNascimento = valorDataNascimento,
                Cpf = cpf,
                Email = email,
                Senha = senha,
                UserRoleId = idPermissao
            };

            _usuarioDAO.Atualizar(usuario);

            return true;
        }

        public bool ExcluirUsuario(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID de usuário inválido.");

            _usuarioDAO.Excluir(id); 

            return true;
        }

        public List<Usuario> BuscarUsuarios(string termo)
        {
            return _usuarioDAO.Buscar(termo);
        }

        public List<Usuario> ListarTodosUsuarios()
        {
            return _usuarioDAO.ListarTodos();
        }

        public bool ValidarDados(string nome, DateTime? dataNasci, string cpf, string email, string senha, bool ehUpdate)
        {
            if (string.IsNullOrEmpty(nome))
            {
                throw new ArgumentException("O campo 'Nome' é obrigatório.");
            }
            if (!dataNasci.HasValue)
            {
                throw new ArgumentException("O campo 'Data de Nasc' é obrigatório.");
            }
            if (string.IsNullOrWhiteSpace(cpf))
            {
                throw new ArgumentException("O campo 'CPF' é obrigatório.");
            }
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("O campo 'Email' é obrigatório.");
            }
            if (string.IsNullOrWhiteSpace(senha) && ehUpdate is false)
            {
                throw new ArgumentException("O campo 'Senha' é obrigatório.");
            }
            return true;
        }
    }
}
