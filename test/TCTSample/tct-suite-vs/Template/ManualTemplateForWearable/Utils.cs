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
using System.Threading.Tasks;
using NUnit.Framework.TUnit;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using WearableTemplate;
using NUnit.Framework;

namespace WearableTemplate
{
    public class Utils
    {
        public static bool Flag = false;

        public static string GetTCName(string tcID)
        {
            string[] delimiter = { "." };
            string[] stringPieces;
            string returnValue = "";
            try
            {
                stringPieces = tcID.Split(delimiter, StringSplitOptions.None);
                returnValue = stringPieces[stringPieces.Length - 1];
            }
            catch (Exception e)
            {
                LogUtils.Write(LogUtils.ERROR, LogUtils.TAG, "ERROR : " + e.Message);
            }

            return returnValue;
        }

        public static string GetTestFixtureInfoName(string tcID)
        {
            string[] delimiter = { "." };
            string[] stringPieces;
            string tcName = "";
            try
            {
                stringPieces = tcID.Split(delimiter, StringSplitOptions.None);
                tcName = stringPieces[stringPieces.Length - 1];
            }
            catch (Exception e)
            {
                LogUtils.Write(LogUtils.ERROR, LogUtils.TAG, "ERROR : " + e.Message);
            }

            return tcID.Remove(tcID.Length - tcName.Length - 1);
        }

        public static string[] GetClassMethodName(string tcID)
        {
            string[] delimiter = { "." };
            string[] stringPieces;
            stringPieces = tcID.Split(delimiter, StringSplitOptions.None);
            string[] returnValue = { stringPieces[stringPieces.Length - 2], stringPieces[stringPieces.Length - 1] };
            return returnValue;
        }
    }

    public class TestcaseInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _tescaseName;
        public string TestcaseName
        {
            get { return _tescaseName; }
            set
            {
                _tescaseName = value;
                OnPropertyChanged(this, "TestcaseName");
            }
        }

        private List<string> _preconditions;
        public List<string> Preconditions
        {
            get { return _preconditions; }
            set
            {
                _preconditions = value;
                OnPropertyChanged(this, "Preconditions");
            }
        }

        private List<string> _steps;
        public List<string> Steps
        {
            get { return _steps; }
            set
            {
                _steps = value;
                OnPropertyChanged(this, "Steps");
            }
        }

        private List<string> _postconditions;
        public List<string> Postconditions
        {
            get { return _postconditions; }
            set
            {
                _postconditions = value;
                OnPropertyChanged(this, "Postconditions");
            }
        }
    }

    public class TestcaseDescription : INotifyPropertyChanged
    {
        public TestcaseDescription(string description)
        {
            Description = description;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(this, "Description");
            }
        }
    }

    public class ResultNumber
    {
        public static int Total { get; set; }

        public static int Pass { get; set; }

        public static int Fail { get; set; }

        public static int NotRun { get; set; }

        public static int Block { get; set; }
    }

    public class ItemData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
        public int No { get; set; }

        public string TCName { get; set; }

        private string _result;
        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                if (_result.Equals(StrResult.PASS))
                {
                    ResultColor = Color.Green;
                }
                else if (_result.Equals(StrResult.FAIL))
                {
                    ResultColor = Color.Red;
                }
                else if (_result.Equals(StrResult.BLOCK))
                {
                    ResultColor = Color.Orange;
                }
                else
                {
                    ResultColor = Color.White;
                }
                OnPropertyChanged(this, "Result");
            }
        }

        private Color _resultColor;
        public Color ResultColor
        {
            get { return _resultColor; }
            set
            {
                _resultColor = value;
                OnPropertyChanged(this, "ResultColor");
            }
        }
    }

    public class StrResult
    {
        public static string PASS = "PASS";
        public static string FAIL = "FAIL";
        public static string NOTRUN = "NOT RUN";
        public static string BLOCK = "BLOCK";
    }

    public class RunType
    {
        public static string AUTO = "AUTO";
        public static string MANUAL = "MANUAL";

        public static string Value { get; set; }
    }

    public class ManualTest
    {
        private static bool Confirmed = true;
        private static int _type = 0;
        private static string _messageFail = "";

        public static async Task WaitForConfirm()
        {
            _messageFail = "";
            Confirmed = false;
            TestPage.GetInstance().UnlockUIButton();
            while (true)
            {
                await Task.Delay(200);
                if (Confirmed && _type == 1)
                {
                    _type = 0;
                    break;
                }
                else if (_type == 2)
                {
                    _type = 0;
                    Assert.Fail(_messageFail);
                    break;
                }
            }
        }

        public static bool IsConfirmed()
        {
            return Confirmed;
        }

        public static void Confirm()
        {
            Confirmed = true;
            _type = 1;
        }

        public static void ConfirmFail(String message)
        {
            Confirmed = true;
            _type = 2;
            _messageFail = message;
        }

        public static void DisplayLabel(String msg)
        {
            var _testPage = TestPage.GetInstance();
            var _label = new Label()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = msg + " is not support on this profile. So, you can not test this TC. please mark as PASS",
                TextColor = global::Xamarin.Forms.Color.FromHex("#1E90FF"),
                FontSize = 4,
            };

            var layout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    _label,
                },
                Padding = new Thickness()
                {
                    Top = 0,
                },
            };
            _testPage.ExecuteTC(layout);
        }

        public static void DisplayCustomLabel(String msg)
        {
            var _testPage = TestPage.GetInstance();
            var _label = new Label()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = msg,
                TextColor = global::Xamarin.Forms.Color.FromHex("#1E90FF"),
                FontSize = 4,
            };

            var layout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    _label,
                },
                Padding = new Thickness()
                {
                    Top = 0,
                },
            };
            _testPage.ExecuteTC(layout);
        }

    }

    enum NavigationButton
    {
        Next,
        Previous,
        Home,
        NA
    }
}
