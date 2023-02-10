using System.Collections.Generic;

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
 * Encapsulates a Character Set ECI, according to "Extended Channel Interpretations" 5.3.1.1
 * of ISO 18004.
 * 
 * @author Sean Owen
 */
public class CharacterSetECI
{
    private static Dictionary<string, CharacterSetECI> NAME_TO_ECI;

    private readonly string encodingName;
    private readonly int value;

    private CharacterSetECI(int value, string encodingName)
    {
        this.value = value;
        this.encodingName = encodingName;
    }

    private static void Initialize()
    {
        var n = new Dictionary<string, CharacterSetECI>();
        // TODO figure out if these values are even right!
        AddCharacterSet(0, "Cp437", n);
        AddCharacterSet(1, new[] { "ISO8859_1", "ISO-8859-1" }, n);
        AddCharacterSet(2, "Cp437", n);
        AddCharacterSet(3, new[] { "ISO8859_1", "ISO-8859-1" }, n);
        AddCharacterSet(4, new[] { "ISO8859_2", "ISO-8859-2" }, n);
        AddCharacterSet(5, new[] { "ISO8859_3", "ISO-8859-3" }, n);
        AddCharacterSet(6, new[] { "ISO8859_4", "ISO-8859-4" }, n);
        AddCharacterSet(7, new[] { "ISO8859_5", "ISO-8859-5" }, n);
        AddCharacterSet(8, new[] { "ISO8859_6", "ISO-8859-6" }, n);
        AddCharacterSet(9, new[] { "ISO8859_7", "ISO-8859-7" }, n);
        AddCharacterSet(10, new[] { "ISO8859_8", "ISO-8859-8" }, n);
        AddCharacterSet(11, new[] { "ISO8859_9", "ISO-8859-9" }, n);
        AddCharacterSet(12, new[] { "ISO8859_10", "ISO-8859-10" }, n);
        AddCharacterSet(13, new[] { "ISO8859_11", "ISO-8859-11" }, n);
        AddCharacterSet(15, new[] { "ISO8859_13", "ISO-8859-13" }, n);
        AddCharacterSet(16, new[] { "ISO8859_14", "ISO-8859-14" }, n);
        AddCharacterSet(17, new[] { "ISO8859_15", "ISO-8859-15" }, n);
        AddCharacterSet(18, new[] { "ISO8859_16", "ISO-8859-16" }, n);
        AddCharacterSet(20, new[] { "SJIS", "Shift_JIS" }, n);
        NAME_TO_ECI = n;
    }

    public virtual string GetEncodingName() => encodingName;

    public virtual int GetValue() => value;

    private static void AddCharacterSet(int value, string encodingName, Dictionary<string, CharacterSetECI> n)
    {
        var eci = new CharacterSetECI(value, encodingName);
        n[encodingName] = eci;
    }

    private static void AddCharacterSet(int value, string[] encodingNames, Dictionary<string, CharacterSetECI> n)
    {
        var eci = new CharacterSetECI(value, encodingNames[0]);
        for (var i = 0; i < encodingNames.Length; i++)
        {
            n[encodingNames[i]] = eci;
        }
    }

    /**
     * @param name character set ECI encoding name
     * @return {@link CharacterSetECI} representing ECI for character encoding, or null if it is legal
     * but unsupported
     */
    public static CharacterSetECI GetCharacterSetECIByName(string name)
    {
        if (NAME_TO_ECI == null)
        {
            Initialize();
        }

        CharacterSetECI c;
        NAME_TO_ECI.TryGetValue(name, out c);
        return c;
    }
}