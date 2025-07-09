using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

using Newtonsoft.Json.Linq;

namespace FreeNet.classes
{
    static internal class Config
    {

        static public bool createDefaultConfig()
        {
            // old -p -m -a -f 2 -k 2 -e 1 --max-payload 2500 --min-ttl 128 --auto-ttl 1-4-128 --reverse-frag --native-frag
            // old 2 -p -e 1 --reverse-frag

            //TODO: DoH FIX (ISSUE #4)
            
            string json = """
                {
                  "GoodByeDPI": {
                    "Arguments": "-p -s -f 1 -e 1 -q --wrong-seq --native-frag --reverse-frag --fake-gen 30 --fake-resend 1 --fake-from-hex 18fc408e68 --fake-from-hex 7e60d032be",
                    "auto_restart": true
                  },
                  "DOH": {
                    "Arguments": "-u https://1.1.1.1/dns-query  --hosts-files=\"%pwd_no_disk%/programs/DoH/windows-386/hosts\"",
                    "auto_restart": true
                  },
                  "Proxy": {
                    "Arguments": "%pwd%/programs/3proxy-0.9.3/bin/cfg.cfg",
                    "auto_restart": true
                  }
                }
                """;

            File.WriteAllText("config.json", json);


            return true;
        }

        static public async Task<bool> writeConfigAsync(JObject JSON)
        {
            string json = JsonConvert.SerializeObject(JSON, Formatting.Indented);
            try
            {
                using (StreamWriter outputFile = new StreamWriter("config.json"))
                {
                   await outputFile.WriteAsync(json);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
                return false;
            }

            return true;
        }

        static private JObject getConfig()
        {
        Start:
            string text = "";

            try
            {
                if (File.Exists("config.json"))
                {
                    using (var myStream = File.Open(@"config.json", FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        StreamReader myReader = new StreamReader(myStream);
                        text = myReader.ReadToEnd();
                    }
                }
                else
                {
                    createDefaultConfig();
                    goto Start;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка чтения конф. файла", MessageBoxButton.OK);
                return null;
            }

            JObject json = JObject.Parse(text);

            return json;
        }

       async static public Task<bool> setCoifig(string name, string arg)
        {
            JObject conf = getConfig();

            if (conf.Count == 0)
            {
                return false;
            }

            conf[name]["Arguments"] = arg;

           await writeConfigAsync(conf);

            return true;
        }

        static public JObject getParams(string name)
        {
            JObject conf = getConfig();
            return (JObject) conf[name];
        }

    }
}
