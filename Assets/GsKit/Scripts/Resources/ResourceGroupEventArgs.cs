using System;
using System.Collections.Generic;

namespace GsKit.Resources
{
    public class ResourceGroupEventArgs : EventArgs
    {
        public string ResourceGroupName { get; set; }
        public IReadOnlyDictionary<string, AbstractResource> Resources { get; set; }
    }
}