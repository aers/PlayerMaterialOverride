using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMaterialOverride.Structs
{
    // Client::Graphics::Scene::Human
    // size - 0xA80
    // ctor - E8 ? ? ? ? 48 8B F8 48 85 C0 74 28 48 8D 55 D7 
    public class SceneHumanOffsets
    {
        public const int SlotCount = 0x98;
        public const int ModelArray = 0xA8;
        public const int MaterialArray = 0x2E8;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct SceneHuman
    {
        [FieldOffset(SceneHumanOffsets.SlotCount)] public int SlotCount;
        [FieldOffset(SceneHumanOffsets.ModelArray)] public RenderModel** ModelArray;
        [FieldOffset(SceneHumanOffsets.MaterialArray)] public RenderMaterial** MaterialArray;
    }
}
