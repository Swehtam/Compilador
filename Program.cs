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
                    int contador = 1;
                    bool comentario = false;
                    int coment_linha = 1;
                    bool escreve = false;
                    string palavra = "";
                    string tipo = "";

                    while (readFile.Peek() >= 0)
                    {
                        int num_letra = readFile.Read();
                        //Quando pular linha somar contador "/r"
                        if(num_letra == 13)
                        {
                            contador++;
                            continue;
                        }
                        //Caso seja "{", então abriu comentário
                        else if (num_letra == 123)
                        {
                            coment_linha = contador;
                            comentario = true;
                            if (!palavra.Equals(""))
                            {
                                WriteOnText(writeFile, palavra, contador);
                                palavra = "";
                                tipo = "";
                            }
                            continue;
                        }
                        //Caso seja "}", então fechou comentário
                        else if (num_letra == 125)
                        {
                            comentario = false;
                            continue;
                        }

                        if (!comentario)
                        {
                            //Ignorar caso seja um /n /t ou " "
                            if (num_letra == 32 || num_letra == 9 || num_letra == 10)
                            {
                                if (!palavra.Equals(""))
                                {
                                    WriteOnText(writeFile, palavra, contador);
                                    palavra = "";
                                    tipo = "";
                                }
                                continue;
                            }

                            //Caso seja um inteiro ou um real
                            if(((num_letra >= 48 && num_letra <= 57) && (palavra.Equals("") || tipo.Equals("Inteiro") || tipo.Equals("Real")) || (num_letra == 46 && tipo.Equals("Inteiro"))))
                            {
                                char letra = (char)num_letra;
                                palavra += letra;
                                if(num_letra == 46)
                                {
                                    tipo = "Real";
                                }
                                if (tipo.Equals(""))
                                {
                                    tipo = "Inteiro";
                                }
                                
                            }
                        }    
                    }
                    if (comentario)
                    {
                        Console.Error.WriteLine("\nComentário aberto e não fechado na linha: " + coment_linha);
                    }
                    readFile.Close();
                    Console.WriteLine($"File has {contador} lines.");
                }
                writeFile.Close();
            }
        }

        static void CheckIfNum(int num_letra)
        {

        }

        static void WriteOnText(StreamWriter writeFile, string palavra, int contador)
        {
            Console.WriteLine(palavra + "       |   blabla  |   " + contador);
            writeFile.WriteLine(palavra + "       |   blabla  |   " + contador);
        }
    }
}
