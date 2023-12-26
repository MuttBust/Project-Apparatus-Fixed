using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using GameNetcodeStuff;

namespace ProjectApparatus
{
    [System.Serializable]
    public static class SettingsData
    {
        /* ESP */
        public static bool b_EnableESP;
        public static bool b_ItemESP;
        public static bool b_EnemyESP;
        public static bool b_PlayerESP;
        public static bool b_DoorESP;
        public static bool b_LandmineESP;
        public static bool b_TurretESP;
        public static bool b_ShipESP;
        public static bool b_SteamHazard;
        public static bool b_DisplayHP, b_DisplaySpeaking, b_DisplayWorth, b_DisplayDistance;
        public static bool b_ItemDistanceLimit = true, b_MineDistanceLimit = true, b_TurretDistanceLimit = true, b_EnemyDistanceLimit;
        public static float fl_ItemDistanceLimit = 80f, fl_MineDistanceLimit = 80f, fl_TurretDistanceLimit = 80f, fl_EnemyDistanceLimit = 120f;

        /* Self */
        public static bool b_GodMode;
        public static bool b_InfiniteStam, b_InfiniteCharge, b_InfiniteZapGun, b_InfiniteShotgunAmmo, b_UnlimitedGrabDistance;
        public static bool b_OneHandAllObjects;
        public static bool b_DisableFallDamage;
        public static bool b_DisableInteractCooldowns;
        public static bool b_InstantInteractions;
        public static bool b_NightVision;
        public static bool b_FastLadderClimbing;
        public static bool b_PlaceAnywhere;
        public static bool b_InteractThroughWalls;
        public static bool b_TauntSlide;
        public static bool b_HearEveryone;
        public static bool b_ChargeAnyItem;
        public static bool b_WalkSpeed;
        public static int i_WalkSpeed;
        public static bool b_SprintSpeed;
        public static int i_SprintSpeed;
        public static bool b_JumpHeight;
        public static int i_JumpHeight;

        /* Misc */
        public static bool b_AllJetpacksExplode;
        public static bool b_LightShow;
        public static bool b_NoMoreCredits;
        public static bool b_SensitiveLandmines;
        public static bool b_LandmineEarrape;
        public static bool b_ForceCloseDoors;
        public static bool b_Noclip;
        public static float fl_NoclipSpeed = 7f;
        public static string str_MoneyToGive = "0";
        public static string str_QuotaFulfilled = "0", str_Quota = "130";

        /* Graphics */
        public static bool b_DisableFog, b_DisableBloom, b_DisableDepthOfField, b_DisableVignette, b_DisableFilmGrain, b_DisableExposure;

        /* Cheat */
        public static bool b_DisplayGroupCredits = true;
        public static bool b_DisplayQuota = true;
        public static bool b_DisplayDaysLeft = true;
        public static bool b_CenteredIndicators = false;
        public static bool b_Crosshair;
        public static bool b_DeadPlayers;

        public static Color c_Theme = new Color(1f, 1f, 1f, 1f);
        public static Color c_Spectator = new Color(0.996f, 0.635f, 0.667f, 1.0f);
        public static Color c_Valve = new Color(1f, 0.49f, 0.851f, 1f);
        public static Color c_Enemy = new Color(0.996f, 0.635f, 0.667f, 1.0f);
        public static Color c_Turret = new Color(0.996f, 0.635f, 0.667f, 1.0f);
        public static Color c_Landmine = new Color(0.996f, 0.635f, 0.667f, 1.0f);
        public static Color c_Player = new Color(0.698f, 0.808f, 0.996f, 1.0f);
        public static Color c_Door = new Color(0.74f, 0.74f, 1f, 1f);
        public static Color c_Loot = new Color(0.5f, 1f, 1f, 1f);
        public static Color c_smallLoot = new Color(0.518f, 0.682f, 0.729f, 1f);
        public static Color c_medLoot = new Color(0.5f, 0.816f, 1f, 1f);
        public static Color c_bigLoot = new Color(1f, 0.629f, 1f, 1f);

        public static KeyBind keyNoclip = new KeyBind();
    }

    public static class Settings
    {
        private const string saveKey = "SettingsData",
            saveName = "PASettings";

        // SettingsData instance is now static

        /* UI */
        public static float TEXT_HEIGHT = 30f;
        public static Rect windowRect = new Rect(50f, 50f, 320f, 400f);
        public static bool b_isMenuOpen;

        /* Players */
        public static Dictionary<PlayerControllerB, bool> b_DemiGod = new Dictionary<PlayerControllerB, bool>();
        public static Dictionary<PlayerControllerB, bool> b_ObjectSpam = new Dictionary<PlayerControllerB, bool>();
        public static string str_DamageToGive = "1", str_HealthToHeal = "1";

        // SettingsData instance is now static
        public static void InitializeDictionaries(PlayerControllerB key)
        {
            if (!b_DemiGod.ContainsKey(key))
                b_DemiGod[key] = false;
            if (!b_ObjectSpam.ContainsKey(key))
                b_ObjectSpam[key] = false;
        }



    }
}