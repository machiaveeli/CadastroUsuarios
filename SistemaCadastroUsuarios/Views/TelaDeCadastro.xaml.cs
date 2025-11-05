using Google.Protobuf.WellKnownTypes;
using SistemaCadastroUsuarios.Controllers;
using SistemaCadastroUsuarios.Models;
using SistemaCadastroUsuarios.Services;
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
using System.Linq;

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

            IPasswordHasher passwordHasher = new BcryptPasswordHasher();

            IUsuarioService servicoDeLogica = new InMemoryUsuarioService();

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
            catch (Exception ex)
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
                if (_usuarioSelecionado == null)
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
            int idUsuarioParaDeletar = _usuarioSelecionado.Id;

            try
            {
                _controller.ExcluirUsuario(idUsuarioParaDeletar);

                MessageBox.Show("Usuário deletado com sucecsso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                LimparFormulario();
                CarregarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string termo = txtPesquisar.Text;

            var usuariosEncontrados = _controller.BuscarUsuario(termo);

            dgUsuarios.ItemsSource = usuariosEncontrados;
        }

        private void BtnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            string termo = txtPesquisar.Text;
            List<Usuario> usuariosParaExibir;

            if (string.IsNullOrWhiteSpace(termo))
            {
                usuariosParaExibir = _todosUsuarios;
            }
            else
            {
                usuariosParaExibir = _controller.BuscarUsuario(termo);
            }
            dgUsuarios.ItemsSource = usuariosParaExibir;
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            MainWindow telaDeLogin = new MainWindow();

            telaDeLogin.Show();

            this.Close();
        }

        private void TxtCpf_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            int caretIndex = textBox.CaretIndex;

            string cpfLimpo = new string(textBox.Text.Where(char.IsDigit).ToArray());

            if (cpfLimpo.Length > 11)
            {
                cpfLimpo = cpfLimpo.Substring(0, 11);
            }

            string cpfFormatado = cpfLimpo;
            if (cpfLimpo.Length > 3)
                cpfFormatado = cpfLimpo.Insert(3, ".");
            if (cpfLimpo.Length > 6)
                cpfFormatado = cpfFormatado.Insert(7, ".");
            if (cpfLimpo.Length > 9)
                cpfFormatado = cpfFormatado.Insert(11, "-");

            textBox.TextChanged -= TxtCpf_TextChanged;
            textBox.Text = cpfFormatado;
            textBox.TextChanged += TxtCpf_TextChanged;

            try
            {
                if (caretIndex > 0 && caretIndex <= cpfFormatado.Length)
                {
                    if (cpfFormatado.Length > e.Changes.First().RemovedLength + e.Changes.First().AddedLength)
                    {
                        textBox.CaretIndex = caretIndex + 1;
                    }
                    else
                    {
                        textBox.CaretIndex = caretIndex;
                    }
                }
                else
                {
                    textBox.CaretIndex = cpfFormatado.Length;
                }
            }
            catch
            {
                textBox.CaretIndex = cpfFormatado.Length;
            }
        }
    }
}
