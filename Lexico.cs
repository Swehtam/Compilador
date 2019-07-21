using System;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace Teste
{
    class Lexico
    {
        static void Main(string[] args)
        {
            string readPath = @"..\..\teste.txt";
            string writePath = @"..\..\writeTeste.txt";
            Hashtable dicionario = GetHashtable();

            // Como ler um arquivo linha por linha
            using (StreamWriter writeFile = new StreamWriter(writePath))
            {
                using (StreamReader readFile = new StreamReader(readPath))
                {
                    int contador = 1;
                    bool comentario = false;
                    int coment_linha = 1;
                    string palavra = "";
                    string tipo = "";
                    
                    while (readFile.Peek() >= 0)
                    {
                        //Checa se o simbolo pertence a linguagem ou não
                        bool pertence = false;
                        int num_letra = readFile.Read();
                        //Quando pular linha somar contador "/r"
                        if(num_letra == 13)
                        {
                            contador++;
                            pertence = true;
                            continue;
                        }
                        //Caso seja "{", então abriu comentário
                        else if (num_letra == 123)
                        {
                            coment_linha = contador;
                            comentario = true;
                            pertence = true;
                            continue;
                        }
                        //Caso seja "}", então fechou comentário
                        else if (num_letra == 125)
                        {
                            comentario = false;
                            pertence = true;
                            continue;
                        }

                        if (!comentario)
                        {
                            //Ignorar caso seja um /n /t ou " "
                            if (num_letra == 32 || num_letra == 9 || num_letra == 10)
                            {
                                pertence = true;
                                continue;
                            }

                            //Caso seja "+" ou "-" 
                            if (palavra.Equals("") && ((num_letra == 43) || (num_letra == 45)))
                            {
                                pertence = true;
                                char caractere = (char)num_letra;
                                palavra += caractere;
                                tipo = "Aditivo";
                                if (!palavra.Equals(""))
                                {
                                    WriteOnText(writeFile, palavra, tipo, contador);
                                    palavra = "";
                                    tipo = "";
                                }
                            }

                            //Caso seja "*" ou "/"
                            if (palavra.Equals("") && ((num_letra == 42) || (num_letra == 47)))
                            {
                                pertence = true;
                                char caractere = (char)num_letra;
                                palavra += caractere;
                                tipo = "Multiplicativo";
                                if (!palavra.Equals(""))
                                {
                                    WriteOnText(writeFile, palavra, tipo, contador);
                                    palavra = "";
                                    tipo = "";
                                }
                            }

                            //Caso seja um inteiro ou um real
                            if (CheckIfNum(num_letra, palavra, tipo))
                            {
                                pertence = true;
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
                                    WriteOnText(writeFile, palavra, tipo, contador);
                                    palavra = "";
                                    tipo = ""; 
                                }
                            }

                            //Caso seja um Delimitador ou um Relacional
                            if(palavra.Equals("") || tipo.Equals("Relacional"))
                            {
                                bool podeImprimir = true;
                                switch (num_letra)
                                {
                                    // DELIMITADORES
                                    //";" (PONTO E VIRGULA)
                                    case 59:
                                        pertence = true;
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    //"." (PONTO)
                                    case 46:
                                        pertence = true;
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    //":" (DOIS PONTOS)
                                    case 58:
                                        pertence = true;
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
                                        pertence = true;
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    //"(" (FECHA PARENTESIS)
                                    case 41:
                                        pertence = true;
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    //"," (VIRGULA)
                                    case 44:
                                        pertence = true;
                                        palavra += (char)num_letra;
                                        tipo = "Delimitador";
                                        break;
                                    // RELACIONAIS
                                    //"=" (IGUAL)
                                    case 61:
                                        pertence = true;
                                        palavra += (char)num_letra;
                                        tipo = "Relacional";
                                        break;
                                    //"<" (MENOR)
                                    case 60:
                                        pertence = true;
                                        palavra += (char)num_letra;
                                        tipo = "Relacional";
                                        //"<=" (MENOR IGUAL) ou "<>" (DIFERENTE)
                                        if ((readFile.Peek() == 61) || (readFile.Peek() == 62))
                                        {
                                            podeImprimir = false;
                                            continue;
                                        }
                                        break;
                                    //">" (MAIOR)
                                    case 62:
                                        pertence = true;
                                        palavra += (char)num_letra;
                                        tipo = "Relacional";
                                        //">=" (MAIOR IGUAL)
                                        if (readFile.Peek() == 61)
                                        {
                                            podeImprimir = false;
                                            continue;
                                        }
                                        break;
                                }

                                if (tipo.Equals("Delimitador"))
                                {
                                    WriteOnText(writeFile, palavra, tipo, contador);
                                    palavra = "";
                                    tipo = "";
                                    continue;
                                }

                                if (podeImprimir && tipo.Equals("Relacional"))
                                {
                                    WriteOnText(writeFile, palavra, tipo, contador);
                                    palavra = "";
                                    tipo = "";
                                }
                            }

                            if(tipo.Equals("Atribuição") && CheckIfAssignment(num_letra))
                            {
                                pertence = true;
                                palavra += (char)num_letra;
                                WriteOnText(writeFile, palavra, tipo, contador);
                                palavra = "";
                                tipo = "";
                            }

                            if(CheckIfId(num_letra, palavra, tipo))
                            {                                
                                pertence = true;
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
                            if (!pertence)
                            {
                                tipo = "Nao pertencente";
                                palavra += (char)num_letra;
                                WriteOnText(writeFile, palavra, tipo, contador);
                                palavra = "";
                                tipo = "";
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

            Sintatico syntacticAnalyzer = new Sintatico();
            syntacticAnalyzer.Program();
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