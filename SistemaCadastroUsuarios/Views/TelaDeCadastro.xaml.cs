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

            IUsuarioDAO servicoDeDados = new MySqlUsuarioDAO();

            IUsuarioService servicoDeLogica = new UsuarioService(servicoDeDados);

            _controller = new UsuarioController(servicoDeLogica);

            CarregarUsuarios();
        }

        private void CarregarUsuarios()
        {
            try 
            {
                _todosUsuarios = _controller.ListarTodosUsuarios();

                AtualizarListaDeUsuarios(_todosUsuarios);
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
            int idPermissao = (chkAdmin.IsChecked ?? false) ? 2 : 1;

            try
            {
                if(_usuarioSelecionado == null)
                {
                    _controller.CriarUsuario( 
                        txtNome.Text,
                        dpDataNascimento.SelectedDate,
                        txtCpf.Text,
                        txtEmail.Text,
                        txtSenha.Password,
                        idPermissao);
                }
                else
                {
                    int idUsuarioParaAtualizar = _usuarioSelecionado.Id;

                    _controller.AtualizarUsuario( 
                        idUsuarioParaAtualizar,
                        txtNome.Text,
                        dpDataNascimento.SelectedDate,
                        txtCpf.Text,
                        txtEmail.Text,
                        txtSenha.Password,
                        idPermissao);
                }

                MessageBox.Show("Operação realizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LimparFormulario();
                CarregarUsuarios();
            }
            catch (ArgumentException valEx)
            {
                MessageBox.Show(valEx.Message, "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
            _todosUsuarios = usuarios;
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
            txtNome.Text = _usuarioSelecionado.Nome;
            txtEmail.Text = _usuarioSelecionado.Email;
            txtCpf.Text = _usuarioSelecionado.Cpf;
            dpDataNascimento.SelectedDate = _usuarioSelecionado.DataNascimento;
            chkAdmin.IsChecked = (_usuarioSelecionado.UserRoleId == 2);
            txtSenha.IsEnabled = true;
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
