using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;

namespace SistemaCadastroUsuarios.Services
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public BcryptPasswordHasher() { }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        public bool VerifyPassword(string password, string storedHash) 
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, storedHash);

            }
            catch (Exception)
            {
                return false;        
            }        
        }
    }
}
