using System;
using System.Text;

class Zadacha1
{
   /* static void Main()
    {
        string input = Console.ReadLine();
        Console.WriteLine($"Сжатая строка: {Compress(input)}");
    }*/
    //реализация сжатия с использованием ASCII
    static string Compress(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length == 1)
            return input;
        StringBuilder result = new StringBuilder();
        //чтение и считывание элементов массива
        int[] AsciiCount = new int[125];
        for (int i = 0; i < input.Length; i++)
        {
            int letterToNum = (int)input[i];
            AsciiCount[letterToNum]++;
        }
        //создание результата в формате sc
        //122 - ascii z
        for (int i = 97; i <= 122; i++)
            if( AsciiCount[i] > 0)
            {
                char letter = (char)i;
                result.Append(letter);
                result.Append(AsciiCount[i]);
            }
        
        return result.ToString();
    }

   

      
}