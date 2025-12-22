using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SistemaCadastroUsuarios.Controllers;
using SistemaCadastroUsuarios.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SistemaCadastroUsuarios
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Provider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public App() 
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Provider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 1. Construir a configuração (Ler o arquivo JSON)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // 2. Registrar a configuração no container (caso precise injetar IConfiguration em algum lugar)
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IUsuarioDAO, MySqlUsuarioDAO>();

            // 3. Exemplo: Configurando o Banco (Entity Framework) usando a connection string
            // services.AddDbContext<SeuDbContext>(options =>
            //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Seus outros serviços continuam aqui...
            services.AddTransient<IUsuarioService, UsuarioService>();
            services.AddTransient<IPasswordHasher, BcryptPasswordHasher>();
            services.AddTransient<UsuarioController>();
            services.AddTransient<MainWindow>();
            services.AddTransient<TelaDeCadastro>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = Provider.GetRequiredService<MainWindow>();

            mainWindow.Show();
        }

    }
}
