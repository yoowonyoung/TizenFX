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
using NUnit.Framework;
using Xamarin.Forms;

namespace XamarinApplication.Tizen
{
    class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
    {
        private Application _app;
        protected override void OnCreate()
        {
            Console.WriteLine("TCT : OnCreate()");
            base.OnCreate();
            if (ManualTest.IsWearable())
            {
                _app = new WearableTemplate.MainPage();
            }
            else
            {
                _app = new ManualTemplate.MainPage();
            }
            LoadApplication(_app);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("TCT : Main()");
            var app = new Program();
            global::Xamarin.Forms.Platform.Tizen.Forms.Init(app);
            app.Run(args);
        }
    }
}
