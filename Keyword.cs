using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace CSharp{
    class Keyword:Cipher{
        public static char firstMissingChar(Dictionary<char,char> dict, bool Decrypt){
            for(int i = 0;i<26;i++){
                if(Decrypt == true){
                    if(!dict.ContainsKey((char)(97+i)))
                        return (char)(97+i);
                }
                else{
                    if(!dict.ContainsValue((char)(97+i))){
                        return (char)(97+i); }
                }
            }
            return '\0';
        }
        public static Dictionary<char,char>  buildKey(string keyword, string key, bool Decrypt){
            Dictionary<char,char> Key = new Dictionary<char, char>();
            int foundLetters = 0;
            for(int i = 0;i<keyword.Length;i++){
                if(Decrypt == true)
                    Key.Add(keyword[i],(char)(97+i));
                else
                    Key.Add((char)(97+i),keyword[i]);
            }
            for(int i = 0;i<26;i++){
                if(Decrypt == true){
                    if(!keyword.Contains((char)(97+i))){
                        Key.Add(firstMissingChar(Key, Decrypt),(char)(97+keyword.Length+foundLetters));
                        foundLetters++;
                    }
                }
                else{
                    if(!keyword.Contains((char)(97+i))){
                        Key.Add((char)(97+keyword.Length+foundLetters),firstMissingChar(Key, Decrypt));
                        foundLetters++;
                    }
                }
            }
            return Key;
        }
        public static void runCrypto(string toEncrypt, string keyword, bool Decrypt){
            Dictionary<char, char> cryptoKey = buildKey(keyword, toEncrypt, Decrypt);
            string newText = null;
            System.Text.StringBuilder cryptoed = new System.Text.StringBuilder();
            for(int i = 0;i<toEncrypt.Length;i++){
                if(!cryptoKey.ContainsKey(toEncrypt[i]))
                    cryptoed.Append(toEncrypt[i]);
                else
                    cryptoed.Append(cryptoKey[toEncrypt[i]]);
            }
            newText = cryptoed.ToString();
            if(writeBackPrompt(newText) == false)
                Console.WriteLine(newText);
        }

        public static void Verify(bool Decrypt){
            string filename = null;
            string keyword = null;
            string toEncrypt = null;
            filePrompt(Decrypt);
            filename = Console.ReadLine();
            while(!File.Exists(filename)){
                if(filename == "cancel")
                    return;
                filePrompt(Decrypt);
                filename = Console.ReadLine();
            }
            toEncrypt = File.ReadAllText(filename);
            toEncrypt = toEncrypt.ToLower();
            Console.WriteLine("\nEnter your desired keyword (letters only): ");
            keyword = Console.ReadLine();
            while(!Regex.IsMatch(keyword,@"^[a-zA-Z]+$")){
                Console.WriteLine("\nEnter your desired keyword (letters only): ");
                keyword = Console.ReadLine(); 
            }
            keyword = new string(keyword.ToCharArray().Distinct().ToArray());
            keyword = keyword.ToLower();
            runCrypto(toEncrypt, keyword, Decrypt);
        }
    }
}