using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang.ExpandedGenealogy
{
    public class GER : PlayerLanguage
    {
        public override bool HasNthUncles
        {
            get
            {
                return true;
            }
        }

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string greats = "";
            for (int i = 1; i < siblingOfAncestorInfo.GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy/RelationNames:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedDownward" : "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Grand"));
        }

        public override string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null)
            {
                return "";
            }
            if (distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelatives)
            {
                return "";
            }
            string degree = (distantRelationInfo.Degree + 1).ToString(), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/ExpandedGenealogy/RelationNames:Nth{0}CousinNxRemoved{1}ward", distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0 && distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
            {
                if (distantRelationInfo.TimesRemoved > 0)
                {
                    if (distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
                    {
                        for (int i = 2; i < distantRelationInfo.TimesRemoved; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
                        }
                        return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats, distantRelationInfo.TimesRemoved > 1 ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Grand") : "");
                    }
                    return "";
                }
                return Localization.LocalizeString(isFemale, distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousin" : "Destrospean/ExpandedGenealogy/RelationNames:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
            }
            return "";
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string greats = "";
            for (int i = 1; i < siblingOfAncestorInfo.GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy/RelationNames:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedUpward" : "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Grand"));
        }
    }
}