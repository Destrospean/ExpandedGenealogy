using Sims3.SimIFace;

namespace Sims3.Gameplay.Destrospean
{
    public static class ExpandedGenealogy
    {
        [Tunable]
        public static bool kAllowRomanceForHalfRelatives;

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
        public static bool kShowHalfRelatives;

        [Tunable]
        public static bool kShowHalfRelativesAsFullRelatives;
    }
}