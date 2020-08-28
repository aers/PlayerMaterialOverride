using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Plugin;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace PlayerMaterialOverride
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "PlayerMaterialOverride";

        private DalamudPluginInterface pi;
        private Configuration configuration;
        private PluginUI ui;

        /*
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr Chara3D_ResolveMDLPath(IntPtr thisChara3D, IntPtr buf, UIntPtr buf_size, uint slot);

       private Hook<Chara3D_ResolveMDLPath> resolveMDLPathHook;

        private IntPtr Chara3D_ResolveMDLPath_Detour(IntPtr thisChara3D, IntPtr buf, UIntPtr buf_size, uint slot)
        {
            PluginLog.Log("resolve MDL path called: Chara3D - {0}, Slot - {1}", thisChara3D.ToString("X16"), slot);

            var res = this.resolveMDLPathHook.Original(thisChara3D, buf, buf_size, slot);

            PluginLog.Log("result: {0}", Marshal.PtrToStringAnsi(res));

            return res;
        }
        */

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr Chara3D_ResolveMTRLPath(IntPtr thisChara3D, IntPtr buf, UIntPtr buf_size, uint slot, IntPtr mtrlString);

        private Hook<Chara3D_ResolveMTRLPath> resolveMTRLPathHook;

        private IntPtr Chara3D_ResolveMTRLPath_Detour(IntPtr thisChara3D, IntPtr buf, UIntPtr buf_size, uint slot, IntPtr mtrlString)
        {
            PluginLog.Log("resolve MTRL path called: Chara3D - {0}, Slot - {1}, MTRL String - {2}", thisChara3D.ToString("X16"), slot, Marshal.PtrToStringAnsi(mtrlString));

            var res = this.resolveMTRLPathHook.Original(thisChara3D, buf, buf_size, slot, mtrlString);

            string resAsString = Marshal.PtrToStringAnsi(res);

            PluginLog.Log("result: {0}", resAsString);

            unsafe
            {
                var pc = this.pi.ClientState.Actors[0];

                if (pc == null)
                {
                    //PluginLog.Log("abuh");
                    return res;
                }
                
                var pc3D = *(Structs.Character3D**)(pc.Address + 0xF0).ToPointer();

                if ((Structs.Character3D*)thisChara3D.ToPointer() == pc3D)
                {
                    //PluginLog.Log("is PC");
                    var oe = configuration.Overrides.Find(x => x.Active && x.OriginalMaterial == resAsString);
                    if (oe != null)
                    {
                        PluginLog.Log("overriding {0} -> {1}", resAsString, oe.OverrideMaterial);
                        var ansiStr = System.Text.Encoding.ASCII.GetBytes(oe.OverrideMaterial);
                        Marshal.Copy(ansiStr, 0, res, oe.OverrideMaterial.Length);
                    }
                }
            }
            /*
            if (resAsString == femaleSkin)
            {
                PluginLog.Log("match");

                var ansiStr = System.Text.Encoding.ASCII.GetBytes(maleSkin);

                Marshal.Copy(ansiStr, 0, res, maleSkin.Length);
            }*/

            return res;
        }

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pi = pluginInterface;
            
            this.configuration = this.pi.GetPluginConfig() as Configuration ?? new Configuration();
            this.configuration.Initialize(this.pi);

            this.ui = new PluginUI(this.configuration, this.pi);

            this.pi.UiBuilder.OnBuildUi += DrawUI;
            this.pi.UiBuilder.OnOpenConfigUi += (sender, args) => DrawConfigUI();

            var addresses = new AddressResolver();
            addresses.Setup(pi.TargetModuleScanner);    

            //this.resolveMDLPathHook = new Hook<Chara3D_ResolveMDLPath>(addresses.ResolveMDLPath, new Chara3D_ResolveMDLPath(Chara3D_ResolveMDLPath_Detour), this);
            //this.resolveMDLPathHook.Enable();

            this.resolveMTRLPathHook = new Hook<Chara3D_ResolveMTRLPath>(addresses.ResolveMTRLPath, new Chara3D_ResolveMTRLPath(Chara3D_ResolveMTRLPath_Detour), this);
            this.resolveMTRLPathHook.Enable();

            this.ui.SettingsVisible = true;
        }

        public void Dispose()
        {
            //this.resolveMDLPathHook.Dispose();
            this.resolveMTRLPathHook.Dispose();

            this.ui.Dispose();

            this.pi.Dispose();
        }

        private void DrawUI()
        {
            this.ui.Draw();
        }

        private void DrawConfigUI()
        {
            this.ui.SettingsVisible = true;
        }
    }
}
