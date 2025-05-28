using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using static Destrospean.Common;
using static Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
    public class GER : PlayerLanguage
    {
        public override bool HasNthUncles => true;

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !kShowHalfRelatives)
                    {
                        return "";
                    }
                    string greats = "";
                    for (int i = 1; i < GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                    }
                    return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedDownward" : "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
                }
            }
            return "";
        }

        public override string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null)
            {
                return "";
            }
            bool isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0], distantRelationInfo.ThroughWhichChildren[1]);
            if (isHalfRelative && !kShowHalfRelatives)
            {
                return "";
            }
            string degree = (distantRelationInfo.Degree + 1).ToString(), greatNxUncleEntryKey = "Destrospean/Genealogy:GreatNx" + (isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)kMaxDegreeCousinsToShow)
                {
                    if (distantRelationInfo.TimesRemoved > 0)
                    {
                        if (distantRelationInfo.TimesRemoved <= (uint)kMaxTimesRemovedCousinsToShow)
                        {
                            for (int i = 2; i < distantRelationInfo.TimesRemoved; i++)
                            {
                                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats, distantRelationInfo.TimesRemoved > 1 ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand") : "");
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousin" : "Destrospean/Genealogy:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
                }
                return "";
            }
            if (distantRelationInfo.TimesRemoved < 1)
            {
                return "";
            }
            for (int i = 2; i < distantRelationInfo.TimesRemoved; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !kShowHalfRelatives)
                    {
                        return "";
                    }
                    string greats = "";
                    for (int i = 1; i < GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                    }
                    return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedUpward" : "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
                }
            }
            return "";
        }
    }
}