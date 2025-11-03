using MySql.Data.MySqlClient;
using SistemaCadastroUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
