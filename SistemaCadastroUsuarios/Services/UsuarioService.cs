using SistemaCadastroUsuarios.Models;
using SistemaCadastroUsuarios.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SistemaCadastroUsuarios.Controllers
{
    /// <summary>
    /// Implementação da IUsuarioService. Esta classe contém a lógica de negócio
    /// do sistema. Ela não sabe "como" os dados são salvos (SQL, Memória, etc),
    /// apenas "o que" fazer com eles (validar, hashear, etc).
    /// </summary>
    public class UsuarioService :  IUsuarioService
    {
        private readonly IUsuarioDAO _usuarioDAO;

        private readonly IPasswordHasher _passwordHasher;

        public UsuarioService(IUsuarioDAO usuarioService, IPasswordHasher passwordHasher)
        {
            _usuarioDAO = usuarioService;

            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Orquestra a adição de um novo usuário.
        /// </summary>
        public bool ValidarLogin(string email, string senha)
        {
            var usuarioDoBanco = _usuarioDAO.GetPorEmail(email);

            if (usuarioDoBanco == null)
            {
                return false; 
            }

            if (usuarioDoBanco.UserRoleId != 2)
            {
                return false;
            }

            return _passwordHasher.VerifyPassword(senha, usuarioDoBanco.Senha);
        }

        public bool AdicionarUsuario(string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao)
        {
            bool ehUpate = false;

            ValidarDados(nome, dataNasci, cpf, email, senha, ehUpate);
            
            if (_usuarioDAO.GetPorEmail(email) != null) 
            {
                throw new ArgumentException("Este e-mail já está em uso.");
            }

            DateTime valorDataNascimento = dataNasci.Value;

            string senhaHash = _passwordHasher.HashPassword(senha);

            var usuario = new Usuario(nome, valorDataNascimento, cpf, email, senhaHash, idPermissao);

            _usuarioDAO.Adicionar(usuario);

            return true;
        }

        public bool AtualizarUsuario(int id, string nome, DateTime? dataNasci, string cpf, string email, string senha, int idPermissao)
        {
            bool ehUpate = true;

            ValidarDados(nome, dataNasci, cpf, email, senha, ehUpate);

            var usuarioExistente = _usuarioDAO.GetPorEmail(email); //
            if (usuarioExistente != null && usuarioExistente.Id != id)
            {
                throw new ArgumentException("Este e-mail já está em uso por outro usuário.");
            }

            DateTime valorDataNascimento = dataNasci.Value;

            string senhaHash = senha;

            if (!string.IsNullOrEmpty(senha)) 
            {
                senhaHash = _passwordHasher.HashPassword(senha);
            }

            var usuario = new Usuario
            {
                Id = id,
                Nome = nome,
                DataNascimento = valorDataNascimento,
                Cpf = cpf,
                Email = email,
                Senha = senhaHash,
                UserRoleId = idPermissao
            };

            _usuarioDAO.Atualizar(usuario);

            return true;
        }

        // Os métodos abaixo são "pass-through", apenas repassam a chamada para o DAO.
        // Em sistemas maiores, poderia haver lógica de cache ou mapeamento aqui.

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

        /// <summary>
        /// Método central de validação de dados de entrada.
        /// </summary>
        /// <param name="ehUpdate">Flag para diferenciar regras de criação e atualização.</param>
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
            if (!IsCpfValido(cpf))
            {
                throw new ArgumentException("O CPF informado é inválido.");
            }
            
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("O campo 'Email' é obrigatório.");
            }

            // Validação de formato (Regex)
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                throw new ArgumentException("O formato do e-mail é inválido.");
            }

            // A senha só é obrigatória na criação (não no update)
            if (string.IsNullOrWhiteSpace(senha) && ehUpdate is false)
            {
                throw new ArgumentException("O campo 'Senha' é obrigatório.");
            }
            return true;
        }

        /// <summary>
        /// Algoritmo de validação de CPF (dígitos verificadores).
        /// </summary>
        private static bool IsCpfValido(string cpf)
        {
            /// <summary>
            /// Algoritmo de validação de CPF (dígitos verificadores).
            /// </summary>
            string cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpfLimpo.Length != 11)
                return false;
            // Verifica CPFs inválidos conhecidos (todos os números iguais)
            for (int i = 0; i <= 9; i++)
            {
                if (new string(i.ToString()[0], 11) == cpfLimpo)
                    return false;
            }

            // Cálculo do primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * (cpfLimpo[i] - '0');

            int resto = soma % 11;
            int digitoVerificador1 = (resto < 2) ? 0 : 11 - resto;

            if (digitoVerificador1 != (cpfLimpo[9] - '0'))
                return false;
            // Cálculo do segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * (cpfLimpo[i] - '0');

            resto = soma % 11;
            int digitoVerificador2 = (resto < 2) ? 0 : 11 - resto;

            // Retorna true se os dois dígitos calculados baterem com os do CPF
            return (digitoVerificador2 == (cpfLimpo[10] - '0'));
        }

    }
}
