using System;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace Teste
{
    class Sintatico
    {
        static Stack errorStack = new Stack();

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
                using(StreamWriter writeFile = new StreamWriter(writePath))
                {
                    if (obj.token.Equals("program"))
                    {
                        obj.NextLine();
                        if (obj.type == "Identificador")
                        {
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
                                                Console.WriteLine("\nYAAAAY DEU TUDO CERTO\n");
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
                                            Console.Error.WriteLine("Comandos Compostos não reconhecidos na linha: " + obj.line + ", metodo Program");
                                            writeFile.WriteLine("Comandos Compostos não reconhecidos na linha: " + obj.line + ", metodo Program");
                                        }
                                    }
                                    else
                                    {
                                        PrintErrors(writeFile);
                                        Console.Error.WriteLine("Declaração de Subprogramas não reconhecida na linha: " + obj.line + ", metodo Program");
                                        writeFile.WriteLine("Declaração de Subprogramas não reconhecida na linha: " + obj.line + ", metodo Program");

                                    }
                                }
                                else
                                {
                                    PrintErrors(writeFile);
                                    Console.Error.WriteLine("Declaração de Variáveis não reconhecida na linha: " + obj.line + ", metodo Program");
                                    writeFile.WriteLine("Declaração de Variáveis não reconhecida na linha: " + obj.line + ", metodo Program");
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
                    errorStack.Push("Lista de Declaração de Variáveis não reconhecida na linha: " + obj.line + ", metodo VariableDeclariation");
                }
            }
            return success;
        }

        static bool VariableDeclariationList(LexLine obj)
        {
            bool success = true;
            if (IdentifierList(obj))
            {
                if (obj.token.Equals(":"))
                {
                    obj.NextLine();
                    if (Type(obj))
                    {
                        if (obj.token.Equals(";"))
                        {
                            obj.NextLine();
                            if (!RecursiveVariableDeclariationList(obj))
                            {
                                success = false;
                                errorStack.Push("Lista de Declaração de Variáveis Recursiva não reconhecida na linha: " + obj.line + ", metodo VariableDeclariationList");
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
                        errorStack.Push("Tipo não reconhecido na linha: " + obj.line + ", metodo VariableDeclariationList");
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
                errorStack.Push("Lista de Identificadores não reconhecida na linha: " + obj.line + ", metodo VariableDeclariationList");
            }
            return success;
        }

        static bool RecursiveVariableDeclariationList(LexLine obj)
        {
            bool success = true;
            
            if (IdentifierList(obj))
            {
                if (obj.token.Equals(":"))
                {
                    obj.NextLine();
                    if (Type(obj))
                    {
                        if (obj.token.Equals(";"))
                        {
                            obj.NextLine();
                            if (!RecursiveVariableDeclariationList(obj))
                            {
                                success = false;
                                errorStack.Push("Lista de Declaração de Variáveis Recursiva não reconhecida na linha: " + obj.line + ", metodo RecursiveVariableDeclariationList");
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
                        errorStack.Push("Tipo não reconhecido na linha: " + obj.line + ", metodo RecursiveVariableDeclariationList");
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
                obj.NextLine();
                if (!RecursiveIdentifierList(obj))
                {
                    success = false;
                    errorStack.Push("Lista de Identificadores Recursiva não reconhecida na linha: " + obj.line + ", metodo IdentifierList");
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
                if (obj.type.Equals("Identificador"))
                {
                    obj.NextLine();
                    if (!RecursiveIdentifierList(obj))
                    {
                        success = false;
                        errorStack.Push("Lista de Identificadores Recursiva não reconhecida na linha: " + obj.line + ", metodo RecursiveIdentifierList");
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
                    obj.NextLine();
                    break;
                case "real":
                    obj.NextLine();
                    break;
                case "boolean":
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
            if (!RecursiveSubprogramsDeclaration(obj))
            {
                success = false;
                errorStack.Push("Declaração de Subprogramas Recursiva não reconhecida na linha: " + obj.line + ", metodo SubprogramsDeclaration");
            }
            return success;
        }

        static bool RecursiveSubprogramsDeclaration(LexLine obj)
        {
            bool success = true;
            //Como o RecursiveParametersList pode ter a opção 'vazio' então não precisar colocar um else
            if (SubroutineDeclaration(obj))
            {
                if (obj.token.Equals(";"))
                {
                    obj.NextLine();
                    if (!RecursiveSubprogramsDeclaration(obj))
                    {
                        success = false;
                        errorStack.Push("Declaração de Subprogramas Recursiva não reconhecida na linha: " + obj.line + ", metodo RecursiveSubprogramsDeclaration");
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
                    obj.NextLine();
                    if (Arguments(obj))
                    {
                        if (obj.token.Equals(";"))
                        {
                            obj.NextLine();
                            if (VariableDeclariation(obj))
                            {
                                if (SubprogramsDeclaration(obj))
                                {
                                    if (!CompoundCommand(obj))
                                    {
                                        success = false;
                                        errorStack.Push("Comandos Compostos não reconhecidos na linha: " + obj.line);
                                    }
                                }
                                else
                                {
                                    success = false;
                                    errorStack.Push("Declaração de Subprogramas não reconhecida na linha: " + obj.line + ", metodo SubroutineDeclaration");
                                }
                            }
                            else
                            {
                                success = false;
                                errorStack.Push("Declaração de Variáveis não reconhecida na linha: " + obj.line + ", metodo SubroutineDeclaration");
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
                        errorStack.Push("Argumentos não reconhecidos na linha: " + obj.line + ", metodo SubroutineDeclaration");
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
                    errorStack.Push("Lista de Parametros não reconhecida na linha: " + obj.line + ", metodo Arguments");
                }
            }
            return success;
        }

        static bool ParametersList(LexLine obj)
        {
            bool success = true;
            if (IdentifierList(obj))
            {
                if (obj.token.Equals(":"))
                {
                    obj.NextLine();
                    if (Type(obj))
                    {
                        if (!RecursiveParametersList(obj))
                        {
                            success = false;
                            errorStack.Push("Lista de Parametros Recursivo não reconhecida na linha: " + obj.line + ", metodo ParametersList");
                        }
                    }
                    else
                    {
                        success = false;
                        errorStack.Push("Tipo não reconhecido na linha: " + obj.line + ", metodo ParametersList");
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
                errorStack.Push("Lista de Identificadores não reconhecida na linha: " + obj.line + ", metodo ParametersList");
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
                    if (obj.token.Equals(":"))
                    {
                        obj.NextLine();
                        if (Type(obj))
                        {
                            if (!RecursiveParametersList(obj))
                            {
                                success = false;
                                errorStack.Push("Lista de Parametros Recursivo não reconhecida na linha: " + obj.line + ", metodo RecursiveParametersList");
                            }
                        }
                        else
                        {
                            success = false;
                            errorStack.Push("Tipo não reconhecido na linha: " + obj.line + ", metodo RecursiveParametersList");
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
                    errorStack.Push("Lista de Identificadores não reconhecida na linha: " + obj.line + ", metodo RecursiveParametersList");
                }
            }

            return success;
        }

        static bool CompoundCommand(LexLine obj)
        {
            bool success = true;
            if (obj.token.Equals("begin"))
            {
                obj.NextLine();
                if (OptionalCommands(obj))
                {
                    if (obj.token.Equals("end"))
                    {
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
                    errorStack.Push("Comandos Opcionais não reconhecidos na linha: " + obj.line + ", metodo CompoundCommand");
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
                return true;
            }
            return success;
        }

        static bool CommandList(LexLine obj)
        {
            bool success = true;
            if (Command(obj))
            {
                if (!RecursiveCommandList(obj))
                {
                    success = false;
                    errorStack.Push("Lista de Comandos Recursiva não reconhecida na linha: " + obj.line + ", metodo CommandList");
                }
            }
            else
            {
                success = false;
                errorStack.Push("Comando não reconhecido na linha: " + obj.line + ", metodo CommandList");
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
                    if (!RecursiveCommandList(obj))
                    {
                        success = false;
                        errorStack.Push("Lista de Comandos Recursiva não reconhecida na linha: " + obj.line + ", metodo RecursiveCommandList");
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Comando não reconhecido na linha: " + obj.line + ", metodo RecursiveCommandList");
                }
            }
            return success;
        }

        static bool Command(LexLine obj)
        {
            bool success = true;
            if (Variable(obj))
            {
                if (obj.token.Equals(":="))
                {
                    obj.NextLine();
                    if (!Expression(obj))
                    {
                        success = false;
                        errorStack.Push("Expressão não reconhecida na linha: " + obj.line + ", metodo Command");
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
                return true;
            }
            else if (CompoundCommand(obj))
            {
                return true;
            }
            else if (obj.token.Equals("if"))
            {
                obj.NextLine();
                if (Expression(obj))
                {
                    if (obj.token.Equals("then"))
                    {
                        obj.NextLine();
                        if (Command(obj))
                        {
                            if (!Else(obj))
                            {
                                success = false;
                                errorStack.Push("Else  não reconhecido na linha: " + obj.line + ", metodo Command");
                            }
                        }
                        else
                        {
                            success = false;
                            errorStack.Push("Comando não reconhecida na linha: " + obj.line + ", metodo Command");
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
                    errorStack.Push("Expressão não reconhecida na linha: " + obj.line + ", metodo Command");
                }
            }
            else if (obj.token.Equals("while"))
            {
                obj.NextLine();
                if (Expression(obj))
                {
                    if (obj.token.Equals("do"))
                    {
                        obj.NextLine();
                        if (!Command(obj))
                        {
                            success = false;
                            errorStack.Push("Comando não reconhecida na linha: " + obj.line + ", metodo Command");
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
                    errorStack.Push("Expressão não reconhecida na linha: " + obj.line + ", metodo Command");
                }
            }
            else if (obj.token.Equals("case"))
            {
                obj.NextLine();
                if (obj.type.Equals("Identificador"))
                {
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
                errorStack.Push("Comando não reconhecido na linha: " + obj.line + ", metodo Command");
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
                    errorStack.Push("Comando não reconhecido na linha: " + obj.line + ", metodo Else");
                }
            }
            return success;
        }

        static bool Variable(LexLine obj)
        {
            bool success = true;
            if (obj.type.Equals("Identificador"))
            {
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
                obj.NextLine();
                if (obj.token.Equals("("))
                {
                    obj.NextLine();
                    if (ExpressionList(obj))
                    {
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
                        errorStack.Push("Expressão não reconhecida na linha: " + obj.line + ", metodo Procedure");
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
                if (!RecursiveExpressionList(obj))
                {
                    success = false;
                    errorStack.Push("Lista de Expressões Recursiva não reconhecida na linha: " + obj.line + ", metodo ExpressionList");
                }
            }
            else
            {
                success = false;
                errorStack.Push("Expressão não reconhecida na linha: " + obj.line + ", metodo ExpressionList");
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
                    if (!RecursiveExpressionList(obj))
                    {
                        success = false;
                        errorStack.Push("Lista de Expressões Recursiva não reconhecida na linha: " + obj.line + ", metodo RecursiveExpressionList");
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Expressão não reconhecida na linha: " + obj.line + ", metodo RecursiveExpressionList");
                }
            }

            return success;
        }

        static bool Expression(LexLine obj)
        {
            bool success = true;
            if (SimpleExpression(obj))
            {
                if (Relational(obj))
                {
                    if (!SimpleExpression(obj))
                    {
                        success = false;
                        errorStack.Push("Expressão Simples não reconhecida na linha: " + obj.line + ", metodo Expression");
                    }
                }
            }
            else
            {
                success = false;
                errorStack.Push("Expressão não reconhecida na linha: " + obj.line + ", metodo Expression");
            }
            return success;
        }

        static bool SimpleExpression(LexLine obj)
        {
            bool success = true;
            if (Term(obj))
            {
                if (!RecursiveSimpleExpression(obj))
                {
                    success = false;
                    errorStack.Push("Expressão Simples Recursiva não reconhecida na linha: " + obj.line + ", metodo SimpleExpression");
                }
            }
            else if (Fundamental(obj))
            {
                if (Term(obj))
                {
                    if (!RecursiveSimpleExpression(obj))
                    {
                        success = false;
                        errorStack.Push("Expressão Simples Recursiva não reconhecida na linha: " + obj.line + ", metodo SimpleExpression");
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Termo não reconhecido na linha: " + obj.line + ", metodo SimpleExpression");
                }
            }
            else
            {
                success = false;
                errorStack.Push("Expressão Simples não reconhecida na linha: " + obj.line + ", metodo SimpleExpression");
            }

            return success;
        }

        static bool RecursiveSimpleExpression(LexLine obj)
        {
            bool success = true;
            //Como o RecursiveSimpleExpression pode ter a opção 'vazio' então não precisar colocar um else
            if (Additive(obj))
            {
                if (Term(obj))
                {
                    if (!RecursiveSimpleExpression(obj))
                    {
                        success = false;
                        errorStack.Push("Expressão Simples Recursiva não reconhecida na linha: " + obj.line + ", metodo RecursiveSimpleExpression");
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Termo não reconhecido na linha: " + obj.line + ", metodo RecursiveSimpleExpression");
                }
            }
            return success;
        }

        static bool Term(LexLine obj)
        {
            bool success = true;

            if (Factor(obj))
            {
                if (!RecursiveTerm(obj))
                {
                    success = false;
                    errorStack.Push("Termo Recursivo não reconhecido na linha: " + obj.line + ", metodo Term");
                }
            }
            else
            {
                success = false;
                errorStack.Push("Termo não reconhecido na linha: " + obj.line + ", metodo Term");
            }
            return success;
        }

        static bool RecursiveTerm(LexLine obj)
        {
            bool success = true;
            //Como o RecursiveTerm pode ter a opção 'vazio' então não precisar colocar um else, caso não seja um operador multiplicativo
            if (Multiplicative(obj))
            {
                if (Factor(obj))
                {
                    if (!RecursiveTerm(obj))
                    {
                        success = false;
                        errorStack.Push("Termo Recursivo não reconhecido na linha: " + obj.line + ", metodo RecursiveTerm");
                    }
                }
                else
                {
                    success = false;
                    errorStack.Push("Fator não reconhecido na linha: " + obj.line + ", metodo RecursiveTerm");
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
                //Consumiu o identificador, então será chamado o próximo token
                obj.NextLine();
                if (obj.token.Equals("("))
                {
                    //Cosumir o próximo
                    obj.NextLine();
                    if (ExpressionList(obj))
                    {
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
                        errorStack.Push("Lista de expressões não reconhecida na linha: " + obj.line + ", metodo Factor");
                    }

                }
            }
            else if (obj.type.Equals("Inteiro"))
            {
                obj.NextLine();
                return true;
            }
            else if (obj.type.Equals("Real"))
            {
                obj.NextLine();
                return true;
            }
            else if (obj.token.Equals("true"))
            {
                obj.NextLine();
                if (obj.type.Equals("Identificador"))
                {
                    obj.NextLine();
                    
                }
                else
                {
                    success = false;
                    errorStack.Push("Esperado identificador na linha: " + obj.line + ", mas recebeu: " + obj.type);
                }
            }
            else if (obj.token.Equals("false"))
            {
                obj.NextLine();
                return true;
            }
            else if (obj.token.Equals("("))
            {
                obj.NextLine();
                if (Expression(obj))
                {
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
                    errorStack.Push("Expressões não reconhecidas na linha: " + obj.line);
                }
            }
            else if (obj.token.Equals("not"))
            {
                obj.NextLine();
                if (!Factor(obj))
                {
                    success = false;
                    errorStack.Push("Fator não reconhecido na linha: " + obj.line);
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
                    obj.NextLine();
                    break;
                case "-":
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
                    obj.NextLine();
                    break;
                case "-":
                    obj.NextLine();
                    break;
                case "or":
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
                    obj.NextLine();
                    break;
                case "/":
                    obj.NextLine();
                    break;
                case "and":
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

    }

    class LexLine
    {
        //Salva o readFile dentro do obj
        private StreamReader readFile = null;
        
        public string token = null;
        public string type = null;
        public int line = 0;

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
        
    }
}