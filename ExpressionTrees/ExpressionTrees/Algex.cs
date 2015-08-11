using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel;

namespace ExpressionTrees
{

    public class Algex
    {
        public static string[] Operators = {"(", ")", "+", "-", "*", "/"};

        /// <summary>
        /// Input string representing algorithmic expression.
        /// </summary>
        string _expression;
        /// <summary>
        /// Input string representing algorithmic expression.
        /// </summary>
        public string InnerExpression
        {
            get {return _expression;}
            set
            {
                if (!IsProperArithmeticExpression(value)) throw new ArgumentException();
                _expression = value;
                FindAllNeededVariables();
                toRPN(InnerExpression);
                IsSolved = false;
            }
        }
        /// <summary>
        /// Reverse polish notation of inner expression.
        /// </summary>
        List<string> RPN;

        /// <summary>
        /// Variable context.
        /// </summary>
        Dictionary<string, double> _varCtx;
        /// <summary>
        /// Variable context.
        /// </summary>
        public Dictionary<string, double> VariableContext
        {
            get { return _varCtx; }
            set
            {
                _varCtx = value;
            }
        }

        /// <summary>
        /// List of variable names this Algex instance is dependant on.
        /// </summary>
        List<string> _dependantOn;
        /// <summary>
        /// List of variable names this Algex instance is dependant on.
        /// </summary>
        public List<string> DependantOn
        {
            get { return _dependantOn; }
            private set
            {
                _dependantOn = value;
            }
        }

        /// <summary>
        /// Indicates wheather this Algex has been solved.
        /// </summary>
        bool _isSolved = false;
        /// <summary>
        /// Indicates wheather this Algex has been solved.
        /// </summary>
        public bool IsSolved
        {
            get { return _isSolved; }
            private set
            {
                _isSolved = value;
            }
        }

        /// <summary>
        /// Calculated value of inner expression.
        /// </summary>
        double _value = double.NaN;
        /// <summary>
        /// Calculated value of inner expression.
        /// </summary>
        public double Value
        {
            get { return _value; }
            private set
            {
                _value = value;
            }
        }

        public Algex(string expression, IDictionary<string, double> variables = null, bool immediateSolve = false)
        {
            if (variables != null) VariableContext = (Dictionary<string, double>)variables;
            InnerExpression = expression;
            if (immediateSolve || DependantOn.Count == 0) Solve();
        }

        /// <summary>
        /// Constructs DependantOn list
        /// </summary>
        private void FindAllNeededVariables()
        {
            var depOn = new List<string>();
            var rgx = new Regex(@"{\w*}");
            var matches = rgx.Matches(InnerExpression);
            foreach(Match match in matches)
            {
                var sbstr = match.Value.Substring(1, match.Value.Length - 2);
                if(!depOn.Contains(sbstr))
                depOn.Add(sbstr);
            }
            depOn.Distinct().ToList();
            depOn.Sort();
            DependantOn = depOn;
        }

        /// <summary>
        /// Checks if there are variables that are not in the variable context.
        /// </summary>
        public bool UnknownVariablePresent
        {
            get
            {
                foreach (string varId in DependantOn)
                {
                    if (VariableContext != null && !VariableContext.Keys.Contains(varId)) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Checks if input expression is properly formed algorithmic expression.
        /// </summary>
        /// <param name="expression">Input expression.</param>
        public static bool IsProperArithmeticExpression(string expression)
        {
            //TODO: Add culture specific decimal separator and enable decimal thingies to be accepted
            var rgx = new Regex(@"^-?((({\w+})|0|([1-9]\d*)))([+\-*/](-?(({\w+})|0|([1-9]\d*))))*$", RegexOptions.IgnorePatternWhitespace);
            var q = new Queue<string>();
            q.Enqueue(expression);
            while(q.Count!=0)
            {
                var exp = q.Dequeue();
                var left = exp.Contains("(");
                var right = exp.Contains(")");
                if (left ^ right) return false;
                if (exp.Contains("("))
                {
                    int open = exp.IndexOf('(');
                    int close = exp.IndexOf(')');
                    if (open > close) return false;
                    //sprawdzanie pomiędzy
                    int n = 1;
                    for (int i = open + 1; i < exp.Length; i++)
                    {
                        if (exp[i] == '(') n++;
                        if (exp[i] == ')')
                        {
                            n--;
                            if (n == 0)
                            {
                                var innerExp = exp.Substring(open + 1, i - open - 1);
                                var sb = new StringBuilder();
                                //if (open - 1 >= 0)
                                sb.Append(exp.Substring(0, open));
                                sb.Append("0");
                                if (i + 1 < exp.Length)
                                    sb.Append(exp.Substring(i+1));
                                q.Enqueue(innerExp);
                                q.Enqueue(sb.ToString());
                                break;
                            }
                        }
                    }
                    if (n > 0) return false; //nawiasy otwarte, nigdy nie zamknięte
                }
                else
                {
                    if(!rgx.IsMatch(exp)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Generates reverse polish notation of input expression.
        /// </summary>
        /// <param name="expression">Input expression.</param>
        private void toRPN(string expression)
        {
            var f = expression.Replace(" ", "");
            f = f.Replace("-(", "-1*(");
            f = f.Replace("+", " + ");
            f = f.Replace("*", " * ");
            f = f.Replace("/", " / ");
            f = f.Replace("-", " - ");
            f = f.Replace("(", " ( ");
            f = f.Replace(")", " ) ");
            var s = f.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < s.Count; i++)
            {
                if(s[i]=="-" && (i==0 || Operators.Contains(s[i-1])))
                {
                    s.RemoveAt(i);
                    s[i] = "-"+s[i];
                }
            }
            var stack = new Stack<string>();
            var output = new List<string>();
            for (int i = 0; i < s.Count; i++)
            {
                switch (s[i])
                {
                    case "(":
                        stack.Push("(");
                        break;
                    case ")":
                        while (stack.Peek() != "(")
                        {
                            output.Add(stack.Pop());
                        }
                        stack.Pop();
                        break;
                    case "+":
                        if (stack.Count != 0)
                        {
                            while (stack.Count > 0 && stack.Peek() != "(")
                            {
                                output.Add(stack.Pop());
                            }
                        }
                        stack.Push("+");
                        break;
                    case "-":
                        if (stack.Count != 0)
                        {
                            while (stack.Count > 0 && stack.Peek() != "(")
                            {
                                output.Add(stack.Pop());
                            }
                        }
                        stack.Push("-");
                        break;
                    case "*":
                        if (stack.Count != 0)
                        {
                            while (stack.Count > 0 && stack.Peek() != "(" && stack.Peek() != "+" && stack.Peek() != "-")
                            {
                                output.Add(stack.Pop());
                            }
                        }
                        stack.Push("*");
                        break;
                    case "/":
                        if (stack.Count != 0)
                        {
                            while (stack.Count > 0 && stack.Peek() != "(" && stack.Peek() != "+" && stack.Peek() != "-" && stack.Peek() != "*" && stack.Peek() != "/")
                            {
                                output.Add(stack.Pop());
                            }
                        }
                        stack.Push("/");
                        break;
                    default:
                        output.Add(s[i]);
                        break;
                }
            }
            output.AddRange(stack);
            RPN = output;
        }

        /// <summary>
        /// Solves inner expression based on inner variable context.
        /// </summary>
        /// <returns>True if succeded, false otherwise.</returns>
        public bool Solve()
        {
            return Solve(this.VariableContext);
        }

        /// <summary>
        /// Solves inner expression based on input variable context.
        /// </summary>
        /// <param name="variableCtx">Input variable context.</param>
        /// <returns>True if succeded, false otherwise.</returns>
        public bool Solve(IDictionary<string, double> variableCtx)
        {
            if (UnknownVariablePresent) return false;
            var s = new Stack<double>();
            for (int i = 0; i < RPN.Count; i++)
            {
                if (Operators.Contains(RPN[i]))
                {
                    var right = s.Pop();
                    var left = s.Pop();
                    double result;
                    switch (RPN[i])
                    {
                        case "+": result = left + right; break;
                        case "-": result = left - right; break;
                        case "*": result = left * right; break;
                        case "/": result = left / right; break;
                        default: throw new NotSupportedException();
                    }
                    s.Push(result);
                }
                else
                {
                    if (RPN[i].EndsWith("}") && RPN[i].StartsWith("{"))
                    {
                        var var = RPN[i].Substring(1, RPN[i].Length - 2);
                        s.Push(variableCtx[var]);
                    }
                    else
                    s.Push(double.Parse(RPN[i]));
                }
            }
            if (s.Count != 1) throw new ArgumentException();
            Value = s.Pop();
            IsSolved = true;
            return true;
        }

        public override string ToString()
        {
            return InnerExpression+" = "+Value;
        }

        /// <summary>
        /// Resets expression to unsolved state;
        /// </summary>
        public void Reset()
        {
            IsSolved = false;
            Value = double.NaN;
        }
    }
}
