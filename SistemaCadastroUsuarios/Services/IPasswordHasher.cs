using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCadastroUsuarios.Services
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Cria um hash de uma senha em texto plano.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica se a senha em texto plano corresponde ao hash salvo.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        bool VerifyPassword(string password, string storedHash);
    }
}
