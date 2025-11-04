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
    public class UsuarioController
    {
        private readonly TelaDeCadastro _view;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(TelaDeCadastro view, IUsuarioService usuarioService)
        {
            _view = view;
            _usuarioService = usuarioService;
        }

        public bool AdicionarUsuario(string nome, DateTime? dataNasci, string cpf, string email, string senha, int roleId, bool senhaHabilitada)
        {
            var dadosValidados = ValidarDados(nome, dataNasci, cpf, email, senha, senhaHabilitada);

            if (!dadosValidados)
            {
                return false;
            }
            
            DateTime valorDataNascimento = dataNasci.Value;

            try
            {
                var usuario = new Usuario(nome, valorDataNascimento, cpf, email, senha, roleId);
                _usuarioService.Adicionar(usuario);

                var listaAtualizada = _usuarioService.ListarTodos();
                _view.AtualizarListaDeUsuarios(listaAtualizada);

                MessageBox.Show($"Usuário '{nome}' cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Erro ao salvar no banco: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }

        public bool AtualizarUsuario(int id, string nome, DateTime? dataNasci, string cpf, string email, string senha, int roleId, bool senhaHabilitada)
        {
            var dadosValidados = ValidarDados(nome, dataNasci, cpf, email, senha, senhaHabilitada);

            if (!dadosValidados)
            {
                return false;
            }

            DateTime valorDataNascimento = dataNasci.Value;

            try
            {
                var usuario = new Usuario { 
                    Id = id,
                    Nome = nome, 
                    DataNascimento = valorDataNascimento, 
                    Cpf = cpf, 
                    Email = email, 
                    Senha = senha,
                    UserRoleId = roleId 
                };
                _usuarioService.Atualizar(usuario);

                ListarTodosUsuarios();

                MessageBox.Show($"Usuário '{nome}' alterado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar no banco: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void ExcluirUsuario()
        {
            var resultado = System.Windows.MessageBox.Show("Tem certeza que deseja excluir este usuário?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }

        public void ListarTodosUsuarios()
        {
            var listaTodosUsuarios = _usuarioService.ListarTodos();
            _view.AtualizarListaDeUsuarios(listaTodosUsuarios);
        }

        private bool ValidarDados(string nome, DateTime? dataNasci, string cpf, string email, string senha, bool senhaHabilitada)
        {
            if (string.IsNullOrEmpty(nome))
            {
                MessageBox.Show("O campo 'Nome' é obrigatório.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!dataNasci.HasValue)
            {
                MessageBox.Show("O campo 'Data de Nasc' é obrigatório.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(cpf))
            {
                MessageBox.Show("O campo 'CPF' é obrigatório.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("O campo 'Email' é obrigatório.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(senha) && senhaHabilitada)
            {
                MessageBox.Show("O campo 'Senha' é obrigatório.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
