using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogStandardizer
{
    class Program
    {
        static void Main(string[] args)
        {
            

            string inputPath = "input.txt";
            string outputPath = "output.txt";
            string problemsPath = "problems.txt";
            
            try
            {
                using (var reader = new StreamReader(inputPath))
                using (var outputWriter = new StreamWriter(outputPath))
                using (var problemsWriter = new StreamWriter(problemsPath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        ProcessLine(line, outputWriter, problemsWriter);
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void ProcessLine(string line, StreamWriter outputWriter, StreamWriter problemsWriter)
        {
            if (TryParseFormat1(line, out var parsed1) && IsValidLogLevel(parsed1.LogLevel))
            {
                WriteParsedLog(parsed1, outputWriter);
            }
            else if (TryParseFormat2(line, out var parsed2) && IsValidLogLevel(parsed2.LogLevel))
            {
                WriteParsedLog(parsed2, outputWriter);
            }
            else
            {
                problemsWriter.WriteLine(line);
            }
        }

        private static bool TryParseFormat1(string line, out ParsedLog parsed)
        {
            parsed = null;
            string[] tokens = line.Split(new[] { ' ' }, 4); // Split into max 4 parts

            if (tokens.Length < 3)
                return false;

            string dateStr = tokens[0];
            string timeStr = tokens[1];
            string levelStr = tokens[2];
            string message = tokens.Length > 3 ? tokens[3] : string.Empty;

            if (!DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return false;

            if (!IsValidTime(timeStr))
                return false;

            string mappedLevel = levelStr switch
            {
                "INFORMATION" => "INFO",
                "WARNING" => "WARN",
                _ => levelStr
            };

            parsed = new ParsedLog
            {
                Date = date.ToString("yyyy-MM-dd"),
                Time = timeStr,
                LogLevel = mappedLevel,
                CallingMethod = "DEFAULT",
                Message = message
            };

            return true;
        }

        private static bool TryParseFormat2(string line, out ParsedLog parsed)
        {
            parsed = null;
            string[] parts = line.Split('|');

            if (parts.Length < 5)
                return false;

            string dateTimePart = parts[0].Trim();
            string levelPart = parts[1].Trim();
            string callingMethodPart = parts[3].Trim();
            string messagePart = parts[4].Trim();

            string[] dateTimeTokens = dateTimePart.Split(new[] { ' ' }, 2);
            if (dateTimeTokens.Length != 2)
                return false;

            string dateStr = dateTimeTokens[0];
            string timeStr = dateTimeTokens[1];

            if (!DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return false;

            if (!IsValidTime(timeStr))
                return false;

            if (!new HashSet<string> { "INFO", "WARN", "ERROR", "DEBUG" }.Contains(levelPart))
                return false;

            parsed = new ParsedLog
            {
                Date = date.ToString("yyyy-MM-dd"),
                Time = timeStr,
                LogLevel = levelPart,
                CallingMethod = string.IsNullOrEmpty(callingMethodPart) ? "DEFAULT" : callingMethodPart,
                Message = messagePart
            };

            return true;
        }

        private static bool IsValidTime(string timeStr)
        {
            return Regex.IsMatch(timeStr, @"^([01]\d|2[0-3]):([0-5]\d):([0-5]\d)(\.\d+)?$");
        }

        private static bool IsValidLogLevel(string level)
        {
            return new HashSet<string> { "INFO", "WARN", "ERROR", "DEBUG" }.Contains(level);
        }

        private static void WriteParsedLog(ParsedLog parsed, StreamWriter writer)
        {
            writer.WriteLine($"{parsed.Date}\t{parsed.Time}\t{parsed.LogLevel}\t{parsed.CallingMethod}\t{parsed.Message}");
        }
    }

    class ParsedLog
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string LogLevel { get; set; }
        public string CallingMethod { get; set; }
        public string Message { get; set; }
    }
}