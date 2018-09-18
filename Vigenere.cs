using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace CSharp{
    class Vigenere:Cipher{
        public static char getUpperChar(int i, int j){
           if(65+i+j>90)
                return (char)(65+i+j-26);
           else
                return (char)(65+i+j);
        }
        public static char getlowerChar(int i, int j){
            if(97+i+j>122)
                return (char)(97+i+j-26);
            else
                return (char)(97+i+j);
        }
        public static char[,] buildVigenereSquare(bool lower){
            char[,] vSquare = new char[26,26];
            for(int i = 0;i<26;i++){
                for(int j = 0;j<26;j++){
                    if(lower == true)
                        vSquare[i,j] = getlowerChar(i,j);
                    else
                        vSquare[i,j] = getUpperChar(i,j);
                }
            }
            return vSquare;
        }
        public static void runCrypto(string toCrypto, string keyword, bool Decrypt){
            char[,] lvSquare = buildVigenereSquare(true);
            char[,] uvSquare = buildVigenereSquare(false);
            char nextChar = '\0';
            string toReturn = null;
            int adjustForNonAlpha = 0;
            System.Text.StringBuilder cryptoed = new System.Text.StringBuilder();
            for(int i = 0;i<toCrypto.Length;i++){
                if(char.IsLower(toCrypto[i])){
                    nextChar = lvSquare[(int)toCrypto[i]-97,(int)keyword[(i-adjustForNonAlpha)%keyword.Length]-97];
                }
                else if(char.IsUpper(toCrypto[i]))
                    nextChar = uvSquare[(int)toCrypto[i]-65,(int)keyword[(i-adjustForNonAlpha)%keyword.Length]-65];
                else{
                    adjustForNonAlpha++;
                    nextChar = toCrypto[i];
                }
                cryptoed.Append(nextChar);
            }
            toReturn = cryptoed.ToString();
            if(writeBackPrompt(toReturn) == false)
                Console.WriteLine(toReturn);
        }
        public static void Verify(bool Decrypt){
            string filename = null;
            string keyword = null;
            string toCrypto = null;
            filePrompt(Decrypt);
            filename = Console.ReadLine();
            while(!File.Exists(filename)){
                if(filename == "cancel")
                    return;
                filePrompt(Decrypt);
                filename = Console.ReadLine();
            }
            toCrypto = File.ReadAllText(filename);
            toCrypto = toCrypto.ToLower();
            Console.WriteLine("\nEnter your desired keyword (letters only): ");
            keyword = Console.ReadLine();
            while(!Regex.IsMatch(keyword,@"^[a-zA-Z]+$")){
                Console.WriteLine("\nEnter your desired keyword (letters only): ");
                keyword = Console.ReadLine(); 
            }
            runCrypto(toCrypto,keyword,true);
        }
    }
}