// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace JsonHelper.Extensions;

using System.IO;
using System.IO.Compression;

public static class StreamEx
{
    /// <summary>
    /// Zip stream to byte array
    /// </summary>
    public static byte[] Zip(this Stream input)
    {
        using var result = new MemoryStream();
        using (var gs = new GZipStream(result, CompressionMode.Compress))
        {
            input.CopyTo(gs);
        }
        return result.ToArray();
    }

    /// <summary>
    /// Unzip stream to byte array
    /// </summary>
    public static byte[] Unzip(this Stream input)
    {
        using (var output = new MemoryStream())
        {
            using (var gs = new GZipStream(input, CompressionMode.Decompress))
            {
                gs.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}
