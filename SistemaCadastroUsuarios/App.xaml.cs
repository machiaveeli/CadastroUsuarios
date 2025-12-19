using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SistemaCadastroUsuarios.Controllers;
using SistemaCadastroUsuarios.Services;

namespace SistemaCadastroUsuarios
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider provider { get; private set; }

        public App() 
        {
            var service = new ServiceCollection();

            service.AddTransient<IUsuarioService, UsuarioService>();
            service.AddTransient<IPasswordHasher, BcryptPasswordHasher>();
            service.AddTransient<UsuarioController>();
            service.AddTransient<MainWindow>();
            service.AddTransient<TelaDeCadastro>();

            service.AddScoped<IUsuarioDAO, MySqlUsuarioDAO>();

            provider = service.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = provider.GetRequiredService<MainWindow>();

            mainWindow.Show();
        }

    }
}
