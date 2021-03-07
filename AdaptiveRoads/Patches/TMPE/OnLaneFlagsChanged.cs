namespace AdaptiveRoads.Patches.TMPE {
    using HarmonyLib;
    using KianCommons;
    using TrafficManager.Manager.Impl;
    using System;
    using System.Diagnostics;
    using AdaptiveRoads.Manager;

    [HarmonyPatch]
    [InGamePatch]
    class OnLaneFlagsChanged {
        // private void LaneArrowManager.OnLaneChange(uint laneId) {
        [HarmonyPatch(typeof(LaneArrowManager))]
        [HarmonyPatch("OnLaneChange")]
        static void Postfix(uint laneId) {
            ushort segmentId = laneId.ToLane().m_segment;
            Log.Debug($"OnLaneFlagsChanged.PostFix() was called for " +
                $"laneid:{laneId} segment:{segmentId} " +
                $"caller:{new StackFrame(1)}");
            NetworkExtensionManager.Instance.UpdateSegment(segmentId);
        }
    }
}
