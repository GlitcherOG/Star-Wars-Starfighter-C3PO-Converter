using System.Diagnostics;
using Newtonsoft.Json;

namespace C3PO_Converter
{
    public class C3POHandlerVersion3
    {
        public string MagicHeader = "LucasArts Class File";
        public int MagicByte = 26;
        public int VersionID = 3;
        public int StructureType;

        public Type1Struct? Type1;
        public Type2Struct? Type2;

        public void Load(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                MagicHeader = StreamUtil.ReadNullEndString(stream);
                MagicByte = StreamUtil.ReadUInt8(stream);
                VersionID = StreamUtil.ReadUInt8(stream);
                stream.Position += 1;
                StructureType = StreamUtil.ReadUInt8(stream);

                if (StructureType == 1)
                {
                    var NewType = new Type1Struct();
                    NewType.ItemName = StreamUtil.ReadNullEndString(stream);
                    NewType.ItemClass = StreamUtil.ReadNullEndString(stream);
                    NewType.ItemModel = StreamUtil.ReadNullEndString(stream);
                    NewType.U1 = StreamUtil.ReadUInt8(stream);
                    NewType.U2 = StreamUtil.ReadNullEndString(stream);

                    NewType.PropertiesCount = StreamUtil.ReadUInt8(stream);
                    NewType.Properties = new List<Data>();
                    for (int i = 0; i < NewType.PropertiesCount; i++)
                    {
                        NewType.Properties.Add(LoadData(stream));
                    }

                    NewType.WeaponGroupCount = StreamUtil.ReadUInt8(stream);
                    NewType.WeaponGroups = new List<WeaponGroupsStruct>();
                    for (int i = 0; i < NewType.WeaponGroupCount; i++)
                    {
                        var NewGroup = new WeaponGroupsStruct();
                        NewGroup.GroupName = StreamUtil.ReadNullEndString(stream);
                        NewGroup.WeaponCount = StreamUtil.ReadUInt8(stream);
                        NewGroup.Weapons = new List<WeaponData>();
                        for (int a = 0; a < NewGroup.WeaponCount; a++)
                        {
                            var NewSubGrup = new WeaponData();
                            NewSubGrup.WeaponName = StreamUtil.ReadNullEndString(stream);
                            NewSubGrup.WeaponClass = StreamUtil.ReadNullEndString(stream);
                            NewSubGrup.WeaponType = StreamUtil.ReadNullEndString(stream);
                            NewGroup.Weapons.Add(NewSubGrup);
                        }

                        NewType.WeaponGroups.Add(NewGroup);
                    }

                    Type1 = NewType;
                }
                else if (StructureType == 2)
                {
                    var NewType = new Type2Struct();
                    NewType.ItemName = StreamUtil.ReadNullEndString(stream);
                    NewType.ItemClass = StreamUtil.ReadNullEndString(stream);
                    NewType.PropertiesCount = StreamUtil.ReadUInt8(stream);
                    NewType.Properties = new List<Data>();
                    for (int i = 0; i < NewType.PropertiesCount; i++)
                    {
                        NewType.Properties.Add(LoadData(stream));
                    }

                    NewType.PropertiesCount1 = StreamUtil.ReadUInt8(stream);
                    NewType.Properties1 = new List<Data>();
                    for (int i = 0; i < NewType.PropertiesCount1; i++)
                    {
                        NewType.Properties1.Add(LoadData(stream));
                    }

                    Type2 = NewType;
                }
                else
                {
                    MessageBox.Show("Error Unknown Data Type");
                }
            }
        }

        public void Save(string path)
        {
            Stream stream = new MemoryStream();

            StreamUtil.WriteNullString(stream, MagicHeader);
            StreamUtil.WriteUInt8(stream, MagicByte);
            StreamUtil.WriteUInt8(stream, VersionID);
            stream.Position += 1;

            StreamUtil.WriteUInt8(stream, StructureType);

            if (StructureType == 1)
            {
                var TempVar = Type1.Value;

                StreamUtil.WriteNullString(stream, TempVar.ItemName);
                StreamUtil.WriteNullString(stream, TempVar.ItemClass);
                StreamUtil.WriteNullString(stream, TempVar.ItemModel);
                StreamUtil.WriteUInt8(stream, TempVar.U1);
                StreamUtil.WriteNullString(stream, TempVar.U2);

                StreamUtil.WriteUInt8(stream, TempVar.Properties.Count);
                for (int i = 0; i < TempVar.Properties.Count; i++)
                {
                    SaveData(stream, TempVar.Properties[i]);
                }

                StreamUtil.WriteUInt8(stream, TempVar.WeaponGroups.Count);
                for (int i = 0; i < TempVar.WeaponGroups.Count; i++)
                {
                    StreamUtil.WriteNullString(stream, TempVar.WeaponGroups[i].GroupName);
                    StreamUtil.WriteUInt8(stream, TempVar.WeaponGroups[i].Weapons.Count);
                    for (int a = 0; a < TempVar.WeaponGroups[i].Weapons.Count; a++)
                    {
                        StreamUtil.WriteNullString(stream, TempVar.WeaponGroups[i].Weapons[a].WeaponName);
                        StreamUtil.WriteNullString(stream, TempVar.WeaponGroups[i].Weapons[a].WeaponClass);
                        StreamUtil.WriteNullString(stream, TempVar.WeaponGroups[i].Weapons[a].WeaponType);
                    }
                }
                //StreamUtil.WriteUInt8(stream, 0);
            }
            else if (StructureType == 2)
            {
                var TempVar = Type2.Value;
                StreamUtil.WriteNullString(stream, TempVar.ItemName);
                StreamUtil.WriteNullString(stream, TempVar.ItemClass);
                StreamUtil.WriteUInt8(stream, TempVar.Properties.Count);
                for (int i = 0; i < TempVar.Properties.Count; i++)
                {
                    SaveData(stream, TempVar.Properties[i]);
                }

                StreamUtil.WriteUInt8(stream, TempVar.Properties1.Count);
                for (int i = 0; i < TempVar.Properties1.Count; i++)
                {
                    SaveData(stream, TempVar.Properties1[i]);
                }
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var file = File.Create(path);
            stream.Position = 0;
            stream.CopyTo(file);
            stream.Dispose();
            file.Close();
        }

        public Data LoadData(Stream stream)
        {
            var NewData = new Data();

            NewData.PropertyName = StreamUtil.ReadNullEndString(stream);
            NewData.Type = StreamUtil.ReadUInt8(stream);
            if (NewData.Type == 1)
            {
                var NewType1 = new DataInt();
                NewType1.U0 = StreamUtil.ReadUInt8(stream);
                NewType1.U1 = StreamUtil.ReadUInt8(stream);
                NewType1.Value = StreamUtil.ReadUInt32(stream);
                NewData.dataInt = NewType1;
            }
            else if (NewData.Type == 2)
            {
                var NewType2 = new DataFloat();
                NewType2.U0 = StreamUtil.ReadUInt8(stream);
                NewType2.U1 = StreamUtil.ReadUInt8(stream);
                NewType2.Value = StreamUtil.ReadFloat(stream);
                NewData.dataFloat = NewType2;
            }
            else if (NewData.Type == 3)
            {
                var NewType3 = new DataString();
                NewType3.U0 = StreamUtil.ReadUInt8(stream);
                NewType3.U1 = StreamUtil.ReadUInt8(stream);
                NewType3.Value = StreamUtil.ReadNullEndString(stream);
                NewData.dataString = NewType3;
            }
            else if (NewData.Type == 4)
            {
                var NewType4 = new DataBool();
                NewType4.U0 = StreamUtil.ReadUInt8(stream);
                NewType4.U1 = StreamUtil.ReadUInt8(stream);
                NewType4.Value = Convert.ToBoolean(StreamUtil.ReadUInt8(stream));
                NewData.dataBool = NewType4;
            }
            else
            {
                Debug.WriteLine("Error Unknown Type " + NewData.Type);
                return NewData;
            }

            return NewData;
        }

        public void SaveData(Stream stream, Data data)
        {
            StreamUtil.WriteNullString(stream, data.PropertyName);
            StreamUtil.WriteUInt8(stream, data.Type);
            if (data.Type == 1)
            {
                StreamUtil.WriteUInt8(stream, data.dataInt.Value.U0);
                StreamUtil.WriteUInt8(stream, data.dataInt.Value.U1);
                StreamUtil.WriteInt32(stream, data.dataInt.Value.Value);
            }
            else if (data.Type == 2)
            {
                StreamUtil.WriteUInt8(stream, data.dataFloat.Value.U0);
                StreamUtil.WriteUInt8(stream, data.dataFloat.Value.U1);
                StreamUtil.WriteFloat32(stream, data.dataFloat.Value.Value);
            }
            else if (data.Type == 3)
            {
                StreamUtil.WriteUInt8(stream, data.dataString.Value.U0);
                StreamUtil.WriteUInt8(stream, data.dataString.Value.U1);
                StreamUtil.WriteNullString(stream, data.dataString.Value.Value);
            }
            else if (data.Type == 4)
            {
                StreamUtil.WriteUInt8(stream, data.dataBool.Value.U0);
                StreamUtil.WriteUInt8(stream, data.dataBool.Value.U1);
                StreamUtil.WriteUInt8(stream, Convert.ToInt32(data.dataBool.Value.Value));
            }
            else
            {
                Debug.WriteLine("Error Unknown Type " + data.Type);
            }
        }

        public void CreateJson(string path, bool Inline = false)
        {
            var TempFormating = Newtonsoft.Json.Formatting.None;
            if (Inline)
            {
                TempFormating = Newtonsoft.Json.Formatting.Indented;
            }

            var serializer = JsonConvert.SerializeObject(this, TempFormating, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText(path, serializer);
        }

        public static C3POHandlerVersion3 LoadJSON(string path)
        {
            string paths = path;
            if (File.Exists(paths))
            {
                var stream = File.ReadAllText(paths);
                var container = JsonConvert.DeserializeObject<C3POHandlerVersion3>(stream);
                return container;
            }
            else
            {
                return new C3POHandlerVersion3();
            }
        }

        public void CreateText(string path)
        {
            string n = "\n";
            string tab = "\t";

            string TextFile = MagicHeader + n;
            TextFile += "Version " + VersionID + n + n;

            if(StructureType==1)
            {
                TextFile += "Type 1" + n;
                TextFile += "ItemName " + Type1.Value.ItemName + n;
                TextFile += "ItemClass " + Type1.Value.ItemClass + n;
                TextFile += "ItemModel " + Type1.Value.ItemModel + n;
                TextFile += "U1 " + Type1.Value.U1 + n;
                TextFile += "U2 " + Type1.Value.U2 + n;

                if (Type1.Value.Properties.Count != 0)
                {
                    TextFile += n;

                    TextFile += "Properties" + n;
                    for (int i = 0; i < Type1.Value.Properties.Count; i++)
                    {
                        TextFile += tab + Type1.Value.Properties[i].PropertyName + " ";

                        if (Type1.Value.Properties[i].Type == 1)
                        {
                            TextFile += Type1.Value.Properties[i].dataInt.Value.U0 + " " + Type1.Value.Properties[i].dataInt.Value.U1 + " " + Type1.Value.Properties[i].dataInt.Value.Value;
                        }

                        if (Type1.Value.Properties[i].Type == 2)
                        {
                            TextFile += Type1.Value.Properties[i].dataFloat.Value.U0 + " " + Type1.Value.Properties[i].dataFloat.Value.U1 + " " + Type1.Value.Properties[i].dataFloat.Value.Value + "f";
                        }

                        if (Type1.Value.Properties[i].Type == 3)
                        {
                            TextFile += Type1.Value.Properties[i].dataString.Value.U0 + " " + Type1.Value.Properties[i].dataString.Value.U1 + " " + QuoteThis(Type1.Value.Properties[i].dataString.Value.Value);
                        }

                        if (Type1.Value.Properties[i].Type == 4)
                        {
                            string textValue = "false";

                            if (Type1.Value.Properties[i].dataBool.Value.Value)
                            {
                                textValue = "true";
                            }

                            TextFile += Type1.Value.Properties[i].dataBool.Value.U0 + " " + Type1.Value.Properties[i].dataBool.Value.U1 + " " + textValue;
                        }

                        TextFile += n;
                    }
                }

                if (Type1.Value.WeaponGroups.Count != 0)
                {
                    TextFile += n;

                    TextFile += "WeaponGroup" + n;
                    for (int i = 0; i < Type1.Value.WeaponGroups.Count; i++)
                    {
                        TextFile += tab + Type1.Value.WeaponGroups[i].GroupName + n;

                        for (int a = 0; a < Type1.Value.WeaponGroups[i].Weapons.Count; a++)
                        {
                            TextFile += tab + tab + Type1.Value.WeaponGroups[i].Weapons[a].WeaponName + " " + Type1.Value.WeaponGroups[i].Weapons[a].WeaponClass + " " + Type1.Value.WeaponGroups[i].Weapons[a].WeaponType + n;
                        }
                    }
                }
            }

            if (StructureType == 2)
            {
                TextFile += "Type 2" + n;
                TextFile += tab + "ItemName " + Type2.Value.ItemName + n;
                TextFile += tab + "ItemClass " + Type2.Value.ItemClass + n;

                if (Type2.Value.Properties.Count != 0)
                {
                    TextFile += n;

                    TextFile += tab + "Properties" + n;
                    for (int i = 0; i < Type2.Value.Properties.Count; i++)
                    {
                        TextFile += tab + tab + Type2.Value.Properties[i].PropertyName + " ";

                        if (Type2.Value.Properties[i].Type == 1)
                        {
                            TextFile += Type2.Value.Properties[i].dataInt.Value.U0 + " " + Type2.Value.Properties[i].dataInt.Value.U1 + " " + Type2.Value.Properties[i].dataInt.Value.Value;
                        }

                        if (Type2.Value.Properties[i].Type == 2)
                        {
                            TextFile += Type2.Value.Properties[i].dataFloat.Value.U0 + " " + Type2.Value.Properties[i].dataFloat.Value.U1 + " " + Type2.Value.Properties[i].dataFloat.Value.Value + "f";
                        }

                        if (Type2.Value.Properties[i].Type == 3)
                        {
                            TextFile += Type2.Value.Properties[i].dataString.Value.U0 + " " + Type2.Value.Properties[i].dataString.Value.U1 + " " + QuoteThis(Type2.Value.Properties[i].dataString.Value.Value);
                        }

                        if (Type2.Value.Properties[i].Type == 4)
                        {
                            string textValue = "false";

                            if (Type2.Value.Properties[i].dataBool.Value.Value)
                            {
                                textValue = "true";
                            }

                            TextFile += Type2.Value.Properties[i].dataBool.Value.U0 + " " + Type2.Value.Properties[i].dataBool.Value.U1 + " " + textValue;
                        }

                        TextFile += n;
                    }
                }

                if (Type2.Value.Properties1.Count != 0)
                {
                    TextFile += n;
                    TextFile += tab + "Properties1" + n;
                    for (int i = 0; i < Type2.Value.Properties1.Count; i++)
                    {
                        TextFile += tab + tab + Type2.Value.Properties1[i].PropertyName + " ";

                        if (Type2.Value.Properties1[i].Type == 1)
                        {
                            TextFile += Type2.Value.Properties1[i].dataInt.Value.U0 + " " + Type2.Value.Properties1[i].dataInt.Value.U1 + " " + Type2.Value.Properties1[i].dataInt.Value.Value;
                        }

                        if (Type2.Value.Properties1[i].Type == 2)
                        {
                            TextFile += Type2.Value.Properties1[i].dataFloat.Value.U0 + " " + Type2.Value.Properties1[i].dataFloat.Value.U1 + " " + Type2.Value.Properties1[i].dataFloat.Value.Value + "f";
                        }

                        if (Type2.Value.Properties1[i].Type == 3)
                        {
                            TextFile += Type2.Value.Properties1[i].dataString.Value.U0 + " " + Type2.Value.Properties1[i].dataString.Value.U1 + " " + QuoteThis(Type2.Value.Properties1[i].dataString.Value.Value);
                        }

                        if (Type2.Value.Properties1[i].Type == 4)
                        {
                            string textValue = "false";

                            if (Type2.Value.Properties1[i].dataBool.Value.Value)
                            {
                                textValue = "true";
                            }

                            TextFile += Type2.Value.Properties1[i].dataBool.Value.U0 + " " + Type2.Value.Properties1[i].dataBool.Value.U1 + " " + textValue;
                        }

                        TextFile += n;
                    }
                }
            }


            var Temp = File.CreateText(path);
            Temp.Write(TextFile);
            Temp.Close();
        }

        public static C3POHandlerVersion3 LoadText(string path)
        {
            C3POHandlerVersion3 handler = new C3POHandlerVersion3();

            string[] Lines = File.ReadAllLines(path);

            //Read all lines testing names
            //if properties or weapon properties

            for (int i = 0; i < Lines.Length; i++)
            {
                string[] Split = Lines[i].Split('"')
                     .Select((element, index) => index % 2 == 0  // If even index
                                           ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                                           : new string[] { element })  // Keep the entire item
                     .SelectMany(element => element).ToArray();

                if (Split.Length != 0)
                {
                    if (Split[0].ToLower() == "version")
                    {
                        if (Split[1] != "3")
                        {
                            return null;
                        }
                    }

                    if (Split[0].ToLower() == "type")
                    {
                        handler.StructureType = int.Parse(Split[1]);
                        if(handler.StructureType==1)
                        {
                            var TempType = new Type1Struct();

                            TempType.Properties = new List<Data>();
                            TempType.WeaponGroups = new List<WeaponGroupsStruct>();

                            handler.Type1 = TempType;
                        }
                        if (handler.StructureType == 2)
                        {
                            handler.Type2 = new Type2Struct();
                        }
                    }

                    if(handler.StructureType==1)
                    {
                        var TempType = handler.Type1.Value;
                        if (Split[0].ToLower() == "itemname")
                        {
                            TempType.ItemName = Split[1];
                        }
                        if (Split[0].ToLower() == "itemclass")
                        {
                            TempType.ItemClass = Split[1];
                        }
                        if (Split[0].ToLower() == "itemmodel")
                        {
                            TempType.ItemModel = Split[1];
                        }
                        if (Split[0].ToLower() == "u1")
                        {
                            TempType.U1 = int.Parse(Split[1]);
                        }
                        if (Split[0].ToLower() == "u2")
                        {
                            TempType.U2 = Split[1];
                        }

                        if (Split[0].ToLower() == "properties")
                        {
                            while(i < Lines.Length)
                            {
                                i++;
                                if (Split.Length==0)
                                {
                                    break;
                                }
                                if (!Split[0].Contains("\t"))
                                {
                                    break;
                                }

                                Data data = new Data();
                                Split[0] = Split[0].TrimStart('\t');

                                if (Lines[i].Contains("\""))
                                {
                                    //String
                                    data.Type = 3;

                                    var TempData = new DataString();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = Lines[3];

                                    data.dataString = TempData;
                                }
                                else if (Split[3].Contains("f"))
                                {
                                    //Float
                                    data.Type = 2;

                                    var TempData = new DataFloat();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = float.Parse(Lines[3]);

                                    data.dataFloat = TempData;
                                }
                                else if (Split[3].Contains("true") || Split[3].Contains("false"))
                                {
                                    //Bool
                                    data.Type = 4;

                                    var TempData = new DataBool();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = bool.Parse(Lines[3]);

                                    data.dataBool = TempData;
                                }
                                else
                                {
                                    //int
                                    data.Type = 1;

                                    var TempData = new DataInt();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = int.Parse(Lines[3]);

                                    data.dataInt = TempData;
                                }

                                TempType.Properties.Add(data);
                            }
                        }

                        handler.Type1 = TempType;
                    }

                    if (handler.StructureType == 2)
                    {
                        var TempType = handler.Type2.Value;
                        if (Split[0].ToLower() == "itemname")
                        {
                            TempType.ItemName = Split[1];
                        }
                        if (Split[0].ToLower() == "itemclass")
                        {
                            TempType.ItemClass = Split[1];
                        }

                        if (Split[0].ToLower() == "properties")
                        {
                            while (i < Lines.Length)
                            {
                                i++;
                                if (Split.Length == 0)
                                {
                                    break;
                                }
                                if (!Split[0].Contains("\t"))
                                {
                                    break;
                                }

                                Data data = new Data();
                                Split[0] = Split[0].TrimStart('\t');

                                if (Lines[i].Contains("\""))
                                {
                                    //String
                                    data.Type = 3;

                                    var TempData = new DataString();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = Lines[3];

                                    data.dataString = TempData;
                                }
                                else if (Split[3].Contains("f"))
                                {
                                    //Float
                                    data.Type = 2;

                                    var TempData = new DataFloat();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = float.Parse(Lines[3]);

                                    data.dataFloat = TempData;
                                }
                                else if (Split[3].Contains("true") || Split[3].Contains("false"))
                                {
                                    //Bool
                                    data.Type = 4;

                                    var TempData = new DataBool();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = bool.Parse(Lines[3]);

                                    data.dataBool = TempData;
                                }
                                else
                                {
                                    //int
                                    data.Type = 1;

                                    var TempData = new DataInt();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = int.Parse(Lines[3]);

                                    data.dataInt = TempData;
                                }

                                TempType.Properties.Add(data);
                            }
                        }

                        if (Split[0].ToLower() == "properties1")
                        {
                            while (i < Lines.Length)
                            {
                                i++;
                                if (Split.Length == 0)
                                {
                                    break;
                                }
                                if (!Split[0].Contains("\t"))
                                {
                                    break;
                                }

                                Data data = new Data();
                                Split[0] = Split[0].TrimStart('\t');

                                if (Lines[i].Contains("\""))
                                {
                                    //String
                                    data.Type = 3;

                                    var TempData = new DataString();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = Lines[3];

                                    data.dataString = TempData;
                                }
                                else if (Split[3].Contains("f"))
                                {
                                    //Float
                                    data.Type = 2;

                                    var TempData = new DataFloat();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = float.Parse(Lines[3]);

                                    data.dataFloat = TempData;
                                }
                                else if (Split[3].Contains("true") || Split[3].Contains("false"))
                                {
                                    //Bool
                                    data.Type = 4;

                                    var TempData = new DataBool();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = bool.Parse(Lines[3]);

                                    data.dataBool = TempData;
                                }
                                else
                                {
                                    //int
                                    data.Type = 1;

                                    var TempData = new DataInt();
                                    TempData.U0 = int.Parse(Split[1]);
                                    TempData.U1 = int.Parse(Split[2]);
                                    TempData.Value = int.Parse(Lines[3]);

                                    data.dataInt = TempData;
                                }

                                TempType.Properties1.Add(data);
                            }
                        }

                        handler.Type2 = TempType;
                    }

                }
            }

            return handler;
        }

        public string QuoteThis(string text)
        {
            return "\""+text+"\"";
        }

        public struct Type1Struct
        {
            public string ItemName;
            public string ItemClass;
            public string ItemModel;
            public int U1;
            public string U2;
            [JsonIgnore]
            public int PropertiesCount;
            public List<Data> Properties;
            [JsonIgnore]
            public int WeaponGroupCount;
            public List<WeaponGroupsStruct> WeaponGroups;
        }

        public struct Type2Struct
        {
            public string ItemName;
            public string ItemClass;
            [JsonIgnore]
            public int PropertiesCount;
            public List<Data> Properties;
            [JsonIgnore]
            public int PropertiesCount1;
            public List<Data> Properties1;
        }

        public struct Data
        {
            public string PropertyName;

            public int Type;
            public DataInt? dataInt;
            public DataFloat? dataFloat;
            public DataString? dataString;
            public DataBool? dataBool;
        }
        //1
        public struct DataInt
        {
            public int U0;
            public int U1;
            public int Value;
        }
        //2
        public struct DataFloat
        {
            public int U0;
            public int U1;
            public float Value;
        }
        //3
        public struct DataString
        {
            public int U0;
            public int U1;
            public string Value;
        }
        //4
        public struct DataBool
        {
            public int U0;
            public int U1;
            public bool Value;
        }

        public struct WeaponGroupsStruct
        {
            [JsonIgnore]
            public int WeaponCount;
            public string GroupName;
            public List<WeaponData> Weapons;
        }

        public struct WeaponData
        {
            public string WeaponName;
            public string WeaponClass;
            public string WeaponType;
        }
    }
}
