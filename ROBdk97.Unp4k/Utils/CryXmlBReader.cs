using ROBdk97.Unp4k.P4kModels;
using System;
using System.Text;
using System.Xml.Linq;

namespace ROBdk97.Unp4k.Utils
{
    public class CryXmlBReader
    {
        public XDocument Parse(Stream stream)
        {
            using var reader = new BinaryReader(stream);

            // Read header
            var header = reader.ReadStruct<CryXmlBHeader>();

            // Validate signature
            var signature = Encoding.ASCII.GetString(header.Signature).TrimEnd('\0');
            if (signature != "CryXmlB")
            {
                throw new InvalidDataException("Invalid CryXmlB signature.");
            }

            // Read string data
            stream.Seek(header.StringDataOffset, SeekOrigin.Begin);
            var stringData = reader.ReadBytes((int)header.StringDataSize);
            var strings = Encoding.UTF8.GetString(stringData).Split('\0');

            // Read nodes
            stream.Seek(header.NodeTableOffset, SeekOrigin.Begin);
            var nodes = new CryXmlBNode[header.NodeCount];
            for (int i = 0; i < header.NodeCount; i++)
            {
                nodes[i] = reader.ReadStruct<CryXmlBNode>();
            }

            // Read attributes
            stream.Seek(header.AttributesTableOffset, SeekOrigin.Begin);
            var attributes = new CryXmlBAttribute[header.AttributesCount];
            for (int i = 0; i < header.AttributesCount; i++)
            {
                attributes[i] = reader.ReadStruct<CryXmlBAttribute>();
            }

            // Read child indices
            stream.Seek(header.ChildTableOffset, SeekOrigin.Begin);
            var childIndices = new uint[header.ChildTableCount];
            for (int i = 0; i < header.ChildTableCount; i++)
            {
                childIndices[i] = reader.ReadUInt32();
            }

            // Build XML document
            var elements = new XElement[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var tagName = GetString(node.TagStringOffset, strings);
                var content = GetString(node.ContentStringOffset, strings);

                var element = new XElement(tagName);
                elements[i] = element;

                // Add attributes
                for (int j = 0; j < node.AttributeCount; j++)
                {
                    var attrIndex = node.FirstAttributeIndex + j;
                    var attribute = attributes[attrIndex];
                    var key = GetString(attribute.KeyStringOffset, strings);
                    var value = GetString(attribute.ValueStringOffset, strings);
                    element.SetAttributeValue(key, value);
                }

                // Set content
                if (!string.IsNullOrEmpty(content))
                {
                    element.Value = content;
                }
            }

            // Build hierarchy
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var element = elements[i];

                // Add children
                for (int j = 0; j < node.ChildCount; j++)
                {
                    var childIndex = (int)childIndices[node.FirstChildIndex + j];
                    var childElement = elements[childIndex];
                    element.Add(childElement);
                }
            }

            // Find root element (node with no parent)
            XElement root = elements[0]; // Assuming first node is root

            return new XDocument(root);
        }

        private string GetString(uint offset, string[] strings)
        {
            if (offset == uint.MaxValue)
                return string.Empty;

            int currentOffset = 0;
            foreach (var str in strings)
            {
                if (currentOffset == offset)
                {
                    return str;
                }
                currentOffset += Encoding.UTF8.GetByteCount(str) + 1; // +1 for null terminator
            }

            return string.Empty;
        }
    }
}
