/*
 *  Copyright (c) 2018 Samsung Electronics Co., Ltd All Rights Reserved
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
using NUnit.Framework;
using NUnit.Framework.TUnit;
using Xamarin.Forms;

namespace Tizen.Sample.Tests
{
    [TestFixture]
    [Description("Tizen.Sample.Button test")]
    public class ButtonTests
    {
        private TestPage _testPage = TestPage.GetInstance();
        private Button _button;

        [SetUp]
        public void Init()
        {
            LogUtils.Write(LogUtils.INFO, LogUtils.TAG, "Preconditions for each TEST");
            _button = new Button();
        }

        [TearDown]
        public void Destroy()
        {
            LogUtils.Write(LogUtils.INFO, LogUtils.TAG, "Postconditions for each TEST");
            _button = null;
        }

        private void CreateButton(string buttonText)
        {
            LogUtils.Write(LogUtils.DEBUG, LogUtils.TAG, "CreateButton");

            var layout = new StackLayout()
            {
                IsVisible = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            _button = new Button()
            {
                Text = buttonText,
                FontSize = 20,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            layout.Children.Add(_button);
            _testPage.ExecuteTC(layout);
        }

        private void OnClick(object sender, EventArgs e)
        {
            // If your test is fail, you should set Assert.True(false , "msg");
            Assert.True(true);
            // ManualTest.Confirm() API will terminate WaitForConfirm() method, and will execute rest of the code.
            ManualTest.Confirm();
        }

        [Test]
        [Category("P1")]
        [Description("Test: Handle event click button.")]
        [Property("SPEC", "Tizen.UI.Button.Clicked E")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "EVL")]
        [Property("AUTHOR", "Pham Phuong Dong, dong.phuong@samsung.com")]
        [Precondition(1, "NA")]
        [Step(1, "Click run TC")]
        [Step(2, "Click button")]
        [Postcondition(1, "NA")]
        public async Task Clicked_CB()
        {
            CreateButton("Click me!! Test: Handle event click button");
            _button.Clicked += OnClick;
            // Waits for user confirmation.
            await ManualTest.WaitForConfirm();
        }

        private void CreateButtonPage(string buttonText)
        {
            LogUtils.Write(LogUtils.DEBUG, LogUtils.TAG, "Init");

            var layout = new StackLayout()
            {
                IsVisible = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            _button = new Button()
            {
                Text = buttonText,
                FontSize = 20,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            layout.Children.Add(_button);
            // Invoke ExecuteTC() method to add UI to test page.
            _testPage.ExecuteTC(new ContentPage()
            {
                Content = layout,
            });
        }

        public void OnPressed(object sender, EventArgs e)
        {
            // pop layout
            _testPage.getNavigationPage().PopAsync();
            // or Assert.True(false);
            Assert.True(true);
            // ManualTest.Confirm() API will terminate WaitForConfirm() method, and will execute rest of the code.
            ManualTest.Confirm();
        }

        [Test]
        [Category("P1")]
        [Description("Test: Handle event click button.")]
        [Property("SPEC", "Tizen.UI.Button.Clicked E")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "EVL")]
        [Property("AUTHOR", "Pham Phuong Dong, dong.phuong@samsung.com")]
        [Precondition(1, "NA")]
        [Step(1, "Click run TC")]
        [Step(2, "Click button after new page shown")]
        [Postcondition(1, "NA")]
        public async Task Pressed_CB()
        {
            CreateButtonPage("Click me!! Test: Handle event press button");
            _button.Clicked += OnPressed;
            // Waits for user confirmation.
            await ManualTest.WaitForConfirm();
        }
    }
}



