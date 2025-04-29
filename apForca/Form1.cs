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
        ListaDupla<Palavra> lista1;
        Situacao situacaoAtual;

        public FrmDicionario()
        {
            InitializeComponent();
        }

        public enum Situacao
        {
            INCLUINDO,
            NAVEGANDO,
            ALTERANDO,
            EXCLUINDO
        }

        private void alterarSituacao(Situacao novaSituacao)
        {
            situacaoAtual = novaSituacao;
            switch (situacaoAtual)
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
                    slSituacao.Text = "EXCLUINDO";
                    break;
            };
        }

        private void FazerLeitura(ref ListaDupla<Palavra> qualLista)
        {
            qualLista = new ListaDupla<Palavra>();

            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                StreamReader arquivo = new StreamReader(dlgAbrir.FileName);
                string linha = "";

                while (!arquivo.EndOfStream)
                {
                    linha = arquivo.ReadLine();
                    qualLista.InserirAposFim(new Palavra(linha));
                }
                arquivo.Close();
            }
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            if (txtPalavra.Text != "" && txtDica.Text != "")
            {
                var novaPalavra = new Palavra(txtPalavra.Text, txtDica.Text);
                if (lista1.InserirEmOrdem(novaPalavra))
                {
                    MessageBox.Show("Palavra cadastrada com sucesso");
                } 
                else
                {
                    MessageBox.Show("Palavra já existe no cadastro");
                }
                ExibirRegistroAtual();
            }
        }


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtPalavra.Text != "")
            {
                var palavraProcurada = new Palavra(txtPalavra.Text, "");
                if (!lista1.Existe(palavraProcurada))
                {
                    MessageBox.Show("Palavra não encontrada");
                }
                else
                {
                    ExibirRegistroAtual();
                }
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (txtPalavra.Text != "")
            {
                if (situacaoAtual == Situacao.EXCLUINDO)
                {
                    if (lista1.Remover(new Palavra(txtPalavra.Text, "")))
                    {
                        MessageBox.Show("Palavra removida");
                        ExibirRegistroAtual();
                    }
                    else
                    {
                        MessageBox.Show("Palavra não encontrada");
                    }
                }
                else
                {
                    alterarSituacao(Situacao.EXCLUINDO);
                    MessageBox.Show("Clique novamente em EXCLUIR para confirmar ou cancele a operação com CANCELAR");
                }
            }
        }

        private void ExibirDados(ListaDupla<Palavra> aLista, ListBox lsb, Direcao qualDirecao)
        {
            lsb.Items.Clear();
            var dadosDaLista = aLista.Listagem(qualDirecao);
            foreach (Palavra palavra in dadosDaLista)
            lsb.Items.Add(palavra.DescricaoPalavra + " - " + palavra.Dica);
        }

        private void tabControl1_Enter(object sender, EventArgs e)
        {
            rbFrente.PerformClick();
        }

        private void rbFrente_Click(object sender, EventArgs e)
        {
            ExibirDados(lista1, lsbDados, Direcao.paraFrente);
        }

        private void rbTras_Click(object sender, EventArgs e)
        {
            ExibirDados(lista1, lsbDados, Direcao.paraTras);
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            lista1.PosicionarNoInicio();
            ExibirRegistroAtual();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
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
            lista1.PosicionarNoFinal();
            ExibirRegistroAtual();
        }

        private void ExibirRegistroAtual()
        {
            if (!lista1.EstaVazia)
            {
                var palavraAtual = lista1[lista1.NumeroDoNoAtual];
                txtPalavra.Text = palavraAtual.DescricaoPalavra;
                txtDica.Text = palavraAtual.Dica;
                slRegistro.Text = $"Registro: {lista1.NumeroDoNoAtual + 1}/{lista1.QuantosNos}";
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (situacaoAtual == Situacao.ALTERANDO) // apertou novamente
            {
                var palavraAAlterar = new Palavra(txtPalavra.Text, "");
                if (lista1.Existe(palavraAAlterar))
                {
                    lista1.Atual.Info.Dica = txtDica.Text;
                    MessageBox.Show($"A dica da palavra {lista1.Atual.Info.DescricaoPalavra} foi alterada com sucesso.");
                }
                else
                {
                    MessageBox.Show($"A palavra {lista1.Atual.Info.DescricaoPalavra} não está cadastrada.");
                }
                alterarSituacao(Situacao.NAVEGANDO);
                ExibirRegistroAtual();
            }
            else
            {
                alterarSituacao(Situacao.ALTERANDO);
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            alterarSituacao(Situacao.NAVEGANDO);
        }

        private void FrmDicionario_Load(object sender, EventArgs e)
        {
            FazerLeitura(ref lista1);
            lista1.PosicionarNoInicio();
            ExibirRegistroAtual();
        }

        private void FrmDicionario_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dlgSalvar.ShowDialog() == DialogResult.OK)
            {
                lista1.GravarDados(dlgSalvar.FileName);
            }
        }
    }
}
