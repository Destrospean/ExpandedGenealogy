using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang.ExpandedGenealogy
{
    public class ALT : ENG
    {
        public override string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null || distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelatives)
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
                        for (int i = 2; i < distantRelationInfo.TimesRemoved; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:Great");
                        }
                        return Localization.LocalizeString(isFemale, string.Format(Common.kLocalizationPath + "/RelationNames:{0}{1}CousinNxRemoved{2}ward", Tuning.kShow1stCousinsAsCousins && distantRelationInfo.Degree == 1 ? "" : "Nth", distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down"), degree, Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats, distantRelationInfo.TimesRemoved > 1 ? Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:Grand") : "");
                    }
                    return "";
                }
                return Localization.LocalizeString(isFemale, distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? Common.kLocalizationPath + "/RelationNames:NthHalfCousin" : Common.kLocalizationPath + "/RelationNames:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)));
            }
            return "";
        }
    }
}