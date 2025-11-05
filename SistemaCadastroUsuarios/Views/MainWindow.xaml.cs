using SistemaCadastroUsuarios.Controllers;
using SistemaCadastroUsuarios.Models;
using SistemaCadastroUsuarios.Services;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SistemaCadastroUsuarios
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UsuarioController _controller;

        public MainWindow()
        {
            InitializeComponent();
                
            IUsuarioDAO servicoDeDados = new MySqlUsuarioDAO();

            IPasswordHasher hasher = new BcryptPasswordHasher();

            IUsuarioService servicoDeLogica = new InMemoryUsuarioService();

            _controller = new UsuarioController(servicoDeLogica);
        }

        private void BtnAcessar_Click(object sender, RoutedEventArgs e)
        {
            var email = txtUsuario.Text;
            var senha = pwdSenha.Password;

            TelaDeCadastro telaDeCadastro = new TelaDeCadastro();
            telaDeCadastro.Show();
        }
    }
}