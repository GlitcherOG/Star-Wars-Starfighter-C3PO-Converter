﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3PO_Converter
{
    public class C3POHandlerVersion4
    {
        public string MagicHeader;
        public int MagicByte;
        public int VersionID;
        public int U0;

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
                U0 = StreamUtil.ReadUInt8(stream);

                if (U0 == 1)
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
                else if (U0==2)
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

            StreamUtil.WriteUInt8(stream, U0);

            if(U0==1)
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
                StreamUtil.WriteUInt8(stream, 0);
            }
            else if (U0==2)
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

        public static C3POHandlerVersion4 LoadJSON(string path)
        {
            string paths = path;
            if (File.Exists(paths))
            {
                var stream = File.ReadAllText(paths);
                var container = JsonConvert.DeserializeObject<C3POHandlerVersion4>(stream);
                return container;
            }
            else
            {
                return new C3POHandlerVersion4();
            }
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
