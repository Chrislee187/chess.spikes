using System;
using System.IO;

namespace chess.games.db.pgnimporter
{
    public class PgnFileArchiver
    {
        private const string ArchivePath = @"D:\Pgn Imported";
        private const string FailedPath = @"D:\Pgn Import Failures";

        public string ArchiveImportedFile(string file, string scanPath)
            => ArchiveImportedFile(file, scanPath, ArchivePath);

        public string ArchiveFailedFile(string file, string scanPath)
            => ArchiveImportedFile(file, scanPath, FailedPath);

        private string ArchiveImportedFile(string file, string scanPath, string archivePath)
        {
            if (!Directory.Exists(archivePath)) Directory.CreateDirectory(archivePath);

            var relativePath = Path.GetFullPath(file).Replace(Path.GetFullPath(scanPath), "");

            while (relativePath[0] == '\\')
            {
                relativePath = relativePath.Substring(1);
            }

            var destPath = Path.Combine(archivePath, relativePath);
            var destFolder = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

            if (!File.Exists(destPath))
            {
                File.Move(file, destPath);
            }

            return destPath;
        }

    }
}