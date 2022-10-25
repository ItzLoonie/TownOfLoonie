using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.CoreScripts;
using HarmonyLib;
using Hazel;
using static TownOfHost.Translator;

namespace TownOfHost
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    class ChatCommands
    {
        public static List<string> ChatHistory = new();
        public static Dictionary<byte, ChatController> allControllers = new();
        public static bool Prefix(ChatController __instance)
        {
            if (__instance.TextArea.text == "") return false;
            __instance.TimeSinceLastMessage = 3f;
            var text = __instance.TextArea.text;
            if (ChatHistory.Count == 0 || ChatHistory[^1] != text) ChatHistory.Add(text);
            ChatControllerUpdatePatch.CurrentHistorySelection = ChatHistory.Count;
            string[] args = text.Split(' ');
            string subArgs = "";
            var canceled = false;
            var cancelVal = "";
            var player = PlayerControl.LocalPlayer;
            Main.isChatCommand = true;
            Logger.Info(text, "SendChat");
            switch (args[0])
            {
                case "/dump":
                    if (!GameStates.IsLobby) break;
                    canceled = true;
                    Utils.DumpLog();
                    break;
                case "/v":
                case "/version":
                    canceled = true;
                    string version_text = "";
                    foreach (var kvp in Main.playerVersion.OrderBy(pair => pair.Key))
                    {
                        version_text += $"{kvp.Key}:{Utils.GetPlayerById(kvp.Key)?.Data?.PlayerName}:{kvp.Value.version}({kvp.Value.tag})\n";
                    }
                    if (version_text != "") HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, version_text);
                    break;
                default:
                    Main.isChatCommand = false;
                    break;

             /*   case "/hat":
                    var betterArgs = String.Compare("-", "_", true);
                    PlayerControl.LocalPlayer.RpcSetHat(betterArgs);
                    break;
                case "/pet":
                    var betterArgsd = String.Compare("-", "_", true);
                    PlayerControl.LocalPlayer.RpcSetPet(betterArgsd);
                    break;
                case "/visor":
                    var betterArgss = String.Compare("-", "_", true);
                    PlayerControl.LocalPlayer.RpcSetVisor(betterArgss);
                    break;
                case "/skin":
                    var betterArgzs = String.Compare("-", "_", true);
                    PlayerControl.LocalPlayer.RpcSetSkin(betterArgzs);
                    break;
                    */
            }
            if (AmongUsClient.Instance.AmHost)
            {
                Main.isChatCommand = true;
                switch (args[0])
                {
                    case "/win":
                    case "/winner":
                        canceled = true;
                        Utils.SendMessage("Winner: " + string.Join(",", Main.winnerList.Select(b => Main.AllPlayerNames[b])));
                        break;

                    case "/l":
                    case "/lastresult":
                        canceled = true;
                        Utils.ShowLastResult();
                        break;

                    case "/setplayers":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        Utils.SendMessage("Max Players set to " + subArgs);
                        var numbereer = System.Convert.ToByte(subArgs);
                        Main.RealOptionsData.MaxPlayers = numbereer;
                        break;
                    case "/guess":
                    case "/shoot":
                        subArgs = args.Length < 2 ? "" : args[1];
                        string subArgs1 = args.Length < 3 ? "" : args[2];
                        Guesser.GuesserShootByID(PlayerControl.LocalPlayer, subArgs, subArgs1);
                        break;

                    case "/r":
                    case "/rename":
                        canceled = true;
                        Main.nickName = args.Length > 1 ? Main.nickName = args[1] : "";
                        break;
                    case "/allids":
                        canceled = true;
                        string senttext = "";
                        List<PlayerControl> AllPlayers = new();
                        foreach (var pc in PlayerControl.AllPlayerControls)
                        {
                            AllPlayers.Add(pc);
                        }
                        senttext += "All Players and their IDs:";
                        foreach (var pc in AllPlayers)
                        {
                            string name = Main.devNames.ContainsKey(pc.PlayerId) ? Main.devNames[pc.PlayerId] : pc.GetRealName(true);
                            senttext += $"\n{name} : {pc.PlayerId}";
                        }
                        if (senttext != "") HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, senttext);
                        break;
                    case "/setimp":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        Utils.SendMessage("Impostors set to " + subArgs);
                        var numberee = System.Convert.ToByte(subArgs);
                        Main.RealOptionsData.numImpostors = numberee;
                        break;
                    case "/m":
                    case "/myrole":
                        canceled = true;
                        var role = PlayerControl.LocalPlayer.GetCustomRole();
                        var subrole = PlayerControl.LocalPlayer.GetCustomSubRole();
                        if (GameStates.IsInGame)
                        {
                            if (role.IsVanilla()) HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Vanilla roles currently have no description.");
                            else HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, GetString(role.ToString()) + GetString($"{role}InfoLong"));
                            if (subrole != CustomRoles.NoSubRoleAssigned)
                                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, GetString(subrole.ToString()) + GetString($"{subrole}InfoLong"));
                        }
                        else { HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Sorry, you can only use this command inside the game."); }
                        break;
                    case "/meeting":
                        canceled = true;
                        PlayerControl.LocalPlayer.CmdReportDeadBody(null);
                        break;
                    case "/colour":
                    case "/color":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        Utils.SendMessage("Color ID set to " + subArgs);
                        var numbere = System.Convert.ToByte(subArgs);
                        PlayerControl.LocalPlayer.RpcSetColor(numbere);
                        break;
                    case "/mimic":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        switch (subArgs)
                        {
                            case "shiftyrose":
                                PlayerControl.LocalPlayer.RpcSetHat("hat_fairywings");
                                PlayerControl.LocalPlayer.RpcSetVisor("visor_BubbleBumVisor");
                                PlayerControl.LocalPlayer.RpcSetSkin("skin_fairy");
                                PlayerControl.LocalPlayer.RpcSetPet("pet_ChewiePet");
                                PlayerControl.LocalPlayer.RpcSetColor(13);
                                PlayerControl.LocalPlayer.RpcSetName("shiftyrose");
                                Utils.SendMessage("You are now mimicking shiftyrose!");
                                break;

                            case "tsc":
                                PlayerControl.LocalPlayer.RpcSetHat("hat_pk05_davehat");
                                PlayerControl.LocalPlayer.RpcSetVisor("visor_pk01_Security1Visor");
                                PlayerControl.LocalPlayer.RpcSetSkin("skin_Bling");
                                PlayerControl.LocalPlayer.RpcSetPet("pet_ChewiePet");
                                PlayerControl.LocalPlayer.RpcSetColor(2);
                                PlayerControl.LocalPlayer.RpcSetName("TSC");
                                Utils.SendMessage("You are now mimicking TSC!");
                                break;

                            case "theta":
                                PlayerControl.LocalPlayer.RpcSetHat("hat_pk02_HaloHat");
                                PlayerControl.LocalPlayer.RpcSetVisor("visor_Blush");
                                PlayerControl.LocalPlayer.RpcSetSkin("");
                                PlayerControl.LocalPlayer.RpcSetPet("pet_Crewmate");
                                PlayerControl.LocalPlayer.RpcSetColor(13);
                                PlayerControl.LocalPlayer.RpcSetName("Thetaa");
                                Utils.SendMessage("You are now mimicking Theta!");
                                break;

                            case "ev":
                                PlayerControl.LocalPlayer.RpcSetHat("hat_w21_mittens");
                                PlayerControl.LocalPlayer.RpcSetVisor("visor_Carrot");
                                PlayerControl.LocalPlayer.RpcSetSkin("skin_w21_msclaus");
                                PlayerControl.LocalPlayer.RpcSetPet("");
                                PlayerControl.LocalPlayer.RpcSetColor(7);
                                PlayerControl.LocalPlayer.RpcSetName("Ev");
                                Utils.SendMessage("You are now mimicking Ev!");
                                break;

                            case "allie":
                                PlayerControl.LocalPlayer.RpcSetHat("hat_Pineapple");
                                PlayerControl.LocalPlayer.RpcSetVisor("visor_Carrot");
                                PlayerControl.LocalPlayer.RpcSetSkin("");
                                PlayerControl.LocalPlayer.RpcSetPet("pet_test");
                                PlayerControl.LocalPlayer.RpcSetColor(5);
                                PlayerControl.LocalPlayer.RpcSetName("Allie");
                                Utils.SendMessage("You are now mimicking Allie!");
                                break;

                            case "candy":
                                PlayerControl.LocalPlayer.RpcSetHat("hat_IceCreamNeo");
                                PlayerControl.LocalPlayer.RpcSetVisor("visor_Blush");
                                PlayerControl.LocalPlayer.RpcSetSkin("skin_mummy");
                                PlayerControl.LocalPlayer.RpcSetPet("");
                                PlayerControl.LocalPlayer.RpcSetColor(17);
                                PlayerControl.LocalPlayer.RpcSetName("candy");
                                Utils.SendMessage("You are now mimicking candy!");
                                break;

                            case "eevee":
                                PlayerControl.LocalPlayer.RpcSetHat("hat_pkHW01_Wolf");
                                PlayerControl.LocalPlayer.RpcSetVisor("");
                                PlayerControl.LocalPlayer.RpcSetSkin("skin_JacketYellowskin");
                                PlayerControl.LocalPlayer.RpcSetPet("");
                                PlayerControl.LocalPlayer.RpcSetColor(9);
                                PlayerControl.LocalPlayer.RpcSetName("Eevee");
                                Utils.SendMessage("You are now mimicking Eevee!");
                                break;

                            case "rissy":
                                PlayerControl.LocalPlayer.RpcSetHat("hat_hl_gura");
                                PlayerControl.LocalPlayer.RpcSetVisor("visor_hl_nothoughts");
                                PlayerControl.LocalPlayer.RpcSetSkin("skin_hl_gura");
                                PlayerControl.LocalPlayer.RpcSetPet("");
                                PlayerControl.LocalPlayer.RpcSetColor(10);
                                PlayerControl.LocalPlayer.RpcSetName("rizzle");
                                Utils.SendMessage("You are now mimicking rissy!");
                                break;

                            default:
                                __instance.AddChat(PlayerControl.LocalPlayer, "Please enter a valid user to mimic.");
                                cancelVal = "/mimic";
                                break;
                        }

                        break;
                    // shiftyrose color command
                    case "/shifty":
                        canceled = true;
                     //   subArgs = args.Length < 2 ? "" : args[1];
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

                        string snssname = sns1 + sns2 + sns3 + sns4 + sns10 + sns5 + sns6 + sns7 + sns8 + "\r\n" + sns91 + sns9 + sns0 + sns01 + sns02 + sns03 + sns92;
                        Utils.SendMessage("♡ You're special! ♡\n\n♡ Your tag has been set and your color was set to Rose! ♡");
                      //  var numbers = System.Convert.ToByte(subArgs);
                        PlayerControl.LocalPlayer.RpcSetColor(13);
                        PlayerControl.LocalPlayer.RpcSetName(snssname);
                        break;
                    // dev color command
                    case "/changecolor":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        Utils.SendMessage("Color ID set to " + subArgs);
                        var numberd = System.Convert.ToByte(subArgs);
                        PlayerControl.LocalPlayer.RpcSetColor(numberd);
                        break;
                    case "/kick":
                        subArgs = args.Length < 2 ? "show" : args[1];
                        canceled = true;
                        if (subArgs == "show")
                        {
                            string sentttext = "";
                            List<PlayerControl> AlllPlayers = new();
                            sentttext += "All Players and their IDs (pick one):";
                            foreach (var pc in PlayerControl.AllPlayerControls)
                            {
                                if (pc == null || pc.Data.Disconnected) continue;
                                string name = Main.devNames.ContainsKey(pc.PlayerId) ? Main.devNames[pc.PlayerId] : pc.GetRealName(true);
                                sentttext += $"\n{name} : {pc.PlayerId}";
                            }
                            if (sentttext != "") HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, sentttext);
                        }
                        else
                        {
                            var kickplayerid = Convert.ToByte(subArgs);
                            AmongUsClient.Instance.KickPlayer(Utils.GetPlayerById(kickplayerid).GetClientId(), false);
                            string name = Main.devNames.ContainsKey(kickplayerid) ? Main.devNames[kickplayerid] : Utils.GetPlayerById(kickplayerid).GetRealName(true);
                            string texttosend = $"{name} was kicked.";
                            if (GameStates.IsInGame)
                            {
                                texttosend += $" Their role was {GetString(Utils.GetPlayerById(kickplayerid).GetCustomRole().ToString())}";
                            }
                            Utils.SendMessage(texttosend);
                        }
                        break;

                    case "/ban":
                        subArgs = args.Length < 2 ? "show" : args[1];
                        canceled = true;
                        if (subArgs == "show")
                        {
                            string sentttext = "";
                            List<PlayerControl> AlllPlayers = new();
                            sentttext += "All Players and their IDs (pick one):";
                            foreach (var pc in PlayerControl.AllPlayerControls)
                            {
                                if (pc == null || pc.Data.Disconnected) continue;
                                string name = Main.devNames.ContainsKey(pc.PlayerId) ? Main.devNames[pc.PlayerId] : pc.GetRealName(true);
                                sentttext += $"\n{name} : {pc.PlayerId}";
                            }
                            if (sentttext != "") HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, sentttext);
                        }
                        else
                        {
                            var banplayerid = Convert.ToByte(subArgs);
                            AmongUsClient.Instance.KickPlayer(Utils.GetPlayerById(banplayerid).GetClientId(), true);
                            string name = Main.devNames.ContainsKey(banplayerid) ? Main.devNames[banplayerid] : Utils.GetPlayerById(banplayerid).GetRealName(true);
                            string texttosend = $"{name} was kicked.";
                            if (GameStates.IsInGame)
                            {
                                texttosend += $" Their role was {GetString(Utils.GetPlayerById(banplayerid).GetCustomRole().ToString())}";
                            }
                            Utils.SendMessage(texttosend);
                        }
                        break;
                    case "/level":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        Utils.SendMessage("Current AU Level Set to " + subArgs, PlayerControl.LocalPlayer.PlayerId);
                        //nt32.Parse("-105");
                        var number = System.Convert.ToUInt32(subArgs);
                        PlayerControl.LocalPlayer.RpcSetLevel(number - 1);
                        break;
                    case "/disc":
                    case "/discord":
                        canceled = true;
                        Utils.SendMessage("Join the official TOH TOR Discord server at:\ndiscord.gg/c4DcpFeQcq");
                        break;
                    case "/n":
                    case "/now":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        switch (subArgs)
                        {
                            case "r":
                            case "roles":
                                Utils.ShowActiveRoles();
                                break;
                            default:
                                Utils.ShowActiveSettings();
                                break;
                        }
                        break;
                    case "/perc":
                    case "/percentages":
                        canceled = true;
                        Utils.ShowPercentages();
                        break;
                    case "/dis":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        switch (subArgs)
                        {
                            case "crewmate":
                                ShipStatus.Instance.enabled = false;
                                ShipStatus.RpcEndGame(GameOverReason.HumansDisconnect, false);
                                break;

                            case "impostor":
                                ShipStatus.Instance.enabled = false;
                                ShipStatus.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
                                break;

                            default:
                                __instance.AddChat(PlayerControl.LocalPlayer, "crewmate | impostor");
                                cancelVal = "/dis";
                                break;
                        }
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Admin, 0);
                        break;

                    case "/h":
                    case "/help":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        switch (subArgs)
                        {
                            case "r":
                            case "roles":
                                subArgs = args.Length < 3 ? "" : args[2];
                                GetRolesInfo(subArgs);
                                break;

                            case "att":
                            case "attributes":
                                subArgs = args.Length < 3 ? "" : args[2];
                                switch (subArgs)
                                {
                                    case "lastimpostor":
                                    case "limp":
                                        Utils.SendMessage(Utils.GetRoleName(CustomRoles.LastImpostor) + GetString("LastImpostorInfoLong"));
                                        break;

                                    default:
                                        Utils.SendMessage($"{GetString("Command.h_args")}:\n lastimpostor(limp)");
                                        break;
                                }
                                break;

                            case "m":
                            case "modes":
                                subArgs = args.Length < 3 ? "" : args[2];
                                switch (subArgs)
                                {
                                    case "hideandseek":
                                    case "has":
                                        Utils.SendMessage(GetString("HideAndSeekInfo"));
                                        break;

                                    case "nogameend":
                                    case "nge":
                                        Utils.SendMessage(GetString("NoGameEndInfo"));
                                        break;

                                    case "syncbuttonmode":
                                    case "sbm":
                                        Utils.SendMessage(GetString("SyncButtonModeInfo"));
                                        break;

                                    case "randommapsmode":
                                    case "rmm":
                                        Utils.SendMessage(GetString("RandomMapsModeInfo"));
                                        break;
                                    case "cc":
                                    case "camocomms":
                                        Utils.SendMessage(GetString("CamoCommsInfo"));
                                        break;

                                    default:
                                        Utils.SendMessage($"{GetString("Command.h_args")}:\n hideandseek(has), nogameend(nge), syncbuttonmode(sbm), randommapsmode(rmm)");
                                        break;
                                }
                                break;


                            case "n":
                            case "now":
                                Utils.ShowActiveSettingsHelp();
                                break;

                            default:
                                Utils.ShowHelp();
                                break;
                        }
                        break;

                    case "/t":
                    case "/template":
                        canceled = true;
                        if (args.Length > 1) SendTemplate(args[1]);
                        else HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{GetString("ForExample")}:\n{args[0]} test");
                        break;

                    case "/mw":
                    case "/messagewait":
                        canceled = true;
                        if (args.Length > 1 && int.TryParse(args[1], out int sec))
                        {
                            Main.MessageWait.Value = sec;
                            Utils.SendMessage(string.Format(GetString("Message.SetToSeconds"), sec), 0);
                        }
                        else Utils.SendMessage($"{GetString("Message.MessageWaitHelp")}\n{GetString("ForExample")}:\n{args[0]} 3", 0);
                        break;

                    case "/exile":
                        canceled = true;
                        if (args.Length < 2 || !int.TryParse(args[1], out int id)) break;
                        Utils.GetPlayerById(id)?.RpcExileV2();
                        break;

                    case "/kill":
                        canceled = true;
                        if (args.Length < 2 || !int.TryParse(args[1], out int id2)) break;
                        Utils.GetPlayerById(id2)?.RpcMurderPlayer(Utils.GetPlayerById(id2));
                        break;

                    case "/changerole":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        switch (subArgs)
                        {
                            case "crewmate":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Crewmate);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;

                            case "impostor":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Impostor);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Impostor);
                                break;

                            case "engineer":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Engineer);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Engineer);
                                break;
                            case "shapeshifter":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Shapeshifter);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Shapeshifter);
                                break;
                            case "opportunist":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Opportunist);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "serialkiller":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Jackal);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "juggernaut":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Juggernaut);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "altruist":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Alturist);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "sheriff":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Sheriff);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "amnesiac":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Amnesiac);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "jester":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Jester);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Engineer);
                                break;
                            case "ga":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.GuardianAngel);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.GuardianAngel);
                                break;
                            case "werewolf":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Werewolf);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "glitch":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.TheGlitch);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Shapeshifter);
                                break;
                            case "bloodknight":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.BloodKnight);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "marksman":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Marksman);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "egoist":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Egoist);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Shapeshifter);
                                break;
                            case "doctor":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Doctor);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Scientist);
                                break;
                            case "scientist":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Scientist);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Scientist);
                                break;
                            case "mayor":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Mayor);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "bodyguard":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Bodyguard);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "medic":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Medic);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "oracle":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Oracle);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "snitch":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Snitch);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "traitor":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.CorruptedSheriff);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            case "parasite":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Parasite);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Shapeshifter);
                                break;
                            case "hitman":
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Hitman);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                            

                            default:
                                PlayerControl.LocalPlayer.RpcSetCustomRole(CustomRoles.Crewmate);
                                PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                                break;
                        }
                        break;
                    default:
                        Main.isChatCommand = false;
                        break;
                }
            }
            if (canceled)
            {
                Logger.Info("Command Canceled", "ChatCommand");
                __instance.TextArea.Clear();
                __instance.TextArea.SetText(cancelVal);
                __instance.quickChatMenu.ResetGlyphs();
            }
            return !canceled;
        }

        public static void GetRolesInfo(string role)
        {
            var roleList = new Dictionary<CustomRoles, string>
            {
                //Impostor役職
                { (CustomRoles)(-1), $"== {GetString("Impostor")} ==" }, //区切り用
                { CustomRoles.BountyHunter, "bo" },
                { CustomRoles.FireWorks, "fw" },
                { CustomRoles.Mare, "ma" },
                { CustomRoles.Mafia, "mf" },
                { CustomRoles.SerialKiller, "merc" },
                //{ CustomRoles.ShapeMaster, "sha" },
                { CustomRoles.TimeThief, "tt"},
                { CustomRoles.VoteStealer, "vs"},
                { CustomRoles.Sniper, "snp" },
                { CustomRoles.Puppeteer, "pup" },
                { CustomRoles.Disperser, "dis" },
                { CustomRoles.Vampire, "va" },
                { CustomRoles.Warlock, "wa" },
                { CustomRoles.Witch, "wi" },
                { CustomRoles.Freezer, "fre" },
                { CustomRoles.Cleaner, "jani" },
                { CustomRoles.Silencer, "si" },
                { CustomRoles.Ninja,"ni"},
                { CustomRoles.Miner,"mi"},
                { CustomRoles.YingYanger,"yy"},
                { CustomRoles.Camouflager,"cf"},
                { CustomRoles.Grenadier,"gr"},
                { CustomRoles.CorruptedSheriff, "trai" },
                {CustomRoles.EvilGuesser, "ass"},
                { CustomRoles.ShapeshifterRemake, "ss" },
                //Madmate役職
                { (CustomRoles)(-2), $"== {GetString("Madmate")} ==" }, //区切り用
                { CustomRoles.MadGuardian, "mg" },
                { CustomRoles.Madmate, "mm" },
                { CustomRoles.MadSnitch, "msn" },
                { CustomRoles.SKMadmate, "skmm" },
                { CustomRoles.Parasite, "para" },
                //両陣営役職
                { (CustomRoles)(-3), $"== {GetString("Impostor")} or {GetString("Crewmate")} ==" }, //区切り用
                { CustomRoles.CrewPostor, "cp" },
                //Crewmate役職
                { (CustomRoles)(-4), $"== {GetString("Crewmate")} ==" }, //区切り用
                { CustomRoles.Dictator, "dic" },
                { CustomRoles.Child, "cd" },
                { CustomRoles.Medium, "med" },
                { CustomRoles.Psychic, "psy" },
                { CustomRoles.Doctor, "doc" },
                { CustomRoles.Lighter, "li" },
                { CustomRoles.Mayor, "my" },
                { CustomRoles.Bodyguard, "bd" },
                { CustomRoles.Oracle, "or" },
                { CustomRoles.Medic, "me" },
                { CustomRoles.Crusader, "crus" },
                { CustomRoles.Escort, "esc" },
                { CustomRoles.Veteran, "vet" },
                { CustomRoles.Transporter, "tr" },
                { CustomRoles.SabotageMaster, "mech" },
                {CustomRoles.NiceGuesser, "vigi"},
                { CustomRoles.Sheriff, "sh" },
                { CustomRoles.Investigator, "inve" },
                { CustomRoles.Mystic,"ms"},
                { CustomRoles.Snitch, "sn" },
                { CustomRoles.SpeedBooster, "sb" },
                { CustomRoles.Trapper, "tra" },
                { CustomRoles.Bastion, "bas"},
                { CustomRoles.Demolitionist, "demo"},
                { CustomRoles.EngineerRemake, "engi" },
              //  { CustomRoles.ScientistRemake, "sci" },
                //Neutral役職
                { (CustomRoles)(-5), $"== {GetString("Neutral")} ==" }, //区切り用
                { CustomRoles.Arsonist, "asor" },
                { CustomRoles.BloodKnight,"bk"},
                { CustomRoles.Egoist, "ego" },
                { CustomRoles.Executioner, "exe" },
                { CustomRoles.Swapper, "sw" },
                { CustomRoles.Jester, "jest" },
                { CustomRoles.Phantom, "ph" },
                { CustomRoles.Opportunist, "oppo" },

                { CustomRoles.Hitman, "hn" },
                { CustomRoles.Survivor, "sur" },
                { CustomRoles.SchrodingerCat, "sc" },
                {CustomRoles.Pirate, "pi"},
                { CustomRoles.Marksman, "mar" },
                { CustomRoles.Terrorist, "te" },
                { CustomRoles.Jackal, "sk" },
                { CustomRoles.Sidekick, "hunt" },
                //{ CustomRoles.Juggernaut, "jn"},
                { CustomRoles.PlagueBearer, "pb" },
                { CustomRoles.Pestilence, "pest" },
                { CustomRoles.Juggernaut, "jugg"},
                { CustomRoles.Vulture, "vu"},
                { CustomRoles.Coven, "cov" },
                { CustomRoles.CovenWitch, "cl" },
                { CustomRoles.Poisoner, "poison" },
                { CustomRoles.HexMaster, "hm" },
                { CustomRoles.Medusa, "medu" },
                { CustomRoles.TheGlitch, "gl" },
                { CustomRoles.Werewolf, "ww" },
                { CustomRoles.Amnesiac, "amne" },
                { CustomRoles.GuardianAngelTOU, "ga" },
                { CustomRoles.Hacker, "hac" },
                //Sub役職
                { (CustomRoles)(-6), $"== {GetString("SubRole")} ==" }, //区切り用
                {CustomRoles.Lovers, "lo" },
                { CustomRoles.Sleuth, "sl" },
                { CustomRoles.Bait, "ba" },
                { CustomRoles.Oblivious, "obl" },
                { CustomRoles.Torch, "to" },
                { CustomRoles.Flash, "fl" },
                { CustomRoles.Bewilder, "be" },
                { CustomRoles.TieBreaker, "tb" },
                { CustomRoles.Watcher, "wat" },
                { CustomRoles.Diseased, "di" },
                //HAS
                { (CustomRoles)(-7), $"== {GetString("HideAndSeek")} ==" }, //区切り用
                { CustomRoles.HASFox, "hfo" },
                { CustomRoles.HASTroll, "htr" },
                { CustomRoles.Supporter, "wor" },
                { CustomRoles.Janitor, "cle" },
                { CustomRoles.Painter, "pan" },

            };
            var msg = "";
            var rolemsg = $"{GetString("Command.h_args")}";
            foreach (var r in roleList)
            {
                var roleName = r.Key.ToString();
                var roleShort = r.Value;

                if (String.Compare(role, roleName, true) == 0 || String.Compare(role, roleShort, true) == 0 || role == "vampress")
                {
                    if (role == "vampress")
                        roleName = CustomRoles.Vampress.ToString();
                    Utils.SendMessage(GetString(roleName) + GetString($"{roleName}InfoLong"));
                    return;
                }

                var roleText = $"{roleName.ToLower()}({roleShort.ToLower()}), ";
                if ((int)r.Key < 0)
                {
                    msg += rolemsg + "\n" + roleShort + "\n";
                    rolemsg = "";
                }
                else if ((rolemsg.Length + roleText.Length) > 40)
                {
                    msg += rolemsg + "\n";
                    rolemsg = roleText;
                }
                else
                {
                    rolemsg += roleText;
                }
            }
            msg += rolemsg;
            Utils.SendMessage(msg);
        }
        public static void PublicGetRolesInfo(string role, byte playerId = 0xff)
        {
            var roleList = new Dictionary<CustomRoles, string>
            {
                //Impostor役職
                { (CustomRoles)(-1), $"== {GetString("Impostor")} ==" }, //区切り用
                { CustomRoles.BountyHunter, "bo" },
                { CustomRoles.FireWorks, "fw" },
                { CustomRoles.Mare, "ma" },
                { CustomRoles.Mafia, "mf" },
                { CustomRoles.SerialKiller, "merc" },
                //{ CustomRoles.ShapeMaster, "sha" },
                { CustomRoles.TimeThief, "tt"},
                { CustomRoles.VoteStealer, "pick"},
                { CustomRoles.Sniper, "snp" },
                { CustomRoles.Puppeteer, "pup" },
                { CustomRoles.Disperser, "dis" },
                { CustomRoles.Vampire, "va" },
                { CustomRoles.Warlock, "wa" },
                { CustomRoles.Witch, "wi" },
                { CustomRoles.Freezer, "fre" },
                { CustomRoles.Cleaner, "jani" },
                { CustomRoles.Silencer, "si" },
                { CustomRoles.Camouflager,"cf"},
                { CustomRoles.Ninja,"ni"},
                { CustomRoles.Grenadier,"gr"},
                { CustomRoles.Miner,"mi"},
                { CustomRoles.YingYanger,"yy"},
                { CustomRoles.CorruptedSheriff, "trai" },
                {CustomRoles.EvilGuesser, "ass"},
                { CustomRoles.ShapeshifterRemake, "ss" },
                //Madmate役職
                { (CustomRoles)(-2), $"== {GetString("Madmate")} ==" }, //区切り用
                { CustomRoles.MadGuardian, "mg" },
                { CustomRoles.Madmate, "mm" },
                { CustomRoles.MadSnitch, "msn" },
                { CustomRoles.SKMadmate, "skmm" },
                { CustomRoles.Parasite, "para" },
                //両陣営役職
                { (CustomRoles)(-3), $"== {GetString("Impostor")} or {GetString("Crewmate")} ==" }, //区切り用
                { CustomRoles.CrewPostor, "cp" },
                //Crewmate役職
                { (CustomRoles)(-4), $"== {GetString("Crewmate")} ==" }, //区切り用
                { CustomRoles.Dictator, "dic" },
                { CustomRoles.Child, "cd" },
                { CustomRoles.Medium, "med" },
                { CustomRoles.Psychic, "psy" },
                { CustomRoles.Doctor, "doc" },
                { CustomRoles.Lighter, "li" },
                { CustomRoles.Mayor, "my" },
                { CustomRoles.Veteran, "vet" },
                { CustomRoles.Bodyguard, "bg" },
                { CustomRoles.Oracle, "or" },
                { CustomRoles.Medic, "me" },
                { CustomRoles.Crusader, "crus" },
                { CustomRoles.Escort, "esc" },
                { CustomRoles.Veteran, "vet" },
                { CustomRoles.Transporter, "tr" },
                { CustomRoles.SabotageMaster, "mech" },
                { CustomRoles.Sheriff, "sh" },
                {CustomRoles.NiceGuesser, "vigi"},
                { CustomRoles.Investigator, "invest" },
                { CustomRoles.Mystic,"ms"},
               // { CustomRoles.CorruptedSheriff, "csh" },
                { CustomRoles.Snitch, "sn" },
                { CustomRoles.SpeedBooster, "sb" },
                { CustomRoles.Trapper, "tra" },
                { CustomRoles.Bastion, "bas"},
                { CustomRoles.Demolitionist, "demo"},
                { CustomRoles.EngineerRemake, "engi" },
           //     { CustomRoles.ScientistRemake, "sci" },
                //Neutral役職
                { (CustomRoles)(-5), $"== {GetString("Neutral")} ==" }, //区切り用
                { CustomRoles.Arsonist, "arso" },
                { CustomRoles.BloodKnight,"bk"},
                { CustomRoles.Egoist, "ego" },
                { CustomRoles.Executioner, "exe" },
                { CustomRoles.Swapper, "sw" },
                { CustomRoles.Jester, "jest" },
                { CustomRoles.Phantom, "ph" },
                { CustomRoles.Hitman, "hn" },
                { CustomRoles.Opportunist, "oppo" },
                { CustomRoles.Survivor, "surv" },
                { CustomRoles.SchrodingerCat, "sc" },
                { CustomRoles.Terrorist, "terror" },
                { CustomRoles.Marksman, "mar" },
                { CustomRoles.Jackal, "sk" },
                { CustomRoles.Sidekick, "hunt" },
                //{ CustomRoles.Juggernaut, "jn"},
                { CustomRoles.PlagueBearer, "pb" },
                { CustomRoles.Pestilence, "pest" },
                { CustomRoles.Juggernaut, "jugg"},
                { CustomRoles.Vulture, "vu"},
                { CustomRoles.Coven, "cov" },
                { CustomRoles.CovenWitch, "cl" },
                { CustomRoles.Poisoner, "poison" },
                { CustomRoles.HexMaster, "hm" },
                { CustomRoles.Medusa, "medu" },
                { CustomRoles.TheGlitch, "gl" },
                { CustomRoles.Werewolf, "ww" },
                {CustomRoles.Pirate, "pi"},
                { CustomRoles.Amnesiac, "amne" },
                { CustomRoles.GuardianAngelTOU, "ga" },
                { CustomRoles.Hacker, "hac" },
                //Sub役職
                { (CustomRoles)(-6), $"== {GetString("SubRole")} ==" }, //区切り用
                {CustomRoles.Lovers, "lo" },
                { CustomRoles.Sleuth, "sl" },
                { CustomRoles.Bait, "ba" },
                { CustomRoles.Oblivious, "obl" },
                { CustomRoles.Torch, "to" },
                { CustomRoles.Flash, "fl" },
                { CustomRoles.Bewilder, "be" },
                { CustomRoles.TieBreaker, "tb" },
                { CustomRoles.Watcher, "wat" },
                { CustomRoles.Diseased, "di" },
                //HAS
                { (CustomRoles)(-7), $"== {GetString("HideAndSeek")} ==" }, //区切り用
                { CustomRoles.HASFox, "hfo" },
                { CustomRoles.HASTroll, "htr" },
                { CustomRoles.Supporter, "wor" },
                { CustomRoles.Janitor, "cle" },
                { CustomRoles.Painter, "pan" },

            };
            var msg = "";
            var rolemsg = $"{GetString("Command.h_args")}";
            foreach (var r in roleList)
            {
                var roleName = r.Key.ToString();
                var roleShort = r.Value;

                if (String.Compare(role, roleName, true) == 0 || String.Compare(role, roleShort, true) == 0)
                {
                    Utils.SendMessage(GetString(roleName) + GetString($"{roleName}InfoLong"), playerId);
                    return;
                }

                //Utils.SendMessage("Sorry, the current role you tried to search up was not inside our databse. Either you misspelled it, or its not there.", playerId);
            }
            //msg += rolemsg;
            //Utils.SendMessage(msg);
        }
        public static void SendTemplate(string str = "", byte playerId = 0xff, bool noErr = false)
        {
            if (!File.Exists("template.txt"))
            {
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Among Us.exeと同じフォルダにtemplate.txtが見つかりませんでした。\n新規作成します。");
                File.WriteAllText(@"template.txt", "test:This is template text.\\nLine breaks are also possible.\ntest:これは定型文です。\\n改行も可能です。");
                return;
            }
            using StreamReader sr = new(@"template.txt", Encoding.GetEncoding("UTF-8"));
            string text;
            string[] tmp = { };
            List<string> sendList = new();
            HashSet<string> tags = new();
            while ((text = sr.ReadLine()) != null)
            {
                tmp = text.Split(":");
                if (tmp.Length > 1 && tmp[1] != "")
                {
                    tags.Add(tmp[0]);
                    if (tmp[0] == str) sendList.Add(tmp.Skip(1).Join(delimiter: "").Replace("\\n", "\n"));
                }
            }
            if (sendList.Count == 0 && !noErr)
            {
                if (playerId == 0xff)
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, string.Format(GetString("Message.TemplateNotFoundHost"), str, tags.Join(delimiter: ", ")));
                else Utils.SendMessage(string.Format(GetString("Message.TemplateNotFoundClient"), str), playerId);
            }
            else for (int i = 0; i < sendList.Count; i++) Utils.SendMessage(sendList[i], playerId);
        }
        public static void OnReceiveChat(PlayerControl player, string text)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (Main.SilencedPlayer.Count != 0)
            {
                //someone is silenced
                foreach (var p in Main.SilencedPlayer)
                {
                    if (player.PlayerId != p.PlayerId) continue;
                    if (!player.Data.IsDead)
                    {
                        text = "Silenced.";
                        Logger.Info($"{p.GetNameWithRole()}:{text}", "Tried To Send Chat But Silenced");
                        Utils.SendMessage("You are currently Silenced. Try talking again when you aren't silenced.", player.PlayerId);
                    }
                }
            }
            string[] args = text.Split(' ');
            string subArgs = "";
            switch (args[0])
            {
                case "/l":
                case "/lastresult":
                    Utils.ShowLastResult(player.PlayerId);
                    break;

                case "/name":
                    if (Options.Customise.GetBool())
                    {
                        var canRename = true;
                        foreach (var pc in PlayerControl.AllPlayerControls)
                        {
                            if (pc.Data.PlayerName == args[1])
                                canRename = false;
                        }
                        if (canRename)
                            player.Data.PlayerName = args.Length > 1 ? player.Data.PlayerName = args[1] : "";
                    }
                    else { Utils.SendMessage("The host has currently disabled access to this command.\nTry again when this command is enabled.", player.PlayerId); }
                    break;
                case "/n":
                case "/now":
                    var name = args.Length > 20 ? "Test" : subArgs;
                    subArgs = args.Length < 2 ? "Test" : name;
                    switch (subArgs)
                    {
                        case "r":
                        case "roles":
                            Utils.ShowActiveRoles(player.PlayerId);
                            break;

                        default:
                            Utils.ShowActiveSettings(player.PlayerId);
                            break;
                    }
                    break;
                case "/guess":
                case "/shoot":
                    Utils.RemoveChat(player.PlayerId);
                    subArgs = args.Length < 2 ? "" : args[1];
                    string subArgs1 = args.Length < 3 ? "" : args[2];
                    Guesser.GuesserShootByID(player, subArgs, subArgs1);
                    break;
                case "/perc":
                case "/percentages":
                    Utils.ShowPercentages(player.PlayerId);
                    break;
                case "/m":
                case "/myrole":
                    var role = player.GetCustomRole();
                    var subrole = player.GetCustomSubRole();
                    if (GameStates.IsInGame)
                    {
                        if (role.IsVanilla()) Utils.SendMessage("Vanilla roles currently have no description.", player.PlayerId);
                        else Utils.SendMessage(GetString(role.ToString()) + GetString($"{role}InfoLong"), player.PlayerId);
                        if (subrole != CustomRoles.NoSubRoleAssigned)
                            Utils.SendMessage(GetString(subrole.ToString()) + GetString($"{subrole}InfoLong"), player.PlayerId);
                    }
                    else { Utils.SendMessage("Sorry, you can only use this command inside the game.", player.PlayerId); }
                    break;
                case "/level":
                    if (Options.Customise.GetBool())
                    {
                        /*subArgs = args.Length < 2 ? "" : args[1];
                        Utils.SendMessage("Current AU Level Set to " + subArgs + ". AU auto adds 1 to your current level. Starting players are at level 0, so AU adds 1 to make you level 1. So no one is level 100, we are all just at level 99.", player.PlayerId);
                        //nt32.Parse("-105");
                        var number = System.Convert.ToUInt32(subArgs);
                        player.RpcSetLevel(number);*/
                        Utils.SendMessage("This command currently does not work as intended for non-host players.\nIn order to prevent kicks, we have disabled this command.", player.PlayerId);
                    }
                    else { Utils.SendMessage("The host has currently disabled access to this command.\nTry again when this command is enabled.", player.PlayerId); }
                    break;
                case "/colour":
                case "/color":
                    if (Options.Customise.GetBool())
                    {
                        subArgs = args.Length < 2 ? "" : args[1];
                        Utils.SendMessage("Color ID set to " + subArgs, player.PlayerId);
                        var numbere = System.Convert.ToByte(subArgs);
                        player.RpcSetColor(numbere);
                    }
                    else { Utils.SendMessage("The host has currently disabled access to this command.\nTry again when this command is enabled.", player.PlayerId); }
                    break;

                // DISABLED AS IT BANS THE HOST WHEN USED
                /*      case "/mimic":
                          if (player.FriendCode is "envykindly#7034" or "nullrelish#9615" or "nebulardot#5943" or "ironbling#3600" or "tillhoppy#6167" or "gnuedaphic#7196" or "pingrating#9371" or "luckyplus#8283" or "sidecurve#9629" or "knottrusty#2834" or "jumbopair#3525" or "retroozone#9714" or "beespotty#5432" or "stormydot#5793" or "moonside#5200" or "legiblepod#9124")
                              subArgs = args.Length < 2 ? "" : args[1];
                          switch (subArgs)
                          {
                              case "shiftyrose":
                                  player.RpcSetHat("hat_fairywings");
                                  player.RpcSetVisor("visor_BubbleBumVisor");
                                  player.RpcSetSkin("skin_fairy");
                                  player.RpcSetPet("pet_Crewmate");
                                  player.RpcSetColor(13);
                                  player.RpcSetName("shiftyrose");
                                  Utils.SendMessage("You are now mimicking shiftyrose!", player.PlayerId);
                                  break;

                              case "tsc":
                                  player.RpcSetHat("hat_pk05_davehat");
                                  player.RpcSetVisor("visor_pk01_Security1Visor");
                                  player.RpcSetSkin("skin_Bling");
                                  player.RpcSetPet("pet_Crewmate");
                                  player.RpcSetColor(2);
                                  player.RpcSetName("TSC");
                                  Utils.SendMessage("You are now mimicking TSC!", player.PlayerId);
                                  break;

                              case "theta":
                                  player.RpcSetHat("hat_pk02_HaloHat");
                                  player.RpcSetVisor("visor_Blush");
                                  player.RpcSetSkin("");
                                  player.RpcSetPet("pet_Crewmate");
                                  player.RpcSetColor(13);
                                  player.RpcSetName("Thetaa");
                                  Utils.SendMessage("You are now mimicking Theta!", player.PlayerId);
                                  break;

                              case "ev":
                                  player.RpcSetHat("hat_w21_mittens");
                                  player.RpcSetVisor("visor_Carrot");
                                  player.RpcSetSkin("skin_w21_msclaus");
                                  player.RpcSetPet("");
                                  player.RpcSetColor(7);
                                  player.RpcSetName("Ev");
                                  Utils.SendMessage("You are now mimicking Ev!", player.PlayerId);
                                  break;

                              case "allie":
                                  player.RpcSetHat("hat_Pineapple");
                                  player.RpcSetVisor("visor_Carrot");
                                  player.RpcSetSkin("");
                                  player.RpcSetPet("pet_test");
                                  player.RpcSetColor(5);
                                  player.RpcSetName("Allie");
                                  Utils.SendMessage("You are now mimicking Allie!", player.PlayerId);
                                  break;
                          }

                                  break; */




                case "/changecolor":

                    if (player.FriendCode is "envykindly#7034" or "nullrelish#9615" or "nebulardot#5943" or "ironbling#3600" or "tillhoppy#6167" or "gnuedaphic#7196" or "pingrating#9371" or "luckyplus#8283" or "sidecurve#9629" or "knottrusty#2834" or "jumbopair#3525" or "retroozone#9714" or "beespotty#5432" or "stormydot#5793" or "moonside#5200" or "legiblepod#9124")
                    {
                        subArgs = args.Length < 2 ? "" : args[1];
                        Utils.SendMessage("Color ID set to " + subArgs, player.PlayerId);
                        var numbere = System.Convert.ToByte(subArgs);
                        player.RpcSetColor(numbere);
                    }
                    else { Utils.SendMessage("You do not have access to this command.\nOnly mod devs and select people may use this command.", player.PlayerId); }
                    break;

                case "/shifty":

                    if (player.FriendCode is "envykindly#7034")
                    {
                        //  subArgs = args.Length < 2 ? "" : args[1];
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

                        string snssname = sns1 + sns2 + sns3 + sns4 + sns10 + sns5 + sns6 + sns7 + sns8 + "\r\n" + sns91 + sns9 + sns0 + sns01 + sns02 + sns03 + sns92;
                        Utils.SendMessage("♡ You're special! ♡\n\n♡ Your tag has been set and your color was set to Rose! ♡", player.PlayerId);
                        //   var numbere = System.Convert.ToByte(subArgs);
                        player.RpcSetColor(13);
                        player.RpcSetName(snssname);
                    }
                    else { Utils.SendMessage("You are not shiftyrose.\nThis command is exclusive to shiftyrose.", player.PlayerId); }
                    break;

                case "/roleinfo":
                    subArgs = args.Length < 3 ? "" : args[1];
                    PublicGetRolesInfo(subArgs, player.PlayerId);
                    //PublicGetRolesInfo();
                    break;
                case "/t":
                case "/template":
                    if (args.Length > 1) SendTemplate(args[1], player.PlayerId);
                    else Utils.SendMessage($"{GetString("ForExample")}:\n{args[0]} test", player.PlayerId);
                    break;

            //    default:
               //     break;

                case "commence":
                case "Commence":
                case "COMMENCE":
                case "S word":
                case "stArt":
                case "Can you start":
                case "Can we start":
                case "can you start":
                case "can we start":
                case "Can u start":
                case "can u start":
                case "staart":
                case "Strt":
                case "starrt":
                case "staarrt":
                case "staarrtt":
                case "plz start":
                case "plz start!":
                case "plz start!!":
                case "plz start!!!":
                case "plz start!!!!":
                case "plz start!!!!!":
                case "plz start!!!!!!":
                case "please start":
                case "please start!":
                case "please start!!":
                case "please start!!!":
                case "please start!!!!":
                case "please start!!!!!":
                case "please start!!!!!!":
                case "pls start":
                case "pls start!":
                case "pls start!!":
                case "pls start!!!":
                case "pls start!!!!":
                case "pls start!!!!!":
                case "pls start!!!!!!":
                case "start!":
                case "start!!":
                case "start!!!":
                case "start!!!!":
                case "start!!!!!":
                case "start!!!!!!":
                case "START!":
                case "START!!":
                case "START!!!":
                case "START!!!!":
                case "START!!!!!":
                case "START!!!!!!":
                case "Start!":
                case "Start!!":
                case "Start!!!":
                case "Start!!!!":
                case "Start!!!!!":
                case "Start!!!!!!":
                case "Pls Start":
                case "Pls Start!":
                case "Pls Start!!":
                case "Pls Start!!!":
                case "Pls Start!!!!":
                case "Pls Start!!!!!":
                case "Pls Start!!!!!!":
                case "Plz Start":
                case "Plz Start!":
                case "Plz Start!!":
                case "Plz Start!!!":
                case "Plz Start!!!!":
                case "Plz Start!!!!!":
                case "Plz Start!!!!!!":
                case "Please Start":
                case "Please Start!":
                case "Please Start!!":
                case "Please Start!!!":
                case "Please Start!!!!":
                case "Please Start!!!!!":
                case "Please Start!!!!!!":
                //aug
                case "Starttttt":
                case "Startttt":
                case "Starttt":
                case "Starrrttt":
                //rosie
                case "staaart":
                case "staaaart":
                case "staart!":
                case "staaart!":
                case "staaaart!":
                case "staart!!":
                case "staaart!!":
                case "staaaart!!":
                case "staart!!!":
                case "staaart!!!":
                case "staaaart!!!":
                case "will you start":
                case "fucking start":
                case "strt":
                case "When are we going to start":
                case "when are we going to start":
                case "are we starting":
                case string a when a.Contains("start"):

                    if (Options.CamoComms.GetBool())

                        AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;

                case string b when b.Contains("Start"):
                    AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;
                case string c when c.Contains("START"):
                    AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;
                case string d when d.Contains("Commence"):
                    AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;
                case string e when e.Contains("commence"):
                    AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;
                case string f when f.Contains("Begin"):
                    AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;
                case string g when g.Contains("begin"):
                    AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;
                case string h when h.Contains("BEGIN"):
                    AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;
                case string i when i.Contains("BEGIN"):
                    AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                    break;
                default:
                    break;
            
            }
        }
    }
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    class ChatUpdatePatch
    {
        public static void Postfix(ChatController __instance)
        {
            if (!AmongUsClient.Instance.AmHost || Main.MessagesToSend.Count < 1 || (Main.MessagesToSend[0].Item2 == byte.MaxValue && Main.MessageWait.Value > __instance.TimeSinceLastMessage)) return;
            var player = PlayerControl.AllPlayerControls.ToArray().OrderBy(x => x.PlayerId).Where(x => !x.Data.IsDead).FirstOrDefault();
            if (player == null) return;
            if (Main.SilencedPlayer.Contains(player)) return;
            (string msg, byte sendTo) = Main.MessagesToSend[0];
            Main.MessagesToSend.RemoveAt(0);
            int clientId = sendTo == byte.MaxValue ? -1 : Utils.GetPlayerById(sendTo).GetClientId();
            if (clientId == -1) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, msg);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)RpcCalls.SendChat, SendOption.None, clientId);
            writer.Write(msg);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            __instance.TimeSinceLastMessage = 0f;
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    class AddChatPatch
    {
        public static void Postfix(string chatText)
        {
            switch (chatText)
            {
                default:
                    break;
            }
            if (!AmongUsClient.Instance.AmHost) return;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
    class RpcSendChatPatch
    {
        public static bool Prefix(PlayerControl __instance, string chatText, ref bool __result)
        {
            if (string.IsNullOrWhiteSpace(chatText))
            {
                __result = false;
                return false;
            }
            if (AmongUsClient.Instance.AmClient && DestroyableSingleton<HudManager>.Instance)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance, chatText);
            if (chatText.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                DestroyableSingleton<Telemetry>.Instance.SendWho();
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte)RpcCalls.SendChat, SendOption.None);
            messageWriter.Write(chatText);
            messageWriter.EndMessage();
            __result = true;
            return false;
        }
    }
}
