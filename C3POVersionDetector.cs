using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        public static string VerifyJSON(string path, int VersionID)
        {
            string json = File.ReadAllText(path);
            string Out = "";
            IList<string> messages = new List<string>();
            bool valid = true;
            if (VersionID == 3)
            {
                JSchemaGenerator generator = new JSchemaGenerator();
                JSchema schema = generator.Generate(typeof(C3POHandlerVersion3));
                JObject file = JObject.Parse(json);
                valid = file.IsValid(schema,out messages);
            }
            else if(VersionID == 4)
            {
                JSchemaGenerator generator = new JSchemaGenerator();
                JSchema schema = generator.Generate(typeof(C3POHandlerVersion4));
                JObject file = JObject.Parse(json);
                valid = file.IsValid(schema, out messages);
            }
            else if (VersionID==5)
            {
                JSchemaGenerator generator = new JSchemaGenerator();
                JSchema schema = generator.Generate(typeof(C3POHandlerVersion5));
                JObject file = JObject.Parse(json);
                valid = file.IsValid(schema, out messages);
            }

            if(!valid)
            {
                foreach (var message in messages)
                {
                    Out += message + "\n";
                }
            }

            //Function doesnt work how i wanted it to
            return "";
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
