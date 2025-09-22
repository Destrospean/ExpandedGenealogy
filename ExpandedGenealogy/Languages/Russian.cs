using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang.ExpandedGenealogy
{
    public class RUS : PlayerLanguage
    {
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
            string greats = "";
            if (distantRelationInfo.Degree > 0 && distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
            {
                if (distantRelationInfo.TimesRemoved == 1)
                {
                    return Localization.LocalizeString(isFemale, string.Format("Destrospean/ExpandedGenealogy/RelationNames:GGG{0}", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "P" : "C"), GetNthUncleDegreeString(distantRelationInfo.Degree), distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Grand") : "");
                }
                if (distantRelationInfo.TimesRemoved > 1)
                {
                    if (distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
                    {
                        for (int i = 2; i < distantRelationInfo.TimesRemoved; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
                        }
                        return Localization.LocalizeString(isFemale, string.Format("Destrospean/ExpandedGenealogy/RelationNames:{0}{1}CousinNxRemoved{2}ward", Tuning.kShow1stCousinsAsCousins && distantRelationInfo.Degree == 1 ? "" : "Nth", distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down"), GetNthUncleDegreeString(distantRelationInfo.Degree), greats);
                    }
                    return "";
                }
                return Localization.LocalizeString(isFemale, distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousin" : "Destrospean/ExpandedGenealogy/RelationNames:NthCousin", GetNthUncleDegreeString(distantRelationInfo.Degree));
            }
            return "";
        }

        public override string GetNthUncleDegreeString(int degree)
        {
            return degree == 10 ? Localization.LocalizeString("Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun0") : degree < 13 ? Localization.LocalizeString("Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun" + degree) : degree < 20 ? Localization.LocalizeString("Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixAdj" + (degree - 13)) : degree.ToString() + "-";
        }
    }
}