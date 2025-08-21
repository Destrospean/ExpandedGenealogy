using Sims3.SimIFace;

namespace Sims3.Gameplay.Destrospean
{
    public static class ExpandedGenealogy
    {
        [Tunable]
        public static bool kAllowRomanceForHalfRelatives;

        [Tunable]
        public static bool kDenyRomanceWithAncestors;

        [Tunable]
        public static bool kDenyRomanceWithSiblings;

        [Tunable]
        public static bool kDenyRomanceWithSiblingsOfAncestors;

        [Tunable]
        public static bool kDenyRomanceWithStepParents;

        [Tunable]
        public static bool kDenyRomanceWithStepSiblings;

        [Tunable]
        public static int kMaxDegreeCousinsToShow;

        [Tunable]
        public static int kMaxTimesRemovedCousinsToShow;

        [Tunable]
        public static int kMinDegreeCousinsToAllowRomance;

        [Tunable]
        public static float kMinRelationshipCoefficientToDenyRomance;

        [Tunable]
        public static int kMinTimesRemovedCousinsToAllowRomance;

        [Tunable]
        public static bool kReplaceRelationsForSimBots;

        [Tunable]
        public static bool kShow1stCousinsAsCousins;

        [Tunable]
        public static bool kShowCheatInteractions;

        [Tunable]
        public static bool kShowHalfRelatives;

        [Tunable]
        public static bool kShowHalfRelativesAsFullRelatives;
    }
}