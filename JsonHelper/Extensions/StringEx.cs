// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace JsonHelper.Extensions;

using System.Text;

public static class StringEx
{
    /// <summary>
    /// Zip string to string
    /// </summary>
    public static string Zip(this string str)
    {
        byte[] gzipBytes = Encoding.UTF8.GetBytes(str).Zip();
        return Encoding.UTF8.GetString(gzipBytes);
    }
}
