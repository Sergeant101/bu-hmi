using System;
using System.Collections.Generic;
using System.IO;
using Monokot.Hmi.Core.Framework.Runtime.Tree;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Core.Utils.Log;

namespace NGO.Elements.Special.IO
{
    public class Topology
    {
        public Topology()
        {
            modules = new List<Backplane>();
        }
        public string textId { get; set; }
        public string img { get; set; }
        public List<Backplane> modules { get; set; }

        public void createModules(string path, RuntimeTagsTree tags_tree)
        {
            try
            {
                var list = new List<Backplane>();
                var dir = Directory.GetDirectories(path);
                foreach (var d in dir)
                {
                    var dinf = new DirectoryInfo(d);
                    list.Add(new Backplane(tags_tree) {name = dinf.Name, location = dinf.FullName});
                }

                foreach (var m in list)
                    m.createIO(m.location, m.name);

                modules = list;
            }
            catch (Exception x)
            {
                LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ","Ошибка при формированиии топологии: " + x.Message);
            }
        }

    }
}
