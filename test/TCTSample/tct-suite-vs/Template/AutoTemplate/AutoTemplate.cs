using Xamarin.Forms;

namespace AutoTemplate
{

    public class App : Application
    {
        public static NavigationPage NaviPage;

        public App()
        {
            // The root page of your application
            NaviPage = new NavigationPage(new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Running .NET TCT..."
                        }
                    }
                }
            });

            MainPage = NaviPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }


    }
}
