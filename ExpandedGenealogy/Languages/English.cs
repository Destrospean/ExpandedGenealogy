using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
    public class ENG : PlayerLanguage
    {
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
            string degree = distantRelationInfo.Degree.ToString(), greatNxUncleEntryKey = "Destrospean/Genealogy:GreatNx" + (isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:{0}{1}CousinNxRemoved{2}ward", Tuning.kShow1stCousinsAsCousins && distantRelationInfo.Degree == 1 ? "" : "Nth", isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down"), step = isStepRelative ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "";
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
                {
                    if (distantRelationInfo.TimesRemoved > 0)
                    {
                        if (distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
                        {
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), distantRelationInfo.TimesRemoved.ToString(), step);
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:NthHalfCousin" : "Destrospean/ExpandedGenealogy:NthCousin", degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), step);
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
            return Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats, step);
        }

        public override string GetOrdinalSuffix(string number)
        {
            if (number.Length > 1)
            {
                switch (number.Substring(number.Length - 2))
                {
                    case "11":
                    case "12":
                    case "13":
                        return "0";
                }
            }
            return number.Substring(number.Length - 1);
        }
    }
}