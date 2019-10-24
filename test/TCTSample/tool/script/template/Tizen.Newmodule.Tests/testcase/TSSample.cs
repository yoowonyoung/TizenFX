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

using NUnit.Framework;
using NUnit.Framework.TUnit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tizen;

namespace Tizen.Sample.Tests {

    [TestFixture]
    [Description("Sample Class Tests")]
    public class SampleTests {

        private string _message = "raycad";

        [SetUp]
        public void Init()
        {
            LogUtils.Write(LogUtils.DEBUG , LogUtils.TAG , "Preconditions for each TEST");
        }

        [TearDown]
        public void Destroy()
        {
            LogUtils.Write(LogUtils.DEBUG , LogUtils.TAG , "Postconditions for each TEST");
        }

        [Test]
        [Category("P1")]
        [Description("Create a test case to test function")]
        [Property("SPEC", "Tizen.XXX.XXX.SampleTest M")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "MR")]
        [Property("AUTHOR", "Nguyen Truong Duong, duong.nt1@samsung.com")]
        public void Test1()
        {
            var getValue = _message;
            Assert.IsInstanceOf<string>(getValue, "Object should be string type.");
            Assert.AreEqual(getValue, _message, "Object is not equal");
        }
    }
}
