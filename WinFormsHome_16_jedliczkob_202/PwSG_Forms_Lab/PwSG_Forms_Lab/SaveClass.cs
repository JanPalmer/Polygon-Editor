using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static PwSG_Forms_Lab.Form1;

namespace PwSG_Forms_Lab
{
    public static class SaveClass
    {
        static public void JsonSerialize(List<Vertex> data, string fileName)
        {
            string s;
            s = JsonSerializer.Serialize<List<Vertex>>(data);
            StreamWriter sw = new StreamWriter(fileName+".dat");
            sw.Write(s);
            sw.Close();
        }

        static public List<Vertex> JsonDeserialize(string fileName)
        {
            string s;
            StreamReader sr = new StreamReader(fileName + ".dat");
            s = sr.ReadToEnd();
            sr.Close();
            List<Vertex> G = JsonSerializer.Deserialize<List<Vertex>>(s);
            return G;
        }
        
    }
}
