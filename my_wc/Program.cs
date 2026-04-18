using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace my_wc
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // Викликаємо функціональне ядро з потоками за замовчуванням
            return Run(args, Console.In, Console.Out, Console.Error);
        }

        // Головна функція, яка приймає залежності (потоки), що робить її легкою для тестування
        public static int Run(string[] args, TextReader inReader, TextWriter outWriter, TextWriter errWriter)
        {
            var (options, files, parseError) = ParseArgs(args);

            if (parseError != null)
            {
                errWriter.WriteLine($"Error: {parseError}");
                return 2; // Код 2: неправильні аргументи
            }

            // Якщо опції не вказані, типова поведінка wc: виводити все (-l -w -c)
            bool showLines = options.Contains("-l");
            bool showWords = options.Contains("-w");
            bool showBytes = options.Contains("-c");
            
            if (!showLines && !showWords && !showBytes)
            {
                showLines = showWords = showBytes = true;
            }

            int exitCode = 0;

            // Форматування виводу результату
            Action<long, long, long, string> printResult = (lines, words, bytes, name) =>
            {
                var parts = new List<string>();
                if (showLines) parts.Add(lines.ToString());
                if (showWords) parts.Add(words.ToString());
                if (showBytes) parts.Add(bytes.ToString());
                if (!string.IsNullOrEmpty(name)) parts.Add(name);
                
                outWriter.WriteLine(string.Join(" ", parts));
            };

            // Читання зі stdin, якщо файли не передані
            if (files.Count == 0)
            {
                // Для stdin байт-рахунок може відрізнятися від оригінального файлу через кодування, 
                // але для CLI-утиліти зчитування в пам'ять (або через стрічку) є прийнятним.
                string input = inReader.ReadToEnd();
                var stats = CountStats(input);
                printResult(stats.Lines, stats.Words, stats.Bytes, "");
                return 0;
            }

            // Обробка файлів
            long totalLines = 0, totalWords = 0, totalBytes = 0;
            foreach (var file in files)
            {
                try
                {
                    if (!File.Exists(file))
                    {
                        errWriter.WriteLine($"my_wc: {file}: No such file or directory");
                        exitCode = 1; // Код 1: часткова помилка
                        continue;
                    }

                    string text = File.ReadAllText(file);
                    var stats = CountStats(text);
                    
                    // У Linux wc рахує байти безпосередньо з файлу:
                    long fileBytes = new FileInfo(file).Length; 
                    long bytesToReport = showBytes ? fileBytes : stats.Bytes;

                    printResult(stats.Lines, stats.Words, bytesToReport, file);

                    totalLines += stats.Lines;
                    totalWords += stats.Words;
                    totalBytes += bytesToReport;
                }
                catch (Exception ex)
                {
                    errWriter.WriteLine($"my_wc: {file}: {ex.Message}");
                    exitCode = 1;
                }
            }

            // Вивід підсумків, якщо файлів більше одного
            if (files.Count > 1)
            {
                printResult(totalLines, totalWords, totalBytes, "total");
            }

            return exitCode;
        }

        // Чиста функція для розбору аргументів
        private static (HashSet<string> Options, List<string> Files, string Error) ParseArgs(string[] args)
        {
            var options = new HashSet<string>();
            var files = new List<string>();
            var validOptions = new[] { "-l", "-w", "-c" };

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if (!validOptions.Contains(arg))
                        return (null, null, $"Unknown option: {arg}");
                    
                    options.Add(arg);
                }
                else
                {
                    files.Add(arg);
                }
            }

            return (options, files, null);
        }

        // Чиста функція для підрахунку (без стану)
        private static (long Lines, long Words, long Bytes) CountStats(string text)
        {
            if (string.IsNullOrEmpty(text)) return (0, 0, 0);

            long bytes = System.Text.Encoding.UTF8.GetByteCount(text);
            long lines = text.Count(c => c == '\n');
            
            // Простий підрахунок слів (за пробілами)
            long words = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

            return (lines, words, bytes);
        }
    }
}