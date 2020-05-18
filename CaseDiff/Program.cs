using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using DiffLib;

namespace CaseDiff
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                for (int index = 0; index < args.Length; index++)
                    Console.Error.WriteLine($"#{index}: {args[index]}");

                foreach (string key in Environment.GetEnvironmentVariables().Keys)
                    if (key.StartsWith("GIT"))
                        Console.Error.WriteLine($"{key} = {Environment.GetEnvironmentVariable(key)}");

                Console.Error.WriteLine("casediff needs 3 filenames");
                Console.Error.WriteLine("Add the following to your global .gitconfig file");
                Console.Error.WriteLine("[difftool \"casediff\"]");
                Console.Error.WriteLine($"    cmd = '{Assembly.GetEntryAssembly().Location}' \\\"$LOCAL\\\" \\\"$REMOTE\\\" \\\"$BASE\\\"");
                Console.Error.WriteLine("And invoke it with:");
                Console.Error.WriteLine("    git difftool -t casediff <rest of options for difftool here>");
                return 1;
            }

            if (args[0] == "nul" || args[1] == "nul")
            {
                // File has been deleted or added
                return 0;
            }

            var leftFilePath = Path.GetFullPath(args[0]);
            var rightFilePath = Path.GetFullPath(args[1]);
            string baseFileName = args[2];

            string[] leftSide = File.ReadAllLines(leftFilePath);
            string[] rightSide = File.ReadAllLines(rightFilePath);

            string[] leftSideUpperCased = leftSide.Select(line => line.ToUpperInvariant()).ToArray();
            string[] rightSideUpperCased = rightSide.Select(line => line.ToUpperInvariant()).ToArray();

            IEnumerable<DiffSection> sections = Diff.CalculateSections(leftSideUpperCased, rightSideUpperCased, StringComparer.Ordinal);
            var aligned = Diff.AlignElements(leftSideUpperCased, rightSideUpperCased, sections, new StringSimilarityDiffElementAligner()).ToList();

            int lastLeftLineNo = int.MinValue;
            int lastRightLineNo = int.MinValue;
            foreach (var section in aligned)
            {
                switch (section.Operation)
                {
                    case DiffOperation.Match:
                    case DiffOperation.Insert:
                    case DiffOperation.Delete:
                        // We don't care about these
                        break;

                    case DiffOperation.Replace:
                    case DiffOperation.Modify:
                        int leftLineIndex = section.ElementIndexFromCollection1.Value;
                        int rightLineIndex = section.ElementIndexFromCollection2.Value;

                        var leftLine = leftSide[leftLineIndex].Trim();
                        var rightLine = rightSide[rightLineIndex].Trim();

                        if (leftLine != rightLine && StringComparer.OrdinalIgnoreCase.Equals(leftLine, rightLine))
                        {
                            int leftLineNo = section.ElementIndexFromCollection1.Value + 1;
                            int rightLineNo = section.ElementIndexFromCollection2.Value + 1;

                            if (leftLineNo != lastLeftLineNo + 1 || rightLineNo != lastRightLineNo + 1)
                            {
                                if (leftLineNo != rightLineNo)
                                    Console.WriteLine($"{baseFileName} #{leftLineNo} vs. #{rightLineNo}:");
                                else
                                    Console.WriteLine($"{baseFileName} #{leftLineNo}");
                            }

                            Console.WriteLine($" < {leftSide[leftLineIndex]}");
                            Console.WriteLine($" > {rightSide[rightLineIndex]}");

                            lastLeftLineNo = leftLineNo;
                            lastRightLineNo = rightLineNo;
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return 0;
        }
    }
}