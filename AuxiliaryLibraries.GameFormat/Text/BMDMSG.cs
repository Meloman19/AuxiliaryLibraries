using AuxiliaryLibraries.Extensions;
using AuxiliaryLibraries.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AuxiliaryLibraries.GameFormat.Text
{
    public class BMDMSG
    {
        public int Index { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public int NameIndex { get; set; }
        public byte[][] MsgStrings { get; set; }

        public int GetSize()
        {
            int returned = 32 + 4 * MsgStrings.Length;
            if (Type == 1)
                returned += 4;
            returned += MsgStrings.Sum(x => x.Length);
            returned += IOTools.Alignment(returned, 4);
            return returned;
        }

        public void Write(BinaryWriter writer, int offset, List<int> pointers)
        {
            int size = 32 + 4 * MsgStrings.Length;
            writer.WriteString(Name, Encoding.ASCII, 24);

            if (Type == 0)
            {
                writer.Write((ushort)MsgStrings.Length);

                if (NameIndex == -1) { writer.Write((ushort)0xFFFF); }
                else { writer.Write((ushort)NameIndex); }
            }
            else if (Type == 1)
            {
                writer.Write((ushort)0);
                writer.Write((ushort)MsgStrings.Length);
                writer.Write((int)0);

                size += 4;
            }
            else
                throw new Exception("BMD Write Error: Unknown type");

            int sum = 0;
            foreach (var a in MsgStrings)
            {
                pointers.Add((int)writer.BaseStream.Position);
                writer.Write(offset + size + sum);
                sum += a.Length;
            }

            writer.Write(sum);

            foreach (var a in MsgStrings)
                writer.Write(a);

            writer.Write(new byte[IOTools.Alignment(size + sum, 4)]);
        }
    }
}