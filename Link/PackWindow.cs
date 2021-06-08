using System;
using System.Collections.Generic;
using System.Text;

namespace Link
{
    [Serializable]
    public class PackWindow
    {
        public List<Pack> Packs { get; set; }
        public int UsefulPack { get; set; }
        public PackWindow(int usefulPack)
        { 
            Packs = new List<Pack>();
            UsefulPack = usefulPack;
        }
    }
}
