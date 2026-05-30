using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;

namespace ExcelToWordFiles
{
    class Program
    {
        private const string ExcelFilePath = "英语词汇例句表.xlsx";
        private const string OutputRoot = "OutputFiles";

        static void Main(string[] args)
        {
            if (!File.Exists(ExcelFilePath))
            {
                Console.WriteLine($"错误：找不到Excel文件 {ExcelFilePath}");
                return;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var wordExampleMap = new Dictionary<string, List<string>>();

            using (var package = new ExcelPackage(new FileInfo(ExcelFilePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                {
                    Console.WriteLine("错误：工作表中没有数据");
                    return;
                }

                int row = 2;
                while (worksheet.Cells[row, 2]?.Value != null)
                {
                    string word = worksheet.Cells[row, 2].Value.ToString().Trim();
                    string example = worksheet.Cells[row, 6].Value?.ToString().Trim();

                    if (!string.IsNullOrEmpty(word) && !string.IsNullOrEmpty(example))
                    {
                        string safeWord = SanitizeFileName(word);
                        if (!wordExampleMap.ContainsKey(safeWord))
                            wordExampleMap[safeWord] = new List<string>();
                        wordExampleMap[safeWord].Add(example);
                    }
                    row++;
                }
            }

            Directory.CreateDirectory(OutputRoot);

            // 1. 生成每个单词的单独 .txt 文件（按首字母分文件夹）
            foreach (var kvp in wordExampleMap)
            {
                string safeWord = kvp.Key;
                List<string> examples = kvp.Value;

                char firstChar = GetFirstLetter(safeWord);
                string targetFolder = Path.Combine(OutputRoot, firstChar.ToString().ToUpper());
                Directory.CreateDirectory(targetFolder);

                string filePath = Path.Combine(targetFolder, $"{safeWord}.txt");
                string content = string.Join("\n\n", examples);
                File.WriteAllText(filePath, content, Encoding.UTF8);
                Console.WriteLine($"已生成文本文件：{filePath}");
            }

            // 2. 按字母分组，生成 Markdown 文件（A.md ~ Z.md）
            var letterGroup = new Dictionary<char, Dictionary<string, List<string>>>();
            foreach (var kvp in wordExampleMap)
            {
                string word = kvp.Key;          // 原始单词（未清理，用于显示）
                char firstLetter = GetFirstLetter(word);
                if (char.IsLetter(firstLetter)) // 只处理字母开头
                {
                    if (!letterGroup.ContainsKey(firstLetter))
                        letterGroup[firstLetter] = new Dictionary<string, List<string>>();
                    letterGroup[firstLetter][word] = kvp.Value;
                }
            }

            string mdFolder = Path.Combine(OutputRoot, "Markdown");
            Directory.CreateDirectory(mdFolder);
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (!letterGroup.ContainsKey(c)) continue;
                var wordDict = letterGroup[c];
                var sb = new StringBuilder();
                sb.AppendLine($"# {c}");
                foreach (var wordKvp in wordDict)
                {
                    string word = wordKvp.Key;
                    List<string> examples = wordKvp.Value;
                    sb.AppendLine($"## {word}");
                    foreach (string ex in examples)
                    {
                        // 使用无序列表格式，清晰展示每条例句
                        sb.AppendLine($"- {ex}");
                    }
                    sb.AppendLine(); // 空行分隔不同单词
                }
                string mdPath = Path.Combine(mdFolder, $"{c}.md");
                File.WriteAllText(mdPath, sb.ToString(), Encoding.UTF8);
                Console.WriteLine($"已生成Markdown文件：{mdPath}");
            }

            Console.WriteLine("全部处理完成！");
        }

        private static string SanitizeFileName(string word)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                if (word.Contains(c))
                    word = word.Replace(c, '-');
            }
            word = word.Replace('/', '-').Replace('\\', '-');
            return word;
        }

        private static char GetFirstLetter(string word)
        {
            if (string.IsNullOrEmpty(word)) return '0';
            char first = word[0];
            return char.IsLetter(first) ? char.ToUpper(first) : '0';
        }
    }
}