using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SistemaCadastroUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SistemaCadastroUsuarios.Services
{
    /// <summary>
    /// Implementação da interface IUsuarioDAO específica para o banco de dados MySql.
    /// Esta classe é a única responsável por construir e executar queries SQL.
    /// </summary>
    public class MySqlUsuarioDAO : IUsuarioDAO
    {
        private readonly string _connectionString;

        public MySqlUsuarioDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Adiciona um novo usuário ao banco de dados, distribuindo os dados
        /// entre as tabelas 'Pessoa' e 'Usuario'.
        /// </summary>
        /// <param name="usuario">O objeto Usuario. A senha já deve estar hasheada.</param>
        public void Adicionar(Usuario usuario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Inicia uma transação para garantir a atomicidade.
                // Ou as duas inserções (Pessoa e Usuario) funcionam, ou nenhuma funciona.
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string queryPessoa = @"INSERT INTO pessoa (Nome, DataNascimento, Cpf) 
                                               VALUES (@nome, @dataNasc, @cpf);";
                        
                        MySqlCommand cmdPessoa = new MySqlCommand(queryPessoa, connection, transaction);
                        cmdPessoa.Parameters.AddWithValue("@nome", usuario.Nome);
                        cmdPessoa.Parameters.AddWithValue("@dataNasc", usuario.DataNascimento);
                        cmdPessoa.Parameters.AddWithValue("@cpf", usuario.Cpf);

                        cmdPessoa.ExecuteNonQuery();

                        long pessoaId = cmdPessoa.LastInsertedId;

                        string queryUsuario = @"INSERT INTO usuario (PessoaId, Email, Senha, RoleId) 
                                                VALUES (@pessoaId, @email, @senha, @roleId);";

                        MySqlCommand cmdUsuario = new MySqlCommand(queryUsuario, connection, transaction);
                        cmdUsuario.Parameters.AddWithValue("@pessoaId", pessoaId);
                        cmdUsuario.Parameters.AddWithValue("@email", usuario.Email);
                        cmdUsuario.Parameters.AddWithValue("@senha", usuario.Senha);
                        cmdUsuario.Parameters.AddWithValue("@roleId", usuario.UserRoleId);

                        cmdUsuario.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex) 
                    { 
                        transaction.Rollback();
                        throw new Exception("Erro de banco de dados ao adicionar: " + ex.Message, ex);
                    }
                }
            }
        }

        public List<Usuario> ListarTodos() 
        { 
            var _usuarios = new List<Usuario>();

            using (var connection = new MySqlConnection(_connectionString)) 
            {
                connection.Open();

                string query = @"SELECT 
                                    p.Id, 
                                    u.Email, 
                                    p.Nome, 
                                    p.Cpf, 
                                    p.DataNascimento, 
                                    u.RoleId 
                                 FROM Usuario u
                                 JOIN Pessoa p ON p.Id = u.PessoaId";

                using (var command = new MySqlCommand(query, connection)) 
                {
                    using (var reader = command.ExecuteReader()) 
                    {
                        while (reader.Read()) 
                        {
                            var usuario = new Usuario
                            {
                                Id = reader.GetInt32("Id"),
                                Nome = reader.GetString("Nome"),
                                DataNascimento = reader.GetDateTime("DataNascimento"),
                                Cpf = reader.GetString("Cpf"),
                                Email = reader.GetString("Email"),
                                UserRoleId = reader.GetInt32("RoleId")
                            };
                                    
                            _usuarios.Add(usuario);
                        }
                    }
                }
            }

            return _usuarios;
        }

        public void Atualizar(Usuario usuario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string queryPessoa = @"UPDATE Pessoa 
                                               SET Nome = @nome,
                                                   DataNascimento = @dataNasc,
                                                   Cpf = @cpf
                                               WHERE 
                                                   Id = @id";

                        MySqlCommand cmdPessoa = new MySqlCommand(queryPessoa, connection, transaction);
                        cmdPessoa.Parameters.AddWithValue("@id", usuario.Id);
                        cmdPessoa.Parameters.AddWithValue("@nome", usuario.Nome);
                        cmdPessoa.Parameters.AddWithValue("@dataNasc", usuario.DataNascimento);
                        cmdPessoa.Parameters.AddWithValue("@cpf", usuario.Cpf);

                        cmdPessoa.ExecuteNonQuery();

                        var queryUsuario = new StringBuilder();
                        queryUsuario.Append(@"UPDATE Usuario 
                                              SET Email = @email,
                                                  RoleId = @roleId ");

                        if (!string.IsNullOrEmpty(usuario.Senha))
                        {
                            queryUsuario.Append(", Senha = @senha ");
                        }

                        queryUsuario.Append("WHERE PessoaId = @pessoaId");

                        MySqlCommand cmdUsuario = new MySqlCommand(queryUsuario.ToString(), connection, transaction);
                        cmdUsuario.Parameters.AddWithValue("@pessoaId", usuario.Id);
                        cmdUsuario.Parameters.AddWithValue("@email", usuario.Email);
                        cmdUsuario.Parameters.AddWithValue("@roleId", usuario.UserRoleId);

                        if (!string.IsNullOrEmpty(usuario.Senha))
                        {
                            cmdUsuario.Parameters.AddWithValue("@senha", usuario.Senha);
                        }

                        cmdUsuario.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Erro de banco de dados  ao atualizar: " + ex.Message, ex);
                    }
                }
            }
        }

        public void Excluir(int id) 
        {
            using (var connection = new MySqlConnection(_connectionString))
            { 
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"DELETE FROM Pessoa
                                         WHERE Id = @id";

                        MySqlCommand cmdPessoa = new MySqlCommand(query, connection, transaction);
                        cmdPessoa.Parameters.AddWithValue("@id", id);

                        cmdPessoa.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Erro de banco de dados ao excluir: " + ex.Message, ex);
                    }
                }
            }
        }

        public List<Usuario> Buscar(string termo) 
        { 
            var usuarios = new List<Usuario>();
            using (var connection = new MySqlConnection(_connectionString))
            { 
                connection.Open();

                string query = @"SELECT 
                                    p.Id,
                                    p.Nome,
                                    p.DataNascimento,
                                    p.Cpf,
                                    u.Email,
                                    u.RoleId
                                  FROM 
                                    Pessoa p
                                  JOIN 
                                    Usuario u ON u.PessoaId = p.Id
                                  WHERE
                                    p.Nome  LIKE @termo 
                                    OR
                                    u.Email LIKE @termo
                                    OR 
                                    p.Cpf LIKE @termo";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@termo", $"%{termo}%");

                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var usuario = new Usuario
                            {
                                Id = reader.GetInt32("Id"),
                                Nome = reader.GetString("Nome"),
                                DataNascimento = reader.GetDateTime("DataNascimento"),
                                Cpf = reader.GetString("Cpf"),
                                Email = reader.GetString("Email"),
                                UserRoleId = reader.GetInt32("RoleId")
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            
            }
            return usuarios;
        }

        /// <summary>
        /// Busca um usuário específico pelo seu e-mail.
        /// Crucial para a lógica de Login e para verificar duplicidade.
        /// </summary>
        /// <param name="email">O e-mail exato a ser buscado.</param>
        /// <returns>O objeto Usuario (incluindo a senha hasheada) ou null se não encontrado.</returns>
        public Usuario GetPorEmail(string email)
        {
            Usuario usuario = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT 
                                    p.Id, 
                                    p.Nome, 
                                    p.DataNascimento, 
                                    p.Cpf, 
                                    u.Email, 
                                    u.Senha, 
                                    u.RoleId 
                                FROM 
                                    Usuario u
                                JOIN 
                                    Pessoa p ON p.Id = u.PessoaId
                                WHERE 
                                    u.Email = @email";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32("Id"),
                                Nome = reader.GetString("Nome"),
                                DataNascimento = reader.GetDateTime("DataNascimento"),
                                Cpf = reader.GetString("Cpf"),
                                Email = reader.GetString("Email"),
                                Senha = reader.GetString("Senha"),
                                UserRoleId = reader.GetInt32("RoleId")
                            };
                        }
                    }

                }
            }
            return usuario;
        }

    }
}
