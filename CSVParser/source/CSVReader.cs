using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CSVParser
{
    public static class CSVReader
    {
        public static IEnumerable<List<string>> ReadLines(StreamReader sr)
        {
            using (var enumerator = new StreamEnumerator(sr))
                foreach (var dataLine in ReadLines(enumerator))
                    yield return dataLine;
        }

        public static IEnumerable<List<string>> ReadLines(IEnumerable<string> data)
        {
            List<string> retLine = new List<string>();
            bool hasNewLine = false;
           
            foreach(string line in data)
            {
                int iStart = 0;
                if (hasNewLine)
                {
                    retLine[retLine.Count - 1] += Environment.NewLine;
                    int iEnd = SearchCloseQuot(line, iStart);
                    if (iEnd == -1)
                    {
                        retLine[retLine.Count - 1] += line.Replace("\"\"", "\"");
                        continue;
                    }
                    retLine[retLine.Count - 1] += line.Substring(iStart, iEnd).Replace("\"\"", "\""); 
                    hasNewLine = false;

                    iStart = line.IndexOf(',', ++iEnd);

                    if(iStart == -1)
                    {
                        yield return retLine;
                        retLine = new List<string>();
                        continue;
                    }

                    iStart++;
                }
                
                for (; iStart < line.Length; iStart++)
                {
                    if(line[iStart] == '"')
                    {
                        int iEnd = SearchCloseQuot(line, iStart + 1);
                        if (iEnd == -1)
                        {
                            retLine.Add(line.Substring(iStart + 1).Replace("\"\"", "\""));
                            hasNewLine = true;
                            break;
                        }
                        retLine.Add(line.Substring(iStart + 1, iEnd - iStart - 1).Replace("\"\"", "\""));
                        if(iEnd == line.Length - 1)
                        {
                            break;
                        }
                        else
                        {
                            iStart = line.IndexOf(',', iEnd + 1);
                            continue;
                        }
                    }
                    else if (line[iStart] == ',')
                    {
                        retLine.Add("");
                        continue;
                    }
                    else
                    {
                        int iEnd = line.IndexOf(',', iStart);
                        if(iEnd == -1)
                        {
                            retLine.Add(line.Substring(iStart).Trim());
                            break;
                        }
                        else
                        {
                            retLine.Add(line.Substring(iStart, iEnd - iStart));
                            iStart = iEnd;
                            continue;
                        }
                    }
                }

                if (!hasNewLine)
                {
                    if (line.Length == 0 || line[line.Length - 1] == ',')
                    {
                        retLine.Add("");
                    }
                    yield return retLine;
                    retLine = new List<string>();
                }
            }

            if (hasNewLine)
                throw new MalformedDataException();
        }

        private static int SearchCloseQuot(string data, int index)
        {
            for (int i = index; i < data.Length; i += 2)
            {
                i = data.IndexOf('"', i);
                if (i == data.Length - 1 || data[i + 1] != '"' || i == -1)
                {
                    return i;
                }
            }
            return -1;
        }

        public class MalformedDataException : Exception { }
    }
}
