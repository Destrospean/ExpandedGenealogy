using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
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

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowHalfRelatives)
                    {
                        return "";
                    }
                    string greats = "";
                    for (int i = 1; i < Common.GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy/RelationNames:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedDownward" : "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Grand"));
                }
            }
            return "";
        }

        public override string GetDistantRelationString(bool isFemale, Genealogy sim, Common.DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null)
            {
                return "";
            }
            bool isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0], distantRelationInfo.ThroughWhichChildren[1]);
            if (isHalfRelative && !Tuning.kShowHalfRelatives)
            {
                return "";
            }
            string degree = (distantRelationInfo.Degree + 1).ToString(), greatNxUncleEntryKey = "Destrospean/ExpandedGenealogy/RelationNames:GreatNx" + (isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/ExpandedGenealogy/RelationNames:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
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
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousin" : "Destrospean/ExpandedGenealogy/RelationNames:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
                }
                return "";
            }
            if (distantRelationInfo.TimesRemoved < 1)
            {
                return "";
            }
            for (int i = 2; i < distantRelationInfo.TimesRemoved; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Grand"));
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowHalfRelatives)
                    {
                        return "";
                    }
                    string greats = "";
                    for (int i = 1; i < Common.GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy/RelationNames:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedUpward" : "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Grand"));
                }
            }
            return "";
        }
    }
}