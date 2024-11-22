using ICSharpCode.SharpZipLib.Zip;
using ROBdk97.Unp4k.ICSharpCode.SharpZipLib.Zip;
using ROBdk97.Unp4k.Utils;

namespace ROBdk97.Unp4k.P4kModels
{
    public class P4KEntry : P4KItem
    {
        private readonly ZipEntry _zipEntry;
        private readonly ZipFile _zipFile;


        public override string Name => Path.GetFileName(_zipEntry.Name);
        public override string FullPath => _zipEntry.Name;
        public override bool IsDirectory => _zipEntry.IsDirectory;
        public CompressionMethod CompressionMethod => _zipEntry.CompressionMethod;
        public bool IsCryXmlB
        {
            get
            {
                if (Name.EndsWith(".xml", System.StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }
        }

        public bool IsDataCore
        {
            get
            {
                if (Name.EndsWith(".dcb", System.StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }
        }


        public P4KEntry(ZipEntry zipEntry, ZipFile zipFile)
        {
            _zipEntry = zipEntry;
            _zipFile = zipFile;
        }

        public async Task<string> ReadAsStringAsync()
        {
            if (IsCryXmlB)
            {
                return ReadAsXml();
            }
            //if (IsDataCore)
            //{
            //    return await ReadAsDataCoreAsync();
            //}
            using var stream = Open();
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        private Task<string> ReadAsDataCoreAsync()
        {
            throw new NotImplementedException();
        }


        private string ReadAsXml()
        {
            using var stream = Open();
            var cryXmlReader = new CryXmlBReader();
            var xDoc = cryXmlReader.Parse(stream);
            return xDoc.ToString();
        }

        public void ExtractTo(string destinationPath)
        {
            string fullPath = Path.Combine(destinationPath, FullPath);
            if (IsDirectory)
            {
                Directory.CreateDirectory(fullPath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                using var inputStream = Open();
                using var outputStream = File.Create(fullPath);
                inputStream.CopyTo(outputStream);
            }
        }

        public void SaveTo(string destinationPath)
        {
            if (IsDirectory)
            {
                throw new InvalidOperationException("Cannot save a directory entry");
            }
            using var inputStream = Open();
            using var outputStream = File.Create(destinationPath);
            inputStream.CopyTo(outputStream);
        }

        public Stream Open()
        {
            return _zipFile.GetInputStream(_zipEntry);
        }
    }
}