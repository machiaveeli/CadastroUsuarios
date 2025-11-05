using SistemaCadastroUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaCadastroUsuarios.Services
{
    public class InMemoryUsuarioService : IUsuarioService
    {

        private readonly List<Usuario> _usuarios = new List<Usuario>();

        private int _proximoId = 1;

        public bool AdicionarUsuario(string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao)
        {
            ValidarDados(nome, dataNasci, cpf, email, senha, false);
            DateTime valorDataNascimento = dataNasci.Value;

            var usuario = new Usuario(nome, valorDataNascimento, cpf, email, senha, idPermissao)
            {
                Id = _proximoId++
            };

            _usuarios.Add(usuario);
            return true;
        }

        public bool AtualizarUsuario(int id, string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao)
        {
            ValidarDados(nome, dataNasci, cpf, email, senha, true);
            DateTime valorDataNascimento = dataNasci.Value;

            var usuarioExistente = _usuarios.FirstOrDefault(u => u.Id == id);

            if (usuarioExistente == null)
            {
                throw new KeyNotFoundException("Usuário com o ID especificado não foi encontrado para atualização.");
            }

            usuarioExistente.Nome = nome;
            usuarioExistente.DataNascimento = valorDataNascimento;
            usuarioExistente.Cpf = cpf;
            usuarioExistente.Email = email;
            usuarioExistente.UserRoleId = idPermissao;

            if (!string.IsNullOrEmpty(senha))
            {
                usuarioExistente.Senha = senha;
            }

            return true;
        }

        public bool ExcluirUsuario(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID de usuário inválido.");

            var usuario = _usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario != null)
            {
                _usuarios.Remove(usuario);
                return true;
            }

            return false;
        }

        public List<Usuario> BuscarUsuarios(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return ListarTodosUsuarios();
            }

            string termoBusca = termo.ToLower().Trim();

            return _usuarios.Where(u =>
                u.Nome.ToLower().Contains(termoBusca) ||
                u.Email.ToLower().Contains(termoBusca) ||
                u.Cpf.Contains(termo) 
            ).ToList();
        }

        public List<Usuario> ListarTodosUsuarios()
        {
            return _usuarios.ToList();
        }

        public bool ValidarLogin(string email, string senha)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (usuario == null)
            {
                return false;
            }

            return (usuario.Senha == senha && usuario.UserRoleId == 2);
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