using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CSharp{
    class Vigenere:Cipher{
        public static char getUpperChar(int i, int j){ //get shifted uppercase letter
           if(65+i+j>90)
                return (char)(65+i+j-26);
           else
                return (char)(65+i+j);
        }
        public static char getlowerChar(int i, int j){ //get shifted uppercase letter
            if(97+i+j>122)
                return (char)(97+i+j-26);
            else
                return (char)(97+i+j);
        }
        public static char[,] buildVigenereSquare(bool lower){ //build vignere square for upper or lowercase
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
            char[,] lvSquare = buildVigenereSquare(true); //lowercase square
            char[,] uvSquare = buildVigenereSquare(false);//uppercase square
            char nextChar = '\0';
            string toReturn = null; //set up string to return
            int adjustForNonAlpha = 0; //adjust shifting for any non-alpha characters
            System.Text.StringBuilder cryptoed = new System.Text.StringBuilder(); //string builder to build string
            for(int i = 0;i<toCrypto.Length;i++){ //use squares to shift characters
                if(char.IsLower(toCrypto[i])){ //lowercase
                    nextChar = lvSquare[(int)toCrypto[i]-97,(int)keyword[(i-adjustForNonAlpha)%keyword.Length]-97];
                }
                else if(char.IsUpper(toCrypto[i])) //uppercase
                    nextChar = uvSquare[(int)toCrypto[i]-65,(int)keyword[(i-adjustForNonAlpha)%keyword.Length]-65];
                else{ //nonalphabetic
                    adjustForNonAlpha++;
                    nextChar = toCrypto[i];
                }
                cryptoed.Append(nextChar);
            }
            toReturn = cryptoed.ToString(); //convert string builder to string
            if(writeBackPrompt(toReturn) == false) //write back prompt
                Console.WriteLine(toReturn);
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