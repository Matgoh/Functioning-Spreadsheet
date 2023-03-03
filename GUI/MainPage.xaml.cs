using SS;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private object label;

        public MainPage()
        {
            InitializeComponent();

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

            label = null;
            for (int i = 0; i < 52; i++)
            {
                GridH.Add(
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
