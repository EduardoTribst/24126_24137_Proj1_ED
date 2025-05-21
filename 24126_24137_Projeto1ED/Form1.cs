// Projeto desenvolvido por:
// Eduardo 24126
// Júlio 24137

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace apListaLigada
{
    public partial class FrmDicionario : Form
    {
        ListaDupla<PalavraEDica> lista1;
        Situacao situacaoAtual; // variável para armazenar a situação atual

		public FrmDicionario()
        {
            InitializeComponent();
        }

        // enum de situacao
        public enum Situacao
        {
            INCLUINDO,
            NAVEGANDO,
            ALTERANDO,
            EXCLUINDO
        }

		// método para alterar a situação atual, exibindo a mensagem correspondente
		// e atualizando o status da situação no formulário
		private void alterarSituacao(Situacao novaSituacao)
        {
            situacaoAtual = novaSituacao;
            switch (situacaoAtual) // exibe a mensagem
            {
                case Situacao.INCLUINDO:
                    slSituacao.Text = "INCLUINDO (aperte CANCELAR para terminar o processo de inclusão).";
                    break;
                case Situacao.NAVEGANDO:
                    slSituacao.Text = "NAVEGANDO";
                    break;
                case Situacao.ALTERANDO:
                    slSituacao.Text = "ALTERANDO (clique em CANCELAR para cancelar o processo ou ALTERAR novamente para aplicar as alteração).";
                    break;
                case Situacao.EXCLUINDO:
                    slSituacao.Text = "EXCLUINDO (clique em CANCELAR para cancelar o processo ou EXCLUIR novamente para confirmar)";
                    break;
            };
        }

        private void FazerLeitura(ref ListaDupla<PalavraEDica> qualLista)
        {
            qualLista = new ListaDupla<PalavraEDica>(); // instancia a lista

            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                StreamReader arquivo = new StreamReader(dlgAbrir.FileName);
                string linha = "";

                while (!arquivo.EndOfStream)
                {
					// cria uma nova palavra e dica a partir da linha lida e insere na lista
					linha = arquivo.ReadLine();
                    qualLista.InserirAposFim(new PalavraEDica(linha));
				}
                arquivo.Close();
            }
        }   


        private void btnIncluir_Click(object sender, EventArgs e)
        {
            if (situacaoAtual == Situacao.INCLUINDO) // se já estiver em inclusão, tenta incluir a palavra
            {
                if (txtPalavra.Text != "" && txtDica.Text != "")
                {
                    // tenta incluir a palavra e dica na lista
                    if (lista1.InserirEmOrdem(new PalavraEDica(txtPalavra.Text, txtDica.Text)))
                    {
                        MessageBox.Show("Palavra incluída com sucesso.");
                        ExibirRegistroAtual();
                    }
                    else
                    {
                        MessageBox.Show("Palavra já cadastrada.");
                    }
                }
                else
                {
                    MessageBox.Show("Preencha os campos corretamente.");
                }
            }
            else // se não, altera a situação para inclusão
            {
                alterarSituacao(Situacao.INCLUINDO);
			}
		}


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtPalavra.Text != "")
            {
                // instancia uma palvra para buscar na lista
                var palavraProcurada = new PalavraEDica(txtPalavra.Text, "");
                if (!lista1.Existe(palavraProcurada))
                {
                    MessageBox.Show("Palavra não encontrada");
                }
                else
                {
					// posiciona a lista na palavra atual, encontrada pelo método existe
					ExibirRegistroAtual();
                }
            }
			else
			{
				MessageBox.Show("Preencha os campos corretamente.");
			}
		}

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (txtPalavra.Text != "")
            {
                // verifica se a ação atual é de exclusão ou não
				if (situacaoAtual == Situacao.EXCLUINDO) // se for, tenta remover a palavra digitada
                {
                    // pede a confirmação para exclusão
                    DialogResult resultado = MessageBox.Show("Deseja realmente excluir a palavra?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resultado == DialogResult.Yes)
                    {
                        // tenta remover a palavra digitada
                        if (lista1.Remover(new PalavraEDica(txtPalavra.Text, "")))
                        {
                            MessageBox.Show("Palavra removida");
                            ExibirRegistroAtual();
                        }
                        else
                        {
                            MessageBox.Show("Palavra não encontrada");
                        }
                    } 
				}
                else // se não, altera a situação para a de exclusão
                {
                    alterarSituacao(Situacao.EXCLUINDO);
                }
            }
			else
			{
				MessageBox.Show("Preencha os campos corretamente.");
			}
		}

        private void ExibirDados(ListaDupla<PalavraEDica> aLista, ListBox lsb, Direcao qualDirecao)
        {
            // limpa a lista e adiciona os dados alinhados
            lsb.Items.Clear();
            var dadosDaLista = aLista.Listagem(qualDirecao);
            foreach (PalavraEDica palavra in dadosDaLista)
            lsb.Items.Add(palavra.Palavra.PadRight(30) + " - " + palavra.Dica);
        }

        private void rbFrente_Click(object sender, EventArgs e)
        {
            // exibe os dados na ordem crescente
            ExibirDados(lista1, lsbDados, Direcao.paraFrente);
        }

        private void rbTras_Click(object sender, EventArgs e)
        {
			// exibe os dados na ordem decrescente
			ExibirDados(lista1, lsbDados, Direcao.paraTras);
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
			// posiciona a lista no início
			lista1.PosicionarNoInicio();
            ExibirRegistroAtual();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
			// se o nó atual não for o primeiro, retrocede para o nó anterior
			if (lista1.NumeroDoNoAtual == 0)
            {
                MessageBox.Show("Já está no primeiro nó! Não é possível retroceder.");
            }
            else
            {
                lista1.Retroceder();
                ExibirRegistroAtual();
            }
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
			// se o nó atual não for o último, avança para o próximo nó
			if (lista1.NumeroDoNoAtual == lista1.QuantosNos-1)
            {
                MessageBox.Show("Já está no último nó! Não é possível avançar.");
            }
            else
            {
                lista1.Avancar();
                ExibirRegistroAtual();
            }
        }

        private void btnFim_Click(object sender, EventArgs e)
        {
			// posiciona a lista no fim
			lista1.PosicionarNoFinal();
            ExibirRegistroAtual();
        }

        private void ExibirRegistroAtual()
        {
            if (!lista1.EstaVazia)
            {
				// exibe os dados do nó atual nas caixas de texto e atualiza o índice na parte inferior do formulário
				var palavraAtual = lista1[lista1.NumeroDoNoAtual];
                txtPalavra.Text = palavraAtual.Palavra;
                txtDica.Text = palavraAtual.Dica;
                slRegistro.Text = $"Registro: {lista1.NumeroDoNoAtual + 1}/{lista1.QuantosNos}";
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (situacaoAtual == Situacao.ALTERANDO) // apertou novamente para confirmar a edição
            {
                if (txtPalavra.Text != "")
                {
                    var palavraAAlterar = new PalavraEDica(txtPalavra.Text, "");
                    if (lista1.Existe(palavraAAlterar))
                    {
                        lista1.Atual.Info.Dica = txtDica.Text;
                        MessageBox.Show($"A dica da palavra {lista1.Atual.Info.Palavra} foi alterada com sucesso.");
                    }
                    else
                    {
                        MessageBox.Show($"A palavra {lista1.Atual.Info.Palavra} não está cadastrada.");
                    }
                }
                else
                {
                    MessageBox.Show("Preencha os campos corretamente.");
				}
                
                ExibirRegistroAtual();
            }
            else // altera a situação para edição
            {
                alterarSituacao(Situacao.ALTERANDO);
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            // fecha o formulario
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // cancela qualquer estado
            alterarSituacao(Situacao.NAVEGANDO);
        }

        private void FrmDicionario_Load(object sender, EventArgs e)
        {
			// pede abertura de arquivo e lê os dados
			FazerLeitura(ref lista1);

			// exibe os dados na ordem crescente
			alterarSituacao(Situacao.NAVEGANDO);
			lista1.PosicionarNoInicio();
            ExibirRegistroAtual();
        }

        private void FrmDicionario_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dlgSalvar.ShowDialog() == DialogResult.OK)
            {
                // salva os dados
                lista1.GravarDados(dlgSalvar.FileName);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
			switch ((sender as TabControl).SelectedIndex)
			{
                case 1: // listagem
                    rbFrente.PerformClick(); // exibe os dados na ordem crescente
                    break;
			}
        }

    }
}
