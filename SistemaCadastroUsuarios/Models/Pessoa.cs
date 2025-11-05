using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaCadastroUsuarios.Models
{
    public class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Cpf { get; set; }

        public int Idade
        {
            get
            {
                var hoje = DateTime.Today;
                var idade = hoje.Year - DataNascimento.Year;
                if (DataNascimento.Date > hoje.AddYears(-idade))
                {
                    idade--;
                }
                return idade < 0 ? 0 : idade;
            }
        }

        public Pessoa() { }
        public Pessoa(string nome, DateTime dataNascimento, string cpf)
        {
            Nome = nome;
            DataNascimento = dataNascimento;
            Cpf = cpf;
        }
    }
}
