﻿/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Tizen.Location
{
    public static class LocatorHelper
    {
        /// <summary>
        /// Checks if the specified geographical positioning type is supported or not.
        /// </summary>
        /// <since_tizen>3</since_tizen>
        /// <param name="locationType"> The back-end positioning method to be used for LBS.</param>
        /// <returns>Returns a boolean value indicating whether or not the specified method is supported.</returns>
        public static bool IsSupportedType(LocationType locationType)
        {
            bool status = Interop.LocatorHelper.IsSupported((int)locationType);
            Log.Info(Globals.LogTag, "Checking if the Location Manager type is supported ," + status);
            return status;
        }

        /// <summary>
        /// Checks if the specified geographical positioning type is enabled or not.
        /// </summary>
        /// <since_tizen>3</since_tizen>
        /// <param name="locationType"> The back-end positioning method to be used for LBS.</param>
        /// <returns>Returns a boolean value indicating whether or not the specified method is supported.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state.</exception>
        /// <exception cref="ArgumentException">Thrown when an invalid argument is used.</exception>
        /// <exception cref="NotSupportedException">Thrown when the location is not supported.</exception>
        public static bool IsEnabledType(LocationType locationType)
        {
            Log.Info(Globals.LogTag, "Checking if the Location Manager type is Enabled");
            bool status;
            int ret = Interop.LocatorHelper.IsEnabled((int)locationType, out status);
            if (((LocationError)ret != LocationError.None))
            {
                Log.Error(Globals.LogTag, "Error Checking the Location Manager type is Enabled," + (LocationError)ret);
                throw LocationErrorFactory.ThrowLocationException(ret);
            }
            return status;
        }
    }
}
