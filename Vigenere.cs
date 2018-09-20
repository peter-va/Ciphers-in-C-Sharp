using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharp{
    class Vigenere:Cipher{
        public static string getKnownKeyword()
        {
            Console.WriteLine("\nEnter your desired keyword (letters only): ");
            string keyword = Console.ReadLine();
            while (!Regex.IsMatch(keyword, @"^[a-zA-Z]+$"))
            {
                Console.WriteLine("\nEnter your desired keyword (letters only): ");
                keyword = Console.ReadLine();
            }
            return keyword;
        }
        public static void getKeyword(string toDecipher, int keylength)
        {
            StringBuilder keywordBuild = new StringBuilder();
            char nextChar;
            string currAlphabet;
            for(int i = 0; i < keylength; i++)
            {
                currAlphabet = getAlphabet(toDecipher, i, keylength);
                nextChar = (char)(Caesar.calcFreq(currAlphabet, true) + 97);
                keywordBuild.Append(nextChar);
            }
            string keyword = keywordBuild.ToString();
            Console.WriteLine("The most likely keyword is " + keyword);
            runCrypto(toDecipher, keyword, true);
        }
        public static string getAlphabet(string text, int offset, int interval)
        {
            int index = offset; //hold initial index for alphabet
            Regex rgx = new Regex("[^a-z -]");
            text = rgx.Replace(text, "");
            text = text.Replace(" ", "");
            StringBuilder stringer = new StringBuilder();
            while(text.Length >= index+1)
            {
                stringer.Append(text[index]); //add each n-th character
                index += interval;
            }
            return stringer.ToString();
        }
        public static int indexOfCoincidence(string text, int maxLen)
        {
            text = string.Concat(text.Where(c => !char.IsWhiteSpace(c))); //strip whitespace
            int keylen = 0; //most likely key length
            double keylenvalue = 0; //ioc value for most likely keylength
            double freqs = 0; //calculating ioc values
            for (int i = 1; i <= maxLen; i++) //go through all key lengths up to max
            {
                freqs = 0; //reset ioc value
                for (int k = 0; k < i; k++) //get each sub-alphabet for each key length
                {
                    string newText = getAlphabet(text, k, i); //getting the sub-alphabet
                    Regex rgx = new Regex("[^a-z -]");
                    newText = rgx.Replace(newText, ""); //strip non alphabetic characters
                    double charCount = 0; //count of each character
                    double stringLen = newText.Length; //length of sub-alphabet
                    for (int j = 0; j < 26; j++) //for each letter
                    {
                        charCount = newText.Count(x => x == (char)(97 + j));
                        freqs += (charCount / stringLen) * ((charCount - 1) / (stringLen - 1)); //sum up all sub-alphabets for average
                    }                    
                }
                freqs /= (double)i; //divide by key length for average
                if (freqs > keylenvalue)
                {
                    keylenvalue = freqs; //new highest average ioc
                    keylen = i; //new most likely key length
                }
            }
            return keylen;
        }
        public static char convertChar(char toConvert, char keyChar, bool Decrypt, bool isUpper)
        {
            char toReturn;
            if (Decrypt)
                toReturn = (char)(toConvert - (keyChar - 97));
            else
                toReturn = (char)(toConvert + (keyChar - 97));
            if (toReturn > (char)122)
                toReturn -= (char)26;
            else if (toReturn < (char)97)
                toReturn += (char)26;
            return isUpper ? char.ToUpper(toReturn) : toReturn;
        }
        public static string runCrypto(string toCrypto, string keyword, bool Decrypt, bool solving = false){
            char nextChar = '\0';
            int numAlpha = 0; //account for nonalphabetic characters
            string toReturn = null; //set up string to return
            StringBuilder cryptoed = new StringBuilder(); //string builder to build string
            for(int i = 0;i<toCrypto.Length;i++){ //use squares to shift characters
                if(char.IsLetter(toCrypto[i])) //letter
                    nextChar = convertChar(toCrypto[i], keyword[numAlpha++ % keyword.Length], Decrypt, char.IsUpper(toCrypto[i]));
                else //nonletter
                    nextChar = toCrypto[i];
                cryptoed.Append(nextChar);
            }
            toReturn = cryptoed.ToString(); //convert string builder to string
            if(!solving)
                if(writeBackPrompt(toReturn) == false) //write back prompt
                    Console.WriteLine(toReturn);
            return toReturn;
        }
        public static void Verify(bool Decrypt){
            string filename = null; //self explanatory
            string keyword = null; //self explanatory
            string toCrypto = null;//hold string for crypto-ing
            filePrompt(Decrypt);
            filename = Console.ReadLine();
            while(!File.Exists(filename)){ //check for valid filename
                if(filename == "cancel")
                    return;
                filePrompt(Decrypt);
                filename = Console.ReadLine();
            }
            toCrypto = File.ReadAllText(filename); //get text from file
            toCrypto = toCrypto.ToLower();
            if (Decrypt)
            {
                if (keyKnown())
                {
                    keyword = getKnownKeyword();
                    runCrypto(toCrypto, keyword, true);
                }
                else
                {
                    Console.WriteLine("\nEnter the maximum length keyword to check: ");
                    int maxLen = 0;
                    string max = Console.ReadLine();
                    while (!int.TryParse(max, out maxLen))
                    {
                        Console.WriteLine("\nEnter the maximum length keyword to check: ");
                        max = Console.ReadLine();
                    }
                    int ioc = indexOfCoincidence(toCrypto, maxLen);
                    Console.WriteLine("\nThe most likely keylength is " + ioc);
                    getKeyword(toCrypto, ioc);
                    return;
                }
            }
            else
            {
                keyword = getKnownKeyword();
                runCrypto(toCrypto, keyword, false);
            }
        }
    }
}