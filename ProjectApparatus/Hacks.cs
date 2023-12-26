using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using Steamworks;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.UI;

using System.Windows.Forms;
using UnityEngine.Diagnostics;

namespace ProjectApparatus
{

    internal class Hacks : MonoBehaviour
    {
        private static GUIStyle Style = null;


        bool IsPlayerValid(PlayerControllerB plyer)
        {
            return (plyer != null &&
                    !plyer.disconnectedMidGame &&
                    !plyer.playerUsername.Contains("Player #"));
        }

        public void OnGUI()
        {
            Style = new GUIStyle(GUI.skin.label);
            Style.normal.textColor = Color.white;
            Style.fontStyle = FontStyle.Bold;

            this.menuButton.Enable();
            this.unloadMenu.Enable();
            if (SettingsData.b_EnableESP)
            {
                if (GameObjectManager.Instance != null && TimeOfDay.Instance != null)
                {
                    this.DisplayLoot();
                    this.DisplayPlayers();
                    this.DisplayDoors();
                    this.DisplayLandmines();
                    this.DisplayTurrets();
                    this.DisplaySteamHazard();
                    this.DisplayEnemyAI();
                    this.DisplayShip();
                    this.DisplayDeadPlayers();
                }
            }
            Vector2 centeredPos = new Vector2(UnityEngine.Screen.width / 2f, UnityEngine.Screen.height / 2f);
            GUI.color = SettingsData.c_Theme;
            if (SettingsData.b_CenteredIndicators)
            {
                if (TimeOfDay.Instance != null)
                {
                    float iY = Settings.TEXT_HEIGHT;
                    if (SettingsData.b_DisplayGroupCredits && GameObjectManager.Instance.shipTerminal != null) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Group Credits: " + GameObjectManager.Instance.shipTerminal.groupCredits, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                    if (SettingsData.b_DisplayQuota && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Profit Quota: " + TimeOfDay.Instance.quotaFulfilled + "/" + TimeOfDay.Instance.profitQuota, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                    if (SettingsData.b_DisplayDaysLeft && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Days Left: " + TimeOfDay.Instance.daysUntilDeadline, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                }
            }
            string Watermark = "Project Astolfo";
            if (!SettingsData.b_CenteredIndicators)
            {
                if (TimeOfDay.Instance != null)
                {
                    if (SettingsData.b_DisplayGroupCredits && GameObjectManager.Instance.shipTerminal != null)
                        Watermark += " | Group Credits: " + GameObjectManager.Instance.shipTerminal.groupCredits;
                    if (SettingsData.b_DisplayQuota && TimeOfDay.Instance)
                        Watermark += " | Profit Quota: " + TimeOfDay.Instance.quotaFulfilled + "/" + TimeOfDay.Instance.profitQuota;
                    if (SettingsData.b_DisplayDaysLeft && TimeOfDay.Instance)
                        Watermark += " | Days Left: " + TimeOfDay.Instance.daysUntilDeadline;
                }
            }
            Render.String(Style, 10f, 5f, 150f, Settings.TEXT_HEIGHT, Watermark, GUI.color);
            if (Settings.b_isMenuOpen)
            {
                Settings.windowRect = GUILayout.Window(0, Settings.windowRect, new GUI.WindowFunction(this.MenuContent), "Project Astolfo", Array.Empty<GUILayoutOption>());
            }

            if (SettingsData.b_Crosshair)
            {
                Render.FilledCircle(centeredPos, 5, Color.black);
                Render.FilledCircle(centeredPos, 3, SettingsData.c_Theme);
            }
        }

        private PlayerControllerB selectedPlayer = null;

        private void MenuContent(int windowID)
        {
            GUILayout.BeginHorizontal();
            UI.Tab("Self", ref UI.nTab, UI.Tabs.Self);
            UI.Tab("Misc", ref UI.nTab, UI.Tabs.Misc);
            UI.Tab("ESP", ref UI.nTab, UI.Tabs.ESP);
            UI.Tab("Players", ref UI.nTab, UI.Tabs.Players);
            UI.Tab("Graphics", ref UI.nTab, UI.Tabs.Graphics);
            UI.Tab("Cheat", ref UI.nTab, UI.Tabs.Cheat);
            GUILayout.EndHorizontal();


            UI.TabContents("Self", UI.Tabs.Self, () =>
            {
                SettingsData.b_GodMode = GUILayout.Toggle(SettingsData.b_GodMode, "God Mode", Array.Empty<GUILayoutOption>());
                SettingsData.b_InfiniteStam = GUILayout.Toggle(SettingsData.b_InfiniteStam, "Infinite Stamina", Array.Empty<GUILayoutOption>());
                SettingsData.b_InfiniteCharge = GUILayout.Toggle(SettingsData.b_InfiniteCharge, "Infinite Charge", Array.Empty<GUILayoutOption>());
                SettingsData.b_InfiniteZapGun = GUILayout.Toggle(SettingsData.b_InfiniteZapGun, "Infinite Zap Gun", Array.Empty<GUILayoutOption>());
                SettingsData.b_InfiniteShotgunAmmo = GUILayout.Toggle(SettingsData.b_InfiniteShotgunAmmo, "Infinite Shotgun Ammo", Array.Empty<GUILayoutOption>());
                SettingsData.b_NightVision = GUILayout.Toggle(SettingsData.b_NightVision, "Night Vision", Array.Empty<GUILayoutOption>());
                SettingsData.b_InteractThroughWalls = GUILayout.Toggle(SettingsData.b_InteractThroughWalls, "Interact Through Walls", Array.Empty<GUILayoutOption>());
                SettingsData.b_UnlimitedGrabDistance = GUILayout.Toggle(SettingsData.b_UnlimitedGrabDistance, "No Grab Distance Limit", Array.Empty<GUILayoutOption>());
                SettingsData.b_OneHandAllObjects = GUILayout.Toggle(SettingsData.b_OneHandAllObjects, "One Hand All Objects", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisableFallDamage = GUILayout.Toggle(SettingsData.b_DisableFallDamage, "Disable Fall Damage", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisableInteractCooldowns = GUILayout.Toggle(SettingsData.b_DisableInteractCooldowns, "Disable Interact Cooldowns", Array.Empty<GUILayoutOption>());
                SettingsData.b_InstantInteractions = GUILayout.Toggle(SettingsData.b_InstantInteractions, "Instant Interact", Array.Empty<GUILayoutOption>());
                SettingsData.b_PlaceAnywhere = GUILayout.Toggle(SettingsData.b_PlaceAnywhere, "Place Anywhere", Array.Empty<GUILayoutOption>());
                SettingsData.b_TauntSlide = GUILayout.Toggle(SettingsData.b_TauntSlide, "Taunt Slide", Array.Empty<GUILayoutOption>());
                SettingsData.b_FastLadderClimbing = GUILayout.Toggle(SettingsData.b_FastLadderClimbing, "Fast Ladder Climbing", Array.Empty<GUILayoutOption>());
                SettingsData.b_HearEveryone = GUILayout.Toggle(SettingsData.b_HearEveryone, "Hear Everyone", Array.Empty<GUILayoutOption>());
                SettingsData.b_ChargeAnyItem = GUILayout.Toggle(SettingsData.b_ChargeAnyItem, "Charge Any Item", Array.Empty<GUILayoutOption>());
                SettingsData.b_WalkSpeed = GUILayout.Toggle(SettingsData.b_WalkSpeed, "Walk Speed (" + SettingsData.i_WalkSpeed + ")", Array.Empty<GUILayoutOption>());
                SettingsData.i_WalkSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(SettingsData.i_WalkSpeed, 1, 20));
                SettingsData.b_SprintSpeed = GUILayout.Toggle(SettingsData.b_SprintSpeed, "Sprint Speed (" + SettingsData.i_SprintSpeed + ")", Array.Empty<GUILayoutOption>());
                SettingsData.i_SprintSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(SettingsData.i_SprintSpeed, 1, 20));
                SettingsData.b_JumpHeight = GUILayout.Toggle(SettingsData.b_JumpHeight, "Jump Height (" + SettingsData.i_JumpHeight + ")", Array.Empty<GUILayoutOption>());
                SettingsData.i_JumpHeight = Mathf.RoundToInt(GUILayout.HorizontalSlider(SettingsData.i_JumpHeight, 1, 100));
                if (GUILayout.Button("Respawn"))
                    ReviveLocalPlayer();

                GUILayout.BeginHorizontal();
                SettingsData.b_Noclip = GUILayout.Toggle(SettingsData.b_Noclip, "Noclip (" + SettingsData.fl_NoclipSpeed + ")", Array.Empty<GUILayoutOption>());
                SettingsData.keyNoclip.Menu();
                GUILayout.EndHorizontal();
                SettingsData.fl_NoclipSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(SettingsData.fl_NoclipSpeed, 1, 100));
            });

            UI.TabContents("Misc", UI.Tabs.Misc, () =>
            {
                SettingsData.b_NoMoreCredits = GUILayout.Toggle(SettingsData.b_NoMoreCredits, "No More Credits", Array.Empty<GUILayoutOption>());
                SettingsData.b_SensitiveLandmines = GUILayout.Toggle(SettingsData.b_SensitiveLandmines, "Sensitive Landmines", Array.Empty<GUILayoutOption>());
                SettingsData.b_AllJetpacksExplode = GUILayout.Toggle(SettingsData.b_AllJetpacksExplode, "All Jetpacks Explode", Array.Empty<GUILayoutOption>());
                SettingsData.b_LightShow = GUILayout.Toggle(SettingsData.b_LightShow, "Light Show", Array.Empty<GUILayoutOption>());
                if (!SettingsData.b_NoMoreCredits)
                {
                    SettingsData.str_MoneyToGive = GUILayout.TextField(SettingsData.str_MoneyToGive, Array.Empty<GUILayoutOption>());
                    if (GUILayout.Button("Give Credits") && GameObjectManager.Instance.shipTerminal)
                        GameObjectManager.Instance.shipTerminal.groupCredits += int.Parse(SettingsData.str_MoneyToGive);

                    GUILayout.BeginHorizontal();
                    SettingsData.str_QuotaFulfilled = GUILayout.TextField(SettingsData.str_QuotaFulfilled, GUILayout.Width(42));
                    GUILayout.Label("/", GUILayout.Width(4));
                    SettingsData.str_Quota = GUILayout.TextField(SettingsData.str_Quota, GUILayout.Width(42));
                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Set Quota") && TimeOfDay.Instance)
                    {
                        TimeOfDay.Instance.profitQuota = int.Parse(SettingsData.str_Quota);
                        TimeOfDay.Instance.quotaFulfilled = int.Parse(SettingsData.str_QuotaFulfilled);
                        TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                    }
                }

                if (GUILayout.Button("Start Server")) StartOfRound.Instance.StartGameServerRpc();
                if (GUILayout.Button("Stop Server")) StartOfRound.Instance.EndGameServerRpc(0);
                if (GUILayout.Button("Unlock All Doors"))
                    foreach (DoorLock obj in GameObjectManager.Instance.door_locks)
                        if (obj != null)
                            obj.UnlockDoorServerRpc();
                if (GUILayout.Button("Explode All Mines"))
                    foreach (Landmine obj in GameObjectManager.Instance.landmines)
                        if (obj != null)
                            obj.ExplodeMineServerRpc();
                if (GUILayout.Button("Kill All Enemies"))
                    foreach (EnemyAI obj in GameObjectManager.Instance.enemies)
                        if (obj != null)
                            obj.KillEnemyServerRpc(false);
                if (GUILayout.Button("Delete All Enemies"))
                    foreach (EnemyAI obj in GameObjectManager.Instance.enemies)
                        if (obj != null)
                            obj.KillEnemyServerRpc(true);
                if (GUILayout.Button("Attack Players at Deposit Desk"))
                {
                    if (GameObjectManager.Instance.itemsDesk)
                        GameObjectManager.Instance.itemsDesk.AttackPlayersServerRpc();
                }
            });


            UI.TabContents("ESP", UI.Tabs.ESP, () => {
                SettingsData.b_EnableESP = GUILayout.Toggle(SettingsData.b_EnableESP, "Enabled", Array.Empty<GUILayoutOption>());
                SettingsData.b_ItemESP = GUILayout.Toggle(SettingsData.b_ItemESP, "Items", Array.Empty<GUILayoutOption>());
                SettingsData.b_EnemyESP = GUILayout.Toggle(SettingsData.b_EnemyESP, "Enemies", Array.Empty<GUILayoutOption>());
                SettingsData.b_PlayerESP = GUILayout.Toggle(SettingsData.b_PlayerESP, "Players", Array.Empty<GUILayoutOption>());
                SettingsData.b_ShipESP = GUILayout.Toggle(SettingsData.b_ShipESP, "Ships", Array.Empty<GUILayoutOption>());
                SettingsData.b_DoorESP = GUILayout.Toggle(SettingsData.b_DoorESP, "Doors", Array.Empty<GUILayoutOption>());
                SettingsData.b_SteamHazard = GUILayout.Toggle(SettingsData.b_SteamHazard, "Steam Hazards", Array.Empty<GUILayoutOption>());
                SettingsData.b_LandmineESP = GUILayout.Toggle(SettingsData.b_LandmineESP, "Landmines", Array.Empty<GUILayoutOption>());
                SettingsData.b_TurretESP = GUILayout.Toggle(SettingsData.b_TurretESP, "Turrets", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisplayHP = GUILayout.Toggle(SettingsData.b_DisplayHP, "Show Health", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisplayWorth = GUILayout.Toggle(SettingsData.b_DisplayWorth, "Show Value", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisplayDistance = GUILayout.Toggle(SettingsData.b_DisplayDistance, "Show Distance", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisplaySpeaking = GUILayout.Toggle(SettingsData.b_DisplaySpeaking, "Show Is Speaking", Array.Empty<GUILayoutOption>());

                SettingsData.b_ItemDistanceLimit = GUILayout.Toggle(SettingsData.b_ItemDistanceLimit, "Item Distance Limit (" + Mathf.RoundToInt(SettingsData.fl_ItemDistanceLimit) + ")", Array.Empty<GUILayoutOption>());
                SettingsData.fl_ItemDistanceLimit = GUILayout.HorizontalSlider(SettingsData.fl_ItemDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                SettingsData.b_EnemyDistanceLimit = GUILayout.Toggle(SettingsData.b_EnemyDistanceLimit, "Enemy Distance Limit (" + Mathf.RoundToInt(SettingsData.fl_EnemyDistanceLimit) + ")", Array.Empty<GUILayoutOption>());
                SettingsData.fl_EnemyDistanceLimit = GUILayout.HorizontalSlider(SettingsData.fl_EnemyDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                SettingsData.b_MineDistanceLimit = GUILayout.Toggle(SettingsData.b_MineDistanceLimit, "Landmine Distance Limit (" + Mathf.RoundToInt(SettingsData.fl_MineDistanceLimit) + ")", Array.Empty<GUILayoutOption>());
                SettingsData.fl_MineDistanceLimit = GUILayout.HorizontalSlider(SettingsData.fl_MineDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                SettingsData.b_TurretDistanceLimit = GUILayout.Toggle(SettingsData.b_TurretDistanceLimit, "Turret Distance Limit (" + Mathf.RoundToInt(SettingsData.fl_TurretDistanceLimit) + ")", Array.Empty<GUILayoutOption>());
                SettingsData.fl_TurretDistanceLimit = GUILayout.HorizontalSlider(SettingsData.fl_TurretDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());
            });

            UI.TabContents(null, UI.Tabs.Players, () =>
            {
                GUILayout.BeginHorizontal();
                foreach (PlayerControllerB player in GameObjectManager.Instance.players)
                {
                    if (!IsPlayerValid(player)) continue;
                    UI.Tab(PAUtils.TruncateString(player.playerUsername, 12), ref selectedPlayer, player, true);
                }
                GUILayout.EndHorizontal();

                if (!IsPlayerValid(selectedPlayer))
                    selectedPlayer = null;

                if (selectedPlayer)
                {
                    UI.Header("Selected Player: " + selectedPlayer.playerUsername);
                    Settings.InitializeDictionaries(selectedPlayer);

                    Settings.b_DemiGod[selectedPlayer] = GUILayout.Toggle(Settings.b_DemiGod[selectedPlayer], "Demigod", Array.Empty<GUILayoutOption>());

                    if (GUILayout.Button("Kill"))
                        selectedPlayer.DamagePlayerFromOtherClientServerRpc(1000, new Vector3(900, 900, 900), 0);
                    if (GUILayout.Button("Teleport To"))
                        GameObjectManager.Instance.localPlayer.TeleportPlayer(selectedPlayer.playerGlobalHead.position);
                    Settings.str_DamageToGive = GUILayout.TextField(Settings.str_DamageToGive, Array.Empty<GUILayoutOption>());
                    if (GUILayout.Button("Damage"))
                        selectedPlayer.DamagePlayerFromOtherClientServerRpc(int.Parse(Settings.str_DamageToGive), new Vector3(900, 900, 900), 0);
                    Settings.str_HealthToHeal = GUILayout.TextField(Settings.str_HealthToHeal, Array.Empty<GUILayoutOption>());
                    if (GUILayout.Button("Heal"))
                        selectedPlayer.DamagePlayerFromOtherClientServerRpc(-int.Parse(Settings.str_HealthToHeal), new Vector3(900, 900, 900), 0);
                    if (GUILayout.Button("Steam Profile"))
                        SteamFriends.OpenUserOverlay(selectedPlayer.playerSteamId, "steamid");
                }
            });

            UI.TabContents("Graphics", UI.Tabs.Graphics, () =>
            {
                SettingsData.b_DisableFog = GUILayout.Toggle(SettingsData.b_DisableFog, "Disable Fog", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisableDepthOfField = GUILayout.Toggle(SettingsData.b_DisableDepthOfField, "Disable Depth of Field", Array.Empty<GUILayoutOption>());
            });

            UI.TabContents("Cheat", UI.Tabs.Cheat, () =>
            {
                SettingsData.b_Crosshair = GUILayout.Toggle(SettingsData.b_Crosshair, "Crosshair", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisplayGroupCredits = GUILayout.Toggle(SettingsData.b_DisplayGroupCredits, "Display Group Credits", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisplayQuota = GUILayout.Toggle(SettingsData.b_DisplayQuota, "Display Quota", Array.Empty<GUILayoutOption>());
                SettingsData.b_DisplayDaysLeft = GUILayout.Toggle(SettingsData.b_DisplayDaysLeft, "Display Days Left", Array.Empty<GUILayoutOption>());
                SettingsData.b_CenteredIndicators = GUILayout.Toggle(SettingsData.b_CenteredIndicators, "Centered Indicators", Array.Empty<GUILayoutOption>());
                SettingsData.b_DeadPlayers = GUILayout.Toggle(SettingsData.b_DeadPlayers, "Dead Player List", Array.Empty<GUILayoutOption>());

                UI.Header("Colors");
                UI.ColorPicker("Theme", ref SettingsData.c_Theme);
                UI.ColorPicker("Valve", ref SettingsData.c_Valve);
                UI.ColorPicker("Enemy", ref SettingsData.c_Enemy);
                UI.ColorPicker("Turret", ref SettingsData.c_Turret);
                UI.ColorPicker("Landmine", ref SettingsData.c_Landmine);
                UI.ColorPicker("Player", ref SettingsData.c_Player);
                UI.ColorPicker("Door", ref SettingsData.c_Door);
                UI.ColorPicker("Loot", ref SettingsData.c_Loot);
                UI.ColorPicker("Small Loot", ref SettingsData.c_smallLoot);
                UI.ColorPicker("Medium Loot", ref SettingsData.c_medLoot);
                UI.ColorPicker("Big Loot", ref SettingsData.c_bigLoot);
            });

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }


        private void DisplayObjects<T>(IEnumerable<T> objects, bool shouldDisplay, Func<T, string> labelSelector, Func<T, Color> colorSelector) where T : Component
        {
            if (!shouldDisplay) return;

            foreach (T obj in objects)
            {
                if (obj != null && obj.gameObject.activeSelf)
                {
                    float distanceToPlayer = PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        obj.transform.position);
                    Vector3 pos;
                    if (PAUtils.WorldToScreen(GameObjectManager.Instance.localPlayer.gameplayCamera, obj.transform.position, out pos))
                    {
                        string ObjName = PAUtils.ConvertFirstLetterToUpperCase(labelSelector(obj));
                        if (SettingsData.b_DisplayDistance)
                            ObjName += " [" + distanceToPlayer.ToString().ToUpper() + "M]";
                        Render.String(Style, pos.x, pos.y, 150f, 50f, ObjName, colorSelector(obj));
                    }
                }
            }
        }

        public void DisplayDeadPlayers()
        {
            if (!SettingsData.b_DeadPlayers) return;

            float yOffset = 30f;

            foreach (PlayerControllerB playerControllerB in GameObjectManager.Instance.players)
            {
                if (playerControllerB != null && playerControllerB.isPlayerDead)
                {
                    string playerUsername = playerControllerB.playerUsername;

                    Render.String(Style, 10f, yOffset, 200f, Settings.TEXT_HEIGHT, playerUsername, GUI.color);
                    yOffset += (Settings.TEXT_HEIGHT - 10f);
                }
            }
        }

        private void DisplayShip()
        {
            DisplayObjects(
                new[] { GameObjectManager.Instance.shipDoor },
                SettingsData.b_ShipESP,
                _ => "Ship",
                _ => SettingsData.c_Door
            );
        }

        private void DisplayDoors()
        {
            DisplayObjects(
                GameObjectManager.Instance.entrance_doors,
                SettingsData.b_DoorESP,
                entranceTeleport => entranceTeleport.isEntranceToBuilding ? "Entrance" : "Exit",
                _ => SettingsData.c_Door
            );
        }

        private void DisplayLandmines()
        {
            DisplayObjects(
                GameObjectManager.Instance.landmines.Where(landmine => landmine != null && landmine.IsSpawned && !landmine.hasExploded &&
                    ((SettingsData.b_MineDistanceLimit &&
                    PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        landmine.transform.position) < SettingsData.fl_MineDistanceLimit) ||
                        !SettingsData.b_MineDistanceLimit)),
                SettingsData.b_LandmineESP,
                _ => "Landmine",
                _ => SettingsData.c_Landmine
            );
        }

        private void DisplayTurrets()
        {
            DisplayObjects(
                GameObjectManager.Instance.turrets.Where(turret => turret != null && turret.IsSpawned &&
                    ((SettingsData.b_TurretDistanceLimit &&
                    PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        turret.transform.position) < SettingsData.fl_TurretDistanceLimit) ||
                        !SettingsData.b_TurretDistanceLimit)),
                SettingsData.b_TurretESP,
                _ => "Turret",
                _ => SettingsData.c_Turret
            );
        }

        private void DisplaySteamHazard()
        {
            DisplayObjects(
                GameObjectManager.Instance.steamValves.Where(steamValveHazard => steamValveHazard != null && steamValveHazard.triggerScript.interactable),
                SettingsData.b_SteamHazard,
                _ => "Steam Valve",
                _ => SettingsData.c_Valve
            );
        }

        private void DisplayPlayers()
        {
            DisplayObjects(
                GameObjectManager.Instance.players.Where(playerControllerB =>
                    IsPlayerValid(playerControllerB) &&
                    !playerControllerB.IsLocalPlayer &&
                     playerControllerB.playerUsername != GameObjectManager.Instance.localPlayer.playerUsername &&
                    !playerControllerB.isPlayerDead
                ),
                SettingsData.b_PlayerESP,
                playerControllerB =>
                {
                    string str = playerControllerB.playerUsername;
                    if (SettingsData.b_DisplaySpeaking && playerControllerB.voicePlayerState.IsSpeaking)
                        str += " [VC]";
                    if (SettingsData.b_DisplayHP)
                        str += " [" + playerControllerB.health + "HP]";
                    return str;
                },
                _ => SettingsData.c_Player
            );
        }

        private void DisplayEnemyAI()
        {
            DisplayObjects(
                GameObjectManager.Instance.enemies.Where(enemyAI =>
                    enemyAI != null &&
                    enemyAI.eye != null &&
                    enemyAI.enemyType != null &&
                    !enemyAI.isEnemyDead &&
                    ((SettingsData.b_EnemyDistanceLimit &&
                    PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        enemyAI.transform.position) < SettingsData.fl_EnemyDistanceLimit) ||
                        !SettingsData.b_EnemyDistanceLimit)
                ),
                SettingsData.b_EnemyESP,
                enemyAI =>
                {
                    string name = enemyAI.enemyType.enemyName;
                    return string.IsNullOrWhiteSpace(name) ? "Enemy" : name;
                },
                _ => SettingsData.c_Enemy
            );
        }

        private Color GetLootColor(int value)
        {
            int[] thresholds = { 15, 35 };
            Color[] colors = { SettingsData.c_smallLoot, SettingsData.c_medLoot, SettingsData.c_bigLoot };

            for (int i = 0; i < thresholds.Length; i++)
                if (value <= thresholds[i])
                    return colors[i];

            return SettingsData.c_Loot;
        }


        private void DisplayLoot()
        {
            DisplayObjects(
                GameObjectManager.Instance.items.Where(grabbableObject =>
                    grabbableObject != null &&
                    !grabbableObject.isHeld &&
                    !grabbableObject.isPocketed &&
                    grabbableObject.itemProperties != null &&
                    ((SettingsData.b_ItemDistanceLimit &&
                    PAUtils.GetDistance(GameObjectManager.Instance.localPlayer.gameplayCamera.transform.position,
                        grabbableObject.transform.position) < SettingsData.fl_ItemDistanceLimit) ||
                        !SettingsData.b_ItemDistanceLimit)
                ),
                SettingsData.b_ItemESP,
                grabbableObject =>
                {
                    string text = "Object";
                    Item itemProperties = grabbableObject.itemProperties;
                    if (itemProperties.itemName != null)
                        text = itemProperties.itemName;
                    int scrapValue = grabbableObject.scrapValue;
                    if (SettingsData.b_DisplayWorth && scrapValue > 0)
                        text += " [" + scrapValue.ToString() + "C]";
                    return text;
                },
                grabbableObject => GetLootColor(grabbableObject.scrapValue)
            );
        }

        public void Start()
        {
            Harmony harmony = new Harmony("com.waxxyTF2.ProjectApparatus");
            harmony.PatchAll();

            StartCoroutine(GameObjectManager.Instance.CollectObjects());
        }

        public void Update()
        {
            if (this.menuButton.WasPerformedThisFrame())
            {
                Settings.b_isMenuOpen = !Settings.b_isMenuOpen;
            }
            if (this.unloadMenu.WasPressedThisFrame())
            {
                Loader.Unload();
                base.StopCoroutine(GameObjectManager.Instance.CollectObjects());
            }

            if (SettingsData.b_NoMoreCredits && GameObjectManager.Instance.shipTerminal)
                GameObjectManager.Instance.shipTerminal.groupCredits = 0;

            Noclip();

            SettingsData.keyNoclip.Update();
        }

        private void Noclip()
        {
            PlayerControllerB localPlayer = GameObjectManager.Instance.localPlayer;
            if (!localPlayer) return;

            Collider localCollider = localPlayer.GetComponent<CharacterController>();
            if (!localCollider) return;

            Transform localTransform = localPlayer.transform;
            localCollider.enabled = !(localTransform && PAUtils.GetAsyncKeyState(SettingsData.keyNoclip.inKey) != 0); 

            if (!localCollider.enabled)
            {
                bool WKey = PAUtils.GetAsyncKeyState((int)Keys.W) != 0,
                    AKey = PAUtils.GetAsyncKeyState((int)Keys.A) != 0,
                    SKey = PAUtils.GetAsyncKeyState((int)Keys.S) != 0,
                    DKey = PAUtils.GetAsyncKeyState((int)Keys.D) != 0,
                    SpaceKey = PAUtils.GetAsyncKeyState((int)Keys.Space) != 0,
                    CtrlKey = PAUtils.GetAsyncKeyState((int)Keys.LControlKey) != 0;

                Vector3 inVec = new Vector3(0, 0, 0);

                if (WKey)
                    inVec += localTransform.forward;
                if (SKey)
                    inVec -= localTransform.forward;
                if (AKey)
                    inVec -= localTransform.right;
                if (DKey)
                    inVec += localTransform.right;
                if (SpaceKey)
                    inVec.y += localTransform.up.y;
                if (CtrlKey)
                    inVec.y -= localTransform.up.y;

                localPlayer.transform.position += inVec * (SettingsData.fl_NoclipSpeed * Time.deltaTime);
            }
        }

        private void ReviveLocalPlayer() // This is a modified version of StartOfRound.ReviveDeadPlayers
        {
            PlayerControllerB localPlayer = GameObjectManager.Instance.localPlayer;
            StartOfRound.Instance.allPlayersDead = false;
            localPlayer.ResetPlayerBloodObjects(localPlayer.isPlayerDead);
            if (localPlayer.isPlayerDead || localPlayer.isPlayerControlled)
            {
                localPlayer.isClimbingLadder = false;
                localPlayer.ResetZAndXRotation();
                localPlayer.thisController.enabled = true;
                localPlayer.health = 100;
                localPlayer.disableLookInput = false;
                if (localPlayer.isPlayerDead)
                {
                    localPlayer.isPlayerDead = false;
                    localPlayer.isPlayerControlled = true;
                    localPlayer.isInElevator = true;
                    localPlayer.isInHangarShipRoom = true;
                    localPlayer.isInsideFactory = false;
                    localPlayer.wasInElevatorLastFrame = false;
                    StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
                    localPlayer.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].position, false, 0f, false, true);
                    localPlayer.setPositionOfDeadPlayer = false;
                    localPlayer.DisablePlayerModel(StartOfRound.Instance.allPlayerObjects[localPlayer.playerClientId], true, true);
                    localPlayer.helmetLight.enabled = false;
                    localPlayer.Crouch(false);
                    localPlayer.criticallyInjured = false;
                    if (localPlayer.playerBodyAnimator != null)
                        localPlayer.playerBodyAnimator.SetBool("Limp", false);
                    localPlayer.bleedingHeavily = false;
                    localPlayer.activatingItem = false;
                    localPlayer.twoHanded = false;
                    localPlayer.inSpecialInteractAnimation = false;
                    localPlayer.disableSyncInAnimation = false;
                    localPlayer.inAnimationWithEnemy = null;
                    localPlayer.holdingWalkieTalkie = false;
                    localPlayer.speakingToWalkieTalkie = false;
                    localPlayer.isSinking = false;
                    localPlayer.isUnderwater = false;
                    localPlayer.sinkingValue = 0f;
                    localPlayer.statusEffectAudio.Stop();
                    localPlayer.DisableJetpackControlsLocally();
                    localPlayer.health = 100;
                    localPlayer.mapRadarDotAnimator.SetBool("dead", false);
                    if (localPlayer.IsOwner)
                    {
                        HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
                        localPlayer.hasBegunSpectating = false;
                        HUDManager.Instance.RemoveSpectateUI();
                        HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                        localPlayer.hinderedMultiplier = 1f;
                        localPlayer.isMovementHindered = 0;
                        localPlayer.sourcesCausingSinking = 0;
                        localPlayer.reverbPreset = StartOfRound.Instance.shipReverb;
                    }
                }
                SoundManager.Instance.earsRingingTimer = 0f;
                localPlayer.voiceMuffledByEnemy = false;
                SoundManager.Instance.playerVoicePitchTargets[localPlayer.playerClientId] = 1f;
                SoundManager.Instance.SetPlayerPitch(1f, (int)localPlayer.playerClientId);
                if (localPlayer.currentVoiceChatIngameSettings == null)
                {
                    StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
                }
                if (localPlayer.currentVoiceChatIngameSettings != null)
                {
                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        localPlayer.currentVoiceChatIngameSettings.InitializeComponents();

                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        return;

                    localPlayer.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
                }
            }
            PlayerControllerB playerControllerB = GameNetworkManager.Instance.localPlayerController;
            playerControllerB.bleedingHeavily = false;
            playerControllerB.criticallyInjured = false;
            playerControllerB.playerBodyAnimator.SetBool("Limp", false);
            playerControllerB.health = 100;
            HUDManager.Instance.UpdateHealthUI(100, false);
            playerControllerB.spectatedPlayerScript = null;
            HUDManager.Instance.audioListenerLowPass.enabled = false;
            StartOfRound.Instance.SetSpectateCameraToGameOverMode(false, playerControllerB);
            RagdollGrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<RagdollGrabbableObject>();
            for (int j = 0; j < array.Length; j++)
            {
                if (!array[j].isHeld)
                {
                    if (StartOfRound.Instance.IsServer)
                    {
                        if (array[j].NetworkObject.IsSpawned)
                            array[j].NetworkObject.Despawn(true);
                        else
                            UnityEngine.Object.Destroy(array[j].gameObject);
                    }
                }
                else if (array[j].isHeld && array[j].playerHeldBy != null)
                {
                    array[j].playerHeldBy.DropAllHeldItems(true, false);
                }
            }
            DeadBodyInfo[] array2 = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
            for (int k = 0; k < array2.Length; k++)
            {
                UnityEngine.Object.Destroy(array2[k].gameObject);
            }
            StartOfRound.Instance.livingPlayers = StartOfRound.Instance.connectedPlayersAmount + 1;
            StartOfRound.Instance.allPlayersDead = false;
            StartOfRound.Instance.UpdatePlayerVoiceEffects();
            StartOfRound.Instance.shipAnimator.ResetTrigger("ShipLeave");
        }

        public LayerMask s_layerMask = LayerMask.GetMask(new string[]
        {
            "Room"
        });

        private readonly InputAction menuButton = new InputAction(null, InputActionType.Button, "<Keyboard>/insert", null, null, null);
        private readonly InputAction unloadMenu = new InputAction(null, InputActionType.Button, "<Keyboard>/pause", null, null, null);
    }
}
