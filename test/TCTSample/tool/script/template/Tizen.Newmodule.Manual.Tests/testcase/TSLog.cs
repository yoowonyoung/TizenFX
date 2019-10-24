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

using System.Threading.Tasks;
using NUnit.Framework;

namespace Tizen.Sample.Tests
{

    [TestFixture]
    [Description("Tizen::Log Tests")]
    public class LogTests {

        private string TAG = "TCT";
        private string _message;

        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Destroy()
        {
        }

        [Test]
        [Category("P1")]
        [Description("MANUAL TEST : Send debug log to dlog.")]
        [Property("SPEC", " Tizen.Log.Debug M")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "MR")]
        [Property("AUTHOR", "Nguyen Duc Han, duchan.ng@samsung.com")]
        [Precondition(1, "Open terminal to view logs.")]
        [Precondition(2, "Enter command \"sdb dlog -c\" to clear log")]
        [Precondition(3, "Enter command \"sdb dlog TCT\" to terminal")]
        [Step(1, "Click run TC")]
        [Step(2, "check if log show \"Debug log message\" with Tag \"TCT\" and type log \"D\".")]
        [Postcondition(1, "Close the terminal")]
        public async Task Debug_PRINT_TO_DLOG() {
            /*
             * TEST CODE - MANUAL TEST
             * use command: "sdb dlog | grep TCT" to see log.
             */
            _message = "Debug log message";
            Log.Debug(TAG, _message);

            /*
             * RESULT : PASS
             * log show "Debug log message" with Tag "TCT" and type log "D"
             */
            // Waits for user confirmation.
            await ManualTest.WaitForConfirm();
        }

        [Test]
        [Category("P1")]
        [Description("MANUAL TEST : Send error log to dlog.")]
        [Property("SPEC", " Tizen.Log.Error M")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "MR")]
        [Property("AUTHOR", "Nguyen Duc Han, duchan.ng@samsung.com")]
        [Precondition(1, "Open terminal to view logs.")]
        [Precondition(2, "Enter command \"sdb dlog -c\" to clear log")]
        [Precondition(3, "Enter command \"sdb dlog TCT\" to terminal")]
        [Step(1, "Click run TC")]
        [Step(2, "Check if log show \"Error log message\" with Tag \"TCT\" and type log \"E\".")]
        [Postcondition(1, "Close the terminal")]
        public async Task Error_PRINT_TO_DLOG() {
            /*
             * TEST CODE - MANUAL TEST
             * use command: "sdb dlog | grep TCT" to see log.
             */
            _message = "Error log message";
            Log.Error(TAG, _message);

            /*
             * RESULT : PASS
             * log show "Error log message" with Tag "TCT" and type log "E"
             */
            // Waits for user confirmation.
            await ManualTest.WaitForConfirm();
        }

        [Test]
        [Category("P1")]
        [Description("MANUAL TEST : Send fatal log to dlog.")]
        [Property("SPEC", " Tizen.Log.Fatal M")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "MR")]
        [Property("AUTHOR", "Nguyen Duc Han, duchan.ng@samsung.com")]
        [Precondition(1, "Open terminal to view logs.")]
        [Precondition(2, "Enter command \"sdb dlog -c\" to clear log")]
        [Precondition(3, "Enter command \"sdb dlog TCT\" to terminal")]
        [Step(1, "Click run TC")]
        [Step(2, "Check if log show \"Fatal log message\" with Tag \"TCT\" and type log \"F\".")]
        [Postcondition(1, "Close the terminal")]
        public async Task Fatal_PRINT_TO_DLOG() {
            /*
             * TEST CODE - MANUAL TEST
             * use command: "sdb dlog | grep TCT" to see log.
             */
            _message = "Fatal log message";
            Log.Fatal(TAG, _message);

            /*
             * RESULT : PASS
             * log show "Fatal log message" with Tag "TCT" and type log "F"
             */
            // Waits for user confirmation.
            await ManualTest.WaitForConfirm();
        }

        [Test]
        [Category("P1")]
        [Description("MANUAL TEST : Send info log to dlog.")]
        [Property("SPEC", " Tizen.Log.Info M")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "MR")]
        [Property("AUTHOR", "Nguyen Duc Han, duchan.ng@samsung.com")]
        [Precondition(1, "Open terminal to view logs.")]
        [Precondition(2, "Enter command \"sdb dlog -c\" to clear log")]
        [Precondition(3, "Enter command \"sdb dlog TCT\" to terminal")]
        [Step(1, "Click run TC")]
        [Step(2, "Check if log show \"Info log message\" with Tag \"TCT\" and type log \"I\".")]
        [Postcondition(1, "Close the terminal")]
        public async Task Info_PRINT_TO_DLOG() {
            /*
             * TEST CODE - MANUAL TEST
             * use command: "sdb dlog | grep TCT" to see log.
             */
            _message = "Info log message";
            Log.Info(TAG, _message);

            /*
             * RESULT : PASS
             * log show "Info log message" with Tag "TCT" and type log "I"
             */
            // Waits for user confirmation.
            await ManualTest.WaitForConfirm();
        }

        [Test]
        [Category("P1")]
        [Description("MANUAL TEST : Send Verbose log to dlog.")]
        [Property("SPEC", " Tizen.Log.Verbose M")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "MR")]
        [Property("AUTHOR", "Nguyen Duc Han, duchan.ng@samsung.com")]
        [Precondition(1, "Open terminal to view logs.")]
        [Precondition(2, "Enter command \"sdb dlog -c\" to clear log")]
        [Precondition(3, "Enter command \"sdb dlog TCT\" to terminal")]
        [Step(1, "Click run TC")]
        [Step(2, "Check if log show \"Verbose log message\" with Tag \"TCT\" and type log \"V\".")]
        [Postcondition(1, "Close the terminal")]
        public async Task Verbose_PRINT_TO_DLOG() {
            /*
             * TEST CODE - MANUAL TEST
             * use command: "sdb dlog TCT" to see log.
             */
            _message = "Verbose log message";
            Log.Verbose(TAG, _message);

            /*
             * RESULT : PASS
             * log not show "Verbose log message"
             */
            // Waits for user confirmation.
            await ManualTest.WaitForConfirm();
        }

        [Test]
        [Category("P1")]
        [Description("MANUAL TEST : Send warn log to dlog.")]
        [Property("SPEC", " Tizen.Log.Warn M")]
        [Property("SPEC_URL", "-")]
        [Property("CRITERIA", "MR")]
        [Property("AUTHOR", "Nguyen Duc Han, duchan.ng@samsung.com")]
        [Precondition(1, "Open terminal to view logs.")]
        [Precondition(2, "Enter command \"sdb dlog -c\" to clear log")]
        [Precondition(3, "Enter command \"sdb dlog TCT\" to terminal")]
        [Step(1, "Click run TC")]
        [Step(2, "Check if log show \"Warn log message\" with Tag \"TCT\" and type log \"W\".")]
        [Postcondition(1, "Close the terminal")]
        public async Task Warn_PRINT_TO_DLOG() {
            /*
             * TEST CODE - MANUAL TEST
             * use command: "sdb dlog | grep TCT" to see log.
             */
            _message = "Warn log message";
            Log.Warn(TAG, _message);

            /*
             * RESULT : PASS
             * log show "Warn log message" with Tag "TCT" and type log "W"
             */
            // Waits for user confirmation.
            await ManualTest.WaitForConfirm();
        }
    }
}

