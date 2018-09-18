using System;
using System.IO;
using System.Collections.Generic;

namespace CSharp{
    class Caesar:Cipher{
        public static int calcFreq(string toCrypto)
        {
            toCrypto = toCrypto.ToLower();
            double[] letterFreqs = {82,15,28,43,127,22,20,61,70,2,8,40,24,67,75,19,1,60,63,91,28,10,24,2,20,1};
            double[] shiftFreqs = new double[26];
            int bestShift = 0;
            int totalAlphas = 0;
            double bestShiftTotal = Double.MaxValue;
            double totalDiff = 0;
            string bestShiftString = null;
            Dictionary<char,char> shifts = new Dictionary<char,char>();
            for(int i = 0;i<26;i++){
                totalDiff = 0;
                Array.Clear(shiftFreqs,0,shiftFreqs.Length);
                shifts = buildDecryptKey(i+1);
                System.Text.StringBuilder cryptoed = new System.Text.StringBuilder();
                char newChar = 'a';
                Console.WriteLine(toCrypto.Length);
                for(int j = 0;j<toCrypto.Length;j++){
                    if(char.IsLetter(toCrypto[j])){
                        newChar = shifts[toCrypto[j]];
                        cryptoed.Append(newChar);
                        shiftFreqs[25-(122-(int)newChar)]++;
                        totalAlphas++;
                    }
                    else{
                        cryptoed.Append(toCrypto[j]);
                    }
                }
                for(int k = 0;k<26;k++){
                    shiftFreqs[k]*=((float)1000/(float)totalAlphas);
                    totalDiff += Math.Abs(shiftFreqs[k]-letterFreqs[k]);
                }
                if(bestShiftTotal>totalDiff){
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
                if((i >= 65) && (i <= 90)){
                    if(i-shift < 65)
                        decryptKey.Add((char)i, (char)(i-shift+26));
                    else
                        decryptKey.Add((char)i, (char)(i-shift));
                }
                else if((i >= 97) && (i <= 122)){
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
                if((i >= 65) && (i <= 90)){
                    if(i+shift > 90)
                        encryptKey.Add((char)i, (char)(i+shift-26));
                    else
                        encryptKey.Add((char)i, (char)(i+shift));
                }
                else if((i >= 97) && (i <= 122)){
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
            Dictionary<char, char> cryptoKey = new Dictionary<char, char>();
            if(Decrypt == true)
                cryptoKey = buildDecryptKey(shift); 
            else
                cryptoKey = buildEncryptKey(shift); 
            System.Text.StringBuilder cryptoed = new System.Text.StringBuilder();
            char newChar = 'a';
            for(int i = 0;i<toCrypto.Length;i++){
                newChar = cryptoKey[toCrypto[i]];
                cryptoed.Append(newChar);
            }
            string newText = cryptoed.ToString();
            if(writeBackPrompt(newText) == false)
                Console.WriteLine(newText);
            return;
        }
        public static void Verify(bool Decrypt){
            string filename = null;
            string forShift = null;
            string toCrypto = null;
            int shift = 0;
            filePrompt(Decrypt);
            filename = Console.ReadLine();
            while(!File.Exists(filename)){
                if(filename == "cancel")
                    return;
                filePrompt(Decrypt);
                filename = Console.ReadLine();
            }
            toCrypto = File.ReadAllText(filename);
            if(Decrypt == true){
                string freq = "";
                Console.Write("Enter 'y' if you know the shift frequency (any other key if not): ");
                freq = Console.ReadLine();
                if(freq != "y")
                    calcFreq(toCrypto);
            }
            else
            {
                Console.WriteLine("\nEnter your desired shift amount (integer between 1 and 26): ");
                forShift = Console.ReadLine();
                while(!int.TryParse(forShift, out shift) || shift>26 || shift<1){
                    Console.WriteLine("\nEnter your desired shift amount (integer between 1 and 26): ");
                    forShift = Console.ReadLine();
                }
            runCrypto(toCrypto, shift, Decrypt);
            }
        }
    }
}