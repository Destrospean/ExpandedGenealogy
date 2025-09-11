using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang.ExpandedGenealogy
{
    public class ITA : PlayerLanguage
    {
        public override string GetAncestorString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            int generationalDistance = descendant.GetAncestorInfo(ancestor).GenerationalDistance;
            if (generationalDistance == 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGGP", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
            }
            else if (generationalDistance > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrandparent", (generationalDistance + 1).ToString(), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
            }
            return "";
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
            string degree = distantRelationInfo.Degree.ToString(), greats = "";
            if (distantRelationInfo.Degree > 0 && distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
            {
                if (distantRelationInfo.TimesRemoved > 0)
                {
                    if (distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
                    {
                        for (int i = 1; i < distantRelationInfo.TimesRemoved; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
                        }
                        return Localization.LocalizeString(isFemale, string.Format("Destrospean/ExpandedGenealogy/RelationNames:{0}{1}CousinNxRemoved{2}ward", Tuning.kShow1stCousinsAsCousins && distantRelationInfo.Degree == 1 ? "" : "Nth", distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down"), degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats);
                    }
                    return "";
                }
                return Localization.LocalizeString(isFemale, distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousin" : "Destrospean/ExpandedGenealogy/RelationNames:NthCousin", degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)));
            }
            return "";
        }

        public override string GetDescendantString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            int generationalDistance = descendant.GetAncestorInfo(ancestor).GenerationalDistance;
            if (generationalDistance == 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGGC", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
            }
            else if (generationalDistance > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrandchild", (generationalDistance + 1).ToString(), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
            }
            return "";
        }
    }
}