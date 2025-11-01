using SistemaCadastroUsuarios.Models;
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
        private readonly TelaDeCadastro _view;

        public UsuarioController(TelaDeCadastro view)
        {
            _view = view;
        }

        public void adicionarUsuario(string nome, DateTime? dataNasci, string cpf, string email, string senha, int roleId)
        {
            var dadosValidados = validarDados(nome, dataNasci, cpf, email, senha);

            if (!dadosValidados)
            {
                return;
            }
            
            DateTime valorDataNascimento = dataNasci.Value;

            var usuario = new Usuario(nome, valorDataNascimento, cpf, email, senha, roleId);
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
