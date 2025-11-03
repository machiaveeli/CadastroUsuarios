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

        public bool adicionarUsuario(string nome, DateTime? dataNasci, string cpf, string email, string senha, int roleId)
        {
            var dadosValidados = validarDados(nome, dataNasci, cpf, email, senha);

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

        private bool validarDados(string nome, DateTime? dataNasci, string cpf, string email, string senha)
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

            if (string.IsNullOrWhiteSpace(senha))
            {
                MessageBox.Show("O campo 'Senha' é obrigatório.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
