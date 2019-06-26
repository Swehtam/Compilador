using System;
using System.Diagnostics;
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
                    //bool escreve = false;
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
                                continue;
                            }

                            //Caso seja "+" ou "-" 
                            if (palavra.Equals("") && ((num_letra == 43) || (num_letra == 45)))
                            {
                                char caractere = (char)num_letra;
                                palavra += caractere;
                                tipo = "Aditivo";
                                if (!palavra.Equals(""))
                                {
                                    WriteOnText(writeFile, palavra, contador, tipo);
                                    palavra = "";
                                    tipo = "";
                                }
                            }

                            //Caso seja "*" ou "/"
                            if (palavra.Equals("") && ((num_letra == 42) || (num_letra == 47)))
                            {
                                char caractere = (char)num_letra;
                                palavra += caractere;
                                tipo = "Multiplicativo";
                                if (!palavra.Equals(""))
                                {
                                    WriteOnText(writeFile, palavra, contador, tipo);
                                    palavra = "";
                                    tipo = "";
                                }
                            }

                            //Caso seja algum operador relacional
                            if (palavra.Equals("") || tipo.Equals("Relacional"))
                            {
                                bool podeImprimir = true;
                                switch (num_letra)
                                {
                                    //"=" (IGUAL)
                                    case 61:
                                        palavra += (char)num_letra;
                                        tipo = "Relacional";
                                        break;

                                    //"<" (MENOR)
                                    case 60:
                                        palavra += (char)num_letra;
                                        tipo = "Relacional";
                                        if ((readFile.Peek() == 61) || (readFile.Peek() == 62))
                                        {
                                            podeImprimir = false;
                                            continue;
                                        }
                                        break;

                                    //">" (MAIOR)
                                    case 62:
                                        palavra += (char)num_letra;
                                        tipo = "Relacional";
                                        if (readFile.Peek() == 61)
                                        {
                                            podeImprimir = false;
                                            continue;
                                        }
                                        break;

                                }

                                if (podeImprimir && tipo.Equals("Relacional"))
                                {
                                    WriteOnText(writeFile, palavra, contador, tipo);
                                    palavra = "";
                                    tipo = "";
                                }

                            }

                            //Caso seja um inteiro ou um real
                            if (CheckIfNum(num_letra, palavra, tipo))
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
                                
                                if(CheckIfNum(readFile.Peek(), palavra, tipo))
                                {
                                    continue;
                                }
                                else
                                {
                                    if (!palavra.Equals(""))
                                    {
                                        WriteOnText(writeFile, palavra, contador, tipo);
                                        palavra = "";
                                        tipo = "";
                                    }
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

        static bool CheckIfNum(int num_letra, string palavra, string tipo)
        {
            bool numValido = (((num_letra >= 48 && num_letra <= 57) && (palavra.Equals("") || tipo.Equals("Inteiro") || tipo.Equals("Real"))) || (num_letra == 46 && tipo.Equals("Inteiro")));

            return numValido;
        }


        static void WriteOnText(StreamWriter writeFile, string palavra, int contador, string tipo)
        {
            Console.WriteLine(palavra + "       |   " + tipo  + "|   " + contador);
            writeFile.WriteLine(palavra + "       |   blabla  |   " + contador);
        }
    }
}
