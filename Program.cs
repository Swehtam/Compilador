using System;
using System.IO;

namespace Teste
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:/Users/mathe/Desktop/teste.txt";

            // Como ler um arquivo linha por linha
            using (StreamReader file = new StreamReader(path))
            {
                int counter = 0;
                char ln;

                while (file.Peek() >= 0)
                {
                    int num_letra = file.Read();
                    char letra = (char)num_letra;
                    Console.WriteLine(num_letra + "="+ letra);
                    counter++;
                }
                file.Close();
                Console.WriteLine($"File has {counter} characteres.");
            }
        }
    }
}
