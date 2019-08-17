using System;
using System.IO;
using System.Linq;

namespace chess.games.db.pgnimporter
{
    public class PgnFileArchiver
    {
        private const string ArchiveFolder = @"D:\Pgn Imported";
        private const string FailedFolder = @"D:\Pgn Import Failures";

        public string ArchiveImportedFile(string file, string scanPath)
            => ArchiveImportedFile(file, scanPath, ArchiveFolder);

        public string ArchiveFailedFile(string file, string scanPath)
            => ArchiveImportedFile(file, scanPath, FailedFolder);

        private string ArchiveImportedFile(string file, string scanPath, string archiveFolder)
        {
            if (!Directory.Exists(archiveFolder)) Directory.CreateDirectory(archiveFolder);

            var destFile = CreateDestinationFolder(Path.Combine(archiveFolder, GetSubFolderPath(file, scanPath)));

            if (!File.Exists(destFile))
            {
                File.Move(file, destFile);
            }

            RemoveEmptyFolder(Path.GetDirectoryName(file));

            return destFile;
        }

        private static string CreateDestinationFolder(string destinationPath)
        {
            var destFolder = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
            return destinationPath;
        }

        private static string GetSubFolderPath(string file, string scanPath)
        {
            var relativePath = Path.GetFullPath(file).Replace(Path.GetFullPath(scanPath), "");

            while (relativePath[0] == '\\')
            {
                relativePath = relativePath.Substring(1);
            }

            return relativePath;
        }

        private static void RemoveEmptyFolder(string folder)
        {
            if (!Directory.GetFiles(folder).Any())
            {
                Directory.Delete(folder);
            }
        }
    }
}