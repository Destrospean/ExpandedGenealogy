using Sims3.SimIFace;

namespace Sims3.Gameplay.Destrospean
{
    public static class ExpandedGenealogy
    {
        [Tunable]
        public static bool kAccumulateDistantStepRelatives;

        [Tunable]
        public static bool kAllowRomanceForDistantHalfRelatives;

        [Tunable]
        public static bool kAllowRomanceForDistantStepRelatives;

        [Tunable]
        public static int kMaxDegreeCousinsToShow;

        [Tunable]
        public static int kMaxTimesRemovedCousinsToShow;

        [Tunable]
        public static int kMinDegreeCousinsToAllowRomance;

        [Tunable]
        public static int kMinTimesRemovedCousinsToAllowRomance;

        [Tunable]
        public static bool kReplaceRelationsForSimBots;

        [Tunable]
        public static bool kShow1stCousinsAsCousins;

        [Tunable]
        public static bool kShowDistantHalfRelatives;

        [Tunable]
        public static bool kShowDistantHalfRelativesAsFullRelatives;

        [Tunable]
        public static bool kShowDistantStepRelatives;
    }
}