using DocumentFormat.OpenXml.Math;
using SS;
using System.Diagnostics;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        private object label;
        private Dictionary<string, int> Entries;
        private Dictionary<string, Entry> NameAndEntries;
        private Dictionary<Entry, string> EntriesAndName;
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
            NameAndEntries= new Dictionary<string, Entry>();
            EntriesAndName = new Dictionary<Entry, string>();

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

            // Fills the mid section of the grid
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
 //             cell.Unfocused += UnfocusedCell;
            }
            

        }

        //private void UnfocusedCell(object sender, FocusEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}


        /// <summary>
        /// When a cell is focused, display the name of the cell in top left widget 
        /// as well as the content and value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FocusedCell(object sender, FocusEventArgs e)
        {
            value.Text = EntriesAndName[(Entry)sender];
            contents.Text = ((Entry)sender).Text;
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
            var myText = EntryColumn[Entries[name]].Text;

            IList<string> list = spreadsheet.SetContentsOfCell(name, myText);
            
            //foreach (string namee in list)
            //{
            //    // Formula error, double, or string
            //    EntryColumn[Entries[namee]].Text = spreadsheet.GetCellValue(namee);
            //}
            
            Debug.WriteLine($"changed: {col}{row}"); 
            if (row != 10) { EntryColumn[Entries[name] + 26].Focus(); }                       
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
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSaveFile(object sender, EventArgs args)
        {
            spreadsheet.Save("hello");
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
