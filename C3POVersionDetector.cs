using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3PO_Converter
{
    public class C3POVersionDetector
    {

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

    }
}
