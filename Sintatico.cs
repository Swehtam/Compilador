using System;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace Teste
{
    class Sintatico
    {
        //pilha de errors
        static Stack errorStack = new Stack();
        //pilha para salvar os ids antes de atribuir um tipo
        static Stack pilhaId = new Stack();
        //pilha com variaveis do escopo
        static SemStack pilhaEsc = new SemStack();
        //pilha para fazer a checagem de tipos
        static Stack pilhaTipos = new Stack();

        public void Program()
        {
            // Mudar o path do arquivo que será lido
            string readPath = @"..\..\writeTeste.txt";
            string writePath = @"..\..\errorTeste.txt";
            using (StreamReader readFile = new StreamReader(readPath))
            {
                LexLine obj = new LexLine();
                obj.OpenStreamReader(readFile);
                obj.NextLine();
                pilhaEsc.Add("$", "marcador");
                using (StreamWriter writeFile = new StreamWriter(writePath))
                {
                    if (obj.token.Equals("program"))
                    {
                        obj.NextLine();
                        if (obj.type == "Identificador")
                        {
                            pilhaEsc.Add(obj.token, "program");
                            obj.NextLine();
                            if (obj.token == ";")
                            {
                                obj.NextLine();
                                if (VariableDeclariation(obj))
                                {
                                    errorStack.Clear();
                                    if (SubprogramsDeclaration(obj))
                                    {
                                        errorStack.Clear();
                                        if (CompoundCommand(obj))
                                        {
                                            errorStack.Clear();
                                            if (obj.token.Equals("."))
                                            {
                                                Console.WriteLine("\nLexicamente correto.\n");
                                            }
                                            else
                                            {
                                                writeFile.WriteLine("Esperado o token '.' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Program");
                                                Console.Error.WriteLine("Esperado o token '.' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Program");
                                            }
                                        }
                                        else
                                        {

                                            PrintErrors(writeFile);
                                        }
                                    }
                                    else
                                    {
                                        PrintErrors(writeFile);

                                    }
                                }
                                else
                                {
                                    PrintErrors(writeFile);
                                }

                            }
                            else
                            {
                                Console.Error.WriteLine("Esperado o token ';' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Program");
                                writeFile.WriteLine("Esperado o token ';' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Program");
                            }
                        }
                        else
                        {
                            Console.Error.WriteLine("Esperado ID na linha: " + obj.line + ", mas recebeu: " + obj.type + ", metodo Program");
                            writeFile.WriteLine("Esperado ID na linha: " + obj.line + ", mas recebeu: " + obj.type + ", metodo Program");
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("Esperado o token 'program' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Program");
                        writeFile.WriteLine("Esperado o token 'program' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Program");
                    }
                    writeFile.Close();
                }
                obj.CloseStremReader();
                readFile.Close();
            }
        }

        static bool VariableDeclariation(LexLine obj)
        {
            bool success = true;
            if (obj.token.Equals("var"))
            {
                obj.NextLine();
                if (!VariableDeclariationList(obj))
                {
                    success = false;
                }
                else
                {
                    errorStack.Clear();
                }
            }
            return success;
        }

        static bool VariableDeclariationList(LexLine obj)
        {
            bool success = true;
            if (IdentifierList(obj))
            {
                errorStack.Clear();
                if (obj.token.Equals(":"))
                {
                    obj.NextLine();
                    if (Type(obj))
                    {
                        errorStack.Clear();
                        if (obj.token.Equals(";"))
                        {
                            obj.NextLine();
                            if (RecursiveVariableDeclariationList(obj))
                            {
                                errorStack.Clear();
                            }
                            else
                            {
                                success = false;
                            }
                        }
                        else
                        {
                            success = false;
                            errorStack.Push("Esperado o token ';' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo VariableDeclariationList");
                        }

                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Esperado o token ':' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo VariableDeclariationList");
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        static bool RecursiveVariableDeclariationList(LexLine obj)
        {
            bool success = true;

            if (IdentifierList(obj))
            {
                errorStack.Clear();
                if (obj.token.Equals(":"))
                {
                    obj.NextLine();
                    if (Type(obj))
                    {
                        errorStack.Clear();
                        if (obj.token.Equals(";"))
                        {
                            obj.NextLine();
                            if (RecursiveVariableDeclariationList(obj))
                            {
                                errorStack.Clear();
                            }
                            else
                            {
                                success = false;
                            }
                        }
                        else
                        {
                            success = false;
                            errorStack.Push("Esperado o token ';' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo RecursiveVariableDeclariationList");
                        }

                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Esperado o token ':' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo RecursiveVariableDeclariationList");
                }
            }

            return success;
        }

        static bool IdentifierList(LexLine obj)
        {
            bool success = true;
            if (obj.type.Equals("Identificador"))
            {
                //POSSIVELMENTE ESSA CHECAGEM TAMBÉM É DESNECESSARIA
                if(pilhaEsc.X() == 0)
                {
                    pilhaId.Push(new SemVar(obj.token, obj.line));
                }
                //POSIVELMENTE ESSA PARTE É DESNECESSARIA
                else
                {
                    string tipo = pilhaEsc.SearchStack(obj.token, "procedure", false);
                    if (tipo.Equals("Error"))
                    {
                        Console.WriteLine("Variavel '" + obj.token + "' não foi declarada anteriormente, linha: " + obj.line + ", metodo IdentifierList");
                    }
                }
                
                obj.NextLine();
                if (RecursiveIdentifierList(obj))
                {
                    errorStack.Clear();
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
                errorStack.Push("Esperado ID na linha: " + obj.line + ", mas recebeu: " + obj.type + ", metodo IdentifierList");
            }
            return success;
        }

        static bool RecursiveIdentifierList(LexLine obj)
        {
            bool success = true;
            if (obj.token.Equals(","))
            {
                obj.NextLine();
                //MESMA COISA DO METODO ANTERIOR
                if (obj.type.Equals("Identificador") )
                {
                    if (pilhaEsc.X() == 0)
                    {
                        pilhaId.Push(new SemVar(obj.token, obj.line));
                    }
                    else
                    {
                        string tipo = pilhaEsc.SearchStack(obj.token, "procedure", false);
                        if (tipo.Equals("Error"))
                        {
                            Console.WriteLine("Variavel '" + obj.token + "' não foi declarada anteriormente, linha: " + obj.line + ", metodo RecursiveIdentifierList");
                        }
                        else
                        {
                            //COLOCAR DENTRO DA PILHA DE TIPOS
                        }
                    }

                    obj.NextLine();
                    if (RecursiveIdentifierList(obj))
                    {
                        errorStack.Clear();
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Esperado ID na linha: " + obj.line + ", mas recebeu: " + obj.type + ", metodo RecursiveIdentifierList");
                }
            }

            return success;
        }

        static bool Type(LexLine obj)
        {
            bool success = true;
            switch (obj.token)
            {
                case "integer":
                    pilhaEsc.AddMultId(pilhaId, "inteiro");
                    obj.NextLine();
                    break;
                case "real":
                    pilhaEsc.AddMultId(pilhaId, "real");
                    obj.NextLine();
                    break;
                case "boolean":
                    pilhaEsc.AddMultId(pilhaId, "boolean");
                    obj.NextLine();
                    break;
                default:
                    success = false;
                    errorStack.Push("Esperado Inteiro/Real/Boolean na linha: " + obj.line + ", mas recebeu: " + obj.token + ", metodo Type");
                    break;
            }
            
            return success;
        }

        static bool SubprogramsDeclaration(LexLine obj)
        {
            bool success = true;
            if (RecursiveSubprogramsDeclaration(obj))
            {
               errorStack.Clear();
            }
            return success;
        }

        static bool RecursiveSubprogramsDeclaration(LexLine obj)
        {
            bool success = true;
            //Como o RecursiveParametersList pode ter a opção 'vazio' então não precisar colocar um else
            if (SubroutineDeclaration(obj))
            {
                errorStack.Clear();
                if (obj.token.Equals(";"))
                {
                    obj.NextLine();
                    if (RecursiveSubprogramsDeclaration(obj))
                    {
                        errorStack.Clear();
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Esperado ';' na linha: " + obj.line + ", mas recebeu: " + obj.token + ", metodo RecursiveSubprogramsDeclaration");
                }

            }
            return success;
        }

        static bool SubroutineDeclaration(LexLine obj)
        {
            bool success = true;
            if (obj.token.Equals("procedure"))
            {
                obj.NextLine();
                if (obj.type.Equals("Identificador"))
                {
                    if (!pilhaEsc.Add(obj.token, "procedure"))
                    {
                        Console.WriteLine("Procedimento '" + obj.token + "' ja foi declarada nesse mesmo escopo, linha: " + obj.line + ", metodo Factor2.0");
                    }
                    pilhaEsc.Add("$", "marcador");
                    obj.NextLine();
                    if (Arguments(obj))
                    {
                        errorStack.Clear();
                        if (obj.token.Equals(";"))
                        {
                            obj.NextLine();
                            if (VariableDeclariation(obj))
                            {
                                errorStack.Clear();
                                if (SubprogramsDeclaration(obj))
                                {
                                    errorStack.Clear();
                                    if (!CompoundCommand(obj))
                                    {
                                        success = false;
                                    }
                                    else
                                    {
                                        errorStack.Clear();
                                    }
                                }
                                else
                                {
                                    success = false;
                                }
                            }
                            else
                            {
                                success = false;
                            }
                        }
                        else
                        {
                            success = false;
                            errorStack.Push("Esperado ';' na linha: " + obj.line + ", mas recebeu: " + obj.token + ", metodo SubroutineDeclaration");
                        }
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Esperado ID na linha: " + obj.line + ", mas recebeu: " + obj.type + ", metodo SubroutineDeclaration");
                }
            }
            else
            {
                success = false;
                errorStack.Push("Esperado 'procedure' na linha: " + obj.line + ", mas recebeu: " + obj.token + ", metodo SubroutineDeclaration");
            }
            return success;
        }

        static bool Arguments(LexLine obj)
        {
            bool success = true;

            //Como o Arguments pode ter a opção 'vazio' então não precisar colocar um else
            if (obj.token.Equals("("))
            {
                obj.NextLine();
                if (ParametersList(obj))
                {
                    errorStack.Clear();
                    if (obj.token.Equals(")"))
                    {
                        obj.NextLine();
                    }
                    else
                    {
                        success = false;
                        errorStack.Push("Esperado ')' na linha: " + obj.line + ", mas recebeu: " + obj.token + ", metodo Arguments");
                    }
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }

        static bool ParametersList(LexLine obj)
        {
            bool success = true;
            if (IdentifierList(obj))
            {
                errorStack.Clear();
                if (obj.token.Equals(":"))
                {
                    obj.NextLine();
                    if (Type(obj))
                    {
                        errorStack.Clear();
                        if (RecursiveParametersList(obj))
                        {
                            errorStack.Clear();
                        }
                        else
                        {
                            success = false;
                        }
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Esperado o token ':' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo ParametersList");
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        static bool RecursiveParametersList(LexLine obj)
        {
            bool success = true;

            //Como o RecursiveParametersList pode ter a opção 'vazio' então não precisar colocar um else
            if (obj.token.Equals(";"))
            {
                obj.NextLine();
                if (IdentifierList(obj))
                {
                    errorStack.Clear();
                    if (obj.token.Equals(":"))
                    {
                        obj.NextLine();
                        if (Type(obj))
                        {
                            errorStack.Clear();
                            if (RecursiveParametersList(obj))
                            {
                                errorStack.Clear();
                            }
                            else
                            {
                                success = false;
                            }
                        }
                        else
                        {
                            success = false;
                        }
                    }
                    else
                    {
                        success = false;
                        errorStack.Push("Esperado o token ':' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo RecursiveParametersList");
                    }
                }
                else
                {
                    success = false;
                }
            }

            return success;
        }

        static bool CompoundCommand(LexLine obj)
        {
            bool success = true;
            if (obj.token.Equals("begin"))
            {
                pilhaEsc.XIncrement();
                obj.NextLine();
                if (OptionalCommands(obj))
                {
                    errorStack.Clear();
                    if (obj.token.Equals("end"))
                    {
                        pilhaEsc.XDecrement();
                        if(pilhaEsc.X() == 0)
                        {
                            pilhaEsc.CloseBlock();
                        }
                        obj.NextLine();
                    }
                    else
                    {
                        success = false;
                        errorStack.Push("Esperado o token 'end' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo CompoundCommand");
                    }
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
                errorStack.Push("Esperado o token 'begin' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo CompoundCommand");
            }
            return success;
        }

        static bool OptionalCommands(LexLine obj)
        {
            bool success = true;
            if (CommandList(obj))
            {
                errorStack.Clear();
                return true;
            }
            return success;
        }

        static bool CommandList(LexLine obj)
        {
            bool success = true;
            if (Command(obj))
            {
                errorStack.Clear();
                if (RecursiveCommandList(obj))
                {
                    errorStack.Clear();
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        static bool RecursiveCommandList(LexLine obj)
        {
            bool success = true;
            //Como o RecursiveCommandList pode ter a opção 'vazio' então não precisar colocar um else
            if (obj.token.Equals(";"))
            {
                obj.NextLine();
                if (Command(obj))
                {
                    errorStack.Clear();
                    if (RecursiveCommandList(obj))
                    {
                        errorStack.Clear();
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }

        static bool Command(LexLine obj)
        {
            bool success = true;
            if (Variable(obj))
            {
                errorStack.Clear();
                if (obj.token.Equals(":="))
                {
                    obj.NextLine();
                    if (!Expression(obj))
                    {
                        success = false;
                    }
                    else
                    {
                        //Essa variavel de tipo corresponde ao tipo resultante do metodo Expressão
                        string tipo = ((SemVar)pilhaTipos.Pop()).type;
                        //checagem com o tipo que foi empilhado pelo metodo Variavel
                        TypeCheck(tipo);
                        errorStack.Clear();
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Esperado o token ':=' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Command");
                }
            }
            else if (Procedure(obj))
            {
                errorStack.Clear();
                return true;
            }
            else if (CompoundCommand(obj))
            {
                errorStack.Clear();
                return true;
            }
            else if (obj.token.Equals("if"))
            {
                pilhaTipos.Push(new SemVar("if", "boolean", obj.line));
                ///LIMPANDO TODAS AS CHAMADAS DE METODOS ANTERIORES DENTRO DO METODO COMMAND
                errorStack.Clear();
                obj.NextLine();
                if (Expression(obj))
                {
                    //Essa variavel de tipo corresponde ao tipo resultante do metodo Expressão
                    string tipo = ((SemVar)pilhaTipos.Pop()).type;
                    //checagem com o tipo que foi empilhado pelo token "if"
                    TypeCheck(tipo);

                    errorStack.Clear();
                    if (obj.token.Equals("then"))
                    {
                        obj.NextLine();
                        if (Command(obj))
                        {
                            errorStack.Clear();
                            if (Else(obj))
                            {
                                errorStack.Clear();
                            }
                            else
                            {
                                success = false;
                            }
                        }
                        else
                        {
                            success = false;
                        }
                    }
                    else
                    {
                        success = false;
                        errorStack.Push("Esperado o token 'then' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Command");
                    }
                }
                else
                {
                    success = false;
                }
            }
            else if (obj.token.Equals("while"))
            {
                pilhaTipos.Push(new SemVar("while", "boolean", obj.line));
                ///LIMPANDO TODAS AS CHAMADAS DE METODOS ANTERIORES DENTRO DO METODO COMMAND
                errorStack.Clear();
                obj.NextLine();
                if (Expression(obj))
                {
                    //Essa variavel de tipo corresponde ao tipo resultante do metodo Expressão
                    string tipo = ((SemVar)pilhaTipos.Pop()).type;
                    //checagem com o tipo que foi empilhado pelo token "while"
                    TypeCheck(tipo);

                    errorStack.Clear();
                    if (obj.token.Equals("do"))
                    {
                        obj.NextLine();
                        if (!Command(obj))
                        {
                            success = false;
                        }
                        else
                        {
                            errorStack.Clear();
                        }
                    }
                    else
                    {
                        success = false;
                        errorStack.Push("Esperado o token 'do' na linha: " + obj.line + ", mas recebeu o token: " + obj.token + ", metodo Command");
                    }
                }
                else
                {
                    success = false;
                }
            }
            else if (obj.token.Equals("case"))
            {
                pilhaTipos.Push(new SemVar("case", "inteiro", obj.line));
                obj.NextLine();
                if (obj.type.Equals("Identificador"))
                {
                    if (pilhaEsc.X() == 0)
                    {
                        if (!pilhaEsc.Add(obj.token, obj.type))
                        {
                            Console.WriteLine("Variavel '" + obj.token + "' ja foi declarada nesse mesmo escopo, linha: " + obj.line + ", metodo Variable");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ENTROU AQUI NO PROCEDURE: " + obj.token);
                        string tipo = pilhaEsc.SearchStack(obj.token, "procedure", false);
                        if (tipo.Equals("Error"))
                        {
                            Console.WriteLine("Variavel '" + obj.token + "' não foi declarada anteriormente, linha: " + obj.line + ", metodo Variable");
                        }
                        else
                        {
                            Console.WriteLine("EMPILHANDO VARIAVEL: " + obj.token + " DO TIPO: " + tipo + " METODO VARIABLE");
                            pilhaTipos.Push(new SemVar(obj.token, tipo, obj.line));
                        }
                    }
                    //Essa variavel de tipo corresponde ao tipo resultante do metodo Expressão
                    string type = ((SemVar)pilhaTipos.Pop()).type;
                    //checagem com o tipo que foi empilhado pelo token "if"
                    TypeCheck(type);

                    obj.NextLine();
                    if (obj.token.Equals("of"))
                    {
                        obj.NextLine();
                        if (CaseCommandList(obj))
                        {
                            if (Else(obj))
                            {
                                if (obj.token.Equals("end"))
                                {
                                    obj.NextLine();
                                }
                                else
                                {
                                    success = false;
                                    Console.Error.WriteLine("Error no end do método Comando");
                                }
                            }
                            {
                                success = false;
                                Console.Error.WriteLine("Error no Else do método Comando");
                            }
                        }
                        else
                        {
                            success = false;
                            Console.Error.WriteLine("Lista de comandos de case não reconhecida no metodo Comando");
                        }
                    }
                    else
                    {
                        success = false;
                        Console.Error.WriteLine("Error no of do método Comando");
                    }
                }
                else
                {
                    success = false;
                    Console.Error.WriteLine("Error no identificador do método Comando");
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        static bool CaseCommandList(LexLine obj)
        {
            bool success = true;
            if (obj.type.Equals("Inteiro"))
            {
                obj.NextLine();
                if (obj.token.Equals(":"))
                {
                    obj.NextLine();
                    if (CommandList(obj))
                    {
                        if (!RecursiveCaseCommandList(obj))
                        {
                            success = false;
                            Console.Error.WriteLine("Erro na recursao do case, no metodo do case");

                        }
                    }
                    else
                    {
                        success = false;
                        Console.Error.WriteLine("Erro na lista de comandos no metodo do case");
                    }
                }
                else{
                    success = false;
                    Console.Error.WriteLine("Erro no : no metodo lista de comandos case");
                }
            }
            else
            {
                success = false;
                Console.Error.WriteLine("Erro no inteiro da lista de comandos do case");
            }
            return success;
        }

        static bool RecursiveCaseCommandList(LexLine obj)
        {
            bool success = true;
            if (obj.type.Equals("Inteiro"))
            {
                obj.NextLine();
                if (obj.token.Equals(":"))
                {
                    obj.NextLine();
                    if (CommandList(obj))
                    {
                        if (!RecursiveCaseCommandList(obj))
                        {
                            success = false;
                            Console.Error.WriteLine("Erro na recursao do case, no metodo recursivo do case");

                        }
                    }
                    else
                    {
                        success = false;
                        Console.Error.WriteLine("Erro na lista de comandos no metodo na recursao do case");
                    }
                }
                else
                {
                    success = false;
                    Console.Error.WriteLine("Erro no : no metodo lista de comandos case");
                }
            }
            return success;
        }

        static bool Else(LexLine obj)
        {
            bool success = true;

            //Como o Else pode ter a opção 'vazio' então não precisar colocar um else
            if (obj.token.Equals("else"))
            {
                obj.NextLine();
                if (!Command(obj))
                {
                    success = false;
                }
                else
                {
                    errorStack.Clear();
                }
            }
            return success;
        }

        static bool Variable(LexLine obj)
        {
            bool success = true;
            if (obj.type.Equals("Identificador"))
            {
                if (pilhaEsc.X() == 0)
                {
                    if (!pilhaEsc.Add(obj.token, obj.type))
                    {
                        Console.WriteLine("Variavel '" + obj.token + "' ja foi declarada nesse mesmo escopo, linha: " + obj.line + ", metodo Variable");
                    }
                }
                else
                {
                    Console.WriteLine("ENTROU AQUI NO PROCEDURE: " + obj.token);
                    string tipo = pilhaEsc.SearchStack(obj.token, "procedure", false);
                    if (tipo.Equals("Error"))
                    {
                        Console.WriteLine("Variavel '" + obj.token + "' não foi declarada anteriormente, linha: " + obj.line + ", metodo Variable");
                    }
                    else
                    {
                        Console.WriteLine("EMPILHANDO VARIAVEL: " + obj.token + " DO TIPO: " + tipo + " METODO VARIABLE");
                        pilhaTipos.Push(new SemVar(obj.token, tipo, obj.line));
                    }
                }
                obj.NextLine();
            }
            else
            {
                success = false;
                errorStack.Push("Esperado ID na linha: " + obj.line + ", mas recebeu: " + obj.type + ", metodo Variable");
            }
            return success;
        }

        static bool Procedure(LexLine obj)
        {
            bool success = true;
            if (obj.type.Equals("Identificador"))
            {
                if (pilhaEsc.X() == 0)
                {
                    if (!pilhaEsc.Add(obj.token, "procedure"))
                    {
                        Console.WriteLine("Procedimento '" + obj.token + "' ja foi declarada nesse mesmo escopo, linha: " + obj.line + ", metodo Procedure");
                    }
                }
                else
                {
                    string tipo = pilhaEsc.SearchStack(obj.token, "procedure", true);
                    if (tipo.Equals("Error"))
                    {
                        Console.WriteLine("Procedimento '" + obj.token + "' não foi declarada anteriormente, linha: " + obj.line + ", metodo Procedure");
                    }
                }

                obj.NextLine();
                if (obj.token.Equals("("))
                {
                    
                    obj.NextLine();
                    if (ExpressionList(obj))
                    {
                        errorStack.Clear();
                        if (obj.token.Equals(")"))
                        {
                            obj.NextLine();
                        }
                        else
                        {
                            success = false;
                            errorStack.Push("Esperado ')' na linha: " + obj.line + ", mas recebeu: " + obj.token + ", metodo Procedure");
                        }
                    }
                    else
                    {
                        success = false;
                    }
                }
            }
            else
            {
                success = false;
                errorStack.Push("Esperado ID na linha: " + obj.line + ", mas recebeu: " + obj.type);
            }
            return success;
        }

        static bool ExpressionList(LexLine obj)
        {
            bool success = true;
            if (Expression(obj))
            {
                errorStack.Clear();
                if (RecursiveExpressionList(obj))
                {
                    errorStack.Clear();
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        static bool RecursiveExpressionList(LexLine obj)
        {
            bool success = true;
            //Como o RecursiveExpressionList pode ter a opção 'vazio' então não precisar colocar um else
            if (obj.token.Equals(","))
            {
                //Consumi uma ',' então vou consumir o próximo
                obj.NextLine();
                if (Expression(obj))
                {
                    errorStack.Clear();
                    if (RecursiveExpressionList(obj))
                    {
                        errorStack.Clear();
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }
            }

            return success;
        }

        static bool Expression(LexLine obj)
        {
            bool success = true;
            if (SimpleExpression(obj))
            {
                errorStack.Clear();
                if (Relational(obj))
                {
                    errorStack.Clear();
                    if (!SimpleExpression(obj))
                    {
                        success = false;
                    }
                    else
                    {
                        RelaTypeCheck();
                        errorStack.Clear();
                    }
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        static bool SimpleExpression(LexLine obj)
        {
            bool success = true;
            if (Term(obj))
            {
                errorStack.Clear();
                if (RecursiveSimpleExpression(obj))
                {
                    errorStack.Clear();
                }
                else
                {
                    success = false;
                }
            }
            else if (Fundamental(obj))
            {
                errorStack.Clear();
                if (Term(obj))
                {
                    ArithTypeCheck();
                    errorStack.Clear();
                    if (RecursiveSimpleExpression(obj))
                    {
                        errorStack.Clear();
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }

            return success;
        }

        static bool RecursiveSimpleExpression(LexLine obj)
        {
            bool success = true;
            //Como o RecursiveSimpleExpression pode ter a opção 'vazio' então não precisar colocar um else
            if (Additive(obj))
            {
                //Esse tipo foi salvo dentro do metodo do operador aditivo
                string tipo = ((SemVar)pilhaTipos.Pop()).type;
                errorStack.Clear();
                if (Term(obj))
                {
                    //Se o operador aditivo selecionado for "or", então será feito a checagem lógica, caso não seja será feito a checagem aritimetica
                    if (tipo.Equals("inteiro"))
                    {
                        ArithTypeCheck();
                    }
                    else
                    {
                        LogTypeCheck();
                    }

                    errorStack.Clear();
                    if (RecursiveSimpleExpression(obj))
                    {
                        errorStack.Clear();
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }

        static bool Term(LexLine obj)
        {
            bool success = true;

            if (Factor(obj))
            {
                errorStack.Clear();
                if (RecursiveTerm(obj))
                {
                    errorStack.Clear();
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        static bool RecursiveTerm(LexLine obj)
        {
            bool success = true;
            //Como o RecursiveTerm pode ter a opção 'vazio' então não precisar colocar um else, caso não seja um operador multiplicativo
            if (Multiplicative(obj))
            {
                //Esse tipo foi salvo dentro do metodo do operador multiplicativo
                string tipo = ((SemVar)pilhaTipos.Pop()).type;
                errorStack.Clear();

                if (Factor(obj))
                {
                    //Se o operador multiplicativo selecionado for "and", então será feito a checagem lógica, caso não seja será feito a checagem aritimetica
                    if (tipo.Equals("inteiro"))
                    {
                        ArithTypeCheck();
                    }
                    else
                    {
                        LogTypeCheck();
                    }

                    errorStack.Clear();
                    if (RecursiveTerm(obj))
                    {
                        errorStack.Clear();
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    success = false;
                }
            }

            return success;
        }

        //A gramática chegou em um fator
        static bool Factor(LexLine obj)
        {
            bool success = true;

            if (obj.type.Equals("Identificador"))
            {
                if (pilhaEsc.X() == 0)
                {
                    if (!pilhaEsc.Add(obj.token, obj.type))
                    {
                        Console.WriteLine("Variavel '" + obj.token + "' ja foi declarada nesse mesmo escopo, linha: " + obj.line + ", metodo Factor");
                    }
                }
                else
                {
                    string tipo = pilhaEsc.SearchStack(obj.token, "procedure", false);
                    if (tipo.Equals("Error"))
                    {
                        Console.WriteLine("Variavel '" + obj.token + "' não foi declarada anteriormente, linha: " + obj.line + ", metodo Variable");
                    }
                    else
                    {
                        Console.WriteLine("EMPILHANDO VARIAVEL: " + obj.token + " DO TIPO: " + tipo + " METODO FACTOR");
                        pilhaTipos.Push(new SemVar(obj.token, tipo, obj.line));
                    }
                }
                //Consumiu o identificador, então será chamado o próximo token
                obj.NextLine();
                if (obj.token.Equals("("))
                {
                    //Cosumir o próximo
                    obj.NextLine();
                    if (ExpressionList(obj))
                    {
                        errorStack.Clear();
                        if (obj.token.Equals(")"))
                        {
                            obj.NextLine();
                        }
                        else
                        {
                            success = false;
                            errorStack.Push("Esperado ')' na linha: " + obj.line + ", mas recebeu: " + obj.token + ", metodo Factor");
                        }
                    }
                    else
                    {
                        success = false;
                    }

                }
            }
            else if (obj.type.Equals("Inteiro"))
            {
                pilhaTipos.Push(new SemVar(obj.token, "inteiro", obj.line));
                obj.NextLine();
                return true;
            }
            else if (obj.type.Equals("Real"))
            {
                pilhaTipos.Push(new SemVar(obj.token, "real", obj.line));
                obj.NextLine();
                return true;
            }
            else if (obj.token.Equals("true"))
            {
                pilhaTipos.Push(new SemVar(obj.token, "boolean", obj.line));
                obj.NextLine();
                return true;
            }
            else if (obj.token.Equals("false"))
            {
                pilhaTipos.Push(new SemVar(obj.token, "boolean", obj.line));
                obj.NextLine();
                return true;
            }
            else if (obj.token.Equals("("))
            {
                obj.NextLine();
                if (Expression(obj))
                {
                    errorStack.Clear();
                    if (obj.token.Equals(")"))
                    {
                        obj.NextLine();
                    }
                    else
                    {
                        success = false;
                        errorStack.Push("Esperado ')' na linha: " + obj.line + ", mas recebeu: " + obj.token);
                    }
                }
                else
                {
                    success = false;
                }
            }
            else if (obj.token.Equals("not"))
            {
                pilhaTipos.Push(new SemVar(obj.token, "boolean", obj.line));
                obj.NextLine();
                if (!Factor(obj))
                {
                    success = false;
                }
                else
                {
                    //Nesse ponto é uma operação logica, então para que dê certo o retorno dessa função tem q ser boolean
                    LogTypeCheck();
                    errorStack.Clear();
                }
            }
            else
            {
                success = false;
                errorStack.Push("Fator não reconhecido na linha: " + obj.line);
            }

            return success;

        }

        // A gramática chegou em um sinal
        static bool Fundamental(LexLine obj)
        {
            bool success = true;

            switch (obj.token)
            {
                case "+":
                    pilhaTipos.Push(new SemVar("+", "inteiro", obj.line));
                    obj.NextLine();
                    break;
                case "-":
                    pilhaTipos.Push(new SemVar("-", "inteiro", obj.line));
                    obj.NextLine();
                    break;
                default:
                    errorStack.Push("O token: " + obj.token + ", na linha: " + obj.line + ", não é um sinal válido, metodo Fundamenal");
                    success = false;
                    break;
            }
            return success;
        }

        //A gramática chegou em um operador relacional
        static bool Relational(LexLine obj)
        {
            bool success = true;

            //checa se o token atual é, de fato, um op relacional
            switch (obj.token)
            {
                case "=":
                    obj.NextLine();
                    break;
                case ">":
                    obj.NextLine();
                    break;
                case "<":
                    obj.NextLine();
                    break;
                case ">=":
                    obj.NextLine();
                    break;
                case "<=":
                    obj.NextLine();
                    break;
                case "<>":
                    obj.NextLine();
                    break;
                default:
                    errorStack.Push("O token: " + obj.token + ", na linha: " + obj.line + ", não é um operador relacional válido, metodo Relational");
                    success = false;
                    break;
            }

            return success;
        }

        //A gramática chegou em um operador aditivo
        static bool Additive(LexLine obj)
        {
            bool success = true;

            //checa se o token atual é, de fato, um op aditivo
            switch (obj.token)
            {
                case "+":
                    pilhaTipos.Push(new SemVar("+", "inteiro", obj.line));
                    obj.NextLine();
                    break;
                case "-":
                    pilhaTipos.Push(new SemVar("-", "inteiro", obj.line));
                    obj.NextLine();
                    break;
                case "or":
                    pilhaTipos.Push(new SemVar("or", "boolean", obj.line));
                    obj.NextLine();
                    break;
                default:
                    errorStack.Push("O token: " + obj.token + ", na linha: " + obj.line + ", não é um operador aditivo válido, , metodo Additive");
                    success = false;
                    break;
            }
            return success;
        }

        //A gramática chegou em um operador multiplicativo
        static bool Multiplicative(LexLine obj)
        {
            bool success = true;

            //checa se o token atual é, de fato, um op multiplicativo
            switch (obj.token)
            {
                case "*":
                    pilhaTipos.Push(new SemVar("*", "inteiro", obj.line));
                    obj.NextLine();
                    break;
                case "/":
                    pilhaTipos.Push(new SemVar("/", "inteiro", obj.line));
                    obj.NextLine();
                    break;
                case "and":
                    pilhaTipos.Push(new SemVar("and", "boolean", obj.line));
                    obj.NextLine();
                    break;
                default:
                    errorStack.Push("O token: " + obj.token + ", na linha: " + obj.line + ", não é um operador multiplicativo válido, metodo Multiplicative");
                    success = false;
                    break;
            }

            return success;
        }

        static void PrintErrors(StreamWriter writeFile)
        {
            while(errorStack.Count > 0)
            {
                string text = (String)errorStack.Pop();
                writeFile.WriteLine(text);
                Console.WriteLine(text);
            }
        }

        //METODO PARA FAZER A CHECAGEM DE OPERAÇÕES ARITMETICAS
        static void ArithTypeCheck()
        {
            Console.WriteLine("ENTROU NO ARITMETICO");
            SemVar topo = (SemVar)pilhaTipos.Pop();
            SemVar subtopo = (SemVar)pilhaTipos.Pop();
            Console.WriteLine("TOPO: " + topo.token + " tipo: " + topo.type);
            Console.WriteLine("SUBTOPO: " + subtopo.token + " tipo: " + subtopo.type);

            if (topo.type.Equals("inteiro") && subtopo.type.Equals("inteiro"))
            {
                pilhaTipos.Push(new SemVar("", "inteiro", subtopo.line));
            }
            else if (topo.type.Equals("inteiro") && subtopo.type.Equals("real"))
            {
                pilhaTipos.Push(new SemVar("", "real", subtopo.line));
            }
            else if (topo.type.Equals("real") && subtopo.type.Equals("inteiro"))
            {
                pilhaTipos.Push(new SemVar("", "real", subtopo.line));
            }
            else if (topo.type.Equals("real") && subtopo.type.Equals("real"))
            {
                pilhaTipos.Push(new SemVar("", "real", subtopo.line));
            }
            else
            {
                if(!topo.type.Equals("inteiro") && !topo.type.Equals("real"))
                {
                    Console.WriteLine("Esparado valor númerico, mas recebeu: " + topo.type + ", na liha: " + topo.line);
                }

                if (!subtopo.type.Equals("inteiro") && !subtopo.type.Equals("real"))
                {
                    Console.WriteLine("Esparado valor númerico, mas recebeu: '" + subtopo.type + ", na liha: " + subtopo.line);
                }
            }
        }

        //METODO PARA FAZER A CHECAGEM DE OPERAÇÕES RELACIONAIS
        static void RelaTypeCheck()
        {
            SemVar topo = (SemVar)pilhaTipos.Pop();
            SemVar subtopo = (SemVar)pilhaTipos.Pop();

            if (topo.type.Equals("inteiro") && subtopo.type.Equals("inteiro"))
            {
                pilhaTipos.Push(new SemVar("", "boolean", subtopo.line));
            }
            else if (topo.type.Equals("inteiro") && subtopo.type.Equals("real"))
            {
                pilhaTipos.Push(new SemVar("", "boolean", subtopo.line));
            }
            else if (topo.type.Equals("real") && subtopo.type.Equals("inteiro"))
            {
                pilhaTipos.Push(new SemVar("", "boolean", subtopo.line));
            }
            else if (topo.type.Equals("real") && subtopo.type.Equals("real"))
            {
                pilhaTipos.Push(new SemVar("", "boolean", subtopo.line));
            }
            else
            {
                if (!topo.type.Equals("inteiro") && !topo.type.Equals("real"))
                {
                    Console.WriteLine("Esparado valor númerico, mas recebeu: " + topo.type + ", na liha: " + topo.line);
                }

                if (!subtopo.type.Equals("inteiro") && !subtopo.type.Equals("real"))
                {
                    Console.WriteLine("Esparado valor númerico, mas recebeu: '" + subtopo.type + ", na liha: " + subtopo.line);
                }
            }
        }

        //METODO PARA FAZER A CHECAGEM DE OPERAÇÕES LÓGICAS
        static void LogTypeCheck()
        {
            SemVar topo = (SemVar)pilhaTipos.Pop();
            SemVar subtopo = (SemVar)pilhaTipos.Pop();

            if (topo.type.Equals("boolean") && subtopo.type.Equals("boolean"))
            {
                pilhaTipos.Push(new SemVar("", "boolean", subtopo.line));
            }
            else
            {
                if (!topo.type.Equals("boolean"))
                {
                    Console.WriteLine("Esparado valor booleano, mas recebeu: " + topo.type + ", na liha: " + topo.line);
                }

                if (!subtopo.type.Equals("boolean"))
                {
                    Console.WriteLine("Esparado valor booleano, mas recebeu: '" + subtopo.type + ", na liha: " + subtopo.line);
                }
            }
        }

        //METODO PARA FAZER A CHECAGEM DE TIPO RESULTADO COM VARIAVEL ATRIBUIDA
        static void TypeCheck(string type)
        {
            
            SemVar topo = (SemVar)pilhaTipos.Pop();
            
            if (!topo.type.Equals(type) && !(topo.type.Equals("real") && type.Equals("inteiro")))
            {
                Console.WriteLine("Esparado valor '" + topo.type + "' para o token:  '" + topo.token + "', mas recebeu: '" + type + ", na liha: " + topo.line);
            }
            else
            {
                Console.WriteLine("FEZ A CHECAGEM E DEU CERTO");
            }
        }
    }

    class LexLine
    {
        //Salva o readFile dentro do obj
        private StreamReader readFile = null;
        
        public string token = null;
        public string type = null;
        public int line = 0;

        public string stashedToken = null;
        public string stashedType = null;
        public int stashedLine = 0;

        public void OpenStreamReader(StreamReader readFile)
        {
            this.readFile = readFile;
        }

        public void CloseStremReader()
        {
            readFile.Close();
        }

        public void UpdateObject(string token, string type, string line)
        {
            this.token = token.Trim('\t');
            this.type = type.Trim('\t');
            this.line = int.Parse(line.Trim('\t'));
        }

        public void NextLine()
        {
            string line = readFile.ReadLine();
            string[] split_line = line.Split('|');
            UpdateObject(split_line[0], split_line[1], split_line[2]);
        }
        
        public void StashObj()
        {
            stashedLine = this.line;
            stashedType = this.type;
            stashedToken = this.token;
        }
    }

    class SemVar
    {
        public string token;
        public string type;
        public int line;

        public SemVar(string token, string type)
        {
            this.token = token;
            this.type = type;
        }
        
        public SemVar(string token, int line)
        {
            this.token = token;
            this.line = line;
        }

        public SemVar(string token, string type, int line)
        {
            this.token = token;
            this.type = type;
            this.line = line;
        }
    }

    class SemStack
    {
        ArrayList idStack = new ArrayList();
        ArrayList xStack = new ArrayList();
        int idIndex = -1;
        int xIndex = -1;

        public bool Add(string token, string type)
        {
            //tipo do token "$" é  "marcador"
            if (token.Equals("$"))
            {
                idStack.Add(new SemVar(token, type));
                idIndex++;
                xStack.Add(0);
                xIndex++;
                return true;
            }
            else
            {
                if (!SearchBlock(token, type))
                {
                    idStack.Add(new SemVar(token, type));
                    idIndex++;
                    return true;
                }
                else
                { 
                    return false;
                }
            }
        }

        public void CloseBlock()
        {
            while (!((SemVar)idStack[idIndex]).token.Equals("$"))
            {
                idStack.RemoveAt(idIndex);
                idIndex--;
            }
            //Para excluir "$" de dentro da pilha
            idStack.RemoveAt(idIndex);
            idIndex--;
            xStack.RemoveAt(xIndex);
            xIndex--;
        }

        public bool SearchBlock(string token, string type)
        {
            int auxIndex = idIndex;

            while (!((SemVar)idStack[auxIndex]).token.Equals("$"))
            {
                if (token.Equals(((SemVar)idStack[auxIndex]).token))
                {
                    if(type.Equals("procedure") && ((SemVar)idStack[auxIndex]).type.Equals("procedure"))
                    {
                        return true;
                    }
                    else if(type.Equals("procedure") && !((SemVar)idStack[auxIndex]).type.Equals("procedure"))
                    {
                        return false;
                    }else if((type.Equals("integer") || type.Equals("real") || type.Equals("boolean")) && ((SemVar)idStack[auxIndex]).type.Equals("procedure"))
                    {
                        return false;
                    }
                    else if((type.Equals("integer") || type.Equals("real") || type.Equals("boolean")) && !((SemVar)idStack[auxIndex]).type.Equals("procedure"))
                    {
                        return true;
                    }
                    else if(((SemVar)idStack[auxIndex]).type.Equals("program"))
                    {
                        return false;
                    }
                }
                else
                {
                    auxIndex--;
                }
            }

            return false;
        }

        public void AddMultId(Stack pilhaId, string type)
        {
            while(pilhaId.Count > 0)
            {
                SemVar aux = (SemVar)pilhaId.Pop();
                if (!Add(aux.token, type))
                {
                    Console.WriteLine("Variavel '" + aux.token + "' já foi declarada anteriormente, linha: " + aux.line);
                }
            }
        }

        public string SearchStack(string token, string type, bool foundable)
        {
            int auxIndex = idIndex;
            while(auxIndex != 0)
            {
                if (token.Equals(((SemVar)idStack[auxIndex]).token))
                {
                    if (foundable)
                    {
                        if (type.Equals(((SemVar)idStack[auxIndex]).type))
                        {
                            return ((SemVar)idStack[auxIndex]).type;
                        }
                        else
                        {
                            auxIndex--;
                        }
                    }
                    else
                    {
                        if (type.Equals(((SemVar)idStack[auxIndex]).type))
                        {
                            auxIndex--;
                        }
                        else
                        {
                            return ((SemVar)idStack[auxIndex]).type;
                        }
                    }
                }
                else
                {
                    auxIndex--;
                }
            }

            return "Error";
        }

        public void XIncrement()
        {
            xStack[xIndex] = ((int)xStack[xIndex]) + 1;
        }

        public void XDecrement()
        {
            xStack[xIndex] = ((int)xStack[xIndex]) - 1;
        }

        public int X()
        {
            return (int)xStack[xIndex];
        }
    }
}