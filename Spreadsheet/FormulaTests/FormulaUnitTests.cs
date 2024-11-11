// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> [Minh Quoc Vo & Ryan Boardman] </authors>
// <date> [9/19/2024] </date>

namespace CS3500.FormulaUnitTests;
using CS3500.Formula;


/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ when a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula(string.Empty);  // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut and paste error or a "I forgot to put something there" error).
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException
    /// when the expression doesn't contain any valid token but a space.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOnlySpace_Invalid()
    {
        _ = new Formula(" ");
    }
    /// <summary>
    /// This test use a single valid token to create a formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOneToken_Valid()
    {
        _ = new Formula("1");
    }


    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// This test use a single valid variable to create a formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOneVariableToken_Valid()
    {
        _ = new Formula("a1");
    }

    /// <summary>
    /// This test use a single valid decimal number to create a formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOneDecimalNumberToken_Valid()
    {
        _ = new Formula("2.0");
    }

    /// <summary>
    /// This test use a single valid exponential number with a upper case "E" to create a formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOneExponentialNumberUppperCase_Valid()
    {
        _ = new Formula("2E+10");
    }

    /// <summary>
    /// This test use a single valid exponential number with a lower case "e" to create a formula.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOneExponentialNumberLowerCase_Valid()
    {
        _ = new Formula("2e-10");
    }


    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException
    /// when only an invalid token "A" is used to create a formula.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOnlyOneLetterA_Invalid()
    {
        _ = new Formula("A");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException
    /// when only an invalid token "1a" is used to create a formula.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOneNumberFollowByALetter_Invalid()
    {
        _ = new Formula("1a");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException
    /// when only an invalid token "a2c" is used to create a formula.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOneLetterFollowByANumberFollowByALetter_Invalid()
    {
        _ = new Formula("a2c");
    }



    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException
    /// when an invalid operator ">" is used to create a formula.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOneSymbol_Invalid()
    {
        _ = new Formula(">");
    }

    // --- Tests for Closing Parentheses Rule

    [TestMethod]
    public void FormulaConstructor_TestOneOpeningAndClosingParatheses_Valid()
    {
        _ = new Formula("(1 + 1)");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when the expression contains
    /// more closing parenthesis than opening parenthesis.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOneOpeningAndTwoClosingParathese_Invalid()
    {
        _ = new Formula("(2-8))");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when the expression contains
    /// a single closing parenthesis.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOnlyOneClosingParenthesis_Invalid()
    {
        _ = new Formula(")");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when the expression contains
    /// a closing parenthesis before a open parenthesis.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOnlyOneCloseParenthesis_Invalid()
    {
        _ = new Formula(")1+3(");
    }

    // --- Tests for Balanced Parentheses Rule


    [TestMethod]
    public void FormulaConstructor_TestTwoOpeningAndClosingParatheses_Valid()
    {
        _ = new Formula("((1 + 1))");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when the expression contains
    /// one opening parenthesis with no closing parenthesis.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOneOpenParathesis_Invalid()
    {
        _ = new Formula("(3 *5 + 7");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when the expression contains
    /// two closing parentheses with no opening parentheses before it.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTwoCloseParentheses_Invalid()
    {
        _ = new Formula("3 *5 + 7))");
    }

    // --- Tests for First Token Rule

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenVariable_Valid()
    {
        _ = new Formula("a6 + 1");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the input expression start with an operator.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestStartWithAnOperator_Invalid()
    {
        _ = new Formula("+ 1 - 6");
    }

    /// <summary>
    ///   <para>
    ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the input expression start with an symbol(@).
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenSymbol_Invalid()
    {
        _ = new Formula("@+1");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the input expression start with an invalid variable "1a".
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenNumberFollowByLetter_Invalid()
    {
        _ = new Formula("1a+1");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the input expression start with an invalid variable "a".
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenOneLetter_Invalid()
    {
        _ = new Formula("a+1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenClosingParenthesis_Invalid()
    {
        _ = new Formula(") 1+1");
    }

    // --- Tests for  Last Token Rule ---

    /// <summary>
    /// This test create a Formula that end with valid variable token.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenVariable_Valid()
    {
        _ = new Formula("1+aa1");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenDecimalNumber_Valid()
    {
        _ = new Formula("1+1.0");
    }

    /// <summary>
    /// This test create a Formula that end with a number with an exponent where E is in upper case.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenExponentialNumberUpperCase_Valid()
    {
        _ = new Formula("1 + 2E-10");
    }

    /// <summary>
    /// This test create a Formula that end with a number with an exponent where E is in lower case.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenExponentialNumberLowerCase_Valid()
    {
        _ = new Formula("1 - 4e+1");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the input expression end with an invalid token "1a".
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenNumberFollowByLetter_Invalid()
    {
        _ = new Formula("1+1a");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the input expression end with an invalid token "a".
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOneLetter_Invalid()
    {
        _ = new Formula("1+a");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the input expression end with a sumbol "@".
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenSymbol_Invalid()
    {
        _ = new Formula("1+@");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the last token of the expression is an operator.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOperator_Invalid()
    {
        _ = new Formula(" 1 + 1 -");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOpenParenthesis_Invalid()
    {
        _ = new Formula(" 1 + 1 (");
    }


    // --- Tests for Parentheses/Operator Following Rule ---

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException
    /// when an invalid token "a" is used to create a formula.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowByClosingParenthesis_Invalid()
    {
        _ = new Formula("(1 + )");
    }

    /// <summary>
    /// This test try to create a Formula with an expression contains two
    /// consecutive open parentheses.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParathesisFollowingByParathesis_Valid()
    {
        _ = new Formula("7+ ((6*8) - 1)");
    }

    /// <summary>
    /// This test try to create a Formula with an expression contains a parenthesis
    /// following by a variable token.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOpeningParathesisFollowByVariable_Valid()
    {
        _ = new Formula("8 + (a1+3)");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// there is two consecutive operators in the expression.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowByOperator_Invalid()
    {
        _ = new Formula(" 9 ++ 9");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains an opening parenthesis following by a closing parenthesis.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOnlyTwoParatheses_Invalid()
    {
        _ = new Formula("()");

    }
    // --- Tests for Extra Following Rule ---

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a number token following by a number token.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowByNumber_Invalid()
    {
        _ = new Formula("1 + 9 9");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a number token following by a variable token.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowByVariable_Invalid()
    {
        _ = new Formula("1 + 9 Error3");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a variable token following by a variable token.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableFollowByVariable_Invalid()
    {
        _ = new Formula("1 + a1 b3");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a variable token following by a number token.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableFollowByNumber_Invalid()
    {
        _ = new Formula("1 + a1 3");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a variable token following by an opening parenthesis.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableFollowByOpenParenthesis_Invalid()
    {
        _ = new Formula("a1 (1+1)");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a number token following by a opening parenthesis.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowByOpenParenthesis_Invalid()
    {
        _ = new Formula("90 (1+1)");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a closing parenthesis following by a number token.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisFollowByNumber_Invalid()
    {
        _ = new Formula("7 + (1+1) 0");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a closing parenthesis following by a variable token.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisFollowByVariable_Invalid()
    {
        _ = new Formula("7 + (1+1) a1");
    }

    /// <summary>
    /// This test expected the Formula constructor to throw a FormulaFormatException when
    /// the expression contains a closing parenthesis following by an opening parenthesis.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisFollowByOpeningParenthesis_Invalid()
    {
        _ = new Formula("90 (1+1)(");
    }

    // --- Tests GetVariables ---

    /// <summary>
    /// This formula contains no variable so the set of variable from GetVariables should be empty
    /// </summary>
    [TestMethod]
    public void GetVariables_TestGetNoVariable_Valid()
    {
        Formula oneVariable = new Formula("4e10 + 100 - 90 - (105 - 6) + 7 + 1");
        ISet<String> setVariable = oneVariable.GetVariables();
        Assert.AreEqual(0, setVariable.Count);
    }

    /// <summary>
    /// This method contains a variable a1 which expected to A1 in the variable set
    /// </summary>
    [TestMethod]
    public void GetVariables_TestGetOneLowerCaseVariable_Valid()
    {
        Formula oneVariable = new Formula("a1");
        ISet<String> setVariable = oneVariable.GetVariables();
        Assert.AreEqual(1, setVariable.Count);
        Assert.AreEqual("A1", setVariable.First());
    }

    /// <summary>
    /// The normalization of the variable is tested. Every letter in the variable should be capitalized.
    /// </summary>
    [TestMethod]
    public void GetVariables_TestGetOneUpperCaseAndLowerCaseVariable_Valid()
    {
        Formula oneVariable = new Formula("sImON1 + 1");
        ISet<String> setVariable = oneVariable.GetVariables();
        Assert.AreEqual(1, setVariable.Count);
        Assert.AreEqual("SIMON1", setVariable.First());
    }

    [TestMethod]
    public void GetVariables_TestGetThreeVariables_Valid()
    {
        Formula oneVariable = new Formula("sImON1 / 4e-9 + abc8 * h9 - 1");
        ISet<String> setVariable = oneVariable.GetVariables();
        Assert.AreEqual(setVariable.Count, 3);
        Assert.AreEqual("SIMON1", setVariable.First());
        setVariable.Remove("SIMON1");
        Assert.AreEqual("ABC8", setVariable.First());
        setVariable.Remove("ABC8");
        Assert.AreEqual("H9", setVariable.First());
    }


    // --- Tests ToString ---

    /// <summary>
    /// This test ToString method of a singular token formula
    /// </summary>
    [TestMethod]
    public void ToString_TestOneTokenFormula_Valid()
    {
        Formula regularFormula = new Formula("8");
        Assert.AreEqual("8", regularFormula.ToString());
    }

    /// <summary>
    /// This test ToString method of a formula of number tokens 
    /// </summary>
    [TestMethod]
    public void ToString_TestNumberFormula_Valid()
    {
        Formula regularFormula = new Formula("( 6+ 7 -9*8   )");
        Assert.AreEqual("(6+7-9*8)", regularFormula.ToString());
    }

    /// <summary>
    /// This test ToString method of a formula of variable tokens
    /// </summary>
    [TestMethod]
    public void ToString_TestVariableFormula_Valid()
    {
        Formula regularFormula = new Formula("a1 + Ab2 - Avk99 * 8");
        Assert.AreEqual("A1+AB2-AVK99*8", regularFormula.ToString());
    }

    /// <summary>
    /// This test ToString method of a formula of expoential number tokens
    /// </summary>
    [TestMethod]
    public void ToString_TestExponentialNumberFormula_Valid()
    {
        Formula regularFormula = new Formula("4e2 + 3.5E-3 * 7e+5");
        Assert.AreEqual("400+0.0035*700000", regularFormula.ToString());
    }

    [TestMethod]
    public void ToString_TestMixFormula_Valid()
    {
        Formula regularFormula = new Formula("4e-2 + (xyz990/  (78) * 8e1 - 4.5)");
        Assert.AreEqual("0.04+(XYZ990/(78)*80-4.5)", regularFormula.ToString());
    }

    [TestMethod]
    public void Equals_TestTwoSimpleEqualFormula_Valid()
    {
        Formula formula1 = new Formula("1");
        Formula formula2 = new Formula("1");
        Assert.AreEqual(formula1, formula2);   
    }

    [TestMethod]
    public void Equals_TestTwoComplexEqualFormula_Valid()
    {
        Formula formula1 = new Formula("1-5*(9-9)/0+3e7-(abC999)");
        Formula formula2 = new Formula("1-5*(9-9)/0+3E7-(AbC999)");
        Assert.AreEqual(formula1, formula2);
    }

    [TestMethod]
    public void Equals_TestTwoFormulaNotEquals_Valid()
    {
        Formula formula1 = new Formula("1+(2)");
        Formula formula2 = new Formula("1+2");
        Assert.AreNotEqual(formula1, formula2);
    }

    [TestMethod]
    public void Equals_TestFormulaEqualsObjectOfOtherType_Valid()
    {
        Formula formula1 = new Formula("1");
        object intObject = new int();
        object stringObject = new string("hello");
        Assert.AreNotEqual(formula1, intObject);
        Assert.AreNotEqual(formula1, stringObject);
    }

    [TestMethod]
    public void EqualsEqulasOperator_TestTwoSimpleEqualFormula_Valid()
    {
        Formula formula1 = new Formula("1");
        Formula formula2 = new Formula("1");
        Assert.IsTrue(formula1 == formula2);
    }

    [TestMethod]
    public void EqualsEqulasOperator_TestTwoComplexEqualFormula_Valid()
    {
        Formula formula1 = new Formula("1-5*(9-9)/0+3e7-(abC999)");
        Formula formula2 = new Formula("1-5*(9-9)/0+3E7-(AbC999)");
        Assert.IsTrue(formula1 == formula2);
    }

    [TestMethod]
    public void NotEqualsOperator_TestTwoFormulaNotEquals_Valid()
    {
        Formula formula1 = new Formula("1");
        Formula formula2 = new Formula("2");
        Assert.IsTrue(formula1 != formula2);
    }

    [TestMethod]
    public void GetHashCode_TestGetHashCodeTwoEqualFormula_Valid()
    {
        Formula formula1 = new Formula("1");
        Formula formula2 = new Formula("1");
        Assert.IsTrue(formula1 == formula2);
        Assert.AreEqual(formula1.GetHashCode(), formula2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_TestGetHashCodeTwoNotEqualFormula_Valid()
    {
        Formula formula1 = new Formula("1");
        Formula formula2 = new Formula("2");
        Assert.IsTrue(formula1 != formula2);
        Assert.AreNotEqual(formula1.GetHashCode(), formula2.GetHashCode());
    }

    [TestMethod]
    public void Evalutate_TestEvaluateOneNumberTokenFormula_Valid()
    {
        Formula simpleFormula = new Formula("0");
        Assert.AreEqual(0.0, simpleFormula.Evaluate(s => 5));
    }


    /// <summary>
    /// This helper method is used to define variable tokens' value in the formula
    /// </summary>
    /// <param name="variableToken"> variable token that need to be checked </param>
    /// <returns> value of the variable </returns>
    /// <exception cref="ArgumentException"> throw expection if the value of the variable is undefined</exception>
    private static double SimpleLookUp(string variableToken)
    {
        if(variableToken == "A1")
        {
            return 3.0;
        } else if (variableToken == "B1")
        {
            return 5.0; 
        } else
        {
            throw new ArgumentException("Variable is undefined");
        }
    }

    [TestMethod]
    public void Evalutate_TestEvaluateOneVariableTokenFormula_Valid()
    {
        Formula simpleFormula = new Formula("A1");
        Assert.AreEqual(3.0, (double)simpleFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateOneUndefinedVariableFormula_Valid()
    {
        Formula simpleFormula = new Formula("C2");
        Assert.IsInstanceOfType<FormulaError>(simpleFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateOneExponentialTokenFormula_Valid()
    {
        Formula simpleFormula = new Formula("1e2");
        Assert.AreEqual(100.0, (double)simpleFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateAdditionFormula_Valid()
    {
        Formula simpleFormula = new Formula("A1 + 3.0");
        Assert.AreEqual(6.0, (double)simpleFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateSubtrationFormula_Valid()
    {
        Formula simpleFormula = new Formula("A1 - 1.0");
        Assert.AreEqual(2.0, (double)simpleFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateMultiplicationFormula_Valid()
    {
        Formula simpleFormula = new Formula("3.0 * A1");
        Assert.AreEqual(9.0, (double)simpleFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateDivisionFormula_Valid()
    {
        Formula simpleFormula = new Formula("5 / B1");
        Assert.AreEqual(1.0, (double)simpleFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateFormulaWithParenthesis_Valid()
    {
        Formula simpleFormula = new Formula("100 - (50 + 50)");
        Assert.AreEqual(0.0, (double)simpleFormula.Evaluate(SimpleLookUp));
    }


    [TestMethod]
    public void Evalutate_TestEvaluateSubtrationOfTwoDecimalNumberFormula_Valid()
    {
        Formula simpleFormula = new Formula("5.6 - 3.6");
        Assert.AreEqual(2.0, (double)simpleFormula.Evaluate(SimpleLookUp), 1e-9);
    }


    [TestMethod]
    public void Evalutate_TestEvaluateSimpleFormula_Valid()
    {
        Formula simpleFormula = new Formula("1+1-3*2+3/3");
        Assert.AreEqual(-3.0, (double)simpleFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateComplexFormula_Valid()
    {
        Formula complexFormula = new Formula("1e2 - ((B1 + 95/5 - 4) * (10 + (10))/4) + A1");
        Assert.AreEqual(3.0, (double)complexFormula.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateNumberDivisionByZeroFormula_Valid()
    {
        Formula simpleFormula = new Formula("1/0");
        Assert.IsInstanceOfType<FormulaError>(simpleFormula.Evaluate(SimpleLookUp));

        Formula simpleFormula2 = new Formula("1/(2-2)");
        Assert.IsInstanceOfType<FormulaError>(simpleFormula2.Evaluate(SimpleLookUp));
    }

    [TestMethod]
    public void Evalutate_TestEvaluateVariableDivisionByZeroFormula_Valid()
    {
        Formula simpleFormula = new Formula("1/A1");
        Assert.IsInstanceOfType<FormulaError>(simpleFormula.Evaluate(s => 0));
    }

    /////
    ///

    [TestMethod]
    public void TestSimpleAddition()
    {
        Formula f = new Formula("1+1");
        Assert.AreEqual(2.0, f.Evaluate(s => 0)); // No variables, so we pass a dummy lookup function
    }

    [TestMethod]
    public void TestSimpleSubtraction()
    {
        Formula f = new Formula("5-2");
        Assert.AreEqual(3.0, f.Evaluate(s => 0)); // No variables
    }

    [TestMethod]
    public void TestSimpleMultiplication()
    {
        Formula f = new Formula("3*4");
        Assert.AreEqual(12.0, f.Evaluate(s => 0)); // No variables
    }

    [TestMethod]
    public void TestSimpleDivision()
    {
        Formula f = new Formula("8/2");
        Assert.AreEqual(4.0, f.Evaluate(s => 0)); // No variables
    }

    [TestMethod]
    public void TestDivisionByZero()
    {
        Formula f = new Formula("1/0");
        Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError)); // Division by zero
    }

    [TestMethod]
    public void TestAdditionWithVariable()
    {
        Formula f = new Formula("A1+2");
        Assert.AreEqual(7.0, f.Evaluate(s => s == "A1" ? 5 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestMultiplicationWithVariable()
    {
        Formula f = new Formula("A1*3");
        Assert.AreEqual(15.0, f.Evaluate(s => s == "A1" ? 5 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestUndefinedVariable()
    {
        Formula f = new Formula("A1+1");
        Assert.IsInstanceOfType(f.Evaluate(s => throw new ArgumentException("Unknown variable")), typeof(FormulaError));
    }

    [TestMethod]
    public void TestParentheses()
    {
        Formula f = new Formula("(2+3)*5");
        Assert.AreEqual(25.0, f.Evaluate(s => 0)); // No variables
    }

    [TestMethod]
    public void TestComplexFormula()
    {
        Formula f = new Formula("(2+A1)*B1+3");
        Assert.AreEqual(23.0,
            f.Evaluate(s => s == "A1" ? 3 : s == "B1" ? 4 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestSubtractionWithVariable()
    {
        // Test subtraction with a variable using a lambda for lookup
        Formula f = new Formula("X1 - 2");
        Assert.AreEqual(3.0, f.Evaluate(s => s == "X1" ? 5 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestMultiplicationWithMultipleVariables()
    {
        // Test multiplication with multiple variables
        Formula f = new Formula("X1 * Y1");
        Assert.AreEqual(20.0,
            f.Evaluate(s => s == "X1" ? 4 : s == "Y1" ? 5 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestDivisionWithZeroVariable()
    {
        // Test division with variable, leading to division by zero
        Formula f = new Formula("10 / X1");
        Assert.IsInstanceOfType(f.Evaluate(s => s == "X1" ? 0 : throw new ArgumentException("Unknown variable")),
            typeof(FormulaError));
    }

    [TestMethod]
    public void TestComplexFormulaWithLambda()
    {
        // Test complex formula using a lambda function to handle multiple variables
        Formula f = new Formula("(X1 + Y1) * Z1");
        Assert.AreEqual(30.0,
            f.Evaluate(s =>
                s == "X1" ? 2 : s == "Y1" ? 4 : s == "Z1" ? 5 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestFormulaWithUndefinedVariable()
    {
        // Test formula with an undefined variable
        Formula f = new Formula("X1 + 5");
        Assert.IsInstanceOfType(f.Evaluate(s => throw new ArgumentException("Unknown variable")), typeof(FormulaError));
    }

    [TestMethod]
    public void TestEqualityOperatorSameFormula()
    {
        // Test equality operator for the same formula
        Formula f1 = new Formula("X1 + 5");
        Formula f2 = new Formula("X1 + 5");
        Assert.IsTrue(f1 == f2);
    }

    [TestMethod]
    public void TestInequalityOperatorDifferentFormula()
    {
        // Test inequality operator for different formulas
        Formula f1 = new Formula("X1 + 5");
        Formula f2 = new Formula("X1 * 5");
        Assert.IsTrue(f1 != f2);
    }

    [TestMethod]
    public void TestHashCodeForEqualFormulas()
    {
        // Test if the hash code is equal for the same formula
        Formula f1 = new Formula("X1 + 5");
        Formula f2 = new Formula("X1 + 5");
        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    [TestMethod]
    public void TestComplexFormulaNoVariables()
    {
        // Test a complex formula with no variables
        Formula f = new Formula("(2+3)*(6/2)-4");
        Assert.AreEqual(11.0, f.Evaluate(s => 0)); // No variables
    }

    [TestMethod]
    public void TestAdditionWithParentheses()
    {
        // Test addition inside parentheses
        Formula f = new Formula("(2+3)");
        Assert.AreEqual(5.0, f.Evaluate(s => 0)); // No variables
    }

    [TestMethod]
    public void TestOperatorPrecedenceWithoutParentheses()
    {
        // Test operator precedence without parentheses
        Formula f = new Formula("2 + 3 * 5");
        Assert.AreEqual(17.0, f.Evaluate(s => 0)); // No variables
    }

    [TestMethod]
    public void TestSimpleExpressionWithVariables()
    {
        // Test a simple expression with a variable
        Formula f = new Formula("X1 + 2");
        Assert.AreEqual(7.0, f.Evaluate(s => s == "X1" ? 5 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestComplexExpressionWithVariablesAndOperators()
    {
        // Test a more complex expression with multiple variables and operators
        Formula f = new Formula("(X1 + 3) * (Y1 - 2)");
        Assert.AreEqual(28.0, f.Evaluate(s => s == "X1" ? 4 : s == "Y1" ? 6 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestSimpleMultiplicationWithVariables()
    {
        // Test simple multiplication with variables
        Formula f = new Formula("X1 * 4");
        Assert.AreEqual(20.0, f.Evaluate(s => s == "X1" ? 5 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestParenthesesMultiplicationAndAddition()
    {
        // Test parentheses that affect multiplication and addition order
        Formula f = new Formula("(2 + 3) * 5");
        Assert.AreEqual(25.0, f.Evaluate(s => 0)); // No variables
    }

    [TestMethod]
    public void TestDivisionByZeroVariable()
    {
        // Test division by zero with a variable leading to an error
        Formula f = new Formula("10 / X1");
        Assert.IsInstanceOfType(f.Evaluate(s => s == "X1" ? 0 : throw new ArgumentException("Unknown variable")), typeof(FormulaError));
    }

    [TestMethod]
    public void TestHashCodeForDifferentFormulas()
    {
        // Test that different formulas have different hash codes
        Formula f1 = new Formula("X1 + 2");
        Formula f2 = new Formula("X1 * 2");
        Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    [TestMethod]
    public void TestFormulaWithMultipleParenthesesAndOperators()
    {
        // Test a formula with multiple parentheses and operators
        Formula f = new Formula("((X1 + 2) * (Y1 - 3)) / Z1");
        Assert.AreEqual(3.0, f.Evaluate(s => s == "X1" ? 4 : s == "Y1" ? 8 : s == "Z1" ? 10 : throw new ArgumentException("Unknown variable")));
    }

    [TestMethod]
    public void TestOperatorPrecedenceWithVariables()
    {
        // Test operator precedence with variables
        Formula f = new Formula("X1 + 3 * Y1");
        Assert.AreEqual(17.0, f.Evaluate(s => s == "X1" ? 2 : s == "Y1" ? 5 : throw new ArgumentException("Unknown variable")));
    }

}