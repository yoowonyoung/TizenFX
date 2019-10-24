/*
 *  Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

using System;
using System.Collections.Generic;
using NUnitLite.TUnit;
using NUnit.Framework.TUnit;
using NUnit.Framework.Interfaces;
using System.Reflection;
using ManualTemplate;
using Xamarin.Forms;
using Tizen.Applications;

namespace NUnit.Framework
{
    public class TestPage
    {
        private static TestPage instance;
        private static Object lockObject = new object();

        private NavigationPage _navigationPage;
        private ContentPage _tcContentPage;
        private Page _tcUIPage;
        private Label _summaryLabel, _tcResultText, _descriptionTitle;
        private ListView _tcDescriptions;
        private Button _runBtn, _passBtn, _failBtn, _blockBtn;
        private Button _mainBtn, _prevBtn, _nextBtn;
        private StackLayout _mainLayout, _tcUILayout;

        private int _tcIndex = 0;
        private List<string> _tcIDList;
        private List<ItemData> _listItem;
        private List<TestcaseInfo> _tcInfoList;
        private List<TestcaseDescription> _currentTCInfo;
        private TRunner _tunitRunner;
        private TSettings _tsettings;
        private NavigationButton _pressButton = NavigationButton.NA;
        private ToastMessage _toastMessage;

        private const string STEP_ATTRIBUTE_NAME = "NUnit.Framework.StepAttribute";
        private const string PRECONDITION_ATTRIBUTE_NAME = "NUnit.Framework.PreconditionAttribute";
        private const string POSTCONDITION_ATTRIBUTE_NAME = "NUnit.Framework.PostconditionAttribute";

        public event EventHandler<string> TestcaseDone;

        private void OnTestcaseDone(string e)
        {
            EventHandler<string> handler = TestcaseDone;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public NavigationPage getNavigationPage()
        {
            return _navigationPage;
        }

        public static TestPage GetInstance()
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new TestPage();
                }
            }
            return instance;
        }

        private TestPage()
        {
        }

        public void Initialize(TRunner tunitRunner, List<string> testcaseIDList, List<ItemData> listItem)
        {
            _toastMessage = new ToastMessage();
            RunType.Value = RunType.MANUAL;
            _tunitRunner = tunitRunner;
            _tunitRunner.SingleTestDone += OnSingleTestDone;
            _tcInfoList = new List<TestcaseInfo>();
            _tcIDList = testcaseIDList;
            _tsettings = TSettings.GetInstance();
            _tsettings.IsManual = true;
            _listItem = listItem;
            MakeTCInfoList();
            _summaryLabel = new Label()
            {
                Text = "",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                MinimumHeightRequest = 60,
                TextColor = Color.White
            };
            SetResultNumber(0, 0, 0);
            MakeTestPage();
        }

        private void OnSingleTestDone(object sender, SingleTestDoneEventArgs e)
        {
            // check old result
            if (_listItem[_tcIndex].Result.Contains(StrResult.FAIL))
            {
                ResultNumber.Fail = ResultNumber.Fail - 1;
            }
            else if (_listItem[_tcIndex].Result.Contains(StrResult.PASS))
            {
                ResultNumber.Pass = ResultNumber.Pass - 1;
            }
            else if (_listItem[_tcIndex].Result.Contains(StrResult.BLOCK))
                ResultNumber.Block = ResultNumber.Block - 1;

            // Update new result
            _listItem[_tcIndex].Result = e.Result;
            if (e.Result.Contains(StrResult.PASS))
            {
                ResultNumber.Pass += 1;
            }
            else if (e.Result.Contains(StrResult.FAIL))
            {
                ResultNumber.Fail += 1;
            }
            else if (e.Result.Contains(StrResult.BLOCK))
            {
                ResultNumber.Block += 1;
            }

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            SetResultNumber(ResultNumber.Pass, ResultNumber.Fail, ResultNumber.Block);
            _tcResultText.Text = _listItem[_tcIndex].Result;
            SetColor(_tcResultText.Text);
            if (_tcUILayout != null)
            {
                _tcUILayout.Children.Clear();
            }
            if (_pressButton == NavigationButton.Next)
            {
                Next();
            }
            else if (_pressButton == NavigationButton.Previous)
            {
                Previous();
            }
            else if (_pressButton == NavigationButton.Home)
            {
                _navigationPage.PopAsync();
            }
            OnTestcaseDone(null);
            _passBtn.IsEnabled = false;
            _failBtn.IsEnabled = false;
            _blockBtn.IsEnabled = false;
            _runBtn.IsEnabled = true;
            _mainBtn.IsEnabled = true;
            _prevBtn.IsEnabled = true;
            _nextBtn.IsEnabled = true;
        }

        private void SetResultNumber(int pass, int fail, int block)
        {
            ResultNumber.NotRun = ResultNumber.Total - pass - fail - block;
            _summaryLabel.Text = "Total : " + ResultNumber.Total + ", Pass : " + pass + ", Fail : " + fail + ", Block : " + block + ", Not Run : " + ResultNumber.NotRun;
        }

        private void MakeTestPage()
        {
            _mainLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                IsVisible = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 10,
                Padding = new Thickness(10),
            };
            _descriptionTitle = new Label()
            {
                Text = "DESCRIPTION:",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                TextColor = Color.White
            };
            // layout
            StackLayout functionLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                IsVisible = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 10,
                VerticalOptions = LayoutOptions.End,
            };

            _mainBtn = new Button()
            {
                HorizontalOptions = LayoutOptions.Start,
                Text = "Home",
                HeightRequest = 80,
            };

            _mainBtn.Clicked += (sender, e) =>
            {
                _pressButton = NavigationButton.Home;
                if (!ManualTest.IsConfirmed())
                {
                    _tsettings.TCResult = StrResult.NOTRUN;
                    ManualTest.Confirm();
                }
                else
                {
                    _navigationPage.PopAsync();
                }
            };

            _prevBtn = new Button()
            {
                HorizontalOptions = LayoutOptions.Start,
                Text = "<<",
                HeightRequest = 80,
            };

            _prevBtn.Clicked += (sender, e) =>
            {
                PreviousTestcase();
            };

            _nextBtn = new Button()
            {
                Text = ">>",
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = 80,
            };

            _nextBtn.Clicked += (sender, e) =>
            {
                NextTestcase();
            };

            _tcResultText = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HeightRequest = 80,
            };
            functionLayout.Children.Add(_mainBtn);
            functionLayout.Children.Add(_tcResultText);
            functionLayout.Children.Add(_prevBtn);
            functionLayout.Children.Add(_nextBtn);
            // Show description
            ShowDescription();

            var navigationLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                IsVisible = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 10,
                VerticalOptions = LayoutOptions.End,
            };

            _passBtn = new Button()
            {
                HorizontalOptions = LayoutOptions.Start,
                Text = "Pass",
                HeightRequest = 80,
                IsEnabled = false,
            };

            _passBtn.Clicked += (sender, e) =>
            {
                if (!ManualTest.IsConfirmed())
                {
                    _tsettings.TCResult = StrResult.PASS;
                    ManualTest.Confirm();
                }
            };

            _failBtn = new Button()
            {
                HorizontalOptions = LayoutOptions.Start,
                Text = "Fail",
                HeightRequest = 80,
                IsEnabled = false,
            };

            _failBtn.Clicked += (sender, e) =>
            {
                if (!ManualTest.IsConfirmed())
                {
                    _tsettings.TCResult = StrResult.FAIL;
                    ManualTest.Confirm();
                }
            };

            _blockBtn = new Button()
            {
                HorizontalOptions = LayoutOptions.Start,
                Text = "Block",
                HeightRequest = 80,
                IsEnabled = false,
            };

            _blockBtn.Clicked += (sender, e) =>
            {
                if (!ManualTest.IsConfirmed())
                {
                    _tsettings.TCResult = StrResult.BLOCK;
                    ManualTest.Confirm();
                }
            };

            _runBtn = new Button()
            {
                HorizontalOptions = LayoutOptions.Start,
                Text = "Run",
                HeightRequest = 80,
            };

            _runBtn.Clicked += (sender, e) =>
            {
                LockUIButton();
                _pressButton = NavigationButton.NA;
                _tsettings.Testcase_ID = _tcIDList[_tcIndex];
                _tsettings.TCResult = "";
                _tunitRunner.Execute();
            };

            navigationLayout.Children.Add(_passBtn);
            navigationLayout.Children.Add(_failBtn);
            navigationLayout.Children.Add(_blockBtn);
            navigationLayout.Children.Add(_runBtn);

            _tcUILayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 200
            };

            _mainLayout.Children.Add(_summaryLabel);
            _mainLayout.Children.Add(_descriptionTitle);
            _mainLayout.Children.Add(_tcDescriptions);
            _mainLayout.Children.Add(_tcUILayout);
            _mainLayout.Children.Add(navigationLayout);
            _mainLayout.Children.Add(functionLayout);
            _tcContentPage = new ContentPage()
            {
                Content = _mainLayout
            };
            NavigationPage.SetHasNavigationBar(_tcContentPage, false);
        }

        public void LockUIButton()
        {
            if (ManualTest.IsWearable())
            {
                WearableTemplate.TestPage.GetInstance().LockUIButton();
            }
            else
            {
                _runBtn.IsEnabled = false;
                _passBtn.IsEnabled = false;
                _failBtn.IsEnabled = false;
                _blockBtn.IsEnabled = false;
                _mainBtn.IsEnabled = false;
                _prevBtn.IsEnabled = false;
                _nextBtn.IsEnabled = false;
            }
        }

        public void UnlockUIButton()
        {
            if (ManualTest.IsWearable())
            {
                WearableTemplate.TestPage.GetInstance().UnlockUIButton();
            }
            else
            {
                _passBtn.IsEnabled = true;
                _failBtn.IsEnabled = true;
                _blockBtn.IsEnabled = true;
                _mainBtn.IsEnabled = true;
                _prevBtn.IsEnabled = true;
                _nextBtn.IsEnabled = true;
            }
        }

        public void Show(NavigationPage navigationPage, int testcaseIndex)
        {
            _navigationPage = navigationPage;
            _tcIndex = testcaseIndex - 1;
            _descriptionTitle.Text = "DESCRIPTION : #" + testcaseIndex;
            _tcResultText.Text = _listItem[_tcIndex].Result;
            SetColor(_tcResultText.Text);
            UpdateDescriptions();
            navigationPage.PushAsync(_tcContentPage);
            _passBtn.IsEnabled = false;
            _failBtn.IsEnabled = false;
            _blockBtn.IsEnabled = false;
        }

        private void NextTestcase()
        {
            _pressButton = NavigationButton.Next;
            if (!ManualTest.IsConfirmed())
            {
                _tsettings.TCResult = StrResult.NOTRUN;
                ManualTest.Confirm();
            }
            else
            {
                Next();
            }
        }

        private void Next()
        {
            if (_tcIndex < _tcIDList.Count - 1)
            {
                _tcIndex = _tcIndex + 1;
                _descriptionTitle.Text = "DESCRIPTION : #" + (_tcIndex + 1);
                _tcResultText.Text = _listItem[_tcIndex].Result;
                SetColor(_tcResultText.Text);
                UpdateDescriptions();
            }
            else if (_tcIndex == _tcIDList.Count - 1)
            {
                _toastMessage.Message = "This is last testcase";
                _toastMessage.Post();
            }
        }

        private void PreviousTestcase()
        {
            _pressButton = NavigationButton.Previous;
            if (!ManualTest.IsConfirmed())
            {
                _tsettings.TCResult = StrResult.NOTRUN;
                ManualTest.Confirm();
            }
            else
            {
                Previous();
            }
        }

        private void Previous()
        {
            if (_tcIndex > 0)
            {
                _tcIndex = _tcIndex - 1;
                _descriptionTitle.Text = "DESCRIPTION : #" + (_tcIndex + 1);
                _tcResultText.Text = _listItem[_tcIndex].Result;
                SetColor(_tcResultText.Text);
                UpdateDescriptions();
            }
            else if (_tcIndex == 0)
            {
                _toastMessage.Message = "This is first testcase";
                _toastMessage.Post();
            }
        }

        private void SetColor(string result)
        {
            if (result.Equals(StrResult.PASS))
            {
                _tcResultText.TextColor = Color.Green;
            }
            else if (result.Equals(StrResult.FAIL))
            {
                _tcResultText.TextColor = Color.Red;
            }
            else if (result.Equals(StrResult.BLOCK))
            {
                _tcResultText.TextColor = Color.Orange;
            }
            else
            {
                _tcResultText.TextColor = Color.White;
            }
        }

        private int FindMaxLineDescription()
        {
            int maxLine = 0;
            for (int i = 0; i < _tcInfoList.Count; i++)
            {
                int len = _tcInfoList[i].Steps.Count + _tcInfoList[i].Preconditions.Count + _tcInfoList[i].Postconditions.Count;
                maxLine = maxLine < len ? len : maxLine;
            }
            return maxLine;
        }

        private void ShowDescription()
        {
            int lenght = FindMaxLineDescription() + 3;
            var template = new DataTemplate(() =>
            {
                var grid = new Grid();

                var descriptionLabel = new Label
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    LineBreakMode = LineBreakMode.CharacterWrap
                };

                descriptionLabel.SetBinding(Label.TextProperty, new Binding("Description"));

                grid.Children.Add(descriptionLabel);

                return new ViewCell
                {
                    View = grid,
                };
            });

            _tcDescriptions = new ListView()
            {
                ItemTemplate = template,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowHeight = 150,
                Margin = new Thickness(10),
            };
            _currentTCInfo = new List<TestcaseDescription>();

            for (int i = 0; i < lenght; i++)
            {
                _currentTCInfo.Add(new TestcaseDescription(""));
            }
            UpdateDescriptions();
            _tcDescriptions.ItemsSource = _currentTCInfo;
        }

        private void UpdateDescriptions()
        {
            _prevBtn.IsEnabled = false;
            _nextBtn.IsEnabled = false;
            int lenght = FindMaxLineDescription() + 3;
            for (int i = 0; i < lenght; i++)
            {
                _currentTCInfo[i].Description = "";
            }
            int index = 0;
            if (_tcInfoList.Count == 0)
            {
                _toastMessage.Message = "Some testing class wrong, plz recheck";
                _toastMessage.Post();
                return;
            }

            _currentTCInfo[index].Description = "TC : " + _tcInfoList[_tcIndex].TestcaseName;
            TLogger.Write("############### TC Name:" + _tcInfoList[_tcIndex].TestcaseName);
            foreach (string precondition in _tcInfoList[_tcIndex].Preconditions.ToArray())
            {
                index++;
                _currentTCInfo[index].Description = precondition;
            }
            foreach (string step in _tcInfoList[_tcIndex].Steps.ToArray())
            {
                index++;
                _currentTCInfo[index].Description = step;
            }
            foreach (string postcondition in _tcInfoList[_tcIndex].Postconditions.ToArray())
            {
                index++;
                _currentTCInfo[index].Description = postcondition;
            }
            _prevBtn.IsEnabled = true;
            _nextBtn.IsEnabled = true;
        }

        private void MakeTCInfoList()
        {
            foreach (var testcaseItem in _listItem)
            {
                foreach (KeyValuePair<string, ITest> pair in _tunitRunner.GetTestList())
                {
                    if (testcaseItem.TCName.Equals(pair.Key))
                    {
                        IEnumerator<CustomAttributeData> customAttributes = pair.Value.Method.MethodInfo.CustomAttributes.GetEnumerator();
                        List<string> _preconditonsStr = new List<string>(); _preconditonsStr.Add("Preconditions:");
                        List<string> _stepsStr = new List<string>(); _stepsStr.Add("Steps:");
                        List<string> _postconditionsStr = new List<string>(); _postconditionsStr.Add("Postconditions:\n");

                        while (customAttributes.MoveNext())
                        {
                            if (customAttributes.Current.AttributeType.FullName.Equals(STEP_ATTRIBUTE_NAME))
                            {
                                _stepsStr.Add(customAttributes.Current.ConstructorArguments[0].Value + "." + customAttributes.Current.ConstructorArguments[1].Value);
                            }
                            else if (customAttributes.Current.AttributeType.FullName.Equals(PRECONDITION_ATTRIBUTE_NAME))
                            {
                                _preconditonsStr.Add(customAttributes.Current.ConstructorArguments[0].Value + "." + customAttributes.Current.ConstructorArguments[1].Value);
                            }
                            else if (customAttributes.Current.AttributeType.FullName.Equals(POSTCONDITION_ATTRIBUTE_NAME))
                            {
                                _postconditionsStr.Add(customAttributes.Current.ConstructorArguments[0].Value + "." + customAttributes.Current.ConstructorArguments[1].Value);
                            }
                        }

                        _tcInfoList.Add(new TestcaseInfo
                        {
                            TestcaseName = pair.Key,
                            Preconditions = _preconditonsStr,
                            Steps = _stepsStr,
                            Postconditions = _postconditionsStr,
                        });
                        break;
                    }
                }
            }
        }

        public void ExecuteTC(Layout layout)
        {
            if (ManualTest.IsWearable())
            {
                WearableTemplate.TestPage.GetInstance().ExecuteTC(layout);
            }
            else
            {
                _tcUILayout.Children.Add(layout);
            }
        }

        public void ExecuteTC(Page page)
        {
            if (ManualTest.IsWearable())
            {
                if (String.IsNullOrEmpty(page.Title))
                {
                    NavigationPage.SetHasNavigationBar(page, false);
                }
                WearableTemplate.TestPage.GetInstance().ExecuteTC(page);
            }
            else
            {
                _tcUIPage = page;
                _navigationPage.PushAsync(page);
            }
        }
    }
}
