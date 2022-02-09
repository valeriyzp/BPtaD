using System;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToInput, pathToOutput, message;
            string choice;

            while (true)
            {
                Console.Write("Encrypt/decrypt(1,0): ");
                choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Write path to input file: ");
                    pathToInput = Console.ReadLine();
                    Console.Write("Write path to output file: ");
                    pathToOutput = Console.ReadLine();
                    Console.Write("Write message: ");
                    message = Console.ReadLine();

                    if (LSBMethod.WriteMessageToBMP(pathToInput, pathToOutput, message, 4))
                        Console.WriteLine("Encoding successfull");
                    else
                        Console.WriteLine("Encoding failed");
                }
                else if (choice == "0")
                {
                    Console.Write("Write path to input file: ");
                    pathToInput = Console.ReadLine();

                    message = LSBMethod.ReadMessageFromBMP(pathToInput, 4);
                    if (message == "" || message == null)
                        Console.WriteLine("Decoding failed");
                    else
                    {
                        Console.Write("Decoding message: ");
                        Console.WriteLine(message);
                    }
                }
                else if (choice == "end")
                    break;

                Console.WriteLine();
            }
        }
    }
}
