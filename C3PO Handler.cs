using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml;
using static C3PO_Converter.C3PO_Handler;

namespace C3PO_Converter
{
    public class C3PO_Handler
    {
        public string MagicHeader;
        public int VersionID;
        public string ItemName;
        public string ItemClass;
        [JsonIgnore]
        public int PropertiesCount;
        public List<Data> Properties = new List<Data>();

        public int VariblesType;
        public VaribleType0? VariblesType0;
        public VaribleType1? VariblesType1;
        public VaribleType2? VariblesType2;

        public void Load(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                MagicHeader = StreamUtil.ReadNullEndString(stream);
                VersionID = StreamUtil.ReadInt16(stream);
                if (VersionID != 1306)
                {
                    MessageBox.Show("Error Unknown Version " + VersionID + "\n Will Output Blank File");
                    return;
                }
                stream.Position += 1;

                ItemName = StreamUtil.ReadNullEndString(stream);
                ItemClass = StreamUtil.ReadNullEndString(stream);
                PropertiesCount = StreamUtil.ReadUInt8(stream);
                Properties = new List<Data>();
                for (int i = 0; i < PropertiesCount; i++)
                {
                    Properties.Add(LoadData(stream));
                }

                VariblesType = StreamUtil.ReadUInt32(stream);
                if(VariblesType == 10000) //Standard
                {
                    var NewEnd = new VaribleType0();
                    NewEnd.ModelFile = StreamUtil.ReadNullEndString(stream);
                    NewEnd.WeaponGroupCount = StreamUtil.ReadUInt8(stream);
                    NewEnd.WeaponGroups = new List<WeaponGroups>();
                    for (int i = 0; i < NewEnd.WeaponGroupCount; i++)
                    {
                        var NewGroup = new WeaponGroups();
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

                        NewEnd.WeaponGroups.Add(NewGroup);
                    }
                    NewEnd.FDataCount = StreamUtil.ReadUInt8(stream);
                    NewEnd.FDatas = new List<FDataStrcut>();
                    for (int i = 0; i < NewEnd.FDataCount; i++)
                    {
                        var NewGroup = new FDataStrcut();
                        NewGroup.FDataName = StreamUtil.ReadNullEndString(stream);
                        NewGroup.FDataStateCount = StreamUtil.ReadUInt8(stream);
                        NewGroup.FDataStates = new List<FDataState>();
                        for (int a = 0; a < NewGroup.FDataStateCount; a++)
                        {
                            var NewSubGrup = new FDataState();
                            NewSubGrup.StateName = StreamUtil.ReadNullEndString(stream);
                            NewSubGrup.StateClass = StreamUtil.ReadNullEndString(stream);
                            NewGroup.FDataStates.Add(NewSubGrup);
                        }

                        NewGroup.U1 = StreamUtil.ReadUInt8(stream);
                        NewGroup.U2 = StreamUtil.ReadNullEndString(stream);
                        NewGroup.U3 = StreamUtil.ReadUInt8(stream);
                        NewGroup.U4 = StreamUtil.ReadNullEndString(stream);
                        NewGroup.U5 = StreamUtil.ReadNullEndString(stream);
                        NewGroup.U6 = StreamUtil.ReadUInt8(stream);
                        NewGroup.FDataFile = StreamUtil.ReadNullEndString(stream);

                        NewEnd.FDatas.Add(NewGroup);
                    }

                    NewEnd.SubObjectCount = StreamUtil.ReadUInt8(stream);
                    NewEnd.SubObjects = new List<SubObjectStruct>();
                    for (int i = 0; i < NewEnd.SubObjectCount; i++)
                    {
                        var NewData1 = new SubObjectStruct();

                        NewData1.ObjectName = StreamUtil.ReadNullEndString(stream);
                        NewData1.ObjectClass = StreamUtil.ReadNullEndString(stream);
                        NewData1.U3 = StreamUtil.ReadUInt8(stream);

                        NewData1.Position = ReadFloats(3, stream);
                        NewData1.Orentation = ReadFloats(9, stream);
                        NewEnd.SubObjects.Add(NewData1);
                    }

                    NewEnd.SubObjectPropertiesCount = StreamUtil.ReadInt8(stream);
                    NewEnd.SubObjectProperties = new List<SubObjectPropertiesStruct>();
                    for (int i = 0; i < NewEnd.SubObjectPropertiesCount; i++)
                    {
                        var NewGroup = new SubObjectPropertiesStruct();
                        NewGroup.SubObjectName = StreamUtil.ReadNullEndString(stream);
                        NewGroup.PropertiesCount = StreamUtil.ReadUInt8(stream);
                        NewGroup.Properties = new List<SimpleProperties>();

                        for (int a = 0; a < NewGroup.PropertiesCount; a++)
                        {
                            var NewType2 = new SimpleProperties();
                            NewType2.PropertyName = StreamUtil.ReadNullEndString(stream);
                            NewType2.ValueType = StreamUtil.ReadUInt8(stream);
                            if (NewType2.ValueType == 1)
                            {
                                NewType2.StringValue = StreamUtil.ReadNullEndString(stream);
                            }
                            else if (NewType2.ValueType == 8)
                            {
                                NewType2.IntValue = StreamUtil.ReadUInt32(stream);
                            }
                            else if(NewType2.ValueType==10)
                            {
                                NewType2.FloatValue = StreamUtil.ReadFloat(stream);
                            }
                            NewGroup.Properties.Add(NewType2);
                        }

                        NewEnd.SubObjectProperties.Add(NewGroup);
                    }


                    VariblesType0 = NewEnd;
                }   
                else if (VariblesType == 10001) //Laser
                {
                    var NewEnd = new VaribleType1();
                    NewEnd.PropertiesCount = StreamUtil.ReadUInt8(stream);
                    NewEnd.Properties = new List<Data>();
                    for (int i = 0; i < NewEnd.PropertiesCount; i++)
                    {
                        NewEnd.Properties.Add(LoadData(stream));
                    }
                    VariblesType1 = NewEnd;
                }
                else if (VariblesType == 10002) //Light
                {
                    var NewEnd = new VaribleType2();
                    NewEnd.U0 = StreamUtil.ReadNullEndString(stream);
                    NewEnd.PropertiesCount = StreamUtil.ReadUInt8(stream);
                    NewEnd.Properties = new List<Data>();
                    for (int i = 0; i < NewEnd.PropertiesCount; i++)
                    {
                        NewEnd.Properties.Add(LoadData(stream));
                    }
                    VariblesType2 = NewEnd;
                }
                else
                {
                    Debug.WriteLine("Error StructData Type " + VariblesType);
                    return;
                }
                long pos = stream.Position;
            }
        }


        public void Save(string Path)
        {
            Stream stream = new MemoryStream();

            StreamUtil.WriteNullString(stream, MagicHeader);
            StreamUtil.WriteInt16(stream, VersionID);
            stream.Position += 1;

            StreamUtil.WriteNullString(stream, ItemName);
            StreamUtil.WriteNullString(stream, ItemClass);
            StreamUtil.WriteUInt8(stream, Properties.Count);
            for (int i = 0; i < Properties.Count; i++)
            {
                SaveData(stream, Properties[i]);
            }

            StreamUtil.WriteInt32(stream, VariblesType);
            if(VariblesType==10000)
            {
                var TempVar = VariblesType0.Value;

                StreamUtil.WriteNullString(stream, TempVar.ModelFile);
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

                StreamUtil.WriteUInt8(stream, TempVar.FDatas.Count);
                for (int i = 0; i < TempVar.FDatas.Count; i++)
                {
                    StreamUtil.WriteNullString(stream, TempVar.FDatas[i].FDataName);
                    StreamUtil.WriteUInt8(stream, TempVar.FDatas[i].FDataStates.Count);
                    for (int a = 0; a < TempVar.FDatas[i].FDataStates.Count; a++)
                    {
                        StreamUtil.WriteNullString(stream, TempVar.FDatas[i].FDataStates[a].StateName);
                        StreamUtil.WriteNullString(stream, TempVar.FDatas[i].FDataStates[a].StateClass);
                    }

                    StreamUtil.WriteUInt8(stream, TempVar.FDatas[i].U1);
                    StreamUtil.WriteNullString(stream, TempVar.FDatas[i].U2);
                    StreamUtil.WriteUInt8(stream, TempVar.FDatas[i].U3);
                    StreamUtil.WriteNullString(stream, TempVar.FDatas[i].U4);
                    StreamUtil.WriteNullString(stream, TempVar.FDatas[i].U5);
                    StreamUtil.WriteUInt8(stream, TempVar.FDatas[i].U6);
                    StreamUtil.WriteNullString(stream, TempVar.FDatas[i].FDataFile);
                }

                StreamUtil.WriteUInt8(stream, TempVar.SubObjects.Count);
                for (int i = 0; i < TempVar.SubObjects.Count; i++)
                {
                    StreamUtil.WriteNullString(stream, TempVar.SubObjects[i].ObjectName);
                    StreamUtil.WriteNullString(stream, TempVar.SubObjects[i].ObjectClass);
                    StreamUtil.WriteUInt8(stream, TempVar.SubObjects[i].U3);

                    SaveFloats(TempVar.SubObjects[i].Position, stream);
                    SaveFloats(TempVar.SubObjects[i].Orentation, stream);
                }

                StreamUtil.WriteUInt8(stream, TempVar.SubObjectProperties.Count);
                for (int i = 0; i < TempVar.SubObjectProperties.Count; i++)
                {
                    StreamUtil.WriteNullString(stream, TempVar.SubObjectProperties[i].SubObjectName);
                    StreamUtil.WriteUInt8(stream, TempVar.SubObjectProperties[i].Properties.Count);

                    for (int a = 0; a < TempVar.SubObjectProperties[i].Properties.Count; a++)
                    {
                        StreamUtil.WriteNullString(stream, TempVar.SubObjectProperties[i].Properties[a].PropertyName);
                        StreamUtil.WriteUInt8(stream, TempVar.SubObjectProperties[i].Properties[a].ValueType);
                        if(TempVar.SubObjectProperties[i].Properties[a].ValueType==1)
                        {
                            StreamUtil.WriteNullString(stream, TempVar.SubObjectProperties[i].Properties[a].StringValue);
                        }
                        else if (TempVar.SubObjectProperties[i].Properties[a].ValueType == 8)
                        {
                            StreamUtil.WriteInt32(stream, TempVar.SubObjectProperties[i].Properties[a].IntValue.Value);
                        }
                        else if (TempVar.SubObjectProperties[i].Properties[a].ValueType == 10)
                        {
                            StreamUtil.WriteFloat32(stream, TempVar.SubObjectProperties[i].Properties[a].FloatValue.Value);
                        }
                    }
                }

            }
            else if (VariblesType==10001)
            {
                var TempVar = VariblesType1.Value;
                StreamUtil.WriteUInt8(stream, TempVar.Properties.Count);
                for (int i = 0; i < TempVar.Properties.Count; i++)
                {
                    SaveData(stream, TempVar.Properties[i]);
                }
            }
            else if (VariblesType == 10002)
            {
                var TempVar = VariblesType2.Value;
                StreamUtil.WriteNullString(stream, TempVar.U0);
                StreamUtil.WriteUInt8(stream, TempVar.Properties.Count);
                for (int i = 0; i < TempVar.Properties.Count; i++)
                {
                    SaveData(stream, TempVar.Properties[i]);
                }
            }
            else
            {
                Debug.WriteLine("Error StructData Type " + VariblesType);
                return;
            }

            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
            var file = File.Create(Path);
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
            if(data.Type==1)
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

        public static C3PO_Handler LoadJSON(string path)
        {
            string paths = path;
            if (File.Exists(paths))
            {
                var stream = File.ReadAllText(paths);
                var container = JsonConvert.DeserializeObject<C3PO_Handler>(stream);
                return container;
            }
            else
            {
                return new C3PO_Handler();
            }
        }

        public float[] ReadFloats(int count, Stream stream)
        {
            float[] floats = new float[count];
            for (int i = 0; i < count; i++)
            {
                floats[i] = StreamUtil.ReadFloat(stream);
            }
            return floats;
        }

        public void SaveFloats(float[] floatArray, Stream stream)
        {
            for (int i = 0; i < floatArray.Length; i++)
            {
                StreamUtil.WriteFloat32(stream, floatArray[i]);
            }
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
        //10000
        public struct VaribleType0
        {
            public string ModelFile;
            [JsonIgnore]
            public int WeaponGroupCount;
            public List<WeaponGroups> WeaponGroups;
            [JsonIgnore]
            public int FDataCount;
            public List<FDataStrcut> FDatas;
            [JsonIgnore]
            public int SubObjectCount;
            public List<SubObjectStruct> SubObjects;
            [JsonIgnore]
            public int SubObjectPropertiesCount;
            public List<SubObjectPropertiesStruct> SubObjectProperties;
        }

        public struct WeaponGroups
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

        public struct FDataStrcut
        {
            [JsonIgnore]
            public int FDataStateCount;
            public string FDataName;
            public List<FDataState> FDataStates;

            public int U1;
            public string U2;
            public int U3;
            public string U4;
            public string U5;
            public int U6;
            public string FDataFile;
        }

        public struct FDataState
        {
            public string StateName;
            public string StateClass;
        }

        public struct SubObjectPropertiesStruct
        {
            [JsonIgnore]
            public int PropertiesCount;
            public string SubObjectName;
            public List<SimpleProperties> Properties;
        }

        public struct SimpleProperties
        {
            public string PropertyName;
            public int ValueType;
            public string? StringValue;
            public int? IntValue;
            public float? FloatValue;
        }

        public struct SubObjectStruct
        {
            public string ObjectName;
            public string ObjectClass;
            public int U3;

            public float[] Position;
            public float[] Orentation;
        }

        //10001
        public struct VaribleType1
        {
            [JsonIgnore]
            public int PropertiesCount;
            public List<Data> Properties;
        }
        //10002
        public struct VaribleType2
        {
            public string U0;
            [JsonIgnore]
            public int PropertiesCount;
            public List<Data> Properties;
        }
    }
}
