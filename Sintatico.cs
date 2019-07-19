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
