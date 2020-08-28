using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMaterialOverride
{
    [Serializable]
    public class OverrideEntry
    {
        public string OriginalMaterial { get; set; } = "Edit me!";
        public string OverrideMaterial { get; set; } = "Edit me!";
        public bool Active { get; set; } = false;
    }
}
