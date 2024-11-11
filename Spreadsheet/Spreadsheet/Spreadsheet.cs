// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
//     - Updated return types
//     - Updated documentation

// Author : Minh Quoc Vo & Ryan Boardman
// Date : 10/18/2024
namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
    : base(msg)
    {
    }
}

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{
    /// <summary>
    /// This private variable is a DependencyGraph that represent the 
    /// Relationship between each cell in the Spreadsheet
    /// </summary>
    private DependencyGraph cellGraph;

    /// <summary>
    /// This private variable is a Dictionary with name of the cells as its key
    /// and the Cell object as its value
    /// </summary>
    [JsonPropertyName("Cells")]
    [JsonInclude]
    private Dictionary<String, Cell> cells { get; set; }
    /// <summary>
    /// This is a private class represent a cell in the Spreadsheet
    /// Each cell hold content variable which is type double, string or Formula
    /// </summary>
    private class Cell
    {
        /// <summary>
        /// This variable represent the content that is inside of the cell
        /// </summary>

        [JsonIgnore]
        public object content { get; set; }

        [JsonIgnore]
        public object value { get; set; }

        [JsonPropertyName("StringForm")]
        public string StringForm { get; set; }

        public Cell()
        {
            this.content = "";
            this.value = "";
            this.StringForm = "";
        }

        public Cell(Formula content)
        {
            this.content = content;
            this.value = content;
            StringForm = "=" + content.ToString();
        }

        public Cell(string content)
        {
            this.content = content;
            this.value = content;
            StringForm = content.ToString();
        }
        public Cell(double content)
        {
            this.content = content;
            this.value = content;
            StringForm = content.ToString();
        }
    }

    /// <summary>
    /// This constructor create an empty Spreadsheet
    /// </summary>
    public Spreadsheet()
    {
        cellGraph = new DependencyGraph();
        cells = new Dictionary<String, Cell>();
        Changed = false;
    }


    /// <summary>
    ///   Provides a copy of the normalized names of all of the cells in the spreadsheet
    ///   that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        return this.cells.Keys.ToHashSet();
    }


    /// <summary>
    /// This method check if the cell name is in correct format
    /// </summary>
    /// <param name="cellName"> input cell name for to check </param>
    /// <returns> true if the name is incorrect format, false if it not</returns>
    private static bool IsValidName(string cellName)
    {
        string standaloneVarPattern = $"^{@"[a-zA-Z]+\d+"}$"; //Correct format of a cell Name
        return (Regex.IsMatch(cellName, standaloneVarPattern));
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        string normalizeName = name.ToUpper();
        if (!IsValidName(normalizeName))
        {
            throw new InvalidNameException();
        }
        else if (cells.ContainsKey(normalizeName))
        {
            return cells[normalizeName].content;
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cells in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1, followed by C1.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        List<String> reEvaluated = new List<string>();
        string normalizeName = name.ToUpper();
        if (IsValidName(normalizeName))
        {
            cells[normalizeName] = new Cell(number);
            cellGraph.ReplaceDependees(normalizeName, new HashSet<String>()); // initialize the dependency of node name
            return GetCellsToRecalculate(normalizeName).ToList();
        }
        throw new InvalidNameException();
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        string normalizeName = name.ToUpper();
        if (IsValidName(normalizeName))
        {
            //if (text.Equals(""))
            //{
            //    IList<String> recalculateItself = new List<String>();
            //    recalculateItself.Add(normalizeName);
            //    return recalculateItself;
            //} else
            cells[normalizeName] = new Cell(text);
            cellGraph.ReplaceDependees(normalizeName, new HashSet<String>()); // initialize the dependency of node name
            return GetCellsToRecalculate(normalizeName).ToList();
        }
        throw new InvalidNameException();
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException, and no
    ///     change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        string normalizeName = name.ToUpper();

        if (!IsValidName(normalizeName))
        {
            throw new InvalidNameException();
        }

        Cell previousCell = new Cell("");
        HashSet<string> previousDependees = new HashSet<String>();
        bool cellExists = cells.ContainsKey(normalizeName);

        if (cellExists)
        {
            // Capture previous cell and dependees if the cell already exists
            previousCell = cells[normalizeName];
            previousDependees = new HashSet<string>(cellGraph.GetDependees(normalizeName));
        }

        // Update the cell graph with the new dependencies from the formula
        HashSet<string> newDependees = (HashSet<string>)formula.GetVariables();
        cellGraph.ReplaceDependees(normalizeName, newDependees);

        // Update the cell contents
        cells[normalizeName] = new Cell(formula);

        try
        {
            // Attempt to get all cells that need recalculation
            return GetCellsToRecalculate(normalizeName).ToList();
        }
        catch (CircularException)
        {
            // Revert to previous state in case of a circular dependency
            if (cellExists)
            {
                cells[normalizeName] = previousCell;
                cellGraph.ReplaceDependees(normalizeName, previousDependees);
            }
            else
            {
                cells.Remove(normalizeName);
            }

            throw new CircularException();
        }
    }

    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        return cellGraph.GetDependents(name);
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    ///   A helper for the GetCellsToRecalculate method.
    /// This method went through all of the cells that is depended on the the start cell
    /// </summary>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (string n in GetDirectDependents(name))
        {
            if (n.Equals(start))
            {
                throw new CircularException();
            }
            else if (!visited.Contains(n))
            {
                Visit(start, n, visited, changed);
            }
        }

        changed.AddFirst(name);
    }

    /// <summary>
    /// This method help to find value of the input cell name
    /// </summary>
    /// <param name="cellName"> input name of a cell</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"> throw if the cell does not contain any numeric value</exception>
    private double Lookup(string cellName)
    {
        string normalizeName = cellName.ToUpper();
        //try
        //{
        object cellValue = this.GetCellValue(normalizeName);
        if (cellValue is double value)
        {
            return value;
        }
        throw new ArgumentException("Cell " + normalizeName + "doesn't have a numeric value");
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell, as defined by
    ///     <see cref="GetCellValue(string)"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {

        get
        {
            string normalizeName = name.ToUpper();
            if (!IsValidName(normalizeName))
            {
                throw new InvalidNameException();
            }
            return this.GetCellValue(normalizeName);
        }
    }


    /// <summary>
    /// True if this spreadsheet has been changed since it was 
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get; private set; }


    /// <summary>
    /// Constructs a spreadsheet using the saved data in the file refered to by
    /// the given filename. 
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        //initialize the variables
        this.cellGraph = new DependencyGraph();
        this.cells = new Dictionary<string, Cell> { };
        try
        {
            string data = File.ReadAllText(filename); // read the file
            Spreadsheet? deserializedObject = JsonSerializer.Deserialize<Spreadsheet>(data);
            if (deserializedObject != null) //check if the deserializedObject is null
            {
                foreach (var cell in deserializedObject.cells) // reset up the spread sheet from the deserializedObject
                {
                    SetContentsOfCell(cell.Key, cell.Value.StringForm);
                }
            }
            Changed = false;
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("File handling error ");
        }
    }


    /// <summary>
    ///   <para>
    ///     Writes the contents of this spreadsheet to the named file using a JSON format.
    ///     If the file already exists, overwrite it.
    ///   </para>
    ///   <para>
    ///     The output JSON should look like the following.
    ///   </para>
    ///   <para>
    ///     For example, consider a spreadsheet that contains a cell "A1" 
    ///     with contents being the double 5.0, and a cell "B3" with contents 
    ///     being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///   </para>
    ///   <para>
    ///      This method would produce the following JSON string:
    ///   </para>
    ///   <code>
    ///   {
    ///     "Cells": {
    ///       "A1": {
    ///         "StringForm": "5"
    ///       },
    ///       "B3": {
    ///         "StringForm": "=A1+2"
    ///       },
    ///       "C4": {
    ///         "StringForm": "hello"
    ///       }
    ///     }
    ///   }
    ///   </code>
    ///   <para>
    ///     You can achieve this by making sure your data structure is a dictionary 
    ///     and that the contained objects (Cells) have property named "StringForm"
    ///     (if this name does not match your existing code, use the JsonPropertyName 
    ///     attribute).
    ///   </para>
    ///   <para>
    ///     There can be 0 cells in the dictionary, resulting in { "Cells" : {} } 
    ///   </para>
    ///   <para>
    ///     Further, when writing the value of each cell...
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       If the contents is a string, the value of StringForm is that string
    ///     </item>
    ///     <item>
    ///       If the contents is a double d, the value of StringForm is d.ToString()
    ///     </item>
    ///     <item>
    ///       If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file, 
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        try
        {
            string jsonContent = ToJsonString();
            File.WriteAllText(filename, jsonContent);
            Changed = false;
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("File handling error");
        }
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string name)
    {
        string normalizeName = name.ToUpper();

        if (!IsValidName(normalizeName))
        {
            throw new InvalidNameException();
        }
        if (!cells.ContainsKey(normalizeName))
        {
            return string.Empty;
        }
        return this.cells[normalizeName].value;
    }

    /// <summary>
    ///   <para>
    ///     Set the contents of the named cell to be the provided string
    ///     which will either represent (1) a string, (2) a number, or 
    ///     (3) a formula (based on the prepended '=' character).
    ///   </para>
    ///   <para>
    ///     Rules of parsing the input string:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <para>
    ///         If 'content' parses as a double, the contents of the named
    ///         cell becomes that double.
    ///       </para>
    ///     </item>
    ///     <item>
    ///         If the string does not begin with an '=', the contents of the 
    ///         named cell becomes 'content'.
    ///     </item>
    ///     <item>
    ///       <para>
    ///         If 'content' begins with the character '=', an attempt is made
    ///         to parse the remainder of content into a Formula f using the Formula
    ///         constructor.  There are then three possibilities:
    ///       </para>
    ///       <list type="number">
    ///         <item>
    ///           If the remainder of content cannot be parsed into a Formula, a 
    ///           CS3500.Formula.FormulaFormatException is thrown.
    ///         </item>
    ///         <item>
    ///           Otherwise, if changing the contents of the named cell to be f
    ///           would cause a circular dependency, a CircularException is thrown,
    ///           and no change is made to the spreadsheet.
    ///         </item>
    ///         <item>
    ///           Otherwise, the contents of the named cell becomes f.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </summary>
    /// <returns>
    ///   <para>
    ///     The method returns a list consisting of the name plus the names 
    ///     of all other cells whose value depends, directly or indirectly, 
    ///     on the named cell. The order of the list should be any order 
    ///     such that if cells are re-evaluated in that order, their dependencies 
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <example>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.
    ///   </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        IList<String> recalculateCells;
        if (double.TryParse(content, out double doubleValue))
        {
            recalculateCells = this.SetCellContents(name, doubleValue);
        }

        // Check if the content is a formula (starts with '=')
        else if (!string.IsNullOrEmpty(content) && content[0] == '=')
        {
            string formulaContent = content.Substring(1); // Remove '=' from content
            recalculateCells = this.SetCellContents(name, new Formula(formulaContent));

        }

        else // content is a string
        {
            recalculateCells = this.SetCellContents(name, content);
        }

        foreach (string recalculateCell in recalculateCells)
        {
            object cellContent = cells[recalculateCell].content;
            if(cellContent.ToString() == "")
            {
                cells.Remove(recalculateCell);
            }
            if (cellContent is Formula formula)
            {
                cells[recalculateCell].value = formula.Evaluate(Lookup);
            }
        }
        Changed = true;
        return recalculateCells;
    }

    // PS7 Methods

    /// <summary>
    /// Returns the JSON representation of the current spreadsheet
    /// </summary>
    public string ToJsonString()
    {
        var jsonOption = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        return JsonSerializer.Serialize(this, jsonOption);
    }

    /// <summary>
    /// Loads the spreadsheet from a JSON string
    /// </summary>
    public void LoadFromJsonString(string jsonString)
    {
        this.cellGraph = new DependencyGraph();
        this.cells = new Dictionary<string, Cell> { };
        try
        {
            Spreadsheet? deserializedObject = JsonSerializer.Deserialize<Spreadsheet>(jsonString);

            if (deserializedObject != null) //check if the deserializedObject is null
            {
                foreach (var cell in deserializedObject.cells) // reset up the spread sheet from the deserializedObject
                {
                    SetContentsOfCell(cell.Key, cell.Value.StringForm);
                }
                Changed = false;
            }
            else
            {
                throw new SpreadsheetReadWriteException("Failed to deserialize spreadsheet");
            }
        }
        catch (Exception ex)
        {
            throw new SpreadsheetReadWriteException("Failed loading from JSON string: " + ex.Message);
        }
    }


}
