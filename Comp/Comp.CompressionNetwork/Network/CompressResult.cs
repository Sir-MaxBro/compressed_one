using System.Collections.Generic;

namespace Comp.CompressionNetwork.Network
{
    public class CompressResult
    {
        public Dictionary<string, int> CompressMap { get; internal set; }

        public string Result { get; internal set; }
    }
}
