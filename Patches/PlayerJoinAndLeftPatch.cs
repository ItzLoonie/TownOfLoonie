using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using AmongUs.Data;
using InnerNet;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Text;
using Hazel;
using Assets.CoreScripts;
using UnityEngine;
using static TownOfHost.Translator;

namespace TownOfHost
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
    class OnGameJoinedPatch
    {
        public static void Postfix(AmongUsClient __instance)
        {
            Logger.Info($"{__instance.GameId}に参加", "OnGameJoined");
            Main.playerVersion = new Dictionary<byte, PlayerVersion>();
            Main.devNames = new Dictionary<byte, string>();
            RPC.RpcVersionCheck();
            SoundManager.Instance.ChangeMusicVolume(DataManager.Settings.Audio.MusicVolume);

            NameColorManager.Begin();
            Options.Load();
            //Main.devIsHost = PlayerControl.LocalPlayer.GetClient().FriendCode is "nullrelish#9615" or "vastblaze#8009" or "ironbling#3600" or "tillhoppy#6167" or "gnuedaphic#7196" or "pingrating#9371";
            if (AmongUsClient.Instance.AmHost)
            {
                if (PlayerControl.GameOptions.killCooldown == 0.1f)
                    PlayerControl.GameOptions.killCooldown = Main.LastKillCooldown.Value;
            }
        }
    }
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    class OnPlayerJoinedPatch
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
        {
            Logger.Info($"{client.PlayerName}(ClientID:{client.Id}) (FreindCode:{client.FriendCode}) joined the game.", "Session");
            if (DestroyableSingleton<FriendsListManager>.Instance.IsPlayerBlockedUsername(client.FriendCode) && AmongUsClient.Instance.AmHost)
            {
                AmongUsClient.Instance.KickPlayer(client.Id, true);
                Logger.Info($"This is a blocked player. {client?.PlayerName}({client.FriendCode}) was banned.", "BAN");
            }
            Main.playerVersion = new Dictionary<byte, PlayerVersion>();
            RPC.RpcVersionCheck();
            if (AmongUsClient.Instance.AmHost)
            {
                new LateTask(() =>
                {
                    if (client.Character != null)
                    {
                        ChatCommands.SendTemplate("welcome", client.Character.PlayerId, true);
                        string rname = client.Character.Data.PlayerName;
                        bool customTag = false;
                        if (client.FriendCode is "nullrelish#9615" or "vastblaze#8009" or "ironbling#3600" or "tillhoppy#6167" or "gnuedaphic#7196" /*or "pingrating#9371"*/)
                        {
                            customTag = false;
                            Main.devNames.Add(client.Character.PlayerId, rname);
                            string fontSize = "1.5";
                            string fontSize1 = "1.2";
                            string neww = PlayerControl.LocalPlayer.FriendCode == "gnuedaphic#7196" ? $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.serverbooster), " + ServerBooster")}</size>" : "";
                            string dev = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.TheGlitch), "Dev" + neww)}</size>";
                            string name = dev + "\r\n" + rname;
                            client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.TheGlitch), name)}");
                        }
                        /*   if (client.FriendCode is "stormydot#5793")
                           {
                               customTag = true;
                               Main.devNames.Add(client.Character.PlayerId, rname);
                               client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thetaa), rname)}");
                           } */
                        /*     if (client.FriendCode is "envykindlye#7034")
                             {
                                 customTag = true;
                                 Main.devNames.Add(client.Character.PlayerId, rname);
                                 string fontSize0 = "1.5";
                                 string fontSize1 = "0.8";
                                 string fontSize3 = "0.5";
                                 string fontSize4 = "1";

                                 //ROSE TITLE START
                                 string sns1 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns1), "♡")}</size>";
                                 string sns2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "N")}</size>";
                                 string sns3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns3), "O")}</size>";
                                 string sns4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "T")}</size>";
                                 string sns14 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "♡")}</size>";
                                 string sns5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns5), "S")}</size>";
                                 string sns6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns6), "U")}</size>";
                                 string sns7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "S")}</size>";
                                 string sns8 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns8), "♡")}</size>";
                                 //ROSE NAME START
                                 string sns91 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "♡")}</size>";
                                 string sns9 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns6), "shi")}</size>";
                                 string sns0 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns5), "ft")}</size>";
                                 string sns01 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "yr")}</size>";
                                 string sns02 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns3), "os")}</size>";
                                 string sns03 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "e")}</size>";
                                 string sns92 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns1), "♡")}</size>";

                                 string snsname = sns1 + sns2 + sns3 + sns4 + sns14 + sns5 + sns6 + sns7 + sns8 + "\r\n" + sns91 + sns9 + sns0 + sns01 + sns02 + sns03 + sns92; //ROSE NAME & TITLE

                                 client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rosecolor), snsname)}");
                             } */

                        if (client.FriendCode is "envykindly#7034")
                        {
                            customTag = true;
                            string fontSize0 = "1.5";
                            string fontSize1 = "0.8";
                            string fontSize3 = "0.5";
                            string fontSize4 = "1";

                            //ROSE TITLE START
                            string sns1 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns1), "★")}</size>";
                            string sns2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "N")}</size>";
                            string sns3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns3), "O")}</size>";
                            string sns4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "T")}</size>";
                            string sns10 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "♥")}</size>";
                            string sns5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns5), "S")}</size>";
                            string sns6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns6), "U")}</size>";
                            string sns7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "S")}</size>";
                            string sns8 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns8), "★")}</size>";
                            //ROSE NAME START
                            string sns91 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "亗")}</size>";
                            string sns9 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "shi")}</size>";
                            string sns0 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns5), "ft")}</size>";
                            string sns01 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns3), "yr")}</size>";
                            string sns02 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "os")}</size>";
                            string sns03 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "e")}</size>";
                            string sns92 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns1), "亗")}</size>";

                            string snsname = sns1 + sns2 + sns3 + sns4 + sns10 + sns5 + sns6 + sns7 + sns8 + "\r\n" + sns91 + sns9 + sns0 + sns01 + sns02 + sns03 + sns92; //ROSE NAME & TITLE

                            client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rosecolor), snsname)}");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        /*  if (client.FriendCode is "warmtablet#3212" or "famousdove#2275" or "sidecurve#9629" or "twintruck#6031")
                          {
                              customTag = true;
                              Main.devNames.Add(client.Character.PlayerId, rname);
                              string fontSize = "1.2";
                              string fontSize2 = "1.5";
                              string sb = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.serverbooster), "Server Booster")}</size>";
                              string name = sb + "\r\n" + $"<size={fontSize2}>{rname}</size>";
                              client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.serverbooster), name)}");
                          }
                          if (client.FriendCode is "famousdove#2275")
                          {
                              customTag = true;
                              Main.devNames.Add(client.Character.PlayerId, rname);
                              string fontSize = "1.2";
                              string fontSize2 = "1.5";
                              string sb = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.minaa), "Server Booster")}</size>";
                              string name = sb + "\r\n" + $"<size={fontSize2}>{rname}</size>";
                              client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.minaa), name)}");
                          }
                          if (client.FriendCode is "legiblepod#9124")
                          {
                              customTag = true;
                              Main.devNames.Add(client.Character.PlayerId, rname);
                              string fontSize0 = "1.5";
                              string fontSize1 = "0.8";
                              string fontSize3 = "0.5";
                              string fontSize4 = "1";

                              // EEVEE TITLE START
                              string sns1 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "!")}</size>";
                              string sns2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "a")}</size>";
                              string sns3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "E")}</size>";
                              string sns4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "e")}</size>";
                              string sns5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "v")}</size>";
                              string sns6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "e")}</size>";
                              string sns7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "e")}</size>";
                              string sns8 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "!")}</size>";
                              string sns91 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "")}</size>";
                              string sns9 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "Cha")}</size>";
                              string sns0 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "ri")}</size>";
                              string sns01 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "za")}</size>";
                              string sns02 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "r")}</size>";
                              string sns03 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "d")}</size>";
                              string sns92 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "")}</size>";

                              string snsname = sns1 + sns2 + sns3 + sns4 + sns5 + sns6 + sns7 + sns8 + "\r\n" + sns91 + sns9 + sns0 + sns01 + sns02 + sns03 + sns92;

                              client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), snsname)}");
                          }
                          if (client.FriendCode is "luckyplus#8283" or "available#2356") //candy
                          {
                              customTag = true;
                              Main.devNames.Add(client.Character.PlayerId, rname);
                              string fontSize = "1.5"; //name
                              string fontSize1 = "0.8"; //title
                              string fontSize3 = "0.5"; //title hearts
                              string fontSize5 = "1"; //name hearts
                              string fontSize4 = "2"; //name

                              //CANDY TITLE START
                              string kr0 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh1), "♡")}</size>";
                              string kr1 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh1), "C")}</size>";
                              string kr2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh2), "h")}</size>";
                              string kr3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh3), "i")}</size>";
                              string kr4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh4), "l")}</size>";
                              string kr5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh5), "d ")}</size>";
                              string kr6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh6), "O")}</size>";
                              string kr7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh7), "f ")}</size>";
                              string kr8 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh7), "Be")}</size>";
                              string kr9 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh8), "li")}</size>";
                              string kr10 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh9), "al")}</size>";
                              string kr11 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh9), "♡")}</size>";
                              //CANDY NAME START
                              string krz1 = $"<size={fontSize5}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh1), "♡")}</size>";
                              string krz2 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh2), "c")}</size>";
                              string krz3 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh3), "a")}</size>";
                              string krz4 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh4), "n")}</size>";
                              string krz5 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh5), "d")}</size>";
                              string krz6 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh6), "y")}</size>";
                              string krz7 = $"<size={fontSize5}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh7), "♡")}</size>";

                              string krzname = kr0 + kr1 + kr2 + kr3 + kr4 + kr5 + kr6 + kr7 + kr8 + kr9 + kr10 + kr11 + "\r\n" + krz1 + krz2 + krz3 + krz4 + krz5 + krz6 + krz7;//KRZ NAME

                              //client.Character.RpcSetColor(17);
                              client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rosecolor), krzname)}</size>");
                          } */

                        if (client.FriendCode is "legiblepod#9124")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //EEVEE TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "Charizard")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                        if (client.FriendCode is "moonside#5200")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //ALLIE TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.allie), "Pineapple")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(5);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.allie), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                        /*     if (client.FriendCode is "stormydot#5793")
                             {
                                 customTag = true;
                                 string fontSize = "1.5"; //name
                                 string fontSize1 = "1.2"; //title

                                 //THETA TAG
                                 string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.theta), "Theta")}</size>";
                                 string name = dev + "\r\n" + rname; //DEVS

                                 client.Character.RpcSetColor(13);
                                 client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.theta), name)}</size>");
                                 Main.devNames.Add(client.Character.PlayerId, rname);
                             } */
                        if (client.FriendCode is "nebulardot#5943")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //LILAC TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.lilac), "Queen Kitty")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.lilac), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "sharpdrone#0857")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //CREWPOSTOR TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.CrewPostor), "Crewy")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(4);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.CrewPostor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "beespotty#5432")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //RAINBOW TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.coralcolor), "Sidekick")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(17);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.pinkcolor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "retroozone#9714")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //BEN TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.limecolor), "Unicorn")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.cyancolor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "knottrusty#2834" or "jumbopair#3525")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //RISSY TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rissy), "rissy")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rissy), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "sidecurve#9629")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //titlee

                            //EV TAG
                            //  string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.pinkcolor), "The Queen")}</size>";
                            //EV TITLE START
                            string ev1 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev1), "亗 T")}</size>";
                            string ev2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev2), "h")}</size>";
                            string ev3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev3), "e")}</size>";
                            string ev9 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev0), " ")}</size>";
                            string ev4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev4), "Q")}</size>";
                            string ev5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev5), "u")}</size>";
                            string ev6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev6), "e")}</size>";
                            string ev7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev7), "e")}</size>";
                            string ev8 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev8), "n 亗")}</size>";
                            string name = ev1 + ev2 + ev3 + ev9 + ev4 + ev5 + ev6 + ev7 + ev8 + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ev0), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "gnuedaphic#7196")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title
                            // TAG
                            string tscA = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc1), "亗")}</size>";
                            string tscB = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc1), "T")}</size>";
                            string tscC = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc2), "h")}</size>";
                            string tscD = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc3), "e ")}</size>";
                            string tscE = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc4), "K")}</size>";
                            string tscF = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc5), "i")}</size>";
                            string tscG = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc6), "n")}</size>";
                            string tscH = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc7), "g")}</size>";
                            string tscI = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc7), "亗")}</size>";
                            // NAME
                            string tsc1 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc1), "T")}</size>";
                            string tsc2 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc2), "S")}</size>";
                            string tsc3 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc3), "C")}</size>";
                            string tscname = tscA + tscB + tscC + tscD + tscE + tscF + tscG + tscH + tscI + "\r\n" + tsc1 + tsc2 + tsc3;

                            client.Character.RpcSetColor(2);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.tsc1), tscname)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                        // VANILLA IMPOSTOR CONVERT TEST
                        /*       if (client.FriendCode is "vastblaze#8009")
                               {
                                   client.Character.RpcSetRoleDesync(RoleTypes.Impostor);
                               } */

                        if (client.FriendCode is "luckyplus#8283")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //CANDY TITLE START
                            string kr0 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh1), "♡")}</size>";
                            string kr1 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh1), "T")}</size>";
                            string kr2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh2), "h")}</size>";
                            string kr3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh3), "e")}</size>";
                            string kr4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh4), "A")}</size>";
                            string kr5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh5), "r")}</size>";
                            string kr6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh6), "t")}</size>";
                            string kr7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh7), "i")}</size>";
                            string kr8 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh8), "s")}</size>";
                            string kr9 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh9), "t")}</size>";
                            string kr10 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh9), "♡")}</size>";
                            //CANDY NAME START
                            string krz1 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh1), "♡")}</size>";
                            string krz2 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh2), "c")}</size>";
                            string krz3 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh3), "a")}</size>";
                            string krz4 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh4), "n")}</size>";
                            string krz5 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh5), "d")}</size>";
                            string krz6 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh6), "y")}</size>";
                            string krz7 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.psh7), "♡")}</size>";

                            string krzname = kr1 + kr2 + kr3 + kr4 + kr5 + kr6 + kr7 + kr8 + kr9 + "\r\n" + krz1 + krz2 + krz3 + krz4 + krz5 + krz6 + krz7;//KRZ NAME
                                                                                                                                                           //  string fontSize = "1.5"; //name
                                                                                                                                                           //   string fontSize1 = "1.2"; //title

                            //CANDY TAG
                            //  string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.candy), "Kritz")}</size>";
                            //     string name = dev + "\r\n" + rname; //DEVS


                            client.Character.RpcSetColor(17);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.candy), krzname)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "stonechill#0791")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //LATINA TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rosecolor), "latina")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rosecolor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "mossmodel#2348")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //AUGUST TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.august), "August")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            //    client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.august), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "sphinxchic#9616")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //CINNA TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.cinna), "cinnabun")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            //    client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.cinna), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "supbay#9710")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //MITSKI TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.mitski), "Mitski")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            //    client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.mitski), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "farealike#0862")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            //WAW TAG BECAUSE ALLIE MADE ME DO IT FDGVBHJGFDFGHJKM
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.waw), "Allie's Cat")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(16);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.waw), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "rosepeaky#4209")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            // ESSENCE TAG
                            //  string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess), "Ess")}</size>";
                            // TAG
                            string essA = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess1), "E")}</size>";
                            string essB = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess2), "s")}</size>";
                            string essC = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess3), "s")}</size>";
                            // NAME
                            string ess1 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess1), "E")}</size>";
                            string ess2 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess2), "s")}</size>";
                            string ess3 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess3), "s")}</size>";
                            string ess4 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess4), "e")}</size>";
                            string ess5 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess5), "n")}</size>";
                            string ess6 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess6), "c")}</size>";
                            string ess7 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess7), "e")}</size>";
                            string name = essA + essB + essC + "\r\n" + ess1 + ess2 + ess3 + ess4 + ess5 + ess6 + ess7; //DEVS

                            client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ess), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "setloser#1264")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            // FELISA TAG (this is a joke pls dont hurt me)
                            //  string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.felicia), "Usual Thief")}</size>";
                            // TAG
                            string usualA = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief1), "U")}</size>";
                            string usualB = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief2), "s")}</size>";
                            string usualC = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief3), "u")}</size>";
                            string usualD = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief4), "a")}</size>";
                            string usualE = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief5), "l")}</size>";
                            string usualF = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief6), " ")}</size>";
                            string usualG = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief6), "T")}</size>";
                            string usualH = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief7), "h")}</size>";
                            string usualI = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief8), "i")}</size>";
                            string usualJ = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief9), "e")}</size>";
                            string usualK = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief10), "f")}</size>";
                            // NAME
                            string usual1 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief1), "b")}</size>";
                            string usual2 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief2), "y")}</size>";
                            string usual3 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief3), "e")}</size>";
                            string usual4 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief4), "f")}</size>";
                            string usual5 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief5), "e")}</size>";
                            string usual6 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief6), "l")}</size>";
                            string usual7 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief7), "i")}</size>";
                            string usual8 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief8), "c")}</size>";
                            string usual9 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief9), "i")}</size>";
                            string usual10 = $"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thief10), "a")}</size>";
                            string name = usualA + usualB + usualC + usualD + usualE + usualF + usualG + usualH + usualI + usualJ + usualK + "\r\n" + usual1 + usual2 + usual3 + usual4 + usual5 + usual6 + usual7 + usual8 + usual9 + usual10; //DEVS


                            client.Character.RpcSetColor(3);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.felicia), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "sizepetite#0049")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.2"; //title

                            // 2THIC2VENT TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.thic), "she thic")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(12);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.vent), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "smallcook#7028")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.5"; //title

                            // SLEEPYPIE TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.TheGlitch), "Sleepy")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(7);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.Impostor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "sparebank#8022")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.5"; //title

                            // JAX TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.Engineer), "Bad Bitch")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            //   client.Character.RpcSetColor(7);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.Engineer), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "warmtablet#3212" or "twintruck#6031" or "casualclod#9221" or "orangecorn#2095" or "livingwire#7731" or "politictip#0012")
                        {
                            customTag = true;
                            string fontSize = "1.5"; //name
                            string fontSize1 = "1.5"; //title

                            // SERVER BOOSTER TAG PORT
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.serverbooster), "Server Booster")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            //      client.Character.RpcSetColor(7);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.serverbooster), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                    }
                    //nice
                }, 3f, "Welcome Message & Name Check");
            }
        }
    }
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    class OnPlayerLeftPatch
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData data, [HarmonyArgument(1)] DisconnectReasons reason)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (GameStates.IsInGame)
            {
                Utils.CountAliveImpostors();
                if (data.Character.Is(CustomRoles.TimeThief))
                    data.Character.ResetVotingTime();
                if (data.Character.GetCustomSubRole() == CustomRoles.LoversRecode && !data.Character.Data.IsDead)
                    foreach (var lovers in Main.LoversPlayers.ToArray())
                    {
                        Main.isLoversDead = true;
                        Main.LoversPlayers.Remove(lovers);
                        Main.HasModifier.Remove(lovers.PlayerId);
                        Main.AllPlayerCustomSubRoles[lovers.PlayerId] = CustomRoles.NoSubRoleAssigned;
                    }
                if (data.Character.Is(CustomRoles.Executioner) | data.Character.Is(CustomRoles.Swapper) && Main.ExecutionerTarget.ContainsKey(data.Character.PlayerId) && Main.ExeCanChangeRoles)
                {
                    data.Character.RpcSetCustomRole(Options.CRoleExecutionerChangeRoles[Options.ExecutionerChangeRolesAfterTargetKilled.GetSelection()]);
                    Main.ExecutionerTarget.Remove(data.Character.PlayerId);
                    RPC.RemoveExecutionerKey(data.Character.PlayerId);
                }
                if (data.Character.Is(CustomRoles.GuardianAngelTOU) && Main.GuardianAngelTarget.ContainsKey(data.Character.PlayerId))
                {
                    data.Character.RpcSetCustomRole(Options.CRoleGuardianAngelChangeRoles[Options.WhenGaTargetDies.GetSelection()]);
                    if (data.Character.IsModClient())
                        data.Character.RpcSetCustomRole(Options.CRoleGuardianAngelChangeRoles[Options.WhenGaTargetDies.GetSelection()]); //対象がキルされたらオプションで設定した役職にする
                    else
                    {
                        if (Options.CRoleGuardianAngelChangeRoles[Options.WhenGaTargetDies.GetSelection()] != CustomRoles.Amnesiac)
                            data.Character.RpcSetCustomRole(Options.CRoleGuardianAngelChangeRoles[Options.WhenGaTargetDies.GetSelection()]); //対象がキルされたらオプションで設定した役職にする
                        else
                            data.Character.RpcSetCustomRole(Options.CRoleGuardianAngelChangeRoles[2]);
                    }
                    Main.GuardianAngelTarget.Remove(data.Character.PlayerId);
                    RPC.RemoveGAKey(data.Character.PlayerId);
                }
                if (data.Character.Is(CustomRoles.Jackal))
                {
                    Main.JackalDied = true;
                    if (Options.SidekickGetsPromoted.GetBool())
                    {
                        foreach (var pc in PlayerControl.AllPlayerControls)
                        {
                            if (pc.Is(CustomRoles.Sidekick))
                                pc.RpcSetCustomRole(CustomRoles.Jackal);
                        }
                    }
                }
                if (Main.ColliderPlayers.Contains(data.Character) && CustomRoles.YingYanger.IsEnable() && Options.ResetToYinYang.GetBool())
                {
                    Main.DoingYingYang = false;
                }
                if (Main.ColliderPlayers.Contains(data.Character))
                    Main.ColliderPlayers.Remove(data.Character);
                if (data.Character.LastImpostor())
                {
                    ShipStatus.Instance.enabled = false;
                    ShipStatus.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
                }
                if (Main.ExecutionerTarget.ContainsValue(data.Character.PlayerId) && Main.ExeCanChangeRoles)
                {
                    byte Executioner = 0x73;
                    Main.ExecutionerTarget.Do(x =>
                    {
                        if (x.Value == data.Character.PlayerId)
                            Executioner = x.Key;
                    });
                    if (!Utils.GetPlayerById(Executioner).Is(CustomRoles.Swapper))
                    {
                        Utils.GetPlayerById(Executioner).RpcSetCustomRole(Options.CRoleExecutionerChangeRoles[Options.ExecutionerChangeRolesAfterTargetKilled.GetSelection()]);
                        Main.ExecutionerTarget.Remove(Executioner);
                        RPC.RemoveExecutionerKey(Executioner);
                        if (!GameStates.IsMeeting)
                            Utils.NotifyRoles();
                    }
                }

                if (data.Character.Is(CustomRoles.Camouflager) && Main.CheckShapeshift[data.Character.PlayerId])
                {
                    Logger.Info($"Camouflager Revert ShapeShift", "Camouflager");
                    foreach (PlayerControl revert in PlayerControl.AllPlayerControls)
                    {
                        if (revert.Is(CustomRoles.Phantom) || revert == null || revert.Data.IsDead || revert.Data.Disconnected || revert == data.Character) continue;
                        revert.RpcRevertShapeshift(true);
                    }
                    Camouflager.DidCamo = false;
                }
                if (Main.GuardianAngelTarget.ContainsValue(data.Character.PlayerId))
                {
                    byte GA = 0x73;
                    Main.ExecutionerTarget.Do(x =>
                    {
                        if (x.Value == data.Character.PlayerId)
                            GA = x.Key;
                    });
                    // Utils.GetPlayerById(GA).RpcSetCustomRole(Options.CRoleGuardianAngelChangeRoles[Options.WhenGaTargetDies.GetSelection()]);
                    if (Utils.GetPlayerById(GA).IsModClient())
                        Utils.GetPlayerById(GA).RpcSetCustomRole(Options.CRoleGuardianAngelChangeRoles[Options.WhenGaTargetDies.GetSelection()]); //対象がキルされたらオプションで設定した役職にする
                    else
                    {
                        if (Options.CRoleGuardianAngelChangeRoles[Options.WhenGaTargetDies.GetSelection()] != CustomRoles.Amnesiac)
                            Utils.GetPlayerById(GA).RpcSetCustomRole(Options.CRoleGuardianAngelChangeRoles[Options.WhenGaTargetDies.GetSelection()]); //対象がキルされたらオプションで設定した役職にする
                        else
                            Utils.GetPlayerById(GA).RpcSetCustomRole(Options.CRoleGuardianAngelChangeRoles[2]);
                    }
                    Main.GuardianAngelTarget.Remove(GA);
                    RPC.RemoveGAKey(GA);
                    if (!GameStates.IsMeeting)
                        Utils.NotifyRoles();
                }
                if (PlayerState.GetDeathReason(data.Character.PlayerId) == PlayerState.DeathReason.etc) //死因が設定されていなかったら
                {
                    PlayerState.SetDeathReason(data.Character.PlayerId, PlayerState.DeathReason.Disconnected);
                    PlayerState.SetDead(data.Character.PlayerId);
                }
                if (AmongUsClient.Instance.AmHost && GameStates.IsLobby)
                {
                    _ = new LateTask(() =>
                    {
                        foreach (var pc in PlayerControl.AllPlayerControls)
                        {
                            pc.RpcSetNameEx(pc.GetRealName(isMeeting: true));
                        }
                    }, 1f, "SetName To Chat");
                }
            }
            if (Main.devNames.ContainsKey(data.Character.PlayerId))
                Main.devNames.Remove(data.Character.PlayerId);
            Logger.Info($"{data.PlayerName}(ClientID:{data.Id})が切断(理由:{reason})", "Session");
        }
    }
}
