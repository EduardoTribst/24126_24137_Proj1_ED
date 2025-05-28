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

        private bool[] acertou = new bool[30];

        public int TamanhoPalavra
        {
            get { return tamanhoPalavra; }
        }

        public string Dica { get; set; }

        public string Acertou { get; }
        public string Palavra { get => palavra; set => palavra = value; }

        public PalavraEDica(string palavra, string dica)
        {
            Palavra = palavra.Trim().ToUpper();
            Dica = dica;
            for (int i = 0; i < palavra.Length; i++)
            {
                acertou[i] = false;
            } 
        }

        public PalavraEDica(string linha)
        {
            Palavra = linha.Substring(0, tamanhoPalavra).Trim().ToUpper();
            Dica = linha.Substring(tamanhoPalavra);
            for (int i = 0; i < Palavra.Length; i++)
            {
                acertou[i] = false;
            }
        }

        public int CompareTo(PalavraEDica outraPalavraEDica)
        {
            return Palavra.CompareTo(outraPalavraEDica.Palavra);
        }

        public string FormatoDeArquivo()
        {
            return $"{Palavra.PadRight(tamanhoPalavra)}{Dica}";
        }

	    public bool TemNaPalavra(char letra) 
	    {
	        bool tem = false;

	        for (int i = 0; i < Palavra.Length ; i++) 
	        {
		        if (Palavra[i] == letra) 
		        {
		            tem = true;
		            acertou[i] = true;
		        }
	        }
	    
	        return tem;
	    }
    }
}
