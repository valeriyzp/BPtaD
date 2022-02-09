using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    class BaseCiphers
    {
        static private readonly string EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static private readonly string Numbers = "0123456789";
        static private readonly string Signs = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        static private readonly string FullAlphabet = Numbers + EnglishAlphabet + EnglishAlphabet.ToLower() + Signs;
        static private readonly Dictionary<char, double> FrequencyDictionary = new Dictionary<char, double>
        {
            { 'a', 0.0856}, { 'b', 0.0139}, { 'c', 0.0279}, { 'd', 0.0378},
            { 'e', 0.1304}, { 'f', 0.0289}, { 'g', 0.0199}, { 'h', 0.0528},
            { 'i', 0.0627}, { 'j', 0.0013}, { 'k', 0.0042}, { 'l', 0.0339},
            { 'm', 0.0249}, { 'n', 0.0707}, { 'o', 0.0797}, { 'p', 0.0199},
            { 'q', 0.0012}, { 'r', 0.0677}, { 's', 0.0607}, { 't', 0.1045},
            { 'u', 0.0249}, { 'v', 0.0092}, { 'w', 0.0149}, { 'x', 0.0017},
            { 'y', 0.0199}, { 'z', 0.0008}
        };


        static public string CaesarsCipher(string data, int shift, bool encode = true)
        {
            string result = "";

            int charactersQuantityInAlphabet = FullAlphabet.Length;
            shift %= charactersQuantityInAlphabet;
            for (int i = 0; i < data.Length; ++i)
            {
                char currentLetter = data[i];
                int encodeLetterIndex = FullAlphabet.IndexOf(currentLetter);
                if (encodeLetterIndex >= 0)
                {
                    int codeIndex = (charactersQuantityInAlphabet + (encodeLetterIndex + (encode ? 1 : -1) * shift) % charactersQuantityInAlphabet) % charactersQuantityInAlphabet;
                    result += FullAlphabet[codeIndex];
                }
                else result += currentLetter;
            }

            return result;
        }

        static public string CaesarsCipherUniversal(string data, int shift, bool encode = true)
        {
            string result = "";

            foreach(char ch in data)
                result += (char)(ch + (encode ? 1 : -1) * shift);

            return result;
        }

        static private int GetApproximateMaximumSCRT(int number)
        {
            number = number > 0 ? number : -number;
            int result = 1;
            for (; result * result < number; ++result) ;
            return result;
        }

        static public string PolybiusSquareCipher(string data, string key, bool encode = true)
        {
            string result = "";

            string alphabet = FullAlphabet;
            for (int i = 0; i < key.Length;)
            {
                if (alphabet.IndexOf(key[i]) >= 0)
                {
                    alphabet = alphabet.Replace(key[i].ToString(), "");
                    ++i;
                }
                else key = key.Remove(i, 1);
            }
            alphabet = key + alphabet;

            int charactersQuantityInAlphabet = alphabet.Length;
            int shift = GetApproximateMaximumSCRT(charactersQuantityInAlphabet);
            for (int i = 0; i < data.Length; ++i)
            {
                char currentLetter = data[i];
                int encodeLetterIndex = alphabet.IndexOf(currentLetter);
                if (encodeLetterIndex >= 0)
                {
                    int codeIndex;
                    if (encode) codeIndex = (encodeLetterIndex + shift) >= charactersQuantityInAlphabet ? encodeLetterIndex % shift : encodeLetterIndex + shift;
                    else
                    {
                        if ((encodeLetterIndex - shift) < 0)
                        {
                            codeIndex = encodeLetterIndex;
                            while (codeIndex + shift < charactersQuantityInAlphabet) codeIndex += shift;
                        }
                        else codeIndex = encodeLetterIndex - shift;
                    }
                    result += alphabet[codeIndex];
                }
                else result += currentLetter;
            }

            return result;
        }

        static public string TritemiusCipher(string data, int key, bool encode = true)
        {
            string result = "";

            int charactersQuantityInAlphabet = FullAlphabet.Length;
            for (int i = 0; i < data.Length; ++i)
            {
                char currentLetter = data[i];
                int encodeLetterIndex = FullAlphabet.IndexOf(currentLetter);
                if (encodeLetterIndex >= 0)
                {
                    int codeIndex = (charactersQuantityInAlphabet + (encodeLetterIndex + (encode ? -1 : 1) * i + (encode ? -1 : 1) * key) % charactersQuantityInAlphabet) % charactersQuantityInAlphabet;
                    result += FullAlphabet[codeIndex];
                }
                else result += currentLetter;
            }

            return result;
        }

        static private string GetRepeatKey(string key, int size)
        {
            string repeatKey = key;
            while (repeatKey.Length < size)
                repeatKey += key;
            repeatKey = repeatKey.Substring(0, size);

            return repeatKey;
        }

        static public string VigenereCipher(string data, string key, bool encode = true)
        {
            for (int i = 0; i < key.Length;)
                if (FullAlphabet.IndexOf(key[i]) >= 0) ++i;
                else key = key.Remove(i, 1);
            if (key.Length == 0) return data;

            string repeatKey = GetRepeatKey(key, data.Length);
            string result = "";

            int charactersQuantityInAlphabet = FullAlphabet.Length;
            for(int i = 0; i < data.Length; ++i)
            {
                char currentLetter = data[i];
                int encodeLetterIndex = FullAlphabet.IndexOf(currentLetter);
                int codeIndex = FullAlphabet.IndexOf(repeatKey[i]);
                if(encodeLetterIndex >= 0 && codeIndex >= 0)
                {
                    result += FullAlphabet[(charactersQuantityInAlphabet + encodeLetterIndex + (encode ? 1 : -1) * codeIndex) % charactersQuantityInAlphabet];
                }
                else result += currentLetter;
            }

            return result;
        }

        static public string XORCipher(string data, string key)
        {
            string repeatKey = GetRepeatKey(key, data.Length);
            string result = "";

            for (int i = 0; i < data.Length; ++i)
            {
                result += (char)(data[i] ^ repeatKey[i]);
            }

            return result;
        }

        static public string RearrangementCipherByKey(string data, string key, bool encode = true)
        {
            if (key.Length == 0) return data;
            string result = "";
            string sortKey = "";
            for (int i = 0; i < key.Length;)
            {
                if (key.IndexOf(key[i]) != i) key = key.Remove(i, 1);
                else
                {
                    int positionToInsert = 0;
                    for (; positionToInsert < i; ++positionToInsert)
                        if (sortKey[positionToInsert] > key[i]) break;
                    sortKey = sortKey.Insert(positionToInsert, key[i].ToString());
                    ++i;
                }
            }
            int charactersQuantityInKey = key.Length;

            int shift = 0;
            while (shift < data.Length)
            {
                if (encode)
                {
                    for (int i = 0; i < key.Length; ++i)
                    {
                        if (shift + key.IndexOf(sortKey[i]) < data.Length)
                            result += data[shift + key.IndexOf(sortKey[i])];
                    }
                }
                else
                {
                    if (shift + key.Length > data.Length)
                    {
                        for (int j = data.Length - shift; j < key.Length; ++j)
                            sortKey = sortKey.Remove(sortKey.IndexOf(key[j]), 1);
                    }
                    for (int i = 0; i < sortKey.Length; ++i)
                        result += data[shift + sortKey.IndexOf(key[i])];
                }
                shift += key.Length;
            }

            return result;
        }

        static private List<Tuple<char, double>> DictionaryToList(Dictionary<char, double> dict)
        {
            List<Tuple<char, double>> list = new List<Tuple<char, double>>();

            foreach (char key in dict.Keys)
                list.Add(new Tuple<char, double>(key, dict[key]));

            return list;
        }

        static private List<Tuple<char, double>> SortListByFrequency(List<Tuple<char, double>> list)
        {
            for (int i = 0; i < list.Count; ++i)
                for (int j = list.Count - 1; j > i; --j)
                    if (list[j].Item2 > list[j - 1].Item2)
                    {
                        Tuple<char, double> temp = list[j];
                        list[j] = list[j - 1];
                        list[j - 1] = temp;
                    }
            return list;
        }

        static public string SimpleFrequencyAnalysis(string data)
        {
            string resData = "";
            if (data.Length < 1 || data == null) return "";

            int encodedCharactersQuantityInAlphabet = 0;
            Dictionary<char, double> DataCharFrequency = new Dictionary<char, double>();

            foreach (char ch in data)
            {
                if (DataCharFrequency.ContainsKey(ch))
                    ++DataCharFrequency[ch];
                else DataCharFrequency[ch] = 1;
                ++encodedCharactersQuantityInAlphabet;
            }

            foreach (char key in DataCharFrequency.Keys.ToList())
                DataCharFrequency[key] = DataCharFrequency[key] / encodedCharactersQuantityInAlphabet;

            List<Tuple<char, double>> sortedDataCharFrequency = SortListByFrequency(DictionaryToList(DataCharFrequency));
            List<Tuple<char, double>> sortedCharFriquency = SortListByFrequency(DictionaryToList(FrequencyDictionary));
            Dictionary<char, char> encoder = new Dictionary<char, char>();

            for (int i = 0; i < (sortedDataCharFrequency.Count > sortedCharFriquency.Count ? sortedCharFriquency.Count : sortedDataCharFrequency.Count); ++i)
                encoder[sortedDataCharFrequency[i].Item1] = sortedCharFriquency[i].Item1;

            for (int i = 0; i < data.Length; ++i)
                if (encoder.ContainsKey(data[i])) resData += encoder[data[i]];
                else resData += data[i];

            return resData;
        }
    }
}
