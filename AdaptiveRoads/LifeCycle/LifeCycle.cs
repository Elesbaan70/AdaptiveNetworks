namespace AdaptiveRoads.LifeCycle {
    using AdaptiveRoads.Manager;
    using AdaptiveRoads.Patches;
    using CitiesHarmony.API;
    using ICities;
    using KianCommons;
    using System;
    using System.Diagnostics;
    using UnityEngine.SceneManagement;
    using AdaptiveRoads.UI.RoadEditor;

    public static class LifeCycle {
        //public static string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        //public static string HARMONY_ID = "CS.Kian." + AssemblyName;
        public static string HARMONY_ID = "CS.Kian.AdaptiveRoads";
        public static string HARMONY_ID_MANUAL = "CS.Kian.AdaptiveRoads.Manual";

        public static SimulationManager.UpdateMode UpdateMode => SimulationManager.instance.m_metaData.m_updateMode;
        public static LoadMode Mode => (LoadMode)UpdateMode;
        public static string Scene => SceneManager.GetActiveScene().name;

        public static bool Loaded;
        public static bool bHotReload = false;
        const bool fastTestHarmony = false;

        public static void Enable() {
            Log.Debug("Testing StackTrace:\n" + new StackTrace(true).ToString(), copyToGameLog: false);
            KianCommons.UI.TextureUtil.EmbededResources = false;
            HelpersExtensions.VERBOSE = false;
            Loaded = false;

            HarmonyHelper.EnsureHarmonyInstalled();
            //LoadingManager.instance.m_simulationDataReady += SimulationDataReady; // load/update data
            LoadingManager.instance.m_levelPreLoaded += Preload;
            if (LoadingManager.instance.m_loadingComplete)
                HotReload();

            if (fastTestHarmony) {
                HarmonyHelper.DoOnHarmonyReady(() => {
                    HarmonyUtil.InstallHarmony(HARMONY_ID_MANUAL);
                    HarmonyUtil.InstallHarmony(HARMONY_ID);
                });
            }
        }

        public static void HotReload() {
            bHotReload = true;
            Preload();
            //SimulationDataReady();
            Load();
        }


        public static void Disable() {
            //LoadingManager.instance.m_simulationDataReady -= SimulationDataReady;
            LoadingManager.instance.m_levelPreLoaded -= Preload;
            Unload(); // in case of hot unload
            Exit();
            if (fastTestHarmony) {
                HarmonyUtil.UninstallHarmony(HARMONY_ID);
                HarmonyUtil.UninstallHarmony(HARMONY_ID_MANUAL);
            }
        }

        public static void Preload() {
            Log.Info("LifeCycle.Preload() called");
            if (!HideCrosswalksPatch.patched && PluginUtil.GetHideCrossings().IsActive()) {
                HarmonyUtil.ManualPatch(typeof(HideCrosswalksPatch), HARMONY_ID_MANUAL);
                HideCrosswalksPatch.patched = true;
            }
            HelpersExtensions.VERBOSE = false;
        }

        public static void Load() {
            try {
                Log.Info("LifeCycle.Load() called");
                Log.Info("testing stack trace:\n" + Environment.StackTrace, false);

                NetworkExtensionManager.Instance.OnLoad();
                HarmonyUtil.InstallHarmony(HARMONY_ID);
                NetInfoExtionsion.EnsureEditedNetInfosExtended();
                HintBox.Create();
            }
            catch (Exception e) {
                Log.Error(e.ToString() + "\n --- \n");
                throw e;
            }
        }

        public static void Unload() {
            Log.Info("LifeCycle.Release() called");
            HintBox.Release();
            HarmonyUtil.UninstallHarmony(HARMONY_ID);
            NetworkExtensionManager.Instance.OnUnload();
        }

        public static void Exit() {
            Log.Info("LifeCycle.Exit() called");
            HarmonyUtil.UninstallHarmony(HARMONY_ID_MANUAL);
            HideCrosswalksPatch.patched = false;
        }
    }
}
