using DocumentFormat.OpenXml.Math;
using SS;
using System.Diagnostics;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        private object label;
        private Dictionary<string, int> Entries;

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
        private MyEntry[] EntryColumn = new MyEntry[260];

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
            for (int i = 0; i < 10; i++)
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

            // Fills all the mid section of the grid
            char col = 'A';
            int row = 1;
            int entryNum = 0;  

            HorizontalStackLayout horizontal = new HorizontalStackLayout();
            for (int i = 0; i < 10; i++)
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
                                 Text = $"{char.ToString(col) + row.ToString()}",
                                 BackgroundColor = Color.FromRgb(200, 200, 250),
                                 HorizontalTextAlignment = TextAlignment.Center
                             }
                    }
                    );
                    string strCol = char.ToString(col);
                    string strRow = row.ToString();
                    Entries.Add(strCol + strRow, entryNum);
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
            }

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
            Debug.WriteLine($"changed: {col}{row}"); 
            if (row != 10) { EntryColumn[Entries[col.ToString() + row.ToString()] + 26].Focus(); }  // Move the focus                                                                                                  
        }

        /// <summary>
        /// In the event that the grid has loaded, set selected cell to A1
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

            // If yes, clear the spreadsheet
            if (answer == true)
            {
                ClearAll();
            }
        }

        private void FileMenuOpenAsync(object sender, EventArgs args)
        {
            for (int i = 0; i < 10; i++)
            {
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
            }
        }

        private void OnSaveFile(object sender, EventArgs args)
        {

            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.Save("hello");
        }

        private void CellChangedValue(object sender, EventArgs e)
        {

        }

        private void ActionCompleted(object sender, EventArgs e)
        {

        }

        

        /// <summary>
        /// Event handler that will scroll the top labels if the grid sections is scrolled 
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
