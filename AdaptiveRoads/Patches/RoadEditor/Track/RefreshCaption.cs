namespace AdaptiveRoads.Patches.RoadEditor.Track {
    using AdaptiveRoads.Manager;
    using ColossalFramework.UI;
    using HarmonyLib;
    using KianCommons;
    using System;

    [HarmonyPatch(typeof(RoadEditorPanel), "RefreshCaption")]
    public static class RefreshCaption {
        static void Postfix(UILabel ___m_Caption, object ___m_Target) {
            try {
                if(___m_Caption && ___m_Target is NetInfoExtionsion.Track track)
                    ___m_Caption.text = "Track Properties";
            } catch(Exception ex) {
                Log.Exception(ex);
            }
        }
    }
}