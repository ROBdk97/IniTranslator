using ROBdk97.Unp4k.P4kModels;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace ROBdk97.Unp4k.Utils
{
    public static class Extensions
    {
        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] bytes = reader.ReadBytes(size);

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }
        public static P4KDirectory? FindDirectory(this ObservableCollection<P4KItem> items, string name)
        {
            foreach (var item in items)
            {
                if (item.IsDirectory && item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return (P4KDirectory)item;
                }
            }
            return null;
        }
    }
}
