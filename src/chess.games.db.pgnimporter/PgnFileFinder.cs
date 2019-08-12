using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace chess.games.db.pgnimporter
{
    public class PgnFileFinder
    {
        public string[] FindFiles(string path)
        {
            
            var recurse = true;

            var pgnfiles = new List<string>();

            if (Directory.Exists(path))
            {
                pgnfiles.AddRange(Directory
                    .GetFiles(path, "*.pgn", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .OrderBy(o => o).ToArray());
            }
            else if (File.Exists(path))
            {
                pgnfiles.Add(path); 
            }
            else
            {
                throw new ArgumentException($"Non existing filename or path specified@ {path}");
            }

            return pgnfiles.ToArray();
        }
    }
}