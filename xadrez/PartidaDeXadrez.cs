using xadrez_console.tabuleiro;
using System;
using System.Collections.Generic;

namespace xadrez_console.xadrez {
    internal class PartidaDeXadrez {

        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool xeque {get;  private set; }

        public PartidaDeXadrez() {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            terminada = false;
            jogadorAtual = Cor.Branca;
            xeque = false;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public Peca executaMovimento(Posicao origem, Posicao destino) {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if(pecaCapturada != null) {
                capturadas.Add(pecaCapturada);
            }
            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada) {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQteMovimentos();
            if(pecaCapturada != null) {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem);
        }

        public void realizaJogada(Posicao origem, Posicao destino) {
            Peca pecaCapturada = executaMovimento(origem, destino);

            if (estaEmXeque(jogadorAtual)) {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            if (estaEmXeque(adversaria(jogadorAtual))) {
                xeque = true;
            }
            else {
                xeque = false;
            }

            if (testeXequeMate(adversaria(jogadorAtual))) {
                terminada = true;
            }
            else {
                turno++;
                mudaJogador();
            }
        }

        public void validarPosicaoDeOrigem(Posicao origem) {
            if(tab.peca(origem) == null) {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if(jogadorAtual != tab.peca(origem).cor) {
                throw new TabuleiroException("A peça escolhida não é sua!");
            }
            if (!tab.peca(origem).existeMovimentosPossiveis()) {
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida!");
            }
        }

        public void validarPosicaoDeDestino(Posicao origem, Posicao destino) {
            if (!tab.peca(origem).podeMoverPara(destino)) {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }

        private void mudaJogador() {
            if (jogadorAtual == Cor.Branca) {
                jogadorAtual = Cor.Preta;
            }
            else {
                jogadorAtual = Cor.Branca;
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor) {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca x in capturadas) {
                if(x.cor == cor) {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor) { 
            HashSet<Peca> aux = new HashSet<Peca> ();

            foreach (Peca x in pecas) {
                if(x.cor == cor) {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(pecasCapturadas(cor)); // retira as pecas capturadas das pecas que estao em jogo
            return aux;
        }

        private Cor adversaria(Cor cor) {
            if(cor == Cor.Branca){
                return Cor.Preta;
            }
            else{
                return Cor.Branca;
            }
        }


        private Peca rei(Cor cor) {
            foreach(Peca x in pecasEmJogo(cor)) {
                if (x is Rei)
                {
                    return x;
                }
            }
            return null;
        }

        public bool estaEmXeque(Cor cor) {
            Peca R = rei(cor);
            if(R == null) {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");
            }
            foreach(Peca x in pecasEmJogo(adversaria(cor))) {
                bool[,] mat = x.movimentosPossiveis();
                // Se na matriz de movimentos da peça adversária x na posição onde está o rei significa que esta peça está e xeque
                if (mat[R.posicao.linha, R.posicao.coluna]) {
                    //se na pos que o rei tá na matriz de movimentos do adversária estiver aquela pos como verdadeira será possivel comer a peça R logo aquela cor está em xeque!
                    return true; 
                }
            }
            return false;
        }

        public bool testeXequeMate(Cor cor) {
            if (!estaEmXeque(cor)) {
                return false;
            }
            foreach(Peca x in pecasEmJogo(cor)) {
                bool[,] mat = x.movimentosPossiveis();
                // gera uma matriz com todos os movimentos possiveis da peça da cor escolhida
                for(int i = 0; i < tab.linhas; i++) {
                    for(int j = 0; j < tab.colunas; j++) {
                // percorre as linhas e colunas daquela tabela que foi criada
                        if (mat[i, j]) {
                // se na pos [x][y] da minha matriz booleana for verdadeira eu entro no if
                            Posicao origem = x.posicao;
                // pega uma peça x da lista pecas em jogo armazena a posicao dela nql instante
                            Posicao destino = new Posicao(i, j);
                // diz que a posicao da peca em jogo daquela cor vai ir pra posicao i, j ex: [0][1] do tab. 
                            Peca pecaCapturada = executaMovimento(origem, destino);
                // chama a func executaMovimento, armazenando a pecaCapturada
                            bool testeXeque = estaEmXeque(cor);
                // chama func estaEmXeque e armazena o valor
                // ele vai executar o movimento pra todas as posições possiveis pra todos as peças daquela cor e vai testar se quando faz o movimento a peça Rei fica em Xeque e armazena esse valor booleano;
                            desfazMovimento(origem, destino, pecaCapturada);
                // apos de verificar desfaz o movimento chamando a funcao
                            if(!testeXeque) {
                                return false;
                            }
                // se eu passar por todas as peças executar todos mov possiveis e se nenhum movimento foi tirado do xeque eu vou pular o if e entrar no return abaixo retornando um xequemate verdadiero.
                        }
                    }
                }
            }
            return true;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca) {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas() {
            colocarNovaPeca('c', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('b', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('d', 1, new Rei(tab, Cor.Branca));

            colocarNovaPeca('b', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('a', 8, new Rei(tab, Cor.Preta));
                       
        }

    }
}
