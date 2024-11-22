using System.Runtime.InteropServices;

namespace ROBdk97.Unp4k.P4kModels
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CryXmlBHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Signature; // 8 bytes

        public uint XmlSize;
        public uint NodeTableOffset;
        public uint NodeCount;
        public uint AttributesTableOffset;
        public uint AttributesCount;
        public uint ChildTableOffset;
        public uint ChildTableCount;
        public uint StringDataOffset;
        public uint StringDataSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CryXmlBNode
    {
        public uint TagStringOffset;
        public uint ContentStringOffset;
        public ushort AttributeCount;
        public ushort ChildCount;
        public uint ParentIndex;
        public uint FirstAttributeIndex;
        public uint FirstChildIndex;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CryXmlBAttribute
    {
        public uint KeyStringOffset;
        public uint ValueStringOffset;
    }
    public enum ChCrVersion : uint
    {
        CRYTEK_3_6 = 0x746,
        // Other versions can be added here
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ChCrHeader
    {
        public uint Signature; // Should be 'CrCh'
        public ChCrVersion Version;
        public uint NumChunks;
        public uint ChunkHeaderTableOffset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ChunkHeader746
    {
        public ushort Type;
        public ushort Version;
        public uint Id;
        public uint Size;
        public uint Offset;
    }
    public enum IvoVersion : uint
    {
        SC_3_11 = 0x900,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IvoHeader
    {
        public uint Signature; // Should be '#ivo'
        public IvoVersion Version;
        public uint NumChunks;
        public uint ChunkHeaderTableOffset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IvoChunkHeader
    {
        public ushort Id;
        public ushort Type;
        public uint IvoVersion;
        public ulong Offset;
    }

}
