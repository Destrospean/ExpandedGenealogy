using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;
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

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string greats = "";
            for (int i = 1; i < siblingOfAncestorInfo.GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedDownward" : "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
        }

        public override string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null)
            {
                return "";
            }
            bool isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0].Genealogy, distantRelationInfo.ThroughWhichChildren[1].Genealogy);
            if (isHalfRelative && !Tuning.kShowHalfRelatives)
            {
                return "";
            }
            string degree = (distantRelationInfo.Degree + 1).ToString(), greatNxUncleEntryKey = "Destrospean/Genealogy:GreatNx" + (isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down");
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
                                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats, distantRelationInfo.TimesRemoved > 1 ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand") : "");
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousin" : "Destrospean/Genealogy:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
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
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string greats = "";
            for (int i = 1; i < siblingOfAncestorInfo.GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedUpward" : "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
        }
    }
}