using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _provider;

        public MainWindow(UsuarioController controller, IServiceProvider provider)
        {
            InitializeComponent();
            _controller = controller;
            _provider = provider;
        }

        private void BtnAcessar_Click(object sender, RoutedEventArgs e)
        {
            var email = txtUsuario.Text;
            var senha = pwdSenha.Password;

            try
            {
                bool loginValido = _controller.FazerLogin(email, senha); 

                if (loginValido)
                {
                    var telaDeCadastro = _provider.GetRequiredService<TelaDeCadastro>();
                    telaDeCadastro.Show();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Email, senha ou permissão inválidos.", "Erro no Login", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro inesperado: {ex.Message}", "Erro Crítico", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}