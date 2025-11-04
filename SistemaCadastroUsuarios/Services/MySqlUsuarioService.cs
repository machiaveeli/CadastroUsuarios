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
    public class MySqlUsuarioService : IUsuarioService
    {
        private readonly string _connectionString = "Server=localhost;Database=cadastro_db;Uid=root;Pwd=;";

        public void Adicionar(Usuario usuario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

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

                        string queryUsuario = @"UPDATE Usuario 
                                                SET Email = @email,
                                                    RoleId = @roleId
                                                WHERE 
                                                    PessoaId = @pessoaId";

                        MySqlCommand cmdUsuario = new MySqlCommand(queryUsuario, connection, transaction);
                        cmdUsuario.Parameters.AddWithValue("@pessoaId", usuario.Id);
                        cmdUsuario.Parameters.AddWithValue("@email", usuario.Email);
                        cmdUsuario.Parameters.AddWithValue("@roleId", usuario.UserRoleId);

                        cmdUsuario.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
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
                    catch (Exception)
                    {

                        transaction.Rollback();
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
                                    Nome  LIKE @termo 
                                    OR
                                    Email LIKE @termo";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@termo", $"%{termo}%");

                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var usuario = new Usuario
                            {
                                Id = reader.GetInt32("p.Id"),
                                Nome = reader.GetString("p.Nome"),
                                DataNascimento = reader.GetDateTime("p.DataNascimento"),
                                Cpf = reader.GetString("p.Cpf"),
                                Email = reader.GetString("u.Email"),
                                UserRoleId = reader.GetInt32("u.RoleId")
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            
            }
            return usuarios;
        }

    }
}
