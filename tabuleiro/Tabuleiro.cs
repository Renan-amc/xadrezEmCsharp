using System;

namespace xadrez_console.tabuleiro
{
    internal class Tabuleiro
    {
        public int linhas { get; set; }
        public int colunas { get; set; }
        private Peca[,] pecas { get; set; }

        public Tabuleiro(int linhas, int colunas)
        {
            this.linhas = linhas;
            this.colunas = colunas;
            pecas = new Peca[linhas, colunas];
        }

        public Peca peca(int linha, int coluna)
        {
            return pecas[linha, coluna];
        }

        public Peca peca(Posicao pos)
        {
            return pecas[pos.linha, pos.coluna];
        }

        public bool existePeca(Posicao pos)
        {
            validarPosicao(pos); //verfica se a posição é valida se não for lança exceção
            return peca(pos) != null;
        }

        public void colocarPeca(Peca p, Posicao pos)
        {
            if (existePeca(pos))
            {
                throw new TabuleiroException("Já existe uma peça nessa posição: " + pos);
            }
            pecas[pos.linha, pos.coluna] = p;
            p.posicao = pos;
        }


        public Peca retirarPeca(Posicao pos)
        {
            if(peca(pos) == null)
            {
                return null;
            }
            Peca aux = peca(pos);
            aux.posicao = null;
            pecas[pos.linha, pos.coluna] = null;
            return aux;
        }

        //Se a posição que eu inseri está dentro do linhas e colunas estabelecida na instanciação do tabuleiro
        public bool posicaoValida(Posicao pos)         
        {
            if(pos.linha < 0 || pos.linha >= linhas || pos.coluna < 0 || pos.coluna >= colunas)
            {
                return false;
            }
            return true;
        }

        //Lança uma exceção na classe tabuleiroException que é herdada de Exception q vai pegar a msg que eu escrevi e repassar pra classe Exception pra quando der erro ele mostrar na tela esta msg
        public void validarPosicao(Posicao pos)// 
        {
            if (!posicaoValida(pos))
            {
                throw new TabuleiroException("Posição inválida! Está posição está fora do tabuleiro.");
            }
        }
    }
}
