// Projeto desenvolvido por:
// Eduardo 24126
// Júlio 24137

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace apListaLigada
{
    public partial class FrmDicionario : Form
    {
        ListaDupla<PalavraEDica> lista1;
        Situacao situacaoAtual; // variável para armazenar a situação atual
        int tempo;
        Random random = new Random();
        bool bloquearTabControl;
        int erros = 0;
        int pontos = 0;


        public FrmDicionario()
        {
            InitializeComponent();
            tabControl1.Selecting += tabControl1_Selecting;
        }

        // enum de situacao
        public enum Situacao
        {
            INCLUINDO,
            NAVEGANDO,
            ALTERANDO,
            EXCLUINDO,
            JOGANDO
        }

		// método para alterar a situação atual, exibindo a mensagem correspondente
		// e atualizando o status da situação no formulário
		private void AlterarSituacao(Situacao novaSituacao)
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
                case Situacao.JOGANDO:
                    slSituacao.Text = "JOGANDO";
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
                AlterarSituacao(Situacao.INCLUINDO);
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
                            lista1.PosicionarNoInicio(); // posiciona a lista no início
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
                    AlterarSituacao(Situacao.EXCLUINDO);
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
                AlterarSituacao(Situacao.ALTERANDO);
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
            AlterarSituacao(Situacao.NAVEGANDO);
        }

        private void EstadoBotoesNavegacao(bool ligados)
        {
            foreach (var item in toolStrip1.Items)
            {
                if (item is ToolStripButton button && button.Text != "Sair")
                {
                    button.Enabled = ligados;
                }
            }
        }

        private void FrmDicionario_Load(object sender, EventArgs e)
        {
			// pede abertura de arquivo e lê os dados
			FazerLeitura(ref lista1);

			// exibe os dados na ordem crescente
			AlterarSituacao(Situacao.NAVEGANDO);
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
                case 0: // cadastro
                    btnInicio.PerformClick(); // manda para o inicio da lista de palavras
                    EstadoBotoesNavegacao(true);
                    break;

                case 1: // listagem
                    rbFrente.PerformClick(); // exibe os dados na ordem crescente
                    EstadoBotoesNavegacao(false);
                    break;

                case 2: // forca
                    EstadoBotoesNavegacao(false);
                    break;
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // se o jogo estiver em andamento, não permite mudar de aba
            if (bloquearTabControl)
            {
                e.Cancel = true;
            }
        }

        // funcao generica para os botoes de letra
        private void btnLetra_Click(object sender, EventArgs e)
        {
            Button button;
            if (sender is Button) // errou
            {
                // salva o botão clicado e verifica se a letra está na palavra
                button = sender as Button;
                char caracter = button.Text.ToCharArray()[0];
                if (!lista1.Atual.Info.TemNaPalavra(caracter))
                {
                    // soma nos erros e diminui os pontos
                    if (chkModoDificil.Checked && !chkDica.Checked) // modo difícil == mais penalidades
                    {
                        erros += 2;
                        if (pontos > 0)
                        {
                            pontos -= 2;

                            if (pontos < 0)
                            {
                                pontos = 0; // garante que os pontos não fiquem negativos
                            }
                        }
                    }
                    else // modo normal
                    {
                        erros++;
                        if (pontos > 0)
                        {
                            pontos--;
                        }
                    }

                    // muda a cor de fundo para vermelho
                    button.BackColor = System.Drawing.Color.Red;

                    // exibe as pontuacoes e desenha o corpo do personagem
                    lblErros.Text = erros.ToString();
                    lblPontos.Text = pontos.ToString();
                    DesenharCorpo();
                }
                else // acertou
                { 
                    // aumenta os pontos de exibe no formulario
                    pontos++;

                    lblPontos.Text = pontos.ToString();

                    // verificar se ganhou e as letras acertadas no dgv
                    bool achouTodasAsLetras = true;
                    for (int i = 0; i < lista1.Atual.Info.Palavra.Length; i++)
                    {
                        if (lista1.Atual.Info.Acertou[i])
                        {
                            dgvPalavraForca.Rows[0].Cells[i].Value = lista1.Atual.Info.Palavra[i];
                        }
                        else
                        {
                            // caso alguma letra não tenha sido acertada, muda a variável para 
                            // false para indicar que a pessoa ainda nao achou todas as letras
                            achouTodasAsLetras = false;
                        }
                    }

                    // muda a cor do fundo do botão para verde
                    button.BackColor = System.Drawing.Color.Green;

                    if (achouTodasAsLetras)
                    {
                        // venceu!!!
                        Venceu();
                    }
                }

                // desabilita o botão clicado
                button.Enabled = false;
            }
        }

        private void Perdeu()
        {
            // exibe o personagem morto
            pbxMorto.Visible = true;
            pbxCabecaVivo.Visible = false;
            pbxCabecaMorto.Visible = true;

            // preencher a palavra para o usuário saber qual era
            for (int i = 0; i < lista1.Atual.Info.Palavra.Length; i++)
            {
                dgvPalavraForca.Rows[0].Cells[i].Value = lista1.Atual.Info.Palavra[i];
            }

            TerminarJogo();
        }

        private void Venceu()
        {
            // limpa o personagem e exibe a sua versão feliz
            LimparPersonagem();
            pbxPersonagemFeliz.Visible = true;
            VisibilidadeForca(false);
            TerminarJogo();
            MessageBox.Show("Você venceu!!!!");
        }

        private void TerminarJogo()
        {
            // reseta o estado do jogo para o padrão, pronto para o proximo jogo
            AlterarSituacao(Situacao.NAVEGANDO);

            lista1.Atual.Info.AcabouOJogo();
            bloquearTabControl = false;

            label13.Text = "Clique em iniciar para jogar!";
            foreach (Control ctrl in gbxTeclado.Controls)
            {
                if (ctrl is Button)
                {
                    ctrl.Enabled = false; // desabilita todos os botoes de letra

                    ctrl.BackColor = System.Drawing.Color.LightGray; // reseta a cor dos botões
                }
            }

            // para o timer e habilita o botao de iniciar jogo
            timer1.Stop();

            btnInicia.Enabled = true;
            chkDica.Enabled = true;
            chkModoDificil.Enabled = true;
        }

        private void VisibilidadeForca(bool visivel)
        {
            // define a visibilidade das imagens da forca
            PictureBox[] forca = { pbxBaseForca, pbxMeioForca, pbxViradaForca, pbxFimForca, pbxBaseCorda, pbxMeioCorda, pbxCimaCorda };
            foreach (PictureBox imagem in forca)
            {
                imagem.Visible = visivel;
            }
        }

        private void DesenharCorpo()
        {
            // desenha o corpo de acordo com o numero de erros
            PictureBox[] partes = { pbxCabecaVivo, pbxPescoco, pbxTronco, pbxMaoDireita, pbxMaoEsquerda, pbxBermuda, pbxPernaDireita, pbxPernaEsquerda };

            for (int i = 0; i < partes.Length; i++)
            {
                if (i < erros) // se o erro for menor que a parte do corpo, exibe a parte
                {
                    partes[i].Visible = true;
                }
                else // se não, deixa invisível
                {
                    partes[i].Visible = false;
                }
            }

            if (erros == 8)
            {
                Perdeu();
            }
        }

        private void LimparPersonagem()
        {
            // deixa o personagem invisível
            PictureBox[] partes = { pbxCabecaVivo, pbxPescoco, pbxTronco, pbxMaoDireita, pbxMaoEsquerda, pbxBermuda, pbxPernaDireita, pbxPernaEsquerda, pbxCabecaMorto, pbxMorto, pbxPersonagemFeliz };
            for (int i = 0; i < partes.Length; i++)
            {
                partes[i].Visible = false;
            }

        }

        private void btnInicia_Click(object sender, EventArgs e)
        {
            // inicia o jogo, reseta as variaveis e exibe a forca
            AlterarSituacao(Situacao.JOGANDO);

            bloquearTabControl = true;
            btnInicia.Enabled = false;
            chkDica.Enabled = false;
            chkModoDificil.Enabled = false;

            LimparPersonagem();
            VisibilidadeForca(true);

            foreach (Control ctrl in gbxTeclado.Controls)
            {
                if (ctrl is Button)
                {
                    ctrl.Enabled = true;
                }
            }
            label13.Text = "Adivinhe a palavra!";
            erros = 0;
            pontos = 0;
            lblErros.Text = erros.ToString();
            lblPontos.Text = pontos.ToString();

            string palavraAnterior = "";
            for (int i = 0; i< dgvPalavraForca.Columns.Count; i++)
            {
                palavraAnterior += dgvPalavraForca.Rows[0].Cells[i].Value?.ToString(); // concatena as letras da palavra anterior
            }

            if (palavraAnterior == "" || palavraAnterior == null) // é a primeira palavra
            {
                int quantasPalavrasPassadas = random.Next(lista1.QuantosNos);
                lista1.PosicionarNoInicio();
                for (int i = 0; i < quantasPalavrasPassadas; i++)
                {
                    lista1.Avancar();
                }
            }
            else // ja jogou alguma vez
            {
                while (palavraAnterior == lista1.Atual.Info.Palavra)
                {
                    int quantasPalavrasPassadas = random.Next(lista1.QuantosNos);
                    lista1.PosicionarNoInicio();
                    for (int i = 0; i < quantasPalavrasPassadas; i++)
                    {
                        lista1.Avancar();
                    }
                }
            }
            
            // encontra a palavra e exibe os espacos no dgv
            String palavraSelecionada = lista1.Atual.Info.Palavra;
            char[] letrasPalavra = palavraSelecionada.ToCharArray();

            dgvPalavraForca.Rows.Clear();
            dgvPalavraForca.Columns.Clear();
            dgvPalavraForca.ColumnHeadersVisible = false;
            dgvPalavraForca.RowHeadersVisible = false;

            for (int i = 0; i < letrasPalavra.Length; i++)
            {
                dgvPalavraForca.Columns.Add(i.ToString(), i.ToString());
                dgvPalavraForca.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            dgvPalavraForca.AllowUserToResizeColumns = true;
            dgvPalavraForca.Rows[0].Height = 33;
            dgvPalavraForca.AllowUserToResizeColumns = false;

            // opcao de dica
            if (chkDica.Checked)
            {
                if (chkModoDificil.Checked) // menos tempo
                {
                    tempo = 10;
                }
                else // tempo normal
                {
                    tempo = 30;
                }
                    timer1.Start();
                lblDica.Text = lista1.Atual.Info.Dica;
            }
            else
            {
                lblDica.Text = "(desabilitada)";
            }


        }

        private void timerTick(object sender, EventArgs e)
        {
            // a cada tick do timer, diminui o tempo e atualiza o label
            tempo -= 1;
            lblTempo.Text = tempo.ToString();
            if (tempo <= 0)
            {
                while (erros < 8)
                {
                    erros++;
                    DesenharCorpo();
                }
            }
        }

        // coisas abaixo feitas por diversao, gosto de um relogio e data no form
        // nao leva a serio so queria fazer um relogio
        private void AtualizarHorario(object sender, EventArgs e)
        {
            horario.Text = DateTime.Now.ToString().Substring(10, 9);
        }

        private void dtpSelecionarData_ValueChanged(object sender, EventArgs e)
        {
            espacoEmBranco.Spring = false;
            dataSelecionada.Text = dtpSelecionarData.Value.ToString().Substring(0, 10);
            espacoEmBranco.Spring = true;
        }

    }
}
