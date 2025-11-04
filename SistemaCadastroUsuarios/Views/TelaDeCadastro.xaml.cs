using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SistemaCadastroUsuarios.Controllers;
using SistemaCadastroUsuarios.Models;
using SistemaCadastroUsuarios.Services;

namespace SistemaCadastroUsuarios
{
    /// <summary>
    /// Interaction logic for TelaDeCadastro.xaml
    /// </summary>
    public partial class TelaDeCadastro : Window
    {
        private readonly UsuarioController _controller;
        private Usuario _usuarioSelecionado = null;
        private List<Usuario> _todosUsuarios = new List<Usuario>(); 

        public TelaDeCadastro()
        {
            InitializeComponent();

            var servicoDeDados = new MySqlUsuarioService();

            _controller = new UsuarioController(this, servicoDeDados);

            CarregarUsuarios();
        }

        private void CarregarUsuarios()
        {
            try 
            { 
                _controller.ListarTodosUsuarios();
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}", "Erro");
            }
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparFormulario();
        }

        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            int idPermissao = (chkAdmin.IsChecked ?? false) ? 1 : 2;

            bool sucesso = false;

            try
            {
                if (_usuarioSelecionado == null)
                {
                    sucesso = _controller.AdicionarUsuario(
                        txtNome.Text,
                        dpDataNascimento.SelectedDate,
                        txtCpf.Text,
                        txtEmail.Text,
                        txtSenha.Password,
                        idPermissao,
                        txtSenha.IsEnabled);
                }
                else
                {
                    int idUsuarioParaAtualizar = _usuarioSelecionado.Id;

                    sucesso = _controller.AtualizarUsuario(
                        idUsuarioParaAtualizar,
                        txtNome.Text,
                        dpDataNascimento.SelectedDate,
                        txtCpf.Text,
                        txtEmail.Text,
                        txtSenha.Password,
                        idPermissao,
                        txtSenha.IsEnabled);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro");
            }

            if (sucesso)
            {
                LimparFormulario();
            }
            
        }

        private void LimparFormulario()
        {
            txtNome.Clear();
            dpDataNascimento.SelectedDate = null;
            txtCpf.Clear();
            txtEmail.Clear();
            txtSenha.Clear();
            chkAdmin.IsChecked = false;

            _usuarioSelecionado = null;
            dgUsuarios.SelectedItem = null;

            txtSenha.IsEnabled = true;

            btnAdicionar.Content = "Adicionar Usuário";
            btnExcluir.IsEnabled = false;

            if (txtPesquisar.Text.Length > 0)
            {
                txtPesquisar.Clear();
            }
        }

        public void AtualizarListaDeUsuarios(List<Usuario> usuarios)
        {
            dgUsuarios.ItemsSource = null;
            dgUsuarios.ItemsSource = usuarios;
        }

        private void DgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _usuarioSelecionado = dgUsuarios.SelectedItem as Usuario;

            if (_usuarioSelecionado == null)
            {
                LimparFormulario();
                return;
            }

            _usuarioSelecionado.Id = _usuarioSelecionado.Id;

            txtNome.Text = _usuarioSelecionado.Nome;
            txtEmail.Text = _usuarioSelecionado.Email;
            txtCpf.Text = _usuarioSelecionado.Cpf;
            dpDataNascimento.SelectedDate = _usuarioSelecionado.DataNascimento;

            txtSenha.Clear();
            txtSenha.IsEnabled = false;

            btnAdicionar.Content = "Salvar Alterações";
            btnExcluir.IsEnabled = true;
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
