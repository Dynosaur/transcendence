﻿// Material/Shader Inspector for Unity 2017/2018
// Copyright (C) 2019 Thryrallo

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Thry
{
    public class VRCInterface
    {
        private static VRCInterface instance;
        public static VRCInterface Get()
        {
            if (instance == null) instance = new VRCInterface();
            return instance;
        }
        public static void Update()
        {
            instance = new VRCInterface();
        }

        public bool sdk_is_installed;
        public bool sdk_is_up_to_date;
        public string installed_sdk_version;
        public string newest_sdk_version;
        public string sdk_path = null;

        public bool user_logged_in;

        public VRCInterface()
        {
            sdk_is_installed = IsVRCSDKInstalled();
            InitSDKVersionVariables();
            InitUserVariables();
        }

        private void InitSDKVersionVariables()
        {
            if (!sdk_is_installed)
                return;
            installed_sdk_version = GetInstalledSDKVersionAndInitPath();
            newest_sdk_version = "0";
            sdk_is_up_to_date = true;
#if VRC_SDK_EXISTS
            VRC.Core.RemoteConfig.Init(delegate ()
            {
                newest_sdk_version = GetNewestSDKVersion();
                sdk_is_up_to_date = SDKIsUpToDate();
            });
#endif
        }

        private void InitUserVariables()
        {
            user_logged_in = EditorPrefs.HasKey("sdk#username");
        }
        
        private string GetInstalledSDKVersionAndInitPath()
        {
            string[] guids = AssetDatabase.FindAssets("version");
            string path = null;
            foreach (string guid in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (p.Contains("VRCSDK/version"))
                    path = p;
            }
            if (path == null || !File.Exists(path))
                return "";
            sdk_path = Regex.Match(path, @".*\/").Value;
            return Helper.ReadFileIntoString(path);
        }

        private static string GetNewestSDKVersion()
        {
#if VRC_SDK_EXISTS
            string version = VRC.Core.RemoteConfig.GetString("devSdkVersion");
            if(version!=null && version!="")
                return Regex.Match(version, @"[\d\.]+").Value;
#endif
            return "0";
        }

        private bool SDKIsUpToDate()
        {
            return Helper.compareVersions(installed_sdk_version, newest_sdk_version) != 1;
        }

        private static bool IsVRCSDKInstalled()
        {
            return Helper.NameSpaceExists("VRCSDK2");
        }

        public void RemoveVRCSDK()
        {
            RemoveVRCSDK(true);
        }

        public void RemoveVRCSDK(bool refresh)
        {
            Helper.SaveValueToFile("delete_vrc_sdk", "true", PATH.AFTER_COMPILE_DATA);
            Helper.SetDefineSymbol(DEFINE_SYMBOLS.VRC_SDK_INSTALLED, false);
            AssetDatabase.Refresh();
        }

        public static void OnCompile()
        {
            if (Helper.LoadValueFromFile("delete_vrc_sdk", PATH.AFTER_COMPILE_DATA) == "true")
                DeleteVRCSDKFolder();
        }

        private static void DeleteVRCSDKFolder()
        {
            if (!Get().sdk_is_installed)
            {
                Helper.SaveValueToFile("delete_vrc_sdk", "false", PATH.AFTER_COMPILE_DATA);
                if (Helper.LoadValueFromFile("update_vrc_sdk", PATH.AFTER_COMPILE_DATA) == "true")
                    DownloadAndInstallVRCSDK();
                else
                    Settings.is_changing_vrc_sdk = false;
            }
            if (Get().sdk_path != null && Directory.Exists(Get().sdk_path))
            {
                Directory.Delete(Get().sdk_path, true);
                AssetDatabase.Refresh();
            }
            Update();
        }

        public void UpdateVRCSDK()
        {
            Helper.SaveValueToFile("update_vrc_sdk", "true", PATH.AFTER_COMPILE_DATA);
            this.RemoveVRCSDK();
        }

        public static void DownloadAndInstallVRCSDK()
        {
            string url = "https://vrchat.net/download/sdk";

            if (File.Exists(PATH.TEMP_VRC_SDK_PACKAGE))
                File.Delete(PATH.TEMP_VRC_SDK_PACKAGE);
            Helper.DownloadFileASync(url, PATH.TEMP_VRC_SDK_PACKAGE, VRCSDKUpdateCallback);
        }

        public static void VRCSDKUpdateCallback(string data)
        {
            Helper.SaveValueToFile("update_vrc_sdk", "false", PATH.AFTER_COMPILE_DATA);
            AssetDatabase.ImportPackage(PATH.TEMP_VRC_SDK_PACKAGE, false);
            File.Delete(PATH.TEMP_VRC_SDK_PACKAGE);
            Update();
        }

        public static void SetVRCDefineSybolIfSDKImported(string[] importedAssets)
        {
            bool currently_deleteing_sdk = (Helper.LoadValueFromFile("delete_vrc_sdk", PATH.AFTER_COMPILE_DATA) == "true");
            if (!Settings.is_changing_vrc_sdk && !currently_deleteing_sdk && AssetsContainVRCSDK(importedAssets))
            {
                Helper.SetDefineSymbol(DEFINE_SYMBOLS.VRC_SDK_INSTALLED, true);
                Update();
            }
        }

        public static void SetVRCDefineSybolIfSDKDeleted(string[] importedAssets)
        {
            bool currently_deleteing_sdk = (Helper.LoadValueFromFile("delete_vrc_sdk", PATH.AFTER_COMPILE_DATA) == "true");
            if (!Settings.is_changing_vrc_sdk && !currently_deleteing_sdk && AssetsContainVRCSDK(importedAssets))
            {
                Helper.SetDefineSymbol(DEFINE_SYMBOLS.VRC_SDK_INSTALLED, false);
                Update();
            }
        }

        public static bool AssetsContainVRCSDK(string[] assets)
        {
            bool vrcImported = false;
            foreach (string s in assets) if (s.Contains("VRCSDK2.dll")) vrcImported = true;
            return vrcImported; 
        }
    }
}
