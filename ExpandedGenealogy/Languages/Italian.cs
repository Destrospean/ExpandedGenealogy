using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
    public class ITA : PlayerLanguage
    {
        public override string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw = false, bool isStepRelative = false)
        {
            int generationalDistance = Common.GetAncestorInfo(descendant, ancestor).GenerationalDistance;
            if (generationalDistance == 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GGGP", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "");
            }
            else if (generationalDistance > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandparent", (generationalDistance + 1).ToString(), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "");
            }
            return "";
        }

        public override string GetDistantRelationString(bool isFemale, Genealogy sim, Common.DistantRelationInfo distantRelationInfo, bool isStepRelative = false)
        {
            if (distantRelationInfo == null)
            {
                return "";
            }
            bool isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0], distantRelationInfo.ThroughWhichChildren[1]);
            if (isHalfRelative && !Tuning.kShowDistantHalfRelatives)
            {
                return "";
            }
            string degree = distantRelationInfo.Degree.ToString(), greatNxUncleEntryKey = "Destrospean/ExpandedGenealogy:GreatNx" + (isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/ExpandedGenealogy:{0}{1}CousinNxRemoved{2}ward", Tuning.kShow1stCousinsAsCousins && distantRelationInfo.Degree == 1 ? "" : "Nth", isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
                {
                    if (distantRelationInfo.TimesRemoved > 0)
                    {
                        if (distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
                        {
                            for (int i = 1; i < distantRelationInfo.TimesRemoved; i++)
                            {
                                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats);
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:NthHalfCousin" : "Destrospean/ExpandedGenealogy:NthCousin", degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)));
                }
                return "";
            }
            if (distantRelationInfo.TimesRemoved < 1)
            {
                return "";
            }
            for (int i = 2; i < distantRelationInfo.TimesRemoved; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
            }
            return Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats);
        }

        public override string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw = false, bool isStepRelative = false)
        {
            int generationalDistance = Common.GetAncestorInfo(descendant, ancestor).GenerationalDistance;
            if (generationalDistance == 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GGGC", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "");
            }
            else if (generationalDistance > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandchild", (generationalDistance + 1).ToString(), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "");
            }
            return "";
        }
    }
}