using NUnit.Framework.Interfaces;
using NUnit.Framework.TUnit;
using NUnitLite.TUnit;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace WearableTemplate
{
    public class MainPage : Application
    {

        private static MainPage instance;
        private static Object lockObject = new object();
        private TRunner _tunitrunner;
        List<string> _tcIDList;
        private List<ItemData> _listItem;
        private TestPage _testPage;
        private static NavigationPage _navigationPage;
        private ContentPage _mainContentPage;
        private Label _summaryLabel1, _summaryLabel2;
        private ListView _listView;
        private StackLayout _mainLayout;
        private List<string> _listNotPass;


        public static MainPage GetInstance()
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new MainPage();
                }
            }

            return instance;
        }


        public MainPage()
        {
            _tunitrunner = new TRunner();
            _tunitrunner.LoadTestsuite();
            _listNotPass = new List<string>();
            _tcIDList = new List<string>();
            _listItem = new List<ItemData>();
            _listNotPass = TSettings.GetInstance().GetNotPassListManual();

            int count = 0;
            if (_listNotPass.Count == 0)
            {
                foreach (KeyValuePair<string, ITest> pair in _tunitrunner.GetTestList())
                {
                    count++;
                    _listItem.Add(new ItemData { No = count, TCName = pair.Key, Result = StrResult.NOTRUN });
                    _tcIDList.Add(pair.Key);
                }
            }
            else
            {
                foreach (var tc in _listNotPass)
                {
                    count++;
                    _listItem.Add(new ItemData { No = count, TCName = tc, Result = StrResult.NOTRUN });
                    _tcIDList.Add(tc);
                }
            }

            ResultNumber.Total = ResultNumber.NotRun = _tcIDList.Count;

            if (_listItem.Count != 0)
            {
                _testPage = TestPage.GetInstance();
                _testPage.Initialize(_tunitrunner, _tcIDList, _listItem);
                _testPage.TestcaseDone += OnTestcaseDone;
            }
            MakeWindowPage();

            _navigationPage = new NavigationPage(_mainContentPage);
            NavigationPage.SetHasNavigationBar(_mainContentPage, false);
            MainPage = _navigationPage;

        }

        private void OnTestcaseDone(object sender, string e)
        {
            SetSummaryResult();
        }

        private void SetSummaryResult()
        {
            ResultNumber.NotRun = ResultNumber.Total - ResultNumber.Pass - ResultNumber.Fail - ResultNumber.Block;
            _summaryLabel1.Text = "T : " + ResultNumber.Total + ", P : " + ResultNumber.Pass;
            _summaryLabel2.Text = "F : " + ResultNumber.Fail + ", B : " + ResultNumber.Block + ", NR : " + ResultNumber.NotRun;
        }

        private void MakeWindowPage()
        {
            var wrapLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                IsVisible = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            _mainLayout = new StackLayout()
            {
                IsVisible = true,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(10),
                Spacing = 10,
            };

            _summaryLabel1 = new Label()
            {
                Text = "",
                HeightRequest = 25,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                Margin = new Thickness(0, 10, 0, 0)
            };

            _summaryLabel2 = new Label()
            {
                Text = "",
                HeightRequest = 25,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White
            };

            var navigationLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                IsVisible = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
            };

            var runBtn = new Button()
            {
                HorizontalOptions = LayoutOptions.Center,
                Text = "Run",
                WidthRequest = 150,
                HeightRequest = 50,
            };

            runBtn.Clicked += (sender, e) =>
            {
                if (_listItem.Count > 0)
                {
                    RunType.Value = RunType.AUTO;
                    _testPage.Show(_navigationPage, 1);
                }
            };

            var doneBtn = new Button()
            {
                HorizontalOptions = LayoutOptions.Center,
                Text = "Done",
                WidthRequest = 150,
                HeightRequest = 50,
            };

            doneBtn.Clicked += (sender, e) =>
            {
                Console.WriteLine("#####TCT##### doneBtn Clicked!");
                TSettings.GetInstance().SubmitManualResult();
            };

            var template = new DataTemplate(() =>
            {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.125, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.65, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.225, GridUnitType.Star) });

                var noLabel = new Label { VerticalTextAlignment = TextAlignment.Center, FontSize = 6 };
                var nameLabel = new Label { VerticalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.CharacterWrap, FontSize = 6 };
                var resultLabel = new Label { HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, FontSize = 6 };

                noLabel.SetBinding(Label.TextProperty, new Binding("No", stringFormat: "#{0}."));
                nameLabel.SetBinding(Label.TextProperty, new Binding("TCName"));
                resultLabel.SetBinding(Label.TextProperty, new Binding("Result", stringFormat: "[{0}]"));
                resultLabel.SetBinding(Label.TextColorProperty, new Binding("ResultColor"));

                grid.Children.Add(noLabel);
                grid.Children.Add(nameLabel, 1, 0);
                grid.Children.Add(resultLabel, 2, 0);

                return new ViewCell
                {
                    View = grid,
                };
            });

            _listView = new ListView { ItemsSource = _listItem, ItemTemplate = template, HasUnevenRows = true, RowHeight = 180, Margin = new Thickness(5, 0, 5, 0) };
            _listView.ItemSelected += (s, e) =>
            {
                ItemData item = (ItemData)e.SelectedItem;
                _testPage.Show(_navigationPage, item.No);
            };
            SetSummaryResult();


            navigationLayout.Children.Add(runBtn);
            navigationLayout.Children.Add(doneBtn);

            _mainLayout.Children.Add(_summaryLabel1);
            _mainLayout.Children.Add(_summaryLabel2);
            _mainLayout.Children.Add(navigationLayout);
            _mainLayout.Children.Add(_listView);
            wrapLayout.Children.Add(_mainLayout);
            _mainContentPage = new ContentPage()
            {
                Content = wrapLayout
            };
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
