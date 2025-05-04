// Eduardo - 24126
// Júlio - 24137

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apListaLigada
{
    class PalavraEDica : IComparable<PalavraEDica>, IRegistro
    {
        private string palavra;

        const int tamanhoPalavra = 30;

        private string dica;

        public int TamanhoPalavra
        {
            get { return tamanhoPalavra; }
        }

        public string Palavra { get; set; }

        public string Dica { get; set; }

        public PalavraEDica(string palavra, string dica)
        {
            Palavra = palavra;
            Dica = dica;
        }

        public PalavraEDica(string linha)
        {
            Palavra = linha.Substring(0, tamanhoPalavra).Trim();
            Dica = linha.Substring(tamanhoPalavra);
        }

        public int CompareTo(PalavraEDica outraPalavraEDica)
        {
            return Palavra.CompareTo(outraPalavraEDica.Palavra);
        }

        public string FormatoDeArquivo()
        {
            return $"{Palavra.PadRight(tamanhoPalavra)}{Dica}";
        }
    }
}
