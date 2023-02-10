using System.Text;

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
 * A class which wraps a 2D array of bytes. The default usage is signed. If you want to use it as a
 * unsigned container, it's up to you to do byteValue & 0xff at each location.
 * 
 * JAVAPORT: The original code was a 2D array of ints, but since it only ever gets assigned
 * -1, 0, and 1, I'm going to use less memory and go with bytes.
 * 
 * @author dswitkin@google.com (Daniel Switkin)
 */
public sealed class ByteMatrix
{
    private readonly sbyte[][] bytes;
    private readonly int height;
    private readonly int width;

    public ByteMatrix(int width, int height)
    {
        bytes = new sbyte[height][];
        for (var k = 0; k < height; ++k)
        {
            bytes[k] = new sbyte[width];
        }

        this.width = width;
        this.height = height;
    }

    public int GetHeight() => height;

    public int GetWidth() => width;

    public sbyte Get(int x, int y) => bytes[y][x];

    public sbyte[][] GetArray() => bytes;

    public void Set(int x, int y, sbyte value)
    {
        bytes[y][x] = value;
    }

    public void Set(int x, int y, int value)
    {
        bytes[y][x] = (sbyte)value;
    }

    public void Clear(sbyte value)
    {
        for (var y = 0; y < height; ++y)
        {
            for (var x = 0; x < width; ++x)
            {
                bytes[y][x] = value;
            }
        }
    }

    public override string ToString()
    {
        var result = new StringBuilder(2 * width * height + 2);
        for (var y = 0; y < height; ++y)
        {
            for (var x = 0; x < width; ++x)
            {
                switch (bytes[y][x])
                {
                    case 0:
                        result.Append(" 0");
                        break;
                    case 1:
                        result.Append(" 1");
                        break;
                    default:
                        result.Append("  ");
                        break;
                }
            }

            result.Append('\n');
        }

        return result.ToString();
    }
}