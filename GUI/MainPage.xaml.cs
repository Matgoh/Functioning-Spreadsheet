using SS;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        private object label;

        public MainPage()
        {
            InitializeComponent();

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
            for (int i = 0; i < 99; i++)
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
            label = null;
            for (int i = 0; i < 99; i++)
            {
                HorizontalStackLayout horizontal = new HorizontalStackLayout();
                for (int j = 0; j < 26; j++)
                {
                    horizontal.Add(
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
                Grid.Add(horizontal);
            }
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
    }
}
