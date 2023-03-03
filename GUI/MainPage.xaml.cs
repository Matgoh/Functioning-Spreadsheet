using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Vml;
using SS;
using System.Text;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private object label;

        public MainPage()
        {
            InitializeComponent();
            for (int i = 0; i < 26; i++)
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
            }

            async void FileMenuNew(object sender, EventArgs args)
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

            async void OnSaveFile(object sender, EventArgs args)
            {
                Spreadsheet spreadsheet = new Spreadsheet();
                spreadsheet.Save("hello");
            }
        }
    }
}