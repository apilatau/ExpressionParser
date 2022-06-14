
namespace Expression.Parser
{
    /// <summary>
    /// A class of the string expression parsing
    /// </summary>
    public class Parser
    {
        public string InfixExpression { get; private set; }
        public string PostfixExpression { get; private set; }

        //	a List of the supported operators priorities
        private readonly Dictionary<char, int> _operationPriority = new()
        {
            { '(', 0 },
            { '+', 1 },
            { '-', 1 },
            { '*', 2 },
            { '/', 2 },
            { '~', 3 }  
        };

        public Parser(string expression)
        {
            InfixExpression = expression;
            PostfixExpression = ConvertToPostfix(InfixExpression + "\r");
        }
        /// <summary>
        /// Parsing whole and decimal numbers
        /// </summary>
        /// <param name="expression">string for parsing</param>
        /// <param name="position">a current position</param>
        /// <returns>a number like a string</returns>
        private string GetStringNumber(string expression, ref int position)
        {
            string result = "";

            for (; position < expression.Length; position++)
            {
                if (!Char.IsDigit(expression[position]) && expression[position] != '.' && expression[position] != ',')
                {
                    position--;
                    break;
                }
                result += expression[position];
            }
            return result;
        }
        /// <summary>
        /// Convert infix expression to postfix one
        /// </summary>
        /// <param name="infixExpr">an infix expression</param>
        /// <returns>a postfix expression as the result</returns>
        private string ConvertToPostfix(string infixExpr)
        {
            string postfixExpr = "";

            //	A stack with operators in an expression
            Stack<char> operatorsStack = new();

            for (int i = 0; i < infixExpr.Length; i++)
            {
                char c = infixExpr[i];

                //	if it is a number
                if (Char.IsDigit(c) || c == '.' || c == ',')
                {
                    postfixExpr += GetStringNumber(infixExpr, ref i) + " ";
                }
                //	or an opening bracket
                else if (c == '(')
                {
                    //to stack
                    operatorsStack.Push(c);
                }
                //	if an closing bracket
                else if (c == ')')
                {
                    //	Take every operator from stack to output string until it will be encountered the opening bracket
                    while (operatorsStack.Count > 0 && operatorsStack.Peek() != '(')
                    {
                        postfixExpr += operatorsStack.Pop();
                    }
                    //	delete an opening bracket from stack
                    operatorsStack.Pop();
                }
                //	is a symbol in legal operator list
                else if (_operationPriority.ContainsKey(c))
                {
                    char op = c;
                    if (op == '-' && (i > 1 && (Char.IsDigit(infixExpr[i + 1]) || infixExpr[i + 1] == '(')))
                    {
                        op = '~';
                    }
                    //	pop every operator taking into account their priorities
                    while (operatorsStack.Count > 0 && (_operationPriority[operatorsStack.Peek()] >= _operationPriority[op]))
                    {
                        postfixExpr += operatorsStack.Pop();
                    }
                    operatorsStack.Push(op);
                }
            }
            // to output string the rest of the operators
            foreach (char op in operatorsStack)
            {
                postfixExpr += op;
            }

            return postfixExpr;
        }

        /// <summary>
        /// Execute math expression
        /// </summary>
        /// <param name="op">Operand</param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        private double ExecuteExpression(char op, double first, double second) => op switch
        {
            '+' => first + second,                  
            '-' => first - second,                  
            '*' => first * second,                  
            '/' => first / second,                  
            '~' => first - second,
            _ => 0  //	default if nothing has been found
        };
        private int Sign (ref bool isInverted)
        {
            if (isInverted)
            {
                isInverted = false;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// To calculate infix string expression
        /// </summary>
        /// <returns>a double numbers result</returns>
        public double CalculateExpression()
        {
            bool isInverted = false;

            //	A stack for numbers storing
            Stack<double> locals = new();

            for (int i = 0; i < PostfixExpression.Length; i++)
            {
                char c = PostfixExpression[i];

                if (Char.IsDigit(c))
                {
                    string number = GetStringNumber(PostfixExpression, ref i);
                    locals.Push(Convert.ToDouble(number));
                }
                else if (_operationPriority.ContainsKey(c))
                {
                    if ((c == '~') && (i < PostfixExpression.Length - 1) && (_operationPriority.ContainsKey(PostfixExpression[i+1])))
                    {
                        isInverted = true & !isInverted;
                        continue;
                    }

                    double second = Sign(ref isInverted) * (locals.Count > 0 ? locals.Pop() : 0),
                    first = locals.Count > 0 ? locals.Pop() : 0;
                    locals.Push(ExecuteExpression(c, first, second));
                }
            }
            return locals.Pop();
        }
    }
}
