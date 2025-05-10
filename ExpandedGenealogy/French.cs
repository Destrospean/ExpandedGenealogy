using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using static Destrospean.Common;
using static Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
    public class FRE : PlayerLanguage
    {
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
            string degree = distantRelationInfo.Degree.ToString(), greatNxUncleEntryKey = "Destrospean/Genealogy:GreatNx" + (isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:{2}{0}CousinNxRemoved{1}ward", isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down", kShow1stCousinsAsCousins && distantRelationInfo.Degree == 1 ? "" : "Nth");
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)kMaxDegreeCousinsToShow)
                {
                    if (distantRelationInfo.TimesRemoved > 0)
                    {
                        if (distantRelationInfo.TimesRemoved <= (uint)kMaxTimesRemovedCousinsToShow)
                        {
                            for (int i = 1; i < distantRelationInfo.TimesRemoved; i++)
                            {
                                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats, distantRelationInfo.TimesRemoved > 0 ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand") : "");
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousin" : "Destrospean/Genealogy:NthCousin", degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)));
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
            return Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats) ;
        }
    }
}