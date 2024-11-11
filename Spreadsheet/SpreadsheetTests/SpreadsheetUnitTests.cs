// <authors> [Minh Quoc Vo & Ryan Boardman] </authors>
// <date> [10/18/2024] </date>

namespace SpreadsheetTests;
using CS3500.Formula;
using CS3500.Spreadsheet;
using System.Security.Cryptography;

[TestClass]
public class SpreadsheetUnitTests
{
    // Tests for constructor

    [TestMethod]
    public void SpreadsheetConstructor_CreateNewSpreadsheet_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
    }

    // Tests for GetNamesOfAllNonemptyCells

    [TestMethod]
    public void SpreadsheetGetNamesOfAllNonemptyCells_getEmptyNames_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        HashSet<String> emptySetOfNames = (HashSet<String>)sheet.GetNamesOfAllNonemptyCells();
        Assert.AreEqual(0, emptySetOfNames.Count);
    }

    [TestMethod]
    public void SpreadsheetGetNamesOfAllNonemptyCells_get2Names_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "Hello");
        sheet.SetContentsOfCell("A3", "3.0");

        HashSet<String> nonEmptyNames = (HashSet<String>)sheet.GetNamesOfAllNonemptyCells();
        Assert.AreEqual(2, nonEmptyNames.Count);
        Assert.IsTrue(nonEmptyNames.Contains("A1"));
        Assert.IsTrue(nonEmptyNames.Contains("A3"));
    }

    // Tests for SetContentsOfCell

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_GetCellStringContent_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "Hello");

        Assert.AreEqual("Hello",(string) sheet.GetCellContents("A1"));
    }


    [TestMethod]
    public void SpreadsheetSetContentsOfCell_GetCellDoubleContent_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A3", "3.0");

        Assert.AreEqual(3.0,(double) sheet.GetCellContents("A3"));
    }


    [TestMethod]
    public void SpreadsheetSetContentsOfCell_GetCellFormulaContent_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "=1+1");

        Assert.AreEqual(new Formula("1+1"), sheet.GetCellContents("A1"));
    }

    [ExpectedException(typeof(InvalidNameException))]
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_GetInvalidCellName_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "Hello");
        sheet.GetCellContents("1A");
    }

    
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_GetCellContentEmptySpreadsheet_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        Assert.AreEqual(string.Empty, sheet.GetCellContents("A1"));
    }

    // Tests for SetCellContents

    [ExpectedException(typeof(InvalidNameException))]
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_SetInvalidCellNameStringContent_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("1aaaa", "Hello");
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_SetEmptyContent_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("a1", "");
    }

    [ExpectedException(typeof(InvalidNameException))]
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_SetInvalidCellNameDoubleContent_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("Abc", "3.0");
    }

    [ExpectedException(typeof(InvalidNameException))]
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_SetInvalidCellNameFormulaContent_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("a1a", "=1+1");
    }


    [TestMethod]
    public void SpreadsheetSetContentsOfCell_SetFormulaContentToAnExistCellContent_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "=B1+1");
        sheet.SetContentsOfCell("A1", "=C1+1");

        Assert.AreEqual(new Formula("C1+1"), sheet.GetCellContents("A1"));
    }


    [ExpectedException(typeof(CircularException))]
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_SetFormulaContentCreateDirectCirculation_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "=A1+1");
    }

    [ExpectedException(typeof(CircularException))]
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_SetFormulaContentCreateIndirectCirculation_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "=B1+1");
        sheet.SetContentsOfCell("B1", "=C1+1");
        sheet.SetContentsOfCell("C1", "=A1+1");
    }

    /// <summary>
    /// This test make sure there is no change to the previous state of the cell if there is a circular exception
    /// In this case cell B1 should keep its previous content
    /// </summary>
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_NoChangeIsMadeIfThereIsACirculation_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "=B1+1");
        sheet.SetContentsOfCell("B1", "=C1+1");
        Assert.AreEqual(new Formula("C1+1"), sheet.GetCellContents("B1"));
        try
        {
            sheet.SetContentsOfCell("B1", "=A1+1");
        }
        catch (CircularException) { }
        finally
        {
            Assert.AreEqual(new Formula("C1+1"), sheet.GetCellContents("B1"));
        }
    }


    /// <summary>
    /// This test make sure there is no change to the previous state of the cell if there is a circular exception
    /// In this case cell C1 should not exist 
    /// </summary>
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_RemoveInvalidSetCell_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A1", "=B1+1");
        sheet.SetContentsOfCell("B1", "=C1+1");
        Assert.AreEqual(2, sheet.GetNamesOfAllNonemptyCells().Count);
        try
        {
            sheet.SetContentsOfCell("C1", "=A1+1");
        }
        catch (CircularException) { }
        finally
        {
            Assert.AreEqual(2, sheet.GetNamesOfAllNonemptyCells().Count);
            Assert.IsTrue(sheet.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(sheet.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.IsFalse(sheet.GetNamesOfAllNonemptyCells().Contains("C1"));
        }
    }

    /// <summary>
    /// Set cell A1 to formula involve B1 although B1 is empty.
    /// </summary>
    [TestMethod]
    public void SpreadsheetSetContentsOfCell_SetInvalidFormula_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "=B1 + 1");
    }


    //[TestMethod]
    //public void SpreadsheetSetContentsOfCell_SetNullContent_Valid()
    //{
    //    Spreadsheet sp = new Spreadsheet();
    //    sp.SetContentsOfCell("A1", "=B1 + 1");
    //    Assert.AreEqual(string.Empty, sp.GetCellValue("A1"));
    //}

    // PS6 tests

    //Test for GetCellValue

    [TestMethod]
    public void SpreadsheetGetCellValue_GetStringValue_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "Hello");
        Assert.AreEqual("Hello", sp.GetCellValue("A1"));
    }

    [TestMethod]
    public void SpreadsheetGetCellValue_GetDoubleValue_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("B1", "7.0");
        Assert.AreEqual(7.0, sp.GetCellValue("B1"));
    }

    [TestMethod]
    public void SpreadsheetGetCellValue_GetFormulaValue_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "=1+9");
        Assert.AreEqual(10.0, sp.GetCellValue("A1"));
    }

    [TestMethod]
    public void SpreadsheetGetCellValue_GetComplexFormulaValue_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "2.0");
        sp.SetContentsOfCell("B1", "=A1+A1*2");
        sp.SetContentsOfCell("C1", "= A1 + B1 - 2*A1 + 6");
        Assert.AreEqual(10.0, sp.GetCellValue("C1"));
    }

    [ExpectedException(typeof(InvalidNameException))]
    [TestMethod]
    public void SpreadsheetGetCellValue_TestThrowInvalidNameException_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.GetCellValue("1A");
    }

    [TestMethod]
    public void SpreadsheetGetCellValue_TestGetValueEmptyCell_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        Assert.AreEqual("", sp.GetCellValue("C1"));
    }

    /// <summary>
    /// This test expected value of A1 to be FormulaError since B1 is empty
    /// </summary>
    [TestMethod]
    public void SpreadsheetGetCellValue_TestGetValueOfACellDependOnAnotherEmptyCell_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "=B1 + 1");
        Assert.IsInstanceOfType<FormulaError>(sp.GetCellValue("A1"));
    }

    [TestMethod]
    public void SpreadsheetGetCellValue_GetValueOfCellDependOnAnotherStringCell_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "HEllo");
        sp.SetContentsOfCell("B1", "=A1 + 1");
        Assert.IsInstanceOfType<FormulaError>(sp.GetCellValue("B1"));
    }
    // Test for get direct cell value

    [TestMethod]
    public void SpreadsheetGetDirectCellValue_GetStringValue_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "Hello");
        Assert.AreEqual("Hello", sp["A1"]);
    }

    [TestMethod]
    public void SpreadsheetGetDirectCellValue_GetDoubleValue_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("B1", "7.0");
        Assert.AreEqual(7.0, sp["B1"]);
    }

    [TestMethod]
    public void SpreadsheetGetDirectCellValue_GetFormulaValue_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "=1+9");
        Assert.AreEqual(10.0, sp["A1"]);
    }

    [TestMethod]
    public void SpreadsheetGetDirectCellValue_GetComplexFormulaValue_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "2.0");
        sp.SetContentsOfCell("B1", "=A1+A1*2");
        sp.SetContentsOfCell("C1", "= A1 + B1 - 2*A1 + 6");
        Assert.AreEqual(10.0, sp["C1"]);
    }

    [ExpectedException(typeof(InvalidNameException))]
    [TestMethod]
    public void SpreadsheetGetDirectCellValue_TestThrowInvalidNameException_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        var temp = sp["1A"];
    }

    [TestMethod]
    public void SpreadsheetGetDirectCellValue_TestGetValueEmptyCell_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        Assert.AreEqual("", sp["C1"]);
    }

    /// <summary>
    /// This test expected value of A1 to be FormulaError since B1 is empty
    /// </summary>
    [TestMethod]
    public void SpreadsheetGetDirectCellValue_GetValueFormulaError_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "=B1 + 1");
        Assert.IsInstanceOfType<FormulaError>(sp["A1"]);
    }

    [TestMethod]
    public void SpreadsheetGetDirectCellValue_GetValueOfCellDependOnAnotherStringCell_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "HEllo");
        sp.SetContentsOfCell("B1", "=A1 + 1");
        Assert.IsInstanceOfType<FormulaError>(sp["B1"]);
    }

    //Tests for Save

    [TestMethod]
    public void SpreadsheetSave_TestSaveNullSpreadSheet_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.Save("temp.txt");
        Assert.IsTrue(File.ReadAllText("temp.txt").Contains(" \"Cells\": {}"));
    }

    [TestMethod]
    public void SpreadsheetSave_TestSimpleSpreadSheet_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "Hello");
        sp.SetContentsOfCell("B1", "3");
        sp.SetContentsOfCell("C1", "=B1 + 9");
        sp.Save("SimpleSpreadsheet.txt");

    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SpreadsheetSave_TestSaveInNonExistPath_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.Save("/some/nonsense/path.txt");
    }

    // Test for file constructor
    [TestMethod]
    public void SpreadsheetFileConstructor_TestConstructSimpleSpreadsheet_Valid()
    {
        Spreadsheet sp = new Spreadsheet();
        sp.SetContentsOfCell("A1", "Hello");
        sp.SetContentsOfCell("B1", "3");
        sp.SetContentsOfCell("C1", "=B1 + 9");
        sp.SetContentsOfCell("D1", "=G1 + 9");
        sp.Save("SimpleSpreadsheet.txt");

        Spreadsheet simpleSpreadsheet = new Spreadsheet("SimpleSpreadsheet.txt");
        Assert.AreEqual("Hello", simpleSpreadsheet["A1"]);
        Assert.AreEqual(3.0, simpleSpreadsheet["B1"]);
        Assert.AreEqual(12.0, simpleSpreadsheet["C1"]);
        Assert.IsInstanceOfType<FormulaError>(simpleSpreadsheet["D1"]);
        Assert.AreEqual("G1+9", ((Formula)simpleSpreadsheet.GetCellContents("D1")).ToString());
        Assert.AreEqual("B1+9", ((Formula)simpleSpreadsheet.GetCellContents("C1")).ToString());
    }


    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SpreadsheetFileConstructor_TestConstructFromANonExistFile_Valid()
    {
        Spreadsheet sp = new Spreadsheet("/some/nonsense/path.txt");
    }

    // Stress Test

    [TestMethod]
    public void TestStress1()
    {
        Spreadsheet s = new Spreadsheet();
        for (int i = 0; i < 500; i++)
        {
            s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
        }
        LinkedList<string> firstCells = new LinkedList<string>();
        LinkedList<string> lastCells = new LinkedList<string>();
        for (int i = 0; i < 250; i++)
        {
            firstCells.AddFirst("A1" + i);
            lastCells.AddFirst("A1" + (i + 250));
        }
        Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
        Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
    }

    [TestMethod]
    public void TestStress2()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=B1+B2");
        s.SetContentsOfCell("B1", "=C1-C2");
        s.SetContentsOfCell("B2", "=C3*C4");
        s.SetContentsOfCell("C1", "=D1*D2");
        s.SetContentsOfCell("C2", "=D3*D4");
        s.SetContentsOfCell("C3", "=D5*D6");
        s.SetContentsOfCell("C4", "=D7*D8");
        s.SetContentsOfCell("D1", "=E1");
        s.SetContentsOfCell("D2", "=E1");
        s.SetContentsOfCell("D3", "=E1");
        s.SetContentsOfCell("D4", "=E1");
        s.SetContentsOfCell("D5", "=E1");
        s.SetContentsOfCell("D6", "=E1");
        s.SetContentsOfCell("D7", "=E1");
        s.SetContentsOfCell("D8", "=E1");
        IList<String> cells = s.SetContentsOfCell("E1", "0");
        Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
    }
}