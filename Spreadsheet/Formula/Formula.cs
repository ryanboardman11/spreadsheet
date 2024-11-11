// <copyright file="Formula_PS2.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//   <para>
//     This code is provides to start your assignment.  It was written
//     by Profs Joe, Danny, and Jim.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with the other required information.
//     Author: Minh Quoc Vo & Ryan Boardman
//     Date: 9/19/2024

//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// </summary>


namespace CS3500.Formula;

using System.Text;
using System.Text.RegularExpressions;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    /// This private variable is the string version of the formula
    /// </summary>
    private StringBuilder fomulaString;

    /// <summary>
    /// This formula is a list of tokens inside the formula
    /// </summary>
    private List<string> tokens;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        List<string> tokens = GetTokens(formula);
        StringBuilder formulaString = new StringBuilder();
        string previousToken = new string("");
        int openPara = 0;
        int closePara = 0;
        string[] operators = {"+", "-", "*", "/" };
        if (tokens.Count == 0) //One Token Rule
        {
            throw new FormulaFormatException("The formula can not be empty");
        }

        string firstToken = tokens[0]; //First Token Rule
        if (firstToken.Equals("("))
        {
            previousToken = firstToken;
            openPara++;
        }
        else if (IsVar(firstToken))
        {
            firstToken = firstToken.ToUpper();
            tokens[0] = firstToken;
            previousToken = firstToken;
        }

        else if (double.TryParse(firstToken, out double numberToken))
        {
            previousToken = numberToken.ToString();
        }
        else
        {
            throw new FormulaFormatException(firstToken + " is not a valid first token");
        }
        formulaString.Append(previousToken);

        for (int i = 1; i < tokens.Count; i++)
        {
            if ((operators.Contains(previousToken)) || previousToken.Equals("(")) //Parenthesis / Operator Following Rule
            {
                if (tokens[i].Equals("("))
                {
                    openPara++;
                    previousToken = tokens[i];
                }
                else if (IsVar(tokens[i]))
                {
                    tokens[i] = tokens[i].ToUpper();
                    previousToken = tokens[i];
                }
                else if (double.TryParse(tokens[i], out double numberToken))
                {
                    previousToken = numberToken.ToString();
                }
                else
                {
                    throw new FormulaFormatException("Invalid token after an operator/parenthesis");
                }
                formulaString.Append(previousToken);
            }
            else if(IsVar(previousToken) || double.TryParse(previousToken, out _) || previousToken.Equals(")")) //Extra Following Rule
            {
                if (tokens[i].Equals(")")) //Closing Parentheses Rule
                {
                    closePara++;
                    if (closePara > openPara)
                    {
                        throw new FormulaFormatException("Number of closing parenthesis is greater than the number of opening parenthesis");
                    }
                }
                else if (!operators.Contains(tokens[i]))
                {
                    throw new FormulaFormatException("Invalid token");
                }
                previousToken = tokens[i];
                formulaString.Append(previousToken);
            }
        }
        if(closePara != openPara) //Balanced Parentheses Rule
        {
            throw new FormulaFormatException("Unbalanced pair of parenthesis");
        }
        else if (!(IsVar(previousToken) || double.TryParse(previousToken, out _) || previousToken.Equals(")"))) //Last Token Rule
        {
            throw new FormulaFormatException("Invalid last token");
        }
        this.fomulaString = formulaString; 
        this.tokens = tokens;

    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///   </remarks>
    ///   <para>
    ///     For example, if N is a method that converts all the letters in a string to upper case:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should enumerate "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables( )
    {
        HashSet<string> variables = new HashSet<string>();
        foreach(string token in this.tokens)
        {
            if (IsVar(token))
            {
                variables.Add(token);   
            }
        }
        return variables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f 
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized.  This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This code should execute in O(1) time.
    ///   <para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString( )
    {
        return this.fomulaString.ToString();
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar( string token )
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch( token, standaloneVarPattern );
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens( string formula )
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach ( string s in Regex.Split( formula, pattern, RegexOptions.IgnorePatternWhitespace ) )
        {
            if ( !Regex.IsMatch( s, @"^\s*$", RegexOptions.Singleline ) )
            {
                results.Add(s);
            }
        }

        return results;
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !(f1 == f2);
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference 
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.  
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (!(obj is Formula otherFomula) || obj is null)
        {
            return false;
        }
        return otherFomula.ToString().Equals(this.ToString());
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a 
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method will expect 
    ///     variable names to be normalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>
    public object Evaluate(Lookup lookup)
    {
        Stack<double> stackValue = new Stack<double>();
        Stack<String> stackOperator = new Stack<String>();
        foreach (string token in this.tokens )
        {
            if(double.TryParse(token, out double numberToken)){
                if (IsOntop(stackOperator, "*"))
                {
                    stackValue.Push(Calculate(stackValue.Pop(),numberToken, stackOperator.Pop()));
                }
                else if (IsOntop(stackOperator, "/"))
                {
                    if(numberToken == 0)
                    {
                        return new FormulaError("Division by Zero");
                    }
                    stackValue.Push(Calculate(stackValue.Pop(), numberToken, stackOperator.Pop()));
                }
                else
                {
                    stackValue.Push(numberToken);
                }
            }
            else if (IsVar(token))
            {
                try {
                    double variableValue = lookup(token);
                    if (IsOntop(stackOperator, "*"))
                    {
                        stackValue.Push(Calculate(stackValue.Pop(), variableValue, stackOperator.Pop()));
                    }
                    else if (IsOntop(stackOperator, "/"))
                    {
                        if (variableValue == 0)
                        {
                            return new FormulaError("Division by Zero");
                        }
                        stackValue.Push(Calculate(stackValue.Pop(), variableValue, stackOperator.Pop()));
                    }
                    else
                    {
                        stackValue.Push(variableValue);
                    }

                } catch (ArgumentException) 
                {
                    return new FormulaError( token + " is  undefined ");
                }
                
            }
            else if (token == "+" || token == "-")
            {
                if (IsOntop(stackOperator, "+") || IsOntop(stackOperator, "-"))
                {
                    double rightValue = stackValue.Pop();
                    double leftValue = stackValue.Pop();
                    stackValue.Push(Calculate(leftValue, rightValue, stackOperator.Pop()));
                }
                stackOperator.Push(token);
            }
            else if(token == "*" || token == "/")
            {
                stackOperator.Push(token);
            }
            else if (token == "(")
            {
                stackOperator.Push(token);
            }
            else // token == ")"
            {
                if (IsOntop(stackOperator, "+") || IsOntop(stackOperator, "-"))
                {
                    double rightValue = stackValue.Pop();
                    double leftValue = stackValue.Pop();
                    stackValue.Push(Calculate(leftValue, rightValue, stackOperator.Pop()));
                } 
                stackOperator.Pop();
                if(IsOntop(stackOperator, "*") || IsOntop(stackOperator, "/"))
                {
                    double rightValue = stackValue.Pop();
                    double leftValue = stackValue.Pop();
                    if(rightValue == 0)
                    {
                        return new FormulaError("Division by zero ");
                    }
                    stackValue.Push(Calculate(leftValue, rightValue, stackOperator.Pop()));
                }
            }

        }
        if (stackValue.Count == 1)
        {
            return stackValue.Pop();
        }
        else
        {
            double rightValue = stackValue.Pop();
            double leftValue = stackValue.Pop();
            return Calculate(leftValue, rightValue, stackOperator.Pop());
        } 
    }

    /// <summary>
    /// This helper method check the expected operator on top of the given stack
    /// </summary>
    /// <param name="thisStack"> The stack that to be checked </param>
    /// <param name="op"> Which operator is expected on top of the stack </param>
    /// <returns> return true if the expected operator is on top of the stack. If not return false</returns>
    private static Boolean IsOntop(Stack<string> thisStack, string op)
    {
        if(thisStack.Count > 0)
        {
            return thisStack.Peek() == op;
        }
        return false;
    }

    /// <summary>
    /// This helper method performs the calculation between two given number and an op erator
    /// </summary>
    /// <param name="left"> left number to be calculate </param>
    /// <param name="right"> right number to be calculate </param>
    /// <param name="op"> the operator for calculation </param>
    /// <returns> the result after calculation </returns>
    private static double Calculate(double left, double right, string op)
    {
        switch (op)
        {
            case "-":
                return left - right;
            case "*":
                return left * right;
            case "/":
                return left / right;
            default: // case "+"
                return left + right;

        }
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be extremely small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
       return this.ToString().GetHashCode();
    }

}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);



/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException( string message )
        : base( message )
    {
        // All this does is call the base constructor. No extra code needed.
    }
}
