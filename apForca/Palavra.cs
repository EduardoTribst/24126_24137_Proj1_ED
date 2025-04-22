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

        private int tamanho = 30;

        public int Tamanho
        {
            get { return tamanho; }
        }

        public string DescricaoPalavra { get; set; }

        public int CompareTo(Palavra outraPalavra)
        {
            return descricaoPalavra.CompareTo(outraPalavra.DescricaoPalavra);
        }

        public string FormatoDeArquivo()
        {
            return descricaoPalavra.PadLeft(tamanho);
        }
    }
}
