// Program.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VocabSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ParseArguments(args);
            if (config == null)
            {
                PrintUsage();
                return;
            }

            // 收集待处理的文件列表
            var files = CollectFiles(config);
            if (files.Count == 0)
            {
                Console.WriteLine($"未找到需要处理的文件: {config.InputPath}");
                return;
            }

            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  VocabSplitter - 词汇 Markdown 拆分工具");
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine($"  输入路径: {Path.GetFullPath(config.InputPath)}");
            Console.WriteLine($"  输出目录: {Path.GetFullPath(config.OutputDir)}");
            Console.WriteLine($"  待处理文件: {files.Count} 个");
            Console.WriteLine("════════════════════════════════════════\n");

            int totalFiles = 0;
            int totalEntries = 0;
            int totalSuccess = 0;

            foreach (var file in files)
            {
                totalFiles++;
                Console.WriteLine($"[{totalFiles}/{files.Count}] 处理文件: {Path.GetFileName(file)}");

                string content = File.ReadAllText(file);
                var entries = ParseVocabEntries(content);

                if (entries.Count == 0)
                {
                    Console.WriteLine("  ⚠ 未解析到词汇条目，跳过\n");
                    continue;
                }

                totalEntries += entries.Count;
                Console.WriteLine($"  解析到 {entries.Count} 个词汇条目");

                // 是否按源文件名创建子目录（多文件模式下避免文件名冲突）
                string targetDir = config.SubDirPerFile
                    ? Path.Combine(config.OutputDir, SanitizeFileName(Path.GetFileNameWithoutExtension(file)))
                    : config.OutputDir;

                Directory.CreateDirectory(targetDir);

                int fileSuccess = 0;
                foreach (var entry in entries)
                {
                    string safeFileName = SanitizeFileName(entry.Word) + ".txt";
                    string filePath = Path.Combine(targetDir, safeFileName);

                    try
                    {
                        // 如果文件已存在，追加编号避免覆盖
                        filePath = GetUniqueFilePath(filePath);
                        File.WriteAllText(filePath, entry.Content);
                        fileSuccess++;
                        totalSuccess++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  [错误] {safeFileName}: {ex.Message}");
                    }
                }

                Console.WriteLine($"  ✓ 写入 {fileSuccess}/{entries.Count} 个文件 → {Path.GetFullPath(targetDir)}\n");
            }

            // 汇总
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  处理完成!");
            Console.WriteLine($"  文件数: {totalFiles}");
            Console.WriteLine($"  词条总数: {totalEntries}");
            Console.WriteLine($"  成功写入: {totalSuccess}");
            Console.WriteLine("════════════════════════════════════════");
        }

        #region 命令行参数解析

        /// <summary>
        /// 解析后的运行配置
        /// </summary>
        class RunConfig
        {
            /// <summary>输入路径（文件或文件夹）</summary>
            public string InputPath { get; set; }

            /// <summary>输出目录</summary>
            public string OutputDir { get; set; }

            /// <summary>多文件时是否为每个源文件创建子目录</summary>
            public bool SubDirPerFile { get; set; } = true;

            /// <summary>是否递归搜索子文件夹</summary>
            public bool Recursive { get; set; }
        }

        /// <summary>
        /// 解析命令行参数
        /// </summary>
        static RunConfig ParseArguments(string[] args)
        {
            if (args.Length == 0)
                return null;

            var config = new RunConfig();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg.ToLower())
                {
                    // 输出目录
                    case "-o":
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            config.OutputDir = args[++i];
                        }
                        else
                        {
                            Console.WriteLine("错误: -o / --output 需要指定目录路径");
                            return null;
                        }
                        break;

                    // 递归搜索
                    case "-r":
                    case "--recursive":
                        config.Recursive = true;
                        break;

                    // 不为每个文件创建子目录
                    case "--flat":
                        config.SubDirPerFile = false;
                        break;

                    // 帮助
                    case "-h":
                    case "--help":
                        return null;

                    // 默认：当作输入路径
                    default:
                        if (config.InputPath == null)
                        {
                            config.InputPath = arg;
                        }
                        else
                        {
                            Console.WriteLine($"错误: 未知参数 '{arg}'");
                            return null;
                        }
                        break;
                }
            }

            // 输入路径校验
            if (string.IsNullOrWhiteSpace(config.InputPath))
            {
                Console.WriteLine("错误: 未指定输入路径");
                return null;
            }

            if (!File.Exists(config.InputPath) && !Directory.Exists(config.InputPath))
            {
                Console.WriteLine($"错误: 路径不存在 - {config.InputPath}");
                return null;
            }

            // 默认输出目录
            if (string.IsNullOrWhiteSpace(config.OutputDir))
            {
                config.OutputDir = Path.Combine(
                    Directory.GetCurrentDirectory(), "output");
            }

            return config;
        }

        /// <summary>
        /// 收集待处理的 markdown 文件列表
        /// </summary>
        static List<string> CollectFiles(RunConfig config)
        {
            var files = new List<string>();

            if (File.Exists(config.InputPath))
            {
                // 单文件模式
                files.Add(config.InputPath);
            }
            else if (Directory.Exists(config.InputPath))
            {
                // 文件夹模式
                var searchOption = config.Recursive
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

                // 支持 .md 和 .markdown 两种扩展名
                var mdFiles = Directory.GetFiles(config.InputPath, "*.md", searchOption);
                var markdownFiles = Directory.GetFiles(config.InputPath, "*.markdown", searchOption);

                files.AddRange(mdFiles);
                files.AddRange(markdownFiles);

                files.Sort(StringComparer.OrdinalIgnoreCase);
            }

            return files;
        }

        /// <summary>
        /// 打印使用说明
        /// </summary>
        static void PrintUsage()
        {
            Console.WriteLine(@"
════════════════════════════════════════════════
  VocabSplitter - 词汇 Markdown 批量拆分工具
════════════════════════════════════════════════

用法:
  dotnet run <输入路径> [选项]

输入路径:
  可以是单个 .md 文件，也可以是一个文件夹路径
  文件夹模式下会自动扫描其中所有 .md / .markdown 文件

选项:
  -o, --output <目录>    指定输出目录 (默认: ./output)
  -r, --recursive        递归搜索子文件夹中的 markdown 文件
  --flat                 不按源文件名创建子目录（所有 txt 输出到同一目录）
  -h, --help             显示此帮助信息

示例:
  # 处理单个文件
  dotnet run vocab.md

  # 处理整个文件夹
  dotnet run /data/vocab/

  # 处理文件夹，递归搜索，指定输出目录
  dotnet run /data/vocab/ -r -o /data/output/

  # 处理文件夹，所有 txt 输出到同一目录（不建子目录）
  dotnet run /data/vocab/ --flat -o /data/output/
");
        }

        #endregion

        #region 词汇解析

        /// <summary>
        /// 解析 Markdown 中的词汇条目
        /// </summary>
        static List<VocabEntry> ParseVocabEntries(string content)
        {
            var entries = new List<VocabEntry>();

            // 匹配模式: **单词名** 作为每个条目的开始
            var pattern = @"^\*\*(.+?)\*\*\s*$";
            var regex = new Regex(pattern, RegexOptions.Multiline);

            var matches = regex.Matches(content).Cast<Match>().ToList();

            for (int i = 0; i < matches.Count; i++)
            {
                string word = matches[i].Groups[1].Value.Trim();

                int startIdx = matches[i].Index;
                int endIdx = (i + 1 < matches.Count)
                    ? matches[i + 1].Index
                    : content.Length;

                string entryContent = content.Substring(startIdx, endIdx - startIdx).TrimEnd();

                entries.Add(new VocabEntry
                {
                    Word = word,
                    Content = entryContent
                });
            }

            return entries;
        }

        #endregion

        #region 文件名处理

        /// <summary>
        /// 清理文件名中的非法字符
        /// </summary>
        static string SanitizeFileName(string name)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string result = name;
            foreach (char c in invalidChars)
            {
                result = result.Replace(c, '_');
            }

            result = result
                .Replace("/", "-")
                .Replace("\\", "-")
                .Replace("*", "")
                .Replace("?", "")
                .Replace(":", "-")
                .Replace("\"", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "-")
                .Trim()
                .Trim('.');

            if (string.IsNullOrWhiteSpace(result))
                result = "unknown";

            return result;
        }

        /// <summary>
        /// 若文件已存在则在文件名后追加编号，避免覆盖
        /// </summary>
        static string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath))
                return filePath;

            string dir = Path.GetDirectoryName(filePath);
            string nameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string ext = Path.GetExtension(filePath);

            int counter = 2;
            string newPath;
            do
            {
                newPath = Path.Combine(dir, $"{nameWithoutExt}_{counter}{ext}");
                counter++;
            } while (File.Exists(newPath));

            return newPath;
        }

        #endregion
    }

    class VocabEntry
    {
        public string Word { get; set; }
        public string Content { get; set; }
    }
}
