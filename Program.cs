using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace CSharp
{
    abstract class Cipher
    {
        public static void filePrompt(bool Decrypt){
            if(Decrypt == true)
                Console.Write("\nEnter the name of the file you want to decrypt (including file extension) Enter \"cancel\" to cancel: "); 
            else
                Console.Write("\nEnter the name of the file you want to encrypt (including file extension) Enter \"cancel\" to cancel: ");
        }
        public static bool writeBackPrompt(string cryptoed){
            string input = null;
            Console.Write("Press 'w' to write the result to a text file. Press any other key to continue without doing so.");
            input = Console.ReadLine();
            if(input == "w"){
                Console.Write("\nEnter the desired name of your file (include the extension): ");
                input = Console.ReadLine();
                while(input.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || File.Exists(input)){
                    Console.Write("Invalid File Name! Please enter a valid file name: ");
                    input = Console.ReadLine();
                }
                File.WriteAllText(input, cryptoed);
                return true;
            }
            return false;
        }
    }
    class Program
    {
        static void VigenereHelper(){
            string vigenereInput = null;
            Console.WriteLine("\nEnter 1 for decryption, 2 for encryption, or \"cancel\" to cancel: ");
            vigenereInput = Console.ReadLine();
            switch (vigenereInput)
            {
                case "1":
                    Vigenere.Verify(true);
                    return;
                case "2":
                    Vigenere.Verify(false);
                    return;
                case "cancel":
                    return;
                default:
                    Console.WriteLine("Invalid Input!");
                    break;
            }
        }
        static void CaesarHelper(){
            string caesarInput = null;
            while(true){
                Console.WriteLine("\nEnter 1 for decryption, 2 for encryption, or \"cancel\" to cancel: ");
                caesarInput = Console.ReadLine();
                switch (caesarInput)
                {
                    case "1":
                        Caesar.Verify(true);
                        return;
                    case "2":
                        Caesar.Verify(false);
                        return;
                    case "cancel":
                        return;
                    default:
                        Console.WriteLine("Invalid Input!");
                        break;
                }
            }
        }
        static void KeywordHelper(){
            string keywordInput = null;
            while(true){
                Console.WriteLine("\nEnter 1 for decryption, 2 for encryption, or \"cancel\" to cancel: ");
                keywordInput = Console.ReadLine();
                switch (keywordInput)
                {
                    case "1":
                        Keyword.Verify(true);
                        return;
                    case "2":
                        Keyword.Verify(false);
                        return;
                    case "cancel":
                        return;
                    default:
                        Console.WriteLine("Invalid Input!");
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my cipher program!");
            Console.WriteLine("This program is capable of encrypting and decrypting:");
            Console.WriteLine("1. A Caesar Cipher.");
            Console.WriteLine("2. A keyword Cipher.");
            Console.WriteLine("3. A Vigenere Cipher.");
            string userInput = null;
            while(userInput!="exit"){
                Console.Write("\nEnter 1 for Caesar cipher, 2 for keyword cipher, 3 for Vigenere cipher, "
                     +"or \"exit\" to exit the program: ");
                userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        CaesarHelper();
                        break;
                    case "2":
                        KeywordHelper();
                        break;
                    case "3":
                        VigenereHelper();
                        break;
                    case "exit":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.Write("Invalid Input!\n");
                        break;
                }
            }
        }
    }
}
