// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.IO;

namespace JsonHelper.Extensions
{
    public static class ByteArrayEx
    {
        /// <summary>
        /// Zip byte array to byte array
        /// </summary>
        public static byte[] Zip(this byte[] str)
        {
            using var input = new MemoryStream(str);
            return input.Zip();
        }

        /// <summary>
        /// Unzip byte array to string
        /// </summary>
        public static byte[] Unzip(this byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            {
                return input.Unzip();
            }
        }
    }
}
