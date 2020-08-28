using Dalamud.Game;
using Dalamud.Game.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMaterialOverride
{
    class AddressResolver : BaseAddressResolver
    {
        public IntPtr ResolveMDLPath { get; private set; }
        public IntPtr ResolveMTRLPath { get; private set; }

        protected override void Setup64Bit(SigScanner sig)
        {
            ResolveMDLPath = sig.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 40 45 8B D1");
            ResolveMTRLPath = sig.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 50 49 8B F0 48 8B FA");
        }

    }
}
