using System;
using System.IO;
using System.Collections;

namespace Teste
{
    class Program
    {
        static void Main(string[] args)
        {
            string readPath = @"..\..\teste.txt";
            string writePath = @"..\..\writeTeste.txt";
            Hashtable dicionario = GetHashtable();

            // Como ler um arquivo linha por linha
            using (StreamWriter writeFile = new StreamWriter(writePath))
            {
                writeFile.WriteLine("TOKEN\t\t|\t\tTIPO\t\t\t|\t\tLINHA");
                Console.WriteLine("TOKEN\t\t|\t\tTIPO\t\t\t|\t\tLINHA");
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

                            //Caso seja um inteiro ou um real
                            if(CheckIfNum(num_letra, palavra, tipo))
                            {
                                char letra = (char)num_letra;
                                palavra += letra;

                                if(num_letra == 46)
                                    tipo = "Real";

                                if (tipo.Equals(""))
                                    tipo = "Inteiro";
                                
                                if(CheckIfNum(readFile.Peek(), palavra, tipo))
                                {
                                    continue;
                                }
                                else
                                {
                                    //Lembrar de tirar essa checagem
                                    if (!palavra.Equals(""))
                                    {
                                        WriteOnText(writeFile, palavra, tipo, contador);
                                        palavra = "";
                                        tipo = "";
                                    }
                                }
                            }

                            //Caso seja um Delimitador
                            if(palavra.Equals(""))
                            {
                                switch (num_letra)
                                {
                                    //";" (PONTO E VIRGULA)
                                    case 59:
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    //"." (PONTO)
                                    case 46:
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    //":" (DOIS PONTOS)
                                    case 58:
                                        palavra += (char)num_letra;
                                        if (CheckIfAssignment(readFile.Peek()))
                                        {
                                            tipo = "Atribuição";
                                            continue;
                                        }
                                        else{
                                            tipo = "Delimitador";
                                        }
                                        break;
                                    //"(" (ABRE PARENTESIS)
                                    case 40:
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    //"(" (FECHA PARENTESIS)
                                    case 41:
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    //"," (VIRGULA)
                                    case 44:
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                }

                                if (tipo.Equals("Delimitador"))
                                {
                                    WriteOnText(writeFile, palavra, tipo, contador);
                                    palavra = "";
                                    tipo = "";
                                    continue;
                                }
                            }

                            if(tipo.Equals("Atribuição") && CheckIfAssignment(num_letra))
                            {
                                palavra += (char)num_letra;
                                WriteOnText(writeFile, palavra, tipo, contador);
                                palavra = "";
                                tipo = "";
                            }

                            if(CheckIfId(num_letra, palavra, tipo))
                            {
                                palavra += (char)num_letra;

                                if(tipo.Equals(""))
                                    tipo = "Identificador";

                                if(CheckIfId(readFile.Peek(), palavra, tipo))
                                {
                                    continue;
                                }
                                else
                                {
                                    tipo = CheckIfDic(palavra, dicionario);
                                    WriteOnText(writeFile, palavra, tipo, contador);
                                    palavra = "";
                                    tipo = "";
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

        static bool CheckIfAssignment(int num_letra)
        {
            //Checar se o proximo caracter é "="
            bool valido_atr = (num_letra == 61);
            return valido_atr;
        }

        static bool CheckIfLetter(int num_letra)
        {
            bool letra_Valido = ((num_letra >= 65 && num_letra <= 90) || (num_letra >= 97 && num_letra <= 122));
            return letra_Valido;
        }

        static bool CheckIfId(int num_letra, string palavra, string tipo)
        {
            //Caso seja (([a..z] || [A..Z]) && primeira letra) 
            bool num_Valido = ((CheckIfLetter(num_letra) && palavra.Equals("")) || (tipo.Equals("Identificador") && (CheckIfLetter(num_letra) || (num_letra >= 48 && num_letra <= 57) || (num_letra == 95))));
            return num_Valido;
        }

        static string CheckIfDic(string palavra, Hashtable dicionario)
        {
            string tipo = "Identificador";

            if (dicionario.ContainsKey(palavra))
            {
                tipo = (string)dicionario[palavra];
            }

            return tipo;
        }

        static void WriteOnText(StreamWriter writeFile, string palavra, string tipo, int contador)
        {
            Console.WriteLine(palavra + "\t\t|\t\t" + tipo + "\t\t|\t\t" + contador);
            writeFile.WriteLine(palavra + "\t\t|\t\t" + tipo + "\t\t|\t\t" + contador);
        }

        static Hashtable GetHashtable()
        {
            // Cria a retorna um novo Hashtable.
            Hashtable hashtable = new Hashtable();

            hashtable.Add("or", "Aditivo");
            hashtable.Add("and", "Multiplicativo");
            hashtable.Add("program", "P. reservada");
            hashtable.Add("var", "P. reservada");
            hashtable.Add("integer", "P. reservada");
            hashtable.Add("real", "P. reservada");
            hashtable.Add("boolean", "P. reservada");
            hashtable.Add("procedure", "P. reservada");
            hashtable.Add("begin", "P. reservada");
            hashtable.Add("end", "P. reservada");
            hashtable.Add("if", "P. reservada");
            hashtable.Add("then", "P. reservada");
            hashtable.Add("else", "P. reservada");
            hashtable.Add("while", "P. reservada");
            hashtable.Add("do", "P. reservada");
            hashtable.Add("not", "P. reservada");

            return hashtable;
        }
    }
}
