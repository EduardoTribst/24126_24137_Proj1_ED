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

        const int tamanhoPalavra = 30; //NAO TEM Q SER 15 NAO????

        private string dica;

        private bool[] acertou = new bool[30];

        public int TamanhoPalavra
        {
            get { return tamanhoPalavra; }
        }



        public string Dica { get; set; }

        public bool[] Acertou { get => acertou; }
        public string Palavra { get => palavra; set => palavra = value; }

        public void AcabouOJogo() //Precisa deixar as letras do vetor de acertou como false
        {
            for (int i = 0; i < acertou.Length; i++)
            {
                acertou[i] = false;
            }
        }

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
            // variavel para retornar se a letra existe na palavra
            bool tem = false;

	        for (int i = 0; i < Palavra.Length ; i++) 
	        {
		        if (Palavra[i] == letra) 
		        {
                    // marca a letra como acertada no vetor acertou
                    tem = true;
		            acertou[i] = true;
		        }
	        }
	    
	        return tem;
	    }
    }
}
