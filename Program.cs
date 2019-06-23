using System;
using System.IO;

namespace Teste
{
    class Program
    {
        static void Main(string[] args)
        {
            string readPath = @"..\..\teste.txt";
            string writePath = @"..\..\writeTeste.txt";

            // Como ler um arquivo linha por linha
            using (StreamWriter writeFile = new StreamWriter(writePath))
            {
                writeFile.WriteLine("TOKEN   |   TIPO    |   LINHA");
                using (StreamReader readFile = new StreamReader(readPath))
                {
                    int counter = 1;
                    char ln;

                    while (readFile.Peek() >= 0)
                    {
                        bool escreve = true;
                        int num_letra = readFile.Read();
                        if(num_letra == 13)
                        {
                            counter++;
                            escreve = false;
                        }
                        else if(num_letra == 32 || num_letra == 9 || num_letra == 10)
                        {
                            escreve = false;
                        }

                        if (escreve)
                        {
                            char letra = (char)num_letra;
                            Console.WriteLine(letra + "       |   blabla  |   " + counter);
                            writeFile.WriteLine(letra + "       |   blabla  |   " + counter);
                        }
                    }
                    readFile.Close();
                    Console.WriteLine($"File has {counter} lines.");
                }
                writeFile.Close();
            }
        }
    }
}
