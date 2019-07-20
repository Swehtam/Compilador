using System;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace Teste
{
    class Sintatico
    {
        static void Main(string[] args)
        {

            // Mudar o path do arquivo que será lido
            string readPath = @"..\..\teste.txt";
            using (StreamReader readFile = new StreamReader(readPath))
            {
                LexLine obj = new LexLine();
                obj.NextLine(readFile);

                Programa(obj);
            }
        }

        static void Programa(LexLine obj)
        {
            if(obj.token == "program")
            {
                obj.NextLine(); //Fazer ir para outra linha!!!
                if(obj.type == "Identificador")
                {
                    obj.NextLine();
                    if(obj.token == ";")
                    {
                        obj.NextLine();
                        DeclaraVariaveis(obj);

                    }
                }
            }
        }

        static void DeclaraVariaveis(LexLine obj)
        {

        }

        //A gramática chegou em um operador relacional
        static void Relacional(LexLine obj)
        {
            //checa se o token atual é, de fato, um op relacional
            switch (obj.token)
            {
                case "=":
                    return;
                case ">":
                    return;
                case "<":
                    return;
                case ">=":
                    return;
                case "<=":
                    return;
                case "<>":
                    return;
            }
        }

        //A gramática chegou em um operador aditivo
        static void Additive(LexLine obj)
        {
            //checa se o token atual é, de fato, um op aditivo
            switch (obj.token)
            {
                case "+":
                    return;
                case "-":
                    return;
                case "or":
                    return;               
            }
        }

        //A gramática chegou em um operador multiplicativo
        static void Multiplicative(LexLine obj)
        {
            //checa se o token atual é, de fato, um op multiplicativo
            switch (obj.token)
            {
                case "*":
                    return;
                case "/":
                    return;
                case "and":
                    return;
            }
        }

        //Checa se a gramática foi lida com sucesso ou não
        /******* LEMBRAR DE CONTINUAR DEPOIS ******/
        static void Sucess(bool succeded)
        {
            if (succeded)
                Console.WriteLine("Gramática lida com sucesso!");

        }
    }

    class LexLine
    {
        public string token = null;
        public string type = null;
        public int line = 0;

        public void UpdateObject(string token, string type, string line)
        {
            this.token = token.Trim('\t');
            this.type = type.Trim('\t');
            this.line = int.Parse(line.Trim('\t'));
        }

        public void NextLine(StreamReader readFile)
        {
            string line = readFile.ReadLine();
            string[] split_line = line.Split('|');
            UpdateObject(split_line[0], split_line[1], split_line[2]);
        }
    }
}
