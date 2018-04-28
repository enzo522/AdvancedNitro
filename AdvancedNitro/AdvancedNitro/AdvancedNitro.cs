using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AdvancedNitro
{
    public class AdvancedNitro : Script
    {
        public static bool myNitroSystemEnabled;
        public static float myNitroAmount;
        private List<string> exhausts;
        private bool isNitroOn;
        private bool isSoundOn;

        static AdvancedNitro()
        {
            myNitroSystemEnabled = false;
            myNitroAmount = 300.0f;
        }

        public AdvancedNitro()
        {
            exhausts = new List<string>
            {
                "exhaust",
                "exhaust_2",
                "exhaust_3",
                "exhaust_4",
                "exhaust_5",
                "exhaust_6",
                "exhaust_7",
                "exhaust_8",
                "exhaust_9",
                "exhaust_10",
                "exhaust_11",
                "exhaust_12",
                "exhaust_13",
                "exhaust_14",
                "exhaust_15",
                "exhaust_16"
            };
            isNitroOn = false;
            isSoundOn = false;

            Tick += onTick;
            KeyDown += onKeyDown;
            KeyUp += onKeyUp;
        }

        private void onTick(Object sender, EventArgs e)
        {
            if (Game.Player.Character.IsSittingInVehicle() && Game.Player.Character.CurrentVehicle.IsToggleModOn(VehicleToggleMod.Turbo))
                myNitroSystemEnabled = true;
            else
            {
                isNitroOn = false;
                isSoundOn = false;
                myNitroAmount = 300.0f;
                myNitroSystemEnabled = false;
            }

            if (myNitroSystemEnabled)
            {
                if (isNitroOn)
                {
                    if (myNitroAmount > 0)
                    {
                        Game.Player.Character.CurrentVehicle.EnginePowerMultiplier = 7.0f;
                        Game.Player.Character.CurrentVehicle.EngineTorqueMultiplier = 7.0f;

                        float pitch = Function.Call<float>(Hash.GET_ENTITY_PITCH, Game.Player.Character.CurrentVehicle);

                        if (Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, "core"))
                        {
                            foreach (string exhaust in exhausts)
                            {
                                if (Game.Player.Character.CurrentVehicle.HasBone(exhaust))
                                {
                                    float scale = Game.Player.Character.CurrentVehicle.Speed / 50;
                                    Vector3 offset = Game.Player.Character.CurrentVehicle.GetBoneCoord(exhaust);
                                    Vector3 exhPosition = Game.Player.Character.CurrentVehicle.GetOffsetFromWorldCoords(offset);
                                    Function.Call(Hash._SET_PTFX_ASSET_NEXT_CALL, "core");

                                    if (Function.Call<bool>(Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, "veh_backfire", Game.Player.Character.CurrentVehicle, exhPosition.X, exhPosition.Y, exhPosition.Z, 0.0f, pitch, 0.0f, scale, false, false, false))
                                        isSoundOn = true;
                                }
                            }
                        }
                        else Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, "core");

                        myNitroAmount -= 2.0f;
                    }
                    else
                    {
                        Game.Player.Character.CurrentVehicle.EnginePowerMultiplier = 1.0f;
                        Game.Player.Character.CurrentVehicle.EngineTorqueMultiplier = 1.0f;
                        myNitroAmount = 0.0f;
                        isNitroOn = false;
                        isSoundOn = false;
                    }

                    if (isSoundOn) Game.PlaySound("Short_Transition_In", "PLAYER_SWITCH_CUSTOM_SOUNDSET");
                }
                else
                {
                    if (myNitroAmount < 300) myNitroAmount += 0.5f;
                    else myNitroAmount = 300.0f;
                }
            }
        }

        private void onKeyDown(Object sender, KeyEventArgs e)
        {
            if (myNitroSystemEnabled && myNitroAmount >= 100)
            {
                if (e.KeyCode.Equals(Keys.ShiftKey) || e.KeyCode.Equals(Keys.ControlKey)) isNitroOn = true;
            }
        }

        private void onKeyUp(Object sender, KeyEventArgs e)
        {
            if (myNitroSystemEnabled && e.KeyCode.Equals(Keys.ControlKey)) isNitroOn = false;
        }
    }
}