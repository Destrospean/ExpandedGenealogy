using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang.ExpandedGenealogy
{
    public class FRE : PlayerLanguage
    {
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
            string degree = distantRelationInfo.Degree.ToString(), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:{0}{1}CousinNxRemoved{2}ward", Tuning.kShow1stCousinsAsCousins && distantRelationInfo.Degree == 1 ? "" : "Nth", isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0 && distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
            {
                if (distantRelationInfo.TimesRemoved > 0)
                {
                    if (distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
                    {
                        for (int i = 1; i < distantRelationInfo.TimesRemoved; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                        }
                        return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats);
                    }
                    return "";
                }
                return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousin" : "Destrospean/Genealogy:NthCousin", degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)));
            }
            return "";
        }

        public override string GetOrdinalSuffix(string number)
        {
            return number.Length > 1 ? "0" : number.Substring(number.Length - 1);
        }
    }
}