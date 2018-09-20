using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace CSharp{
    class Keyword:Cipher{
        public static char firstMissingChar(Dictionary<char,char> dict, bool Decrypt){ //find first available character for key
            for(int i = 0;i<26;i++){
                if(Decrypt == true)
                    if(!dict.ContainsKey((char)(97+i))) //if key isn't already in dictionary
                        return (char)(97+i);
                else
                    if(!dict.ContainsValue((char)(97+i))) //if key isn't already in dictionary
                        return (char)(97+i);
            }
            return '\0';
        }
        public static Dictionary<char,char> buildKey(string keyword, string key, bool Decrypt){
            Dictionary<char,char> Key = new Dictionary<char, char>();
            int foundLetters = 0;
            for(int i = 0;i<keyword.Length;i++){ //add keyword to key
                if(Decrypt == true)
                    Key.Add(keyword[i],(char)(97+i));
                else
                    Key.Add((char)(97+i),keyword[i]);
            }
            for(int i = 0;i<26;i++){ //add the rest of the letters in the alphabet
                if(Decrypt == true){
                    if(!keyword.Contains((char)(97+i))){
                        Key.Add(firstMissingChar(Key, Decrypt),(char)(97+keyword.Length+foundLetters)); //get first alphabetic character that is missing from the key
                        foundLetters++;
                    }
                }
                else{
                    if(!keyword.Contains((char)(97+i))){
                        Key.Add((char)(97+keyword.Length+foundLetters),firstMissingChar(Key, Decrypt)); //get first alphabetic character that is missing from the key
                        foundLetters++;
                    }
                }
            }
            return Key;
        }
        public static void runCrypto(string toEncrypt, string keyword, bool Decrypt){
            Dictionary<char, char> cryptoKey = buildKey(keyword, toEncrypt, Decrypt); //build crypto key
            string newText = null;
            System.Text.StringBuilder cryptoed = new System.Text.StringBuilder(); //build string to return
            for(int i = 0;i<toEncrypt.Length;i++){
                if(!cryptoKey.ContainsKey(toEncrypt[i])) //if character not found in key
                    cryptoed.Append(toEncrypt[i]); //add character as if nothing happened
                else
                    cryptoed.Append(cryptoKey[toEncrypt[i]]); //add found character's value in key-value pair
            }
            newText = cryptoed.ToString(); //convert to string for printing/writing to file
            if(writeBackPrompt(newText) == false) //write back prompt
                Console.WriteLine(newText);
        }

        public static void Verify(bool Decrypt){
            string filename = null; //self explanatory
            string keyword = null; //self explanatory
            string toEncrypt = null; //get text for crypto
            filePrompt(Decrypt); //file prompt
            filename = Console.ReadLine();
            while(!File.Exists(filename)){ //check for valid filename
                if(filename == "cancel")
                    return;
                filePrompt(Decrypt);
                filename = Console.ReadLine();
            }
            toEncrypt = File.ReadAllText(filename); //read in text
            toEncrypt = toEncrypt.ToLower();
            Console.WriteLine("\nEnter your desired keyword (letters only): ");
            keyword = Console.ReadLine();
            while(!Regex.IsMatch(keyword,@"^[a-zA-Z]+$")){ //check for valid keyword
                Console.WriteLine("\nEnter your desired keyword (letters only): ");
                keyword = Console.ReadLine(); 
            }
            keyword = new string(keyword.ToCharArray().Distinct().ToArray()); //convert keyword to array string
            keyword = keyword.ToLower(); //convert to lowercase for simplicity
            runCrypto(toEncrypt, keyword, Decrypt);
        }
    }
}