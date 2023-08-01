using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3PO_Converter
{
    public class C3POVersionDetector
    {
        public string MagicHeader;
        public int MagicByte;
        public int VersionID;
        public static int ReadVersionID(string path)
        {
            int Version;
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                string Header = StreamUtil.ReadNullEndString(stream);
                stream.Position += 1;
                Version = stream.ReadByte();
            }
            return Version;
        }

        public static int ReadJsonVersionID(string path)
        {
            var JsonData = LoadJSON(path);

            return JsonData.VersionID;
        }

        public static string VerifyJSON(string path)
        {


        }

        public static C3POVersionDetector LoadJSON(string path)
        {
            string paths = path;
            if (File.Exists(paths))
            {
                var stream = File.ReadAllText(paths);
                var container = JsonConvert.DeserializeObject<C3POVersionDetector>(stream);
                return container;
            }
            else
            {
                return new C3POVersionDetector();
            }
        }
    }
}
