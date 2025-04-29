using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apListaLigada
{
    class Palavra : IComparable<Palavra>, IRegistro
    {
        private string descricaoPalavra;

        const int tamanho = 30;

        private string dica;

        public int Tamanho
        {
            get { return tamanho; }
        }

        public string DescricaoPalavra { get; set; }

        public string Dica { get; set; }

        public Palavra(string descricaoPalavra, string dica)
        {
            DescricaoPalavra = descricaoPalavra;
            Dica = dica;
        }

        public Palavra(string linha)
        {
            DescricaoPalavra = linha.Substring(0, tamanho);
            Dica = linha.Substring(tamanho);
        }

        public int CompareTo(Palavra outraPalavra)
        {
            return descricaoPalavra.CompareTo(outraPalavra.DescricaoPalavra);
        }

        public string FormatoDeArquivo()
        {
            return $"{descricaoPalavra.PadLeft(tamanho) + dica}";
        }
    }
}
