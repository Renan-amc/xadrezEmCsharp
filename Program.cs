using xadrez_console.tabuleiro;

namespace xadrez_console {
    internal class Program {
        static void Main(string[] args) {
            Posicao P;

            P = new Posicao(3, 4);

            Console.WriteLine("Posicão: " + P);

            Console.ReadLine();
        }
    }
}