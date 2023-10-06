using System;
using System.util;

/*
 * Copyright 2008 ZXing authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace iTextSharp.text.pdf.qrcode;

/**
 * This object renders a QR Code as a ByteMatrix 2D array of greyscale values.
 * 
 * @author dswitkin@google.com (Daniel Switkin)
 */
public sealed class QRCodeWriter
{
    private const int QUIET_ZONE_SIZE = 4;

    public ByteMatrix Encode(string contents, int width, int height) => Encode(contents, width, height, null);

    public ByteMatrix Encode(string contents,
                             int width,
                             int height,
                             INullValueDictionary<EncodeHintType, object> hints)
    {
        if (string.IsNullOrEmpty(contents))
        {
            throw new ArgumentException("Found empty contents");
        }

        if (width < 0 || height < 0)
        {
            throw new ArgumentException("Requested dimensions are too small: " + width + 'x' +
                                        height);
        }

        var errorCorrectionLevel = ErrorCorrectionLevel.L;
        if (hints != null && hints.TryGetValue(EncodeHintType.ERROR_CORRECTION, out var hint))
        {
            errorCorrectionLevel = (ErrorCorrectionLevel)hint;
        }

        var code = new QRCode();
        Encoder.Encode(contents, errorCorrectionLevel, hints, code);
        return RenderResult(code, width, height);
    }

    // Note that the input matrix uses 0 == white, 1 == black, while the output matrix uses
    // 0 == black, 255 == white (i.e. an 8 bit greyscale bitmap).
    private static ByteMatrix RenderResult(QRCode code, int width, int height)
    {
        var input = code.GetMatrix();
        var inputWidth = input.GetWidth();
        var inputHeight = input.GetHeight();
        var qrWidth = inputWidth + (QUIET_ZONE_SIZE << 1);
        var qrHeight = inputHeight + (QUIET_ZONE_SIZE << 1);
        var outputWidth = Math.Max(width, qrWidth);
        var outputHeight = Math.Max(height, qrHeight);

        var multiple = Math.Min(outputWidth / qrWidth, outputHeight / qrHeight);
        // Padding includes both the quiet zone and the extra white pixels to accommodate the requested
        // dimensions. For example, if input is 25x25 the QR will be 33x33 including the quiet zone.
        // If the requested size is 200x160, the multiple will be 4, for a QR of 132x132. These will
        // handle all the padding from 100x100 (the actual QR) up to 200x160.
        var leftPadding = (outputWidth - inputWidth * multiple) / 2;
        var topPadding = (outputHeight - inputHeight * multiple) / 2;

        var output = new ByteMatrix(outputWidth, outputHeight);
        var outputArray = output.GetArray();

        // We could be tricky and use the first row in each set of multiple as the temporary storage,
        // instead of allocating this separate array.
        var row = new sbyte[outputWidth];

        // 1. Write the white lines at the top
        for (var y = 0; y < topPadding; y++)
        {
            SetRowColor(outputArray[y], -1);
        }

        // 2. Expand the QR image to the multiple
        var inputArray = input.GetArray();
        for (var y = 0; y < inputHeight; y++)
        {
            // a. Write the white pixels at the left of each row
            for (var x = 0; x < leftPadding; x++)
            {
                row[x] = -1;
            }

            // b. Write the contents of this row of the barcode
            var offset = leftPadding;
            for (var x = 0; x < inputWidth; x++)
            {
                var value = inputArray[y][x] == 1 ? (sbyte)0 : (sbyte)-1;
                for (var z = 0; z < multiple; z++)
                {
                    row[offset + z] = value;
                }

                offset += multiple;
            }

            // c. Write the white pixels at the right of each row
            offset = leftPadding + inputWidth * multiple;
            for (var x = offset; x < outputWidth; x++)
            {
                row[x] = -1;
            }

            // d. Write the completed row multiple times
            offset = topPadding + y * multiple;
            for (var z = 0; z < multiple; z++)
            {
                Array.Copy(row, 0, outputArray[offset + z], 0, outputWidth);
            }
        }

        // 3. Write the white lines at the bottom
        var offset2 = topPadding + inputHeight * multiple;
        for (var y = offset2; y < outputHeight; y++)
        {
            SetRowColor(outputArray[y], -1);
        }

        return output;
    }

    private static void SetRowColor(sbyte[] row, sbyte value)
    {
        for (var x = 0; x < row.Length; x++)
        {
            row[x] = value;
        }
    }
}