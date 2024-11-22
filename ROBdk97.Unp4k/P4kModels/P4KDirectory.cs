namespace ROBdk97.Unp4k.P4kModels
{
    public class P4KDirectory : P4KItem
    {
        public override string Name { get; }
        public override string FullPath { get; }
        public override bool IsDirectory => true;

        public P4KDirectory(string name, string fullPath)
        {
            Name = name;
            FullPath = fullPath;
        }
    }
}
