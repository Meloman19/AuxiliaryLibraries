using AuxiliaryLibraries.IO;
using AuxiliaryLibraries.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AuxiliaryLibraries.GameFormat.Text
{
    public struct TextBaseElement
    {
        public TextBaseElement(bool isText, byte[] array)
        {
            Array = array;
            IsText = isText;
        }

        public string GetText(Encoding encoding, bool linesplit = false)
        {
            if (IsText)
                return String.Concat(encoding.GetChars(Array));
            else
            {
                if (Array[0] == 0x0A)
                    if (linesplit)
                        return "\n";
                    else
                        return GetSystem();
                else
                    return GetSystem();
            }
        }

        public string GetSystem()
        {
            string returned = "";

            if (Array.Length > 0)
            {
                returned += "{" + Convert.ToString(Array[0], 16).PadLeft(2, '0').ToUpper();
                for (int i = 1; i < Array.Length; i++)
                    returned += " " + Convert.ToString(Array[i], 16).PadLeft(2, '0').ToUpper();

                returned += "}";
            }

            return returned;
        }

        public bool IsText { get; }
        public byte[] Array { get; }
    }

    public class PTPName
    {
        public PTPName(int index, string oldName, string newName)
        {
            Index = index;
            NewName = newName;
            OldName = StringTool.SplitString(oldName, '-');
        }

        public PTPName(int index, byte[] oldName, string newName)
        {
            Index = index;
            NewName = newName;
            OldName = oldName;
        }

        public PTPName() { }

        public int Index { get; set; }
        public byte[] OldName { get; set; }
        public string NewName { get; set; }
    }

    public class MSGstr
    {
        public MSGstr(int index, string newstring)
        {
            Index = index;
            NewString = newstring;
        }

        public MSGstr(int index, string newstring, byte[] Prefix, byte[] OldString, byte[] Postfix) : this(index, newstring)
        {
            List<TextBaseElement> temp = Prefix.GetTextBaseList();
            foreach (var a in temp)
                this.Prefix.Add(a);

            temp = OldString.GetTextBaseList();
            foreach (var a in temp)
                this.OldString.Add(a);

            temp = Postfix.GetTextBaseList();
            foreach (var a in temp)
                this.Postfix.Add(a);
        }

        public byte[] GetOld()
        {
            List<byte> returned = new List<byte>();
            returned.AddRange(Prefix.GetByteArray());
            returned.AddRange(OldString.GetByteArray());
            returned.AddRange(Postfix.GetByteArray());
            return returned.ToArray();
        }

        public byte[] GetNew(Encoding New)
        {
            List<byte> returned = new List<byte>();
            returned.AddRange(Prefix.GetByteArray());
            returned.AddRange(NewString.GetTextBaseList(New).GetByteArray().ToArray());
            returned.AddRange(Postfix.GetByteArray());
            return returned.ToArray();
        }

        public bool MovePrefixDown()
        {
            if (Prefix.Count == 0)
                return false;

            var temp = Prefix[Prefix.Count - 1];
            Prefix.RemoveAt(Prefix.Count - 1);
            OldString.Insert(0, temp);
            return true;
        }

        public bool MovePrefixUp()
        {
            if (OldString.Count == 0)
                return false;

            var temp = OldString[0];

            if (temp.IsText)
                return false;

            Prefix.Add(temp);
            OldString.RemoveAt(0);
            return true;
        }

        public bool MovePostfixDown()
        {
            if (OldString.Count == 0)
                return false;

            var temp = OldString[OldString.Count - 1];

            if (temp.IsText)
                return false;

            Postfix.Insert(0, temp);
            OldString.RemoveAt(OldString.Count - 1);
            return true;
        }

        public bool MovePostfixUp()
        {
            if (Postfix.Count == 0)
                return false;

            var temp = Postfix[0];
            Postfix.RemoveAt(0);

            OldString.Add(temp);
            return true;
        }

        public int Index { get; set; }
        public int CharacterIndex { get; set; }
        public BindingList<TextBaseElement> Prefix { get; } = new BindingList<TextBaseElement>();
        public BindingList<TextBaseElement> OldString { get; } = new BindingList<TextBaseElement>();
        public BindingList<TextBaseElement> Postfix { get; } = new BindingList<TextBaseElement>();
        public string NewString { get; set; } = "";
    }

    public class MSG
    {
        public MSG(int index, string type, string name, int charindex)
        {
            Index = index;
            Type = type;
            Name = name;
            CharacterIndex = charindex;
        }

        public byte[] GetOld()
        {
            List<byte> returned = new List<byte>();
            foreach (var a in Strings)
                returned.AddRange(a.GetOld());
            return returned.ToArray();
        }

        public byte[] GetNew(Encoding New)
        {
            List<byte> returned = new List<byte>();
            foreach (var a in Strings)
                returned.AddRange(a.GetNew(New));
            return returned.ToArray();
        }

        public int Index { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int CharacterIndex { get; set; }
        public byte[] MsgBytes { get; set; }

        public BindingList<MSGstr> Strings { get; } = new BindingList<MSGstr>();
    }

    public class PTP : IGameFile
    {
        public PTP(byte[] data)
        {
            using (MemoryStream MS = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(MS))
            {
                int ver = GetVersion(reader);
                if (ver == 0)
                    OpenPTP0(reader);
                else
                    OpenXmlPTP(MS);
            }
        }

        public PTP(BMD bmd)
        {
            foreach (var NAME in bmd.Name)
            {
                int Index = NAME.Index;
                byte[] OldNameSource = NAME.NameBytes;
                string NewName = "";

                names.Add(new PTPName(Index, OldNameSource, NewName));
            }

            foreach (var Message in bmd.Msg)
            {
                int Index = Message.Index;
                string Type = Message.Type.ToString();
                string Name = Message.Name;
                int CharacterNameIndex = Message.CharacterIndex;
                MSG temp = new MSG(Index, Type, Name, CharacterNameIndex);
                temp.Strings.ParseStrings(Message.MsgBytes);
                msg.Add(temp);
            }
        }

        public List<PTPName> names { get; } = new List<PTPName>();
        public List<MSG> msg { get; } = new List<MSG>();

        bool Open(Stream stream)
        {
            try
            {
                BinaryReader reader = new BinaryReader(stream);
                stream.Position = 0;
                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) == "PTP0")
                {
                    names.Clear();
                    msg.Clear();

                    int MSGPos = reader.ReadInt32();
                    int MSGCount = reader.ReadInt32();
                    int NamesPos = reader.ReadInt32();
                    int NamesCount = reader.ReadInt32();

                    stream.Position = NamesPos;
                    for (int i = 0; i < NamesCount; i++)
                    {
                        int size = reader.ReadInt32();
                        byte[] OldName = reader.ReadBytes(size);
                        stream.Position += IOTools.Alignment(stream.Position, 4);

                        size = reader.ReadInt32();
                        string NewName = Encoding.UTF8.GetString(reader.ReadBytes(size));
                        stream.Position += IOTools.Alignment(stream.Position, 4);

                        names.Add(new PTPName(i, OldName, NewName));
                    }

                    stream.Position = MSGPos;
                    for (int i = 0; i < MSGCount; i++)
                    {
                        int type = reader.ReadInt32();
                        string Type = type == 0 ? "MSG" : "SEL";
                        byte[] buffer = reader.ReadBytes(24);
                        string Name = Encoding.ASCII.GetString(buffer.Where(x => x != 0).ToArray());
                        int StringCount = reader.ReadInt16();
                        int CharacterIndex = reader.ReadInt16();
                        MSG mSG = new MSG(i, Type, Name, CharacterIndex);

                        for (int k = 0; k < StringCount; k++)
                        {
                            int size = reader.ReadInt32();
                            byte[] Prefix = reader.ReadBytes(size);
                            stream.Position += IOTools.Alignment(stream.Position, 4);

                            size = reader.ReadInt32();
                            byte[] OldString = reader.ReadBytes(size);
                            stream.Position += IOTools.Alignment(stream.Position, 4);

                            size = reader.ReadInt32();
                            byte[] Postfix = reader.ReadBytes(size);
                            stream.Position += IOTools.Alignment(stream.Position, 4);

                            size = reader.ReadInt32();
                            string NewString = Encoding.UTF8.GetString(reader.ReadBytes(size));
                            stream.Position += IOTools.Alignment(stream.Position, 16);

                            mSG.Strings.Add(new MSGstr(k, NewString, Prefix, OldString, Postfix) { CharacterIndex = CharacterIndex });
                        }

                        msg.Add(mSG);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                names.Clear();
                msg.Clear();
                //  Logging.Write("PTPfactory", e.ToString());
                return false;
            }
        }

        public void CopyOld2New(Encoding Old)
        {
            foreach (var Name in names)
                Name.NewName = Name.OldName.GetTextBaseList().GetString(Old, true);

            foreach (var Msg in msg)
                foreach (var Str in Msg.Strings)
                    Str.NewString = Str.OldString.GetString(Old, true);
        }

        public void OpenPTP0(BinaryReader reader)
        {
            reader.ReadInt32();
            int MSGPos = reader.ReadInt32();
            int MSGCount = reader.ReadInt32();
            int NamesPos = reader.ReadInt32();
            int NamesCount = reader.ReadInt32();

            reader.BaseStream.Position = NamesPos;
            for (int i = 0; i < NamesCount; i++)
            {
                int size = reader.ReadInt32();
                byte[] OldName = reader.ReadBytes(size);
                reader.BaseStream.Position += IOTools.Alignment(reader.BaseStream.Position, 4);

                size = reader.ReadInt32();
                string NewName = Encoding.UTF8.GetString(reader.ReadBytes(size));
                reader.BaseStream.Position += IOTools.Alignment(reader.BaseStream.Position, 4);

                names.Add(new PTPName(i, OldName, NewName));
            }

            reader.BaseStream.Position = MSGPos;
            for (int i = 0; i < MSGCount; i++)
            {
                int type = reader.ReadInt32();
                string Type = type == 0 ? "MSG" : "SEL";
                byte[] buffer = reader.ReadBytes(24);
                string Name = Encoding.ASCII.GetString(buffer.Where(x => x != 0).ToArray());
                int StringCount = reader.ReadInt16();
                int CharacterIndex = reader.ReadInt16();
                MSG mSG = new MSG(i, Type, Name, CharacterIndex);

                for (int k = 0; k < StringCount; k++)
                {
                    int size = reader.ReadInt32();
                    byte[] Prefix = reader.ReadBytes(size);
                    reader.BaseStream.Position += IOTools.Alignment(reader.BaseStream.Position, 4);

                    size = reader.ReadInt32();
                    byte[] OldString = reader.ReadBytes(size);
                    reader.BaseStream.Position += IOTools.Alignment(reader.BaseStream.Position, 4);

                    size = reader.ReadInt32();
                    byte[] Postfix = reader.ReadBytes(size);
                    reader.BaseStream.Position += IOTools.Alignment(reader.BaseStream.Position, 4);

                    size = reader.ReadInt32();
                    string NewString = Encoding.UTF8.GetString(reader.ReadBytes(size));
                    reader.BaseStream.Position += IOTools.Alignment(reader.BaseStream.Position, 16);

                    mSG.Strings.Add(new MSGstr(k, NewString, Prefix, OldString, Postfix) { CharacterIndex = CharacterIndex });
                }

                msg.Add(mSG);
            }
        }

        public void OpenXmlPTP(Stream stream)
        {
            XDocument xDoc = XDocument.Load(stream, LoadOptions.PreserveWhitespace);

            XElement MSG1Doc = xDoc.Element("MSG1");

            foreach (var NAME in MSG1Doc.Element("CharacterNames").Elements())
            {
                int Index = Convert.ToInt32(NAME.Attribute("Index").Value);
                string OldNameSource = NAME.Element("OldNameSource").Value;
                string NewName = NAME.Element("NewName").Value;

                names.Add(new PTPName(Index, OldNameSource, NewName));
            }

            foreach (var Message in MSG1Doc.Element("MSG").Elements())
            {
                int Index = Convert.ToInt32(Message.Attribute("Index").Value);
                string Type = Message.Element("Type").Value;
                string Name = Message.Element("Name").Value;
                int CharacterNameIndex = Convert.ToInt32(Message.Element("CharacterNameIndex").Value);

                MSG temp = new MSG(Index, Type, Name, CharacterNameIndex);
                msg.Add(temp);

                foreach (var Strings in Message.Element("MessageStrings").Elements())
                {
                    int StringIndex = Convert.ToInt32(Strings.Attribute("Index").Value);
                    string NewString = Strings.Element("NewString").Value;

                    MSGstr temp2 = new MSGstr(StringIndex, NewString) { CharacterIndex = CharacterNameIndex };
                    temp.Strings.Add(temp2);

                    foreach (var Prefix in Strings.Elements("PrefixBytes"))
                    {
                        int PrefixIndex = Convert.ToInt32(Prefix.Attribute("Index").Value);
                        string PrefixType = Prefix.Attribute("Type").Value;
                        string PrefixBytes = Prefix.Value;

                        temp2.Prefix.Add(new TextBaseElement(PrefixType == "Text" ? true : false, StringTool.SplitString(PrefixBytes, '-')));
                    }

                    foreach (var Old in Strings.Elements("OldStringBytes"))
                    {
                        int OldIndex = Convert.ToInt32(Old.Attribute("Index").Value);
                        string OldType = Old.Attribute("Type").Value;
                        string OldBytes = Old.Value;

                        temp2.OldString.Add(new TextBaseElement(OldType == "Text" ? true : false, StringTool.SplitString(OldBytes, '-')));
                    }

                    foreach (var Postfix in Strings.Elements("PostfixBytes"))
                    {
                        int PostfixIndex = Convert.ToInt32(Postfix.Attribute("Index").Value);
                        string PostfixType = Postfix.Attribute("Type").Value;
                        string PostfixBytes = Postfix.Value;

                        temp2.Postfix.Add(new TextBaseElement(PostfixType == "Text" ? true : false, StringTool.SplitString(PostfixBytes, '-')));
                    }
                }
            }
        }

        public static int GetVersion(BinaryReader reader)
        {
            long temp = reader.BaseStream.Position;
            reader.BaseStream.Position = 0;
            byte[] buffer = reader.ReadBytes(4);
            reader.BaseStream.Position = temp;
            if (buffer.SequenceEqual(new byte[4] { 0x50, 0x54, 0x50, 0x30 }))
                return 0;
            else
                return -1;
        }

        public string[] ExportTXT(string PTPname, bool removesplit, Encoding Old)
        {
            List<string> list = new List<string>();
            foreach (var a in msg)
            {
                foreach (var b in a.Strings)
                {
                    string OldName = "";
                    if (a.Type == "SEL")
                        OldName += "<SELECT>\t";
                    else
                    {
                        var name = names.FirstOrDefault(x => x.Index == a.CharacterIndex);
                        OldName += name == null ? "<EMPTY>\t" : name.OldName.GetTextBaseList().GetString(Old, false).Replace("\n", " ") + "\t";
                    }
                    string OldText = removesplit ? b.OldString.GetString(Old, removesplit).Replace("\n", " ") + "\t" : b.OldString.GetString(Old, removesplit).Replace("\n", "\\n");

                    string returned = $"{PTPname}\t{a.Index}\t{b.Index}\t{OldName}\t{OldText}\t";

                    list.Add(returned);
                }
            }

            return list.ToArray();
        }

        public void ImportNames(Dictionary<string, string> Names, Encoding oldEncoding)
        {
            foreach (var name in names)
            {
                string old = oldEncoding.GetString(name.OldName);
                if (Names.ContainsKey(old))
                    name.NewName = Names[old];
            }
        }

        public void ImportText(string[][] text)
        {
            ImportText(text, (str, msg) =>
            {
                return str.Replace("\\n", "\n");
            });
        }

        public void ImportText(string[][] text, Dictionary<char, int> charWidth)
        {
            ImportText(text, (str, msg) =>
            {
                int count = 1;
                foreach (var a in msg.OldString)
                    if (a.Array.Contains<byte>(0x0A))
                        count++;

                return str.SplitByLineCount(charWidth, count);
            });

            //foreach (var line in text)
            //    if (line.Length == 3)
            //    {
            //        if (int.TryParse(line[0], out int msgInd) && int.TryParse(line[1], out int strInd))
            //        {
            //            if (msg.Count > msgInd && msg[msgInd].Strings.Count > strInd)
            //            {
            //                int count = 1;
            //                foreach (var a in msg[msgInd].Strings[strInd].OldString)
            //                    if (a.Array.Contains<byte>(0x0A))
            //                        count++;

            //                msg[msgInd].Strings[strInd].NewString = line[2].SplitByLineCount(charWidth, count);
            //            }
            //        }
            //    }
        }

        public void ImportText(string[][] text, Encoding encoding, PersonaFont personaFont, int width)
        {
            ImportText(text, (str, msg) =>
            {
                return str.SplitByWidth(encoding, personaFont, width);
            });
            //foreach (var line in text)
            //    if (line.Length == 3)
            //    {
            //        if (int.TryParse(line[0], out int msgInd) && int.TryParse(line[1], out int strInd))
            //        {
            //            if (msg.Count > msgInd && msg[msgInd].Strings.Count > strInd)
            //                msg[msgInd].Strings[strInd].NewString = line[2].SplitByWidth(encoding, personaFont, width);
            //        }
            //    }
        }

        private void ImportText(string[][] text, Func<string, MSGstr, string> func)
        {
            foreach (var line in text)
                if (line.Length == 3)
                {
                    if (int.TryParse(line[0], out int msgInd) && int.TryParse(line[1], out int strInd))
                    {
                        if (msg.Count > msgInd && msg[msgInd].Strings.Count > strInd)
                            msg[msgInd].Strings[strInd].NewString = func.Invoke(line[2], msg[msgInd].Strings[strInd]);
                    }
                }
        }

        #region IGameFormat

        public FormatEnum Type => FormatEnum.PTP;

        public List<ObjectContainer> SubFiles { get; } = new List<ObjectContainer>();

        public int GetSize() => GetData().Length;

        public byte[] GetData()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(MS))
                {
                    byte[] buffer;
                    writer.Write(Encoding.ASCII.GetBytes("PTP0"));
                    long MSGLinkPos = MS.Position;
                    writer.Write(0);
                    writer.Write(msg.Count);
                    long NamesLinkPos = MS.Position;
                    writer.Write(0);
                    writer.Write(names.Count);
                    writer.Write(new byte[IOTools.Alignment(MS.Position, 0x10)]);

                    long MSGPos = MS.Position;
                    foreach (var a in msg)
                    {
                        writer.Write(a.Type == "MSG" ? 0 : 1);
                        buffer = new byte[24];
                        Encoding.ASCII.GetBytes(a.Name, 0, a.Name.Length, buffer, 0);
                        writer.Write(buffer);
                        writer.Write((ushort)a.Strings.Count);
                        writer.Write((ushort)a.CharacterIndex);

                        foreach (var b in a.Strings)
                        {
                            buffer = b.Prefix.GetByteArray();
                            writer.Write(buffer.Length);
                            writer.Write(buffer);
                            writer.Write(new byte[IOTools.Alignment(MS.Position, 4)]);

                            buffer = b.OldString.GetByteArray();
                            writer.Write(buffer.Length);
                            writer.Write(buffer);
                            writer.Write(new byte[IOTools.Alignment(MS.Position, 4)]);

                            buffer = b.Postfix.GetByteArray();
                            writer.Write(buffer.Length);
                            writer.Write(buffer);
                            writer.Write(new byte[IOTools.Alignment(MS.Position, 4)]);

                            buffer = Encoding.UTF8.GetBytes(b.NewString);
                            writer.Write(buffer.Length);
                            writer.Write(buffer);
                            writer.Write(new byte[IOTools.Alignment(MS.Position, 16)]);
                        }

                        writer.Write(new byte[IOTools.Alignment(MS.Position, 0x10)]);
                    }

                    long NamesPos = MS.Position;
                    foreach (var a in names)
                    {
                        buffer = a.OldName;
                        writer.Write(buffer.Length);
                        writer.Write(buffer);
                        writer.Write(new byte[IOTools.Alignment(MS.Position, 4)]);

                        buffer = Encoding.UTF8.GetBytes(a.NewName);
                        writer.Write(buffer.Length);
                        writer.Write(buffer);
                        writer.Write(new byte[IOTools.Alignment(MS.Position, 4)]);
                    }

                    MS.Position = MSGLinkPos;
                    writer.Write((int)MSGPos);
                    MS.Position = NamesLinkPos;
                    writer.Write((int)NamesPos);
                }
                return MS.ToArray();
            }
        }

        #endregion IFile        
    }
}