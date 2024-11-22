using ICSharpCode.SharpZipLib.Zip;
using ROBdk97.Unp4k.ICSharpCode.SharpZipLib.Zip;
using ROBdk97.Unp4k.Utils;
using System;

namespace ROBdk97.Unp4k.P4kModels
{
    public class P4KArchive
    {
        private readonly string FilePath;
        private ZipFile? _zipFile;
        private readonly byte[] key = [0x5E, 0x7A, 0x20, 0x02, 0x30, 0x2E, 0xEB, 0x1A, 0x3B, 0xB6, 0x17, 0xC3, 0x0F, 0xDE, 0x1E, 0x47];
        public P4KDirectory Root { get; } = new P4KDirectory("Root", string.Empty);

        public P4KArchive(string filePath)
        {
            FilePath = filePath;
        }

        public async Task LoadAsync()
        {
            await Task.Run(() =>
            {
                _zipFile = new ZipFile(File.OpenRead(FilePath)) { Key = key };
                foreach (ZipEntry zipEntry in _zipFile)
                {
                    AddEntry(zipEntry);
                }
            });
        }

        private void AddEntry(ZipEntry zipEntry)
        {
            string[] parts = zipEntry.Name.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            P4KDirectory currentDir = Root;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                string part = parts[i];
                var existingDir = currentDir.Children.FindDirectory(part);
                if (existingDir == null)
                {
                    var newDir = new P4KDirectory(part, Path.Combine(currentDir.FullPath, part));
                    currentDir.Children.Add(newDir);
                    currentDir = newDir;
                }
                else
                {
                    currentDir = existingDir;
                }
            }

            if (!string.IsNullOrEmpty(Path.GetFileName(zipEntry.Name)))
            {
                var fileEntry = new P4KEntry(zipEntry, _zipFile!);
                currentDir.Children.Add(fileEntry);
            }
        }
        public IEnumerable<P4KEntry> GetEntriesInDirectory(string directoryPath)
        {
            directoryPath = directoryPath.Replace("\\", "/").TrimEnd('/') + "/";

            return Root.Children.
                Where(entry => entry.FullPath.StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase) && !entry.IsDirectory)
                .Cast<P4KEntry>();
        }

        public List<P4KEntry> FindFiles(string fileName)
        {
            List<P4KEntry> result = [];
            FindFilesRecursive(Root, fileName, result);
            return result;
        }

        private void FindFilesRecursive(P4KDirectory currentDir, string fileName, List<P4KEntry> result)
        {
            foreach (var child in currentDir.Children)
            {
                if (child.IsDirectory)
                {
                    FindFilesRecursive((P4KDirectory)child, fileName, result);
                }
                else
                {
                    if (string.Equals(child.Name, fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add((P4KEntry)child);
                    }
                }
            }
        }

    }
}
