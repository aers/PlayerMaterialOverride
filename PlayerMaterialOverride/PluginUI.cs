using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Actors;
using System.Runtime.InteropServices;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Numerics;
using PlayerMaterialOverride.Structs;
using System.Threading.Tasks;

namespace PlayerMaterialOverride
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;
        private DalamudPluginInterface pi;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool debugVisible = false;
        public bool DebugVisible
        {
            get { return this.debugVisible; }
            set { this.debugVisible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI(Configuration configuration, DalamudPluginInterface pi)
        {
            this.configuration = configuration;
            this.pi = pi;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawSettingsWindow();
            DrawDebugWindow();
        }

        public void DrawDebugWindow()
        {
            if (!DebugVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(700, 800), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(700, 800), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Player Material Override - Debug", ref this.debugVisible))
            {
                var pc = this.pi.ClientState.LocalPlayer;

                if (pc == null)
                    ImGui.Text("Player Character currently unavailable.");
                else
                {
                    unsafe
                    {
                        var pc3D = *(Structs.Character3D**)(pc.Address + 0xF0).ToPointer();
                        if ((long)pc3D == 0x0)
                            ImGui.Text("Player Character 3D currently unavailable.");
                        else
                        {
                            ImGui.Text($"Player Character 3D found {(long)pc3D:X} - Slot Count - {pc3D->SlotCount}");
                            if ((long)(pc3D->ModelArray) != 0x0)
                            {
                                ImGui.Text($"Model Array found {(long)(pc3D->ModelArray):X}");
                                for (int i = 0; i < pc3D->SlotCount; i++)
                                {
                                    if ((long)(pc3D->ModelArray[i]) != 0x0)
                                    {
                                        ImGui.Text($"Slot {i} - {(long)(pc3D->ModelArray[i]):X} - {Marshal.PtrToStringAnsi((IntPtr)pc3D->ModelArray[i]->MDL->FileNameString)}");
                                        for (int j = 0; j < pc3D->ModelArray[i]->MaterialCount; j++)
                                        {
                                            if (ImGui.Button($"Copy###Button_CopyMaterialName_{i}_{j}"))
                                            {
                                                System.Windows.Clipboard.SetText(Marshal.PtrToStringAnsi((IntPtr)pc3D->ModelArray[i]->MaterialArray[j]->MTRL->FileNameString));
                                            }
                                            ImGui.SameLine();
                                            ImGui.Text($"Material {j} - {(long)(pc3D->ModelArray[i]->MaterialArray[j])} - {Marshal.PtrToStringAnsi((IntPtr)pc3D->ModelArray[i]->MaterialArray[j]->MTRL->FileNameString)}");
                                            var oe = configuration.Overrides.Find(x => x.Active && (x.OriginalMaterial == Marshal.PtrToStringAnsi((IntPtr)pc3D->ModelArray[i]->MaterialArray[j]->MTRL->FileNameString) || (x.OverrideMaterial == Marshal.PtrToStringAnsi((IntPtr)pc3D->ModelArray[i]->MaterialArray[j]->MTRL->FileNameString))));
                                            if (oe != null)
                                            {
                                                ImGui.Indent(32);
                                                if (oe.OriginalMaterial == Marshal.PtrToStringAnsi((IntPtr)pc3D->ModelArray[i]->MaterialArray[j]->MTRL->FileNameString))
                                                {
                                                    ImGui.Text($"Matched Override -");
                                                    ImGui.SameLine();
                                                    ImGui.TextColored(new Vector4(255, 0, 0, 255), $"{oe.OriginalMaterial}");
                                                    ImGui.SameLine();
                                                    ImGui.Text($" overrides to {oe.OverrideMaterial}");
                                                }
                                                else
                                                {
                                                    ImGui.Text($"Matched Override - {oe.OriginalMaterial} overrides to");
                                                    ImGui.SameLine();
                                                    ImGui.TextColored(new Vector4(255, 0, 0, 255), $"{oe.OverrideMaterial}");
                                                }
                                                ImGui.Unindent(32);
                                            }
                                        }                                      
                                    }
                                    else
                                    {
                                        ImGui.Text($"Slot {i} - No Model loaded");
                                    }
                                }
                            }
                        }
                    }
                }


            }
            ImGui.End();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(300, 200), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(300, 200), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Player Material Override - Config", ref this.settingsVisible))
            {
                if (ImGui.Button("Reload Player 3D"))
                {
                    RerenderPlayerCharacter();
                }
                ImGui.SameLine();
                if (ImGui.Button("Open Debug Window"))
                {
                    this.DebugVisible = true;
                }
            }

            ImGui.Columns(4);
            ImGui.SetColumnWidth(0, (ImGui.GetWindowWidth() - 150) / 2);
            ImGui.SetColumnWidth(1, (ImGui.GetWindowWidth() - 150) / 2);
            ImGui.SetColumnWidth(2, 60);
            ImGui.SetColumnWidth(3, 80);
            ImGui.Text("Original Material");
            ImGui.NextColumn();
            ImGui.Text("Override Material");
            ImGui.NextColumn();
            ImGui.Text("Active");
            ImGui.NextColumn();
            ImGui.Text("Remove");
            ImGui.Separator();
            ImGui.NextColumn();

            int index = 0;
            OverrideEntry doRemove = null;

            foreach(var oe in configuration.Overrides)
            {
                var orig = oe.OriginalMaterial;
                var over = oe.OverrideMaterial;
                var acti = oe.Active;

                var changed = false;

                ImGui.SetNextItemWidth(-1);
                changed = ImGui.InputText($"###InputText_PMO_Original_{index}", ref orig, 256);
                ImGui.NextColumn();
                ImGui.SetNextItemWidth(-1);
                changed = ImGui.InputText($"###InputText_PMO_Override_{index}", ref over, 256) || changed;
                ImGui.NextColumn();
                changed = ImGui.Checkbox($"###Checkbox_PMO_Active_{index}", ref acti) || changed;
                ImGui.NextColumn();
                if (ImGui.Button($"Remove###Button_PMO_Remove_{index}"))
                {
                    doRemove = oe;
                }
                ImGui.NextColumn();
                index += 1;

                if (!changed) continue;

                oe.OriginalMaterial = orig;
                oe.OverrideMaterial = over;
                oe.Active = acti;
                configuration.Save();
            }
            ImGui.Columns(1);
            if (ImGui.Button("Add"))
            {
                configuration.Overrides.Add(new OverrideEntry());
                configuration.Save();
            }

            if (doRemove != null)
            {
                configuration.Overrides.Remove(doRemove);
                configuration.Save();
            }
            
            ImGui.End();
        }

        // borrowed from VoidList
        private async void RerenderPlayerCharacter()
        {
            await Task.Run(async () => {
                try
                {
                    if (this.pi.ClientState.LocalPlayer == null)
                        return;

                    var addrEntityType = this.pi.ClientState.LocalPlayer.Address + 0x8C;
                    var addrRenderToggle = this.pi.ClientState.LocalPlayer.Address + 0x104;

                    Marshal.WriteByte(addrEntityType, 2);
                    Marshal.WriteInt32(addrRenderToggle, 2050);
                    await Task.Delay(100);
                    Marshal.WriteInt32(addrRenderToggle, 0);
                    await Task.Delay(100);
                    Marshal.WriteByte(addrEntityType, 1);
                }
                catch (Exception ex)
                {
                    PluginLog.LogError(ex.ToString());
                }
            });
        }
    }
}
