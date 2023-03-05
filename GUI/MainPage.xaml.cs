using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Presentation;
using SS;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        private object label;
        private Dictionary<string, object> Entries;
        public MainPage()
        {
            InitializeComponent();
            Entries = new Dictionary<string, object>();

            // Fill top characters of spreadsheet
            Char c = 'A';
            for (int i = 0; i < 26; i++)
            {
                label = c;
                TopLabels.Add(
                  new Border
                  {
                      Stroke = Color.FromRgb(0, 0, 0),
                      StrokeThickness = 1,
                      HeightRequest = 20,
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
                c++;
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
                      HeightRequest = 20,
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
            Entry entry;
            for (int i = 0; i < 10; i++)
            {
                HorizontalStackLayout horizontal = new HorizontalStackLayout();
                for (int j = 0; j < 26; j++)
                {
                    if (j == 0)
                    {
                        row = 'A';
                    }
                    horizontal.Add(
                    new Border
                    {
                        Stroke = Color.FromRgb(0, 0, 0),
                        StrokeThickness = 1,
                        HeightRequest = 20,
                        WidthRequest = 75,
                        HorizontalOptions = LayoutOptions.Center,
                        Content =
                             entry = new Entry
                             {
                                 BackgroundColor = Color.FromRgb(200, 200, 250),
                                 HorizontalTextAlignment = TextAlignment.Center
                             }
                    }
                    );
                    string strCol = char.ToString(col);
                    string strRow = row.ToString();
                    Entries.Add(strRow + col, entry);
                    col++;                  
                }
                Grid.Add(horizontal);
                row++;
            }

        }

        /// <summary>
        /// In the event that the grid has loaded, set selected cell to A1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridLoaded(object sender, EventArgs e)
        {
        }
        void FileMenuNew(object sender, EventArgs args)
        {
            for (int i = 0; i < 10; i++)
            {
                TopLabels.Add(
                  new Border
                  {
                      Stroke = Color.FromRgb(0, 0, 0),
                      StrokeThickness = 1,
                      HeightRequest = 20,
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

        void FileMenuOpenAsync(object sender, EventArgs args)
        {
            for (int i = 0; i < 10; i++)
            {
                TopLabels.Add(
                  new Border
                  {
                      Stroke = Color.FromRgb(0, 0, 0),
                      StrokeThickness = 1,
                      HeightRequest = 20,
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

        void OnSaveFile(object sender, EventArgs args)
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

        private void TableScrolled(object sender, ScrolledEventArgs e)
        {
            double xPos = Table.ScrollX;
            TopLabelScroll.ScrollToAsync(xPos, 0, true);
        }
    }
}
