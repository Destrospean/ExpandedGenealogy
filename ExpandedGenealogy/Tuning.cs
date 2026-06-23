namespace Sims3.Gameplay.Destrospean
{
    public static class ExpandedGenealogy
    {
        [Sims3.SimIFace.Tunable]
        public static bool kAllowRomanceForHalfRelatives, kDenyRomanceWithAncestors, kDenyRomanceWithSiblings, kDenyRomanceWithSiblingsOfAncestors, kDenyRomanceWithStepParents, kDenyRomanceWithStepSiblings, kReplaceRelationsForSimBots, kShow1stCousinsAsCousins, kShowCheatInteractions, kShowHalfRelatives, kShowHalfRelativesAsFullRelatives;

        [Sims3.SimIFace.Tunable]
        public static int kMaxDegreeCousinsToShow, kMaxTimesRemovedCousinsToShow, kMinDegreeCousinsToAllowRomance, kMinTimesRemovedCousinsToAllowRomance;

        [Sims3.SimIFace.Tunable]
        public static float kMinRelationshipCoefficientToDenyRomance;
    }
}