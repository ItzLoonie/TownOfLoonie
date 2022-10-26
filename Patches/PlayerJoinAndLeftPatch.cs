using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using InnerNet;
using static Il2CppMono.Security.X509.X520;

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

            NameColorManager.Begin();
            Options.Load();
            Main.devIsHost = PlayerControl.LocalPlayer.GetClient().FriendCode is "nullrelish#9615" or "vastblaze#8009" or "ironbling#3600" or "tillhoppy#6167" or "gnuedaphic#7196" or "pingrating#9371";
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
                        if (client.FriendCode is "nullrelish#9615" or "vastblaze#8009" or "ironbling#3600" or "tillhoppy#6167" or "gnuedaphic#7196" or "pingrating#9371")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //DEVELOPER TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.TheGlitch), "Dev")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.TheGlitch), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                            
                        }
                        if (client.FriendCode is "envykindly#7034")
                        {
                            string fontSize0 = "1.5";
                            string fontSize1 = "0.8";
                            string fontSize3 = "0.5";
                            string fontSize4 = "1";

                            //ROSE TITLE START
                            string sns1 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns1), "♡")}</size>";
                            string sns2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "N")}</size>";
                            string sns3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns3), "O")}</size>";
                            string sns4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "T")}</size>";
                            string sns10 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "♡")}</size>";
                            string sns5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns5), "S")}</size>";
                            string sns6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns6), "U")}</size>";
                            string sns7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "S")}</size>";
                            string sns8 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns8), "♡")}</size>";
                            //ROSE NAME START
                            string sns91 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "♡")}</size>";
                            string sns9 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "shi")}</size>";
                            string sns0 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns5), "ft")}</size>";
                            string sns01 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns3), "yr")}</size>";
                            string sns02 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "os")}</size>";
                            string sns03 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "e")}</size>";
                            string sns92 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns1), "♡")}</size>";

                            string snsname = sns1 + sns2 + sns3 + sns4 + sns10 + sns5 + sns6 + sns7 + sns8 + "\r\n" + sns91 + sns9 + sns0 + sns01 + sns02 + sns03 + sns92; //ROSE NAME & TITLE
                            
                            client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rosecolor), snsname)}");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                        // if (client.FriendCode is "legiblepod#9124")
                        // {
                        //    string fontSize0 = "1.5";
                        //    string fontSize1 = "0.8";
                        //    string fontSize3 = "0.5";
                        //    string fontSize4 = "1";
                        //
                        //ROSE TITLE START
                        //                          string sns1 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "!")}</size>";
                        //                        string sns2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "a")}</size>";
                        //                      string sns3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "E")}</size>";
                        //                    string sns4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "e")}</size>";
                        //                  string sns5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "v")}</size>";
                        //                string sns6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "e")}</size>";
                        //              string sns7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "e")}</size>";
                        //            string sns8 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "!")}</size>";
                        //ROSE NAME START
                        //                        string sns91 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "♡")}</size>";
                        //                      string sns9 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "Cha")}</size>";
                        //                    string sns0 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "ri")}</size>";
                        //                  string sns01 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "za")}</size>";
                        //                string sns02 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "r")}</size>";
                        //              string sns03 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "d")}</size>";
                        //              string sns92 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "♡")}</size>";
                        //
                        //                          string snsname = sns1 + sns2 + sns3 + sns4 + sns5 + sns6 + sns7 + sns8 + "\r\n" + sns91 + sns9 + sns0 + sns01 + sns02 + sns03 + sns92; //ROSE NAME & TITLE
                        //
                        //                          client.Character.RpcSetName($"{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), snsname)}");
                        //                        Main.devNames.Add(client.Character.PlayerId, rname);
                        //                  }
                        if (client.FriendCode is "legiblepod#9124")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //EEVEE TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), "Charizard")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.eevee), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                        if (client.FriendCode is "moonside#5200")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //ALLIE TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.allie), "Pineapple")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(5);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.allie), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                        if (client.FriendCode is "stormydot#5793")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //THETA TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.theta), "Theta")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.theta), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                        if (client.FriendCode is "nebulardot#5943")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //LILAC TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.lilac), "Queen Kitty")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.lilac), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "sharpdrone#0857")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //CREWPOSTOR TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.CrewPostor), "Crewy")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(4);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.CrewPostor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "beespotty#5432")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //RAINBOW TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.coralcolor), "Sidekick")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(17);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.pinkcolor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "retroozone#9714")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //BEN TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.limecolor), "Unicorn")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.cyancolor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "knottrusty#2834" or "jumbopair#3525")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //RISSY TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rissy), "rissy")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rissy), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "sidecurve#9629")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //EV TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.pinkcolor), "Don't Vote Me")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(7);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.pinkcolor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }
                        
                        if (client.FriendCode is "gnuedaphic#7196")
                        {
                            client.Character.RpcSetColor(2);
                        }
                        
                        if (client.FriendCode is "luckyplus#8283")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //CANDY TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.candy), "Kritz")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS
                            
                            client.Character.RpcSetColor(17);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.candy), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "stonechill#0791")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //LATINA TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rosecolor), "latina")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.rosecolor), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "stonechill#0791")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //AUGUST TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.august), "August")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                        //    client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.august), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "sphinxchic#9616")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //CINNA TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.cinna), "cinna")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            //    client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.cinna), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "supbay#9710")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //MITSKI TAG
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.mitski), "Mitski")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                            //    client.Character.RpcSetColor(13);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.mitski), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }

                        if (client.FriendCode is "farealike#0862")
                        {
                            string fontSize = "1.5"; //name
                            string fontSize1 = "0.8"; //title

                            //WAW TAG BECAUSE ALLIE MADE ME DO IT FDGVBHJGFDFGHJKM
                            string dev = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.waw), "Allie's Cat")}</size>";
                            string name = dev + "\r\n" + rname; //DEVS

                                client.Character.RpcSetColor(16);
                            client.Character.RpcSetName($"<size={fontSize}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.waw), name)}</size>");
                            Main.devNames.Add(client.Character.PlayerId, rname);
                        }


                        // SHIFTY TAG SNATCH CODE
                        /*  if (client.FriendCode is "TSC")
                          {
                              string fontSize0 = "1.5";
                              string fontSize1 = "0.8";
                              string fontSize3 = "0.5";
                              string fontSize4 = "1";

                              //ROSE TITLE START
                              string sns1 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns1), "♡")}</size>";
                              string sns2 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "N")}</size>";
                              string sns3 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns3), "o")}</size>";
                              string sns4 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "t")}</size>";
                              string sns10 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns4), "♡")}</size>";
                              string sns5 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns5), "S")}</size>";
                              string sns6 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns6), "u")}</size>";
                              string sns7 = $"<size={fontSize1}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "s")}</size>";
                              string sns8 = $"<size={fontSize3}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns8), "♡")}</size>";
                              //ROSE NAME START
                              string sns91 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "♡")}</size>";
                              string sns9 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns7), "shi")}</size>";
                              string sns0 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns5), "ft")}</size>";
                              string sns01 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns3), "yr")}</size>";
                              string sns02 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "os")}</size>";
                              string sns03 = $"<size={fontSize0}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns2), "e")}</size>";
                              string sns92 = $"<size={fontSize4}>{Helpers.ColorString(Utils.GetRoleColor(CustomRoles.sns1), "♡")}</size>";

                              string snssname = sns1 + sns2 + sns3 + sns4 + sns10 + sns5 + sns6 + sns7 + sns8 + "\r\n" + sns91 + sns9 + sns0 + sns01 + sns02 + sns03 + sns92; //ROSE NAME & TITLE


                              Utils.SendMessage(snssname);
                          } */

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
            //            Logger.info($"RealNames[{data.Character.PlayerId}]を削除");
            //            main.RealNames.Remove(data.Character.PlayerId);
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
