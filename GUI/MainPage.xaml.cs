using DocumentFormat.OpenXml.Math;
using SpreadsheetUtilities;
using SS;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GUI
{
    /// <summary>
    /// Author:    Matthew Goh
    /// Partner:   None
    /// Date:      6 March 2023
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Matthew Goh - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Matthew Goh and Alex Qi, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    /// 
    /// This main page initializes the grid and all of its components. It supports the modification of
    /// cells that update as dependents are changed as well as saving, opening, and creating new
    /// spreadsheet files. 
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private object label;
        private Dictionary<string, int> Entries;
        private Dictionary<string, Entry> NameAndEntries;
        private Dictionary<Entry, string> EntriesAndName;
        private List<Entry> formulaList;
        Spreadsheet spreadsheet = new Spreadsheet();

        /// <summary>
        ///   Definition of the method signature that must be true for clear methods
        /// </summary>
        private delegate void Clear();

        /// <summary>
        ///   Notifier Pattern.
        ///   When ClearAll(); is called, every "attached" method is called.
        /// </summary>
        private event Clear ClearAll;

        /// <summary>
        ///   List of Entries to show how to "move around" via enter key
        /// </summary>
        private MyEntry[] EntryColumn = new MyEntry[2574];

        /// <summary>
        ///    Definition of what information (method signature) must be sent
        ///    by the Entry when it is modified.
        /// </summary>
        /// <param name="col"> col (char) in grid, e.g., A5 </param>
        /// <param name="row"> row (int) in grid,  e.g., A5 </param>
        public delegate void ActionOnCompleted(char col, int row);

        /// <summary>
        /// Initialize GUI and add to it via code.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Entries = new Dictionary<string, int>();
            NameAndEntries = new Dictionary<string, Entry>();
            EntriesAndName = new Dictionary<Entry, string>();
            formulaList = new List<Entry>();

            // Fill top characters of spreadsheet
            char c = 'A';
            for (int i = 0; i < 27; i++)
            {
                // If statement to have blank cell in top left of spreadsheet
                if (i == 0)
                {
                    label = null;
                }
                else
                {
                    label = c;
                }
                TopLabels.Add(
                  new Border
                  {
                      Stroke = Color.FromRgb(0, 0, 0),
                      StrokeThickness = 1,
                      HeightRequest = 25,
                      WidthRequest = 75,
                      HorizontalOptions = LayoutOptions.Center,
                      Content =
                          new Label
                          {
                              Text = $"{label}",
                              BackgroundColor = Color.FromRgb(200, 200, 250),
                              HorizontalTextAlignment = TextAlignment.Center
                          }
                  }
                  );
                if (i != 0)
                {
                    c++;
                }
            }

            // Fill the left labels of spreadsheet
            int num = 1;
            for (int i = 0; i < 99; i++)
            {
                label = num;
                LeftLabels.Add(
                  new Border
                  {
                      Stroke = Color.FromRgb(0, 0, 0),
                      StrokeThickness = 1,
                      HeightRequest = 25,
                      WidthRequest = 75,
                      HorizontalOptions = LayoutOptions.Center,
                      Content =
                          new Label
                          {
                              Text = $"{label}",
                              BackgroundColor = Color.FromRgb(200, 200, 250),
                              HorizontalTextAlignment = TextAlignment.Center
                          }
                  }
                  );
                num++;
            }

            // Fills the mid section of the grid
            char col = 'A';
            int row = 1;
            int entryNum = 0;

            HorizontalStackLayout horizontal = new HorizontalStackLayout();
            for (int i = 0; i < 99; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    if (j == 0)
                    {
                        col = 'A';
                    }
                    horizontal.Add(
                    new Border
                    {
                        Stroke = Color.FromRgb(0, 0, 0),
                        StrokeThickness = 1,
                        HeightRequest = 25,
                        WidthRequest = 75,
                        HorizontalOptions = LayoutOptions.Center,
                        Content =
                             EntryColumn[entryNum] = new MyEntry(row, col, handleCellChanged)
                             {
                                 Text = $"",
                                 BackgroundColor = Color.FromRgb(200, 200, 250),
                                 HorizontalTextAlignment = TextAlignment.Center
                             }
                    }
                    );
                    string name = char.ToString(col) + row.ToString();

                    NameAndEntries.Add(name, EntryColumn[entryNum]);
                    EntriesAndName.Add(EntryColumn[entryNum], name);
                    Entries.Add(name, entryNum);
                    col++;
                    entryNum++;
                }
                Grid.Add(horizontal);
                horizontal = new HorizontalStackLayout();
                row++;
            }

            foreach (var cell in EntryColumn)
            {
                //              EntryList.Add(cell);
                ClearAll += cell.ClearAndUnfocus;
                cell.Focused += FocusedCell;
                cell.Unfocused += UnfocusedCell;
            }


        }

        /// <summary>
        /// When a cell gets unfocused, calculate value and input into grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnfocusedCell(object sender, FocusEventArgs e)
        {
            string myText = ((Entry)sender).Text;
            try
            {
                IList<string> list = spreadsheet.SetContentsOfCell(EntriesAndName[(Entry)sender], myText);
                foreach (string name in list)
                {
                    try
                    {
                        // Formula error, double, or string
                        if (spreadsheet.GetCellValue(name).GetType() == typeof(string))
                            EntryColumn[Entries[name]].Text = (string)spreadsheet.GetCellValue(name);
                        else
                            // Assume we are returning a double
                            EntryColumn[Entries[name]].Text = spreadsheet.GetCellValue(name).ToString();
                    }
                    // Catch the formula error
                    catch (Exception)
                    {
                        DisplayAlert("ERROR:", "Formula Error", "OK");
                    }
                }
            }
            catch (Exception)
            {
                DisplayAlert("ERROR:", "Invalid Formula", "OK");
            }

            try
            {
                if (myText.Contains("="))
                {
                    ((Entry)sender).Text = spreadsheet.GetCellValue(EntriesAndName[(Entry)sender]).ToString();

                    // Keep track of all the entries that are formulas so we can return contents if cell is selected
                    formulaList.Add((Entry)sender);
                }
            }
            catch (Exception)
            {
                DisplayAlert("ERROR:", "Formula Error", "OK");
            }
        }


        /// <summary>
        /// When a cell is focused, display the contents of the cell and the name of the cell in top left widget 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FocusedCell(object sender, FocusEventArgs e)
        {
            // There must be something in the cell before adding "="
            Regex regex = new Regex(@"[a-zA-Z0-9]");

            // If cell is a formula, add equals so cell calculates to value after being unfocused
            if (regex.IsMatch(((Entry)sender).Text))
            {
                // If cell is a formula, add "=" so cell calculates to value after being unfocused
                if (formulaList.Contains((Entry)sender))
                {
                    ((Entry)sender).Text = "=" + spreadsheet.GetCellContents(EntriesAndName[(Entry)sender]).ToString();
                }
                else
                {
                    ((Entry)sender).Text = spreadsheet.GetCellContents(EntriesAndName[(Entry)sender]).ToString();
                }
            }
            // Fill in the name and contents widgets
            Name.Text = EntriesAndName[(Entry)sender];
            contents.Text = spreadsheet.GetCellContents(EntriesAndName[(Entry)sender]).ToString();
            value.Text = spreadsheet.GetCellValue(EntriesAndName[(Entry)sender]).ToString();
        }

        /// <summary>
        /// Method to handle if the user changes content from upper left entry.
        /// Should result in cell contents changing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryTextChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///   This method will be called by the individual Entry elements when Enter
        ///   is pressed in them. When modifying and pressing enter, the cell below 
        ///   should be focused.
        ///   
        ///   The idea is to move to the next cell in the list.
        /// </summary>
        /// <param name="col"> e.g., The 'A' in A5 </param>
        /// <param name="row"> e.g., The  5  in A5 </param>
        void handleCellChanged(char col, int row)
        {
            string name = col.ToString() + row.ToString();

            // Move focus to the next cell
            Debug.WriteLine($"changed: {col}{row}");
            if (row != 99) { EntryColumn[Entries[name] + 26].Focus(); }
        }

        /// <summary>
        /// In the event that the grid has loaded, set selected cell to A1
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridLoaded(object sender, EventArgs e)
        {
            EntryColumn[0].Focus();
        }

        /// <summary>
        /// In the event of clicking new file, warn before replacing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async private void FileMenuNew(object sender, EventArgs args)
        {
            bool answer = await DisplayAlert("Question?", "Would you like to close the spreadsheet?", "Yes", "No");
            Debug.WriteLine("Answer: " + answer);

            // If yes, clear the spreadsheet and set focus to A1
            if (answer == true)
            {
                ClearAll();
                EntryColumn[0].Focus();
                formulaList.Clear();
            }
        }

        /// <summary>
        /// Opening a spreadsheet from a saved file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async private void FileMenuOpenAsync(object sender, EventArgs args)
        {
            string filepath = await DisplayPromptAsync("File Path:", "Where would you like to open your file from?");
            try
            {
                Spreadsheet s = new Spreadsheet(filepath, s => true, s => s, default);
            }
            catch (Exception)
            {
                await DisplayAlert("ERROR:", "file location not found", "OK");
            }
        }

        /// <summary>
        /// Saving the spreadsheet contents to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async private void OnSaveFile(object sender, EventArgs args)
        {
            string filepath = await DisplayPromptAsync("File Path:", "Where would you like to save your file?");
            try
            {
                spreadsheet.Save(filepath);
            }
            catch (Exception)
            {
                await DisplayAlert("ERROR:", "file location not found", "OK");
            }
        }

        /// <summary>
        /// Saving the spreadsheet contents to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async private void OnHelp(object sender, EventArgs args)
        {
            await DisplayAlert("Help Information:",
                                "This spreadsheet allows modification of cells by clicking on a respective cell and inputting " +
                                "a formula or value then pressing enter or clicking on another cell to save it. You can scroll up " +
                                "and down to access more cells, and the file menu allows you to create a new spreadsheet, save " +
                                "your current spreadsheet, or open up an existing one.",
                                "OK");
        }

        /// <summary>
        /// Event handler that will scroll the top labels if the grid sections are scrolled 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableScrolled(object sender, ScrolledEventArgs e)
        {
            double xPos = Table.ScrollX;
            TopLabelScroll.ScrollToAsync(xPos, 0, true);
        }
    }
}
