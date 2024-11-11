// <copyright file="SpreadsheetPage.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// Author: Minh Quoc Vo and Ryan Boardman
// Date: 10/24/2024

namespace GUI.Client.Pages;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using CS3500.Spreadsheet;
using CS3500.Formula;

/// <summary>
/// This class keeps track of the Spreadsheet Page on our website
/// </summary>
public partial class SpreadsheetPage
{

    /// <summary>
    /// A private instance of the Spreadsheet class
    /// </summary>
    private Spreadsheet currentSpreadsheet = new Spreadsheet();

    /// <summary>
    /// Based on your computer, you could shrink/grow this value based on performance.
    /// </summary>
    private const int ROWS = 50;

    /// <summary>
    /// Number of columns, which will be labeled A-Z.
    /// </summary>
    private const int COLS = 26;

    /// <summary>
    /// Provides an easy way to convert from an index to a letter (0 -> A)
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();


    // Member variables used to keep track of internal state, which as 
    // which row/col is selected, and to control UI elements
    // See lecture for details
    private string SelectedCell = string.Empty;

    /// <summary>
    /// Current Contents of the cell
    /// </summary>
    private string CurrentContents = string.Empty;

    /// <summary>
    /// Private bool to see whether or not an error is shown.
    /// </summary>
    private bool ShowError = false;

    /// <summary>
    /// Private string that keeps track of error message
    /// </summary>
    private string ErrorMessage = "none";

    /// <summary>
    /// The UI of the Spreadsheet
    /// </summary>
    private ElementReference TextArea;

    /// <summary>
    /// The selected row as a coordinate
    /// </summary>
    private int SelectedRow = 0;
    /// <summary>
    /// The selected column as a coordinate
    /// </summary>
    private int SelectedCol = 0;

    /// <summary>
    /// Private string that keep track of the current cell's value 
    /// </summary>
    private string SelectedCellValue = string.Empty;


    /// <summary>
    ///   Gets or sets the name of the file to be saved
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";


    /// <summary>
    ///   <para> Gets or sets the data for all of the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[ROWS, COLS];

    protected override void OnInitialized()
    {
        // Automatically select A1 on initialization
        SelectedCell = "A1";
    }


    /// <summary>
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
    private void CellClicked(int row, int col)
    {
        SelectedCell = $"{Alphabet[col]}{row + 1}"; // converting the coordinate on the spreadsheet into cell name 

        SelectedRow = row;
        SelectedCol = col;
        string newContent = currentSpreadsheet.GetCellContents(SelectedCell)!.ToString() ?? "";
        if ((currentSpreadsheet.GetCellContents(SelectedCell)) is Formula)
            newContent = "=" + newContent;
        CurrentContents = newContent;
        SelectedCellValue = CellsBackingStore[SelectedRow, SelectedCol];
        TextArea.FocusAsync();
    }

    /// <summary>
    /// When a cell needs its content changed this private method will run
    /// </summary>
    /// <param name="e">This parameter is what we are looking at to put in the cell</param>
    private void CellContentChanged(ChangeEventArgs e)
    {
        // This uses the null forgiving (!) and coalescing (??)
        // operators to get either the value that was typed in,
        // or the empty string if it was null
        var data = e.Value!.ToString() ?? string.Empty;
        try
        {
            IList<string> recalculateCells = currentSpreadsheet.SetContentsOfCell(SelectedCell, data);
            foreach (string cell in recalculateCells)
            {
                int[] coordinate = CellNameTCoordinate(cell);
                CellsBackingStore[coordinate[0], coordinate[1]] = currentSpreadsheet.GetCellValue(cell)!.ToString() ?? ""; // update all of the cells after the change
            }
        }
        catch (Exception exception)
        {
            ShowError = true;
            if (exception is InvalidNameException)
            {
                ErrorMessage = "Invalid Name Error";
            }
            else if (exception is CircularException)
            {
                ErrorMessage = "Circular Dependency Error";
            }
            else if (exception is FormulaFormatException) 
            {
                ErrorMessage = "Formula Format Exception";
            }
            else {
                ErrorMessage = exception.ToString();
            }
            CurrentContents = String.Empty;
        }
        CurrentContents = data;
        SelectedCellValue = CellsBackingStore[SelectedRow, SelectedCol]; // update cell value 
        TextArea.FocusAsync();
    }

    /// <summary>
    /// This private helper method used to convert cell name into coordinate of the spreadsheet. 
    /// </summary>
    /// <param name="cellName"> input string name </param>
    /// <returns></returns>
    private int[] CellNameTCoordinate(string cellName)
    {
        int[] coordinate = new int[2];
        coordinate[1] = cellName[0] - 'A';
        int.TryParse(cellName[1].ToString(), out int rowCoordinate);
        coordinate[0] = rowCoordinate - 1;
        return coordinate;
    }

    /// <summary>
    /// Private method that will dismiss the error after the OK button has been clicked
    /// </summary>
    private void DismissError()
    {
        ShowError = false;
        ErrorMessage = "none";
    }

    /// <summary>
    /// Private async that will toggle dark mode
    /// </summary>
    private async Task ToggleDarkMode()
    {
        await JSRuntime.InvokeVoidAsync("toggleDarkMode");
    }


    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet.
    /// </summary>
    private async void SaveFile()
    {
        await JSRuntime.InvokeVoidAsync("downloadFile", FileSaveName, currentSpreadsheet.ToJsonString());
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser(EventArgs args)
    {
        try
        {
            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
            if (eventArgs.FileCount == 1)
            {
                var file = eventArgs.File;
                if (file is null)
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // fileContent will contain the contents of the loaded file
                fileContent = await reader.ReadToEndAsync();

                // Use the loaded fileContent to replace the current spreadsheet
                currentSpreadsheet.LoadFromJsonString(fileContent);
                for(int Row = 0; Row < ROWS; Row++)
                {
                    for(int Column = 0; Column < COLS; Column++)
                    {
                        CellsBackingStore[Row, Column] = currentSpreadsheet.GetCellValue($"{Alphabet[Column]}{Row + 1}").ToString() ?? "";
                    }
                }
                SelectedCell = "A1";
                SelectedCellValue = String.Empty;
                CurrentContents = String.Empty;
                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("an error occurred while loading the file..." + e);
        }
    }

}
