using System;
using System.IO;
using System.Collections.Generic;

namespace CSharp{
    class Caesar:Cipher{
        public static int getShift()
        {
            string forShift;
            int shift;
            Console.WriteLine("\nEnter your desired shift amount (integer between 1 and 26): "); //get shift for enciphering
            forShift = Console.ReadLine();
            while (!int.TryParse(forShift, out shift) || shift > 26 || shift < 1)
            {
                Console.WriteLine("\nEnter your desired shift amount (integer between 1 and 26): "); //keep prompting until valid number
                forShift = Console.ReadLine();
            }
            return shift;
        }
        public static int calcFreq(string toCrypto, bool vignere = false) //uses frequency analysis to find most likely shift for enciphered text
        {
            toCrypto = toCrypto.ToLower(); //convert all to lowercase for simplicity
            double[] shiftFreqs = new double[26];
            int bestShift = 0; //hold best shift value
            int totalAlphas = 0; //total alphabetic characters in message (for frequency calculations
            double bestShiftTotal = Double.MaxValue;
            double totalDiff = 0; //holding abs difference for each shift
            string bestShiftString = null; //hold decrypted string for best fit
            Dictionary<char,char> shifts = new Dictionary<char,char>();
            for(int i = 0;i<26;i++){ //for each shift value 0 to 25
                totalDiff = 0;
                totalAlphas = 0;
                Array.Clear(shiftFreqs,0,shiftFreqs.Length);
                //shifts = buildDecryptKey(i+1); //build decipher key for value
                System.Text.StringBuilder cryptoed = new System.Text.StringBuilder();
                char toAdd;
                for(int j = 0;j<toCrypto.Length;j++){ //decipher with current shift value
                    if(char.IsLetter(toCrypto[j])){ //only shift alphabetic characters
                        toAdd = getShiftedChar(toCrypto[j], i, char.IsUpper(toCrypto[j]), true); //get shifted character
                        cryptoed.Append(toAdd);
                        shiftFreqs[25-(122-(int)toAdd)]++; //increment frequency of character
                        totalAlphas++; //one more alphabetic character (for normalizing frequencies)
                    }
                    else{
                        cryptoed.Append(toCrypto[j]); //put non alphabetic characters back in
                    }
                }
                for(int k = 0;k<26;k++){
                    shiftFreqs[k]*=((float)1000/(float)totalAlphas); //normalize frequencies
                    totalDiff += Math.Abs(shiftFreqs[k]-letterFreqs[k]); //compare to known frequency table and sum absolute differences
                }
                if (bestShiftTotal>totalDiff){ //new best fit
                    bestShift = i;
                    bestShiftTotal = totalDiff;
                    bestShiftString = cryptoed.ToString();
                }
            }
            if (vignere)
                return bestShift;
            Console.WriteLine("The most likely shift for this cipher is "+bestShift+".");
            Console.WriteLine("The message produced by this shift is:");
            Console.WriteLine("\n"+bestShiftString);
            return 0;
        }
        public static char getShiftedChar(char toShift, int shift, bool isUpper, bool Decrypt)
        {
            if (!char.IsLetter(toShift)) //return nonalphabetic characters
                return toShift;
            char shiftChar = isUpper ? 'A' : 'a'; //check case
            if (Decrypt)
            {
                return (char)((((toShift + (26 - shift)) - shiftChar) % 26) + shiftChar); // shift char -> subtract to mod around 26 for simplicity -> add back amount after mod
            }
            else
            {
                return (char)((((toShift + shift) - shiftChar) % 26) + shiftChar); //shift other way on encrypt
            }
        }
        public static void runCrypto(string toCrypto, int shift, bool Decrypt){
            System.Text.StringBuilder cryptoed = new System.Text.StringBuilder(); //string builder for result
            for(int i = 0;i<toCrypto.Length;i++){ //get shifted letter for each letter in text
                cryptoed.Append(getShiftedChar(toCrypto[i], shift, char.IsUpper(toCrypto[i]), Decrypt)); //get shifted character
            }
            string newText = cryptoed.ToString(); //convert to string for return;
            if(writeBackPrompt(newText) == false)
                Console.WriteLine(newText);
            return;
        }
        public static void Verify(bool Decrypt){
            string filename = null; //self explanatory
            string toCrypto = null; //string to return
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
                else
                    runCrypto(toCrypto, getShift(), Decrypt);
            }
            else
                runCrypto(toCrypto, getShift(), Decrypt);
        }
    }
}