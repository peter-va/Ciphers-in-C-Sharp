using System;
using System.IO;
using System.Collections.Generic;

namespace CSharp{
    class Caesar:Cipher{
        public static int calcFreq(string toCrypto) //uses frequency analysis to find most likely shift for enciphered text
        {
            toCrypto = toCrypto.ToLower(); //convert all to lowercase for simplicity
            double[] letterFreqs = {82,15,28,43,127,22,20,61,70,2,8,40,24,67,75,19,1,60,63,91,28,10,24,2,20,1}; //letter frequencies per 1000 letters in English (approximate)
            double[] shiftFreqs = new double[26];
            int bestShift = 0; //hold best shift value
            int totalAlphas = 0; //total alphabetic characters in message (for frequency calculations
            double bestShiftTotal = Double.MaxValue;
            double totalDiff = 0;
            string bestShiftString = null; //hold decrypted string for best fit
            Dictionary<char,char> shifts = new Dictionary<char,char>();
            for(int i = 0;i<26;i++){ //for each shift value 0 to 25
                totalDiff = 0;
                Array.Clear(shiftFreqs,0,shiftFreqs.Length);
                shifts = buildDecryptKey(i+1); //build decipher key for value
                System.Text.StringBuilder cryptoed = new System.Text.StringBuilder();
                char newChar = 'a';
                Console.WriteLine(toCrypto.Length);
                for(int j = 0;j<toCrypto.Length;j++){ //decipher with current shift value
                    if(char.IsLetter(toCrypto[j])){ //only shift alphabetic characters
                        newChar = shifts[toCrypto[j]];
                        cryptoed.Append(newChar);
                        shiftFreqs[25-(122-(int)newChar)]++;
                        totalAlphas++;
                    }
                    else{
                        cryptoed.Append(toCrypto[j]); //put non alphabetic characters back in
                    }
                }
                for(int k = 0;k<26;k++){
                    shiftFreqs[k]*=((float)1000/(float)totalAlphas); //normalize frequencies
                    totalDiff += Math.Abs(shiftFreqs[k]-letterFreqs[k]); //compare to known frequency table and sum absolute differences
                }
                if(bestShiftTotal>totalDiff){ //new best fit
                    bestShift = i+1;
                    bestShiftTotal = totalDiff;
                    bestShiftString = cryptoed.ToString();
                }
            }
            Console.WriteLine("The most likely shift for this cipher is "+bestShift+".");
            Console.WriteLine("The message produced by this shift is:");
            Console.WriteLine("\n"+bestShiftString);
            return 0;
        }
        public static Dictionary<char,char> buildDecryptKey(int shift){
            Dictionary<char, char> decryptKey = new Dictionary<char, char>();
            for(int i = 0;i<128;i++){
                if((i >= 65) && (i <= 90)){ //uppercase
                    if(i-shift < 65)
                        decryptKey.Add((char)i, (char)(i-shift+26)); //mod back to correct shifted value
                    else
                        decryptKey.Add((char)i, (char)(i-shift));
                }
                else if((i >= 97) && (i <= 122)){ //lowercase
                    if(i-shift < 97)
                        decryptKey.Add((char)i, (char)(i-shift+26));
                    else
                        decryptKey.Add((char)i, (char)(i-shift));
                }
                else
                    decryptKey.Add((char)i, (char)i);
            }
            return decryptKey;
        }
        public static Dictionary<char,char> buildEncryptKey(int shift){
            Dictionary<char, char> encryptKey = new Dictionary<char, char>();
            for(int i = 0;i<128;i++){
                if((i >= 65) && (i <= 90)){ //uppercase
                    if(i+shift > 90)
                        encryptKey.Add((char)i, (char)(i+shift-26)); //mod back to correct shifted value
                    else
                        encryptKey.Add((char)i, (char)(i+shift));
                }
                else if((i >= 97) && (i <= 122)){ //lowercase
                    if(i+shift > 122)
                        encryptKey.Add((char)i, (char)(i+shift-26));
                    else
                        encryptKey.Add((char)i, (char)(i+shift));
                }
                else
                    encryptKey.Add((char)i, (char)i);
            }
            return encryptKey;
        }
        public static void runCrypto(string toCrypto, int shift, bool Decrypt){
            Dictionary<char, char> cryptoKey = new Dictionary<char, char>(); //key value pair for shift
            if(Decrypt == true)
                cryptoKey = buildDecryptKey(shift); 
            else
                cryptoKey = buildEncryptKey(shift); 
            System.Text.StringBuilder cryptoed = new System.Text.StringBuilder(); //string builder for result
            char newChar = 'a';
            for(int i = 0;i<toCrypto.Length;i++){ //get shifted letter for each letter in text
                newChar = cryptoKey[toCrypto[i]];
                cryptoed.Append(newChar);
            }
            string newText = cryptoed.ToString(); //convert to string for return;
            if(writeBackPrompt(newText) == false)
                Console.WriteLine(newText);
            return;
        }
        public static void Verify(bool Decrypt){
            string filename = null; //self explanatory
            string forShift = null; //self explanatory
            string toCrypto = null; //string to return
            int shift = 0;
            filePrompt(Decrypt); //prompt for file
            filename = Console.ReadLine();
            while(!File.Exists(filename)){ //check for existing file
                if(filename == "cancel")
                    return;
                filePrompt(Decrypt); //get new file if invalid
                filename = Console.ReadLine();
            }
            toCrypto = File.ReadAllText(filename); //get text from file
            if(Decrypt == true){
                bool shiftKnown = keyKnown();
                if(!shiftKnown) //find key shift through frequency analysis
                    calcFreq(toCrypto);
            }
            else
            {
                Console.WriteLine("\nEnter your desired shift amount (integer between 1 and 26): "); //get shift for enciphering
                forShift = Console.ReadLine();
                while(!int.TryParse(forShift, out shift) || shift>26 || shift<1){
                    Console.WriteLine("\nEnter your desired shift amount (integer between 1 and 26): "); //keep prompting until valid number
                    forShift = Console.ReadLine();
                }
                runCrypto(toCrypto, shift, Decrypt);
            }
        }
    }
}