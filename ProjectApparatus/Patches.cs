﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;
using System.Diagnostics;
using TMPro;
using UnityEngine.Windows;

namespace ProjectApparatus
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public class PlayerControllerB_Update_Patch
    {
        public static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance == GameObjectManager.Instance.localPlayer)
            {
                __instance.disableLookInput = (__instance.quickMenuManager.isMenuOpen || Settings.b_isMenuOpen) ? true : false;
                Cursor.visible = (__instance.quickMenuManager.isMenuOpen || Settings.b_isMenuOpen) ? true : false;
                Cursor.lockState = (__instance.quickMenuManager.isMenuOpen || Settings.b_isMenuOpen) ? CursorLockMode.None : CursorLockMode.Locked;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public class PlayerControllerB_LateUpdate_Patch
    {
        public static void Postfix(PlayerControllerB __instance)
        {
            if (Settings.b_DemiGod.ContainsKey(__instance) && Settings.b_DemiGod[__instance] && __instance.health < 100)
                __instance.DamagePlayerFromOtherClientServerRpc(-(100 - __instance.health), new Vector3(0, 0, 0), 0);

            if (!StartOfRound.Instance)
                return;

            if (SettingsData.b_AllJetpacksExplode)
            {
                if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer.GetType() == typeof(JetpackItem))
                {
                    JetpackItem Jetpack = (__instance.currentlyHeldObjectServer as JetpackItem);// fill it in
                    if (Jetpack != null)
                    {
                        PAUtils.SetValue(__instance, "jetpackPower", float.MaxValue, PAUtils.protectedFlags);
                        PAUtils.CallMethod(__instance, "ActivateJetpack", PAUtils.protectedFlags, null);
                        Jetpack.ExplodeJetpackServerRpc();
                        Jetpack.ExplodeJetpackClientRpc();
                    }
                }
            }

            PlayerControllerB Local = GameObjectManager.Instance.localPlayer;
            if (__instance.actualClientId != Local.actualClientId)
                return;

            if (SettingsData.b_InfiniteStam)
            {
                __instance.sprintMeter = 1f;
                if (__instance.sprintMeterUI != null)
                    __instance.sprintMeterUI.fillAmount = 1f;
            }

            if (SettingsData.b_InfiniteCharge)
            {
                if (__instance.currentlyHeldObjectServer != null
                    && __instance.currentlyHeldObjectServer.insertedBattery != null)
                {
                    __instance.currentlyHeldObjectServer.insertedBattery.empty = false;
                    __instance.currentlyHeldObjectServer.insertedBattery.charge = 1f;
                }
            }

            if (__instance.currentlyHeldObjectServer != null)
            {
                if (SettingsData.b_ChargeAnyItem)
                    __instance.currentlyHeldObjectServer.itemProperties.requiresBattery = true;

                if (SettingsData.b_OneHandAllObjects)
                {
                    __instance.twoHanded = false;
                    __instance.twoHandedAnimation = false;
                    __instance.currentlyHeldObjectServer.itemProperties.twoHanded = false;
                    __instance.currentlyHeldObjectServer.itemProperties.twoHandedAnimation = false;
                }
            }

            if (SettingsData.b_WalkSpeed && !__instance.isSprinting)
                PAUtils.SetValue(__instance, "sprintMultiplier", SettingsData.i_WalkSpeed, PAUtils.protectedFlags);

            if (SettingsData.b_SprintSpeed && __instance.isSprinting)
                PAUtils.SetValue(__instance, "sprintMultiplier", SettingsData.i_SprintSpeed, PAUtils.protectedFlags);

            __instance.climbSpeed = (SettingsData.b_FastLadderClimbing) ? 100f : 4f;

            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            PAUtils.SetValue(__instance, "interactableObjectsMask",
                SettingsData.b_InteractThroughWalls ? LayerMask.GetMask(new string[] { "Props", "InteractableObject" }) : 832,
                bindingAttr);

            __instance.grabDistance = SettingsData.b_UnlimitedGrabDistance ? 9999f : 5f;

            if (__instance.nightVision)
            {
                /* I see a lot of cheats set nightVision.enabled to false when the feature is off, this is wrong as the game sets it to true when you're in-doors. 
                   Also there's no reason to reset it as the game automatically sets it back every time Update is called. */

                if (SettingsData.b_NightVision)
                    __instance.nightVision.enabled = true;

                __instance.nightVision.range = (SettingsData.b_NightVision) ? 9999f : 12f;
                __instance.nightVision.intensity = (SettingsData.b_NightVision) ? 9999f : 366.9317f;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump")]
    public class PlayerControllerB_PlayerJump_Patch
    {
        public static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance.actualClientId == GameObjectManager.Instance.localPlayer.actualClientId
                && SettingsData.b_JumpHeight
                && __instance.fallValue == __instance.jumpForce)
            {
                __instance.fallValue = SettingsData.i_JumpHeight;
                __instance.fallValueUncapped = SettingsData.i_JumpHeight;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects")]
    public class PlayerControllerB_PlayerHitGroundEffects_Patch
    {
        public static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance.actualClientId == GameObjectManager.Instance.localPlayer.actualClientId
                && SettingsData.b_DisableFallDamage)
                __instance.takingFallDamage = false;

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath")]
    public class PlayerControllerB_AllowPlayerDeath_Patch
    {
        public static bool Prefix(PlayerControllerB __instance, ref bool __result)
        {
            if (SettingsData.b_GodMode && __instance == GameObjectManager.Instance.localPlayer)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForEmote")]
    public class PlayerControllerB_CheckConditionsForEmote_Patch
    {
        public static bool Prefix(PlayerControllerB __instance, ref bool __result)
        {
            if (SettingsData.b_TauntSlide)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Landmine), "Update")]
    public class Landmine_Update_Patch
    {
        public static bool Prefix(Landmine __instance)
        {
            if (SettingsData.b_SensitiveLandmines && !__instance.hasExploded)
            {
                foreach (PlayerControllerB plyr in GameObjectManager.Instance.players)
                {
                    Vector3 plyrPosition = plyr.transform.position,
                        minePosition = __instance.transform.position;

                    float distance = Vector3.Distance(plyrPosition, minePosition);
                    if (distance <= 4f)
                        __instance.ExplodeMineServerRpc();
                }
            }

            if (SettingsData.b_LandmineEarrape)
                __instance.ExplodeMineServerRpc();

            return true;
        }
    }

    [HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
    public class ShipBuildModeManager_Update_Patch
    {
        public static void Postfix(ShipBuildModeManager __instance)
        {
            if (SettingsData.b_PlaceAnywhere)
            {
                PlaceableShipObject placingObject = (PlaceableShipObject)PAUtils.GetValue(__instance, "placingObject", PAUtils.protectedFlags);
                placingObject.AllowPlacementOnCounters = true;
                placingObject.AllowPlacementOnWalls = true;
                PAUtils.SetValue(__instance, "CanConfirmPosition", true, PAUtils.protectedFlags);
            }
        }
    }

    [HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
    public class ShipBuildModeManager_PlayerMeetsConditionsToBuild_Patch
    {
        public static bool Prefix(ShipBuildModeManager __instance, ref bool __result)
        {
            if (SettingsData.b_PlaceAnywhere)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GrabbableObject), "RequireCooldown")]
    public class GrabbableObject_RequireCooldown_Patch
    {
        public static bool Prefix(GrabbableObject __instance, ref bool __result)
        {
            if (SettingsData.b_DisableInteractCooldowns)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InteractTrigger), "Interact")]
    public class InteractTrigger_Interact_Patch
    {
        public static bool Prefix(InteractTrigger __instance)
        {
            __instance.interactCooldown = !SettingsData.b_DisableInteractCooldowns;
            return true;
        }
    }

    [HarmonyPatch(typeof(PatcherTool), "LateUpdate")]
    public class PatcherTool_LateUpdate_Patch
    {
        public static void Postfix(PatcherTool __instance)
        {
            if (SettingsData.b_InfiniteZapGun)
            {
                __instance.gunOverheat = 0f;
                __instance.bendMultiplier = 999f;
                __instance.pullStrength = 999f;
                PAUtils.SetValue(__instance, "timeSpentShocking", 0.01f, PAUtils.protectedFlags);
            }
        }
    }

    [HarmonyPatch(typeof(ShotgunItem), "ItemActivate")]
    public class ShotgunItem_ItemActivate_Patch
    {
        public static bool Prefix(ShotgunItem __instance)
        {
            if (SettingsData.b_InfiniteShotgunAmmo)
            {
                __instance.isReloading = false;
                __instance.shellsLoaded++;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "UpdatePlayerVoiceEffects")]
    public class StartOfRound_UpdatePlayerVoiceEffects_Patch
    {
        public static void Postfix(StartOfRound __instance)
        {
            if (SettingsData.b_HearEveryone
                && !StartOfRound.Instance.shipIsLeaving /* Without this you'll be stuck at "Wait for ship to land" - cba to find out way this happens */)
            {
                for (int i = 0; i < __instance.allPlayerScripts.Length; i++)
                {
                    PlayerControllerB playerControllerB = __instance.allPlayerScripts[i];
                    AudioSource currentVoiceChatAudioSource = playerControllerB.currentVoiceChatAudioSource;

                    currentVoiceChatAudioSource.GetComponent<AudioLowPassFilter>().enabled = false;
                    currentVoiceChatAudioSource.GetComponent<AudioHighPassFilter>().enabled = false;
                    currentVoiceChatAudioSource.panStereo = 0f;
                    SoundManager.Instance.playerVoicePitchTargets[(int)((IntPtr)playerControllerB.playerClientId)] = 1f;
                    SoundManager.Instance.SetPlayerPitch(1f, unchecked((int)playerControllerB.playerClientId));

                    currentVoiceChatAudioSource.spatialBlend = 0f;
                    playerControllerB.currentVoiceChatIngameSettings.set2D = true;
                    playerControllerB.voicePlayerState.Volume = 1f;
                }
            }
        }
    }


    [HarmonyPatch(typeof(HUDManager), "HoldInteractionFill")]
    public class HUDManager_HoldInteractionFill_Patch
    {
        public static bool Prefix(HUDManager __instance, ref bool __result)
        {
            if (SettingsData.b_InstantInteractions)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    /* Graphical */

    [HarmonyPatch(typeof(Fog), "IsFogEnabled")]
    public class Fog_IsFogEnabled_Patch
    {
        public static bool Prefix(Fog __instance, ref bool __result)
        {
            if (SettingsData.b_DisableFog)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Fog), "IsVolumetricFogEnabled")]
    public class Fog_IsVolumetricFogEnabled_Patch
    {
        public static bool Prefix(Fog __instance, ref bool __result)
        {
            if (SettingsData.b_DisableFog)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Fog), "IsPBRFogEnabled")]
    public class Fog_IsPBRFogEnabled_Patch
    {
        public static bool Prefix(Fog __instance, ref bool __result)
        {
            if (SettingsData.b_DisableFog)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Bloom), "IsActive")]
    public class Bloom_IsActive_Patch
    {
        public static bool Prefix(Bloom __instance, ref bool __result)
        {
            if (SettingsData.b_DisableBloom)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DepthOfField), "IsActive")]
    public class DepthOfField_IsActive_Patch
    {
        public static bool Prefix(DepthOfField __instance, ref bool __result)
        {
            if (SettingsData.b_DisableDepthOfField)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Vignette), "IsActive")]
    public class Vignette_IsActive_Patch
    {
        public static bool Prefix(Vignette __instance, ref bool __result)
        {
            if (SettingsData.b_DisableVignette)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FilmGrain), "IsActive")]
    public class FilmGrain_IsActive_Patch
    {
        public static bool Prefix(FilmGrain __instance, ref bool __result)
        {
            if (SettingsData.b_DisableFilmGrain)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(Exposure), "IsActive")]
    public class Exposure_IsActive_Patch
    {
        public static bool Prefix(Exposure __instance, ref bool __result)
        {
            if (SettingsData.b_DisableFilmGrain)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}