using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang.ExpandedGenealogy
{
    public class DUT : PlayerLanguage
    {
        public override string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            string text = "";
            List<string> binaryGroups = new List<string>();
            int generationalDistance = descendant.GetAncestorInfo(ancestor).GenerationalDistance;
            while (generationalDistance > 511)
            {
                binaryGroups.Insert(0, "111111111");
                generationalDistance -= 511;
            }
            binaryGroups.Insert(0, Convert.ToString(generationalDistance - (binaryGroups.Count == 0 ? 0 : 1), 2));
            while (binaryGroups[0].Length < 9)
            {
                binaryGroups[0] = "0" + binaryGroups[0];
            }
            string[] ancestorPrefixes = new string[binaryGroups.Count];
            for (int i = 0; i < ancestorPrefixes.Length; i++)
            {
                ancestorPrefixes[i] = "";
                for (int j = 0; j < 7; j++)
                {
                    if (binaryGroups[i][j] == '1')
                    {
                        ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + (9 - j).ToString());
                    }
                }
                switch (binaryGroups[i].Substring(binaryGroups[i].Length - 2))
                {
                    case "01":
                        ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand");
                        break;
                    case "10":
                        ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1");
                        break;
                    case "11":
                        ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun2");
                        break;
                }
            }
            for (int i = 0; i < ancestorPrefixes.Length; i++)
            {
                text += Localization.LocalizeString(isFemale, i == 0 ? "Destrospean/Genealogy:GreatNxGrandparent" : "Destrospean/Genealogy:OrdinalSuffixNoun12", ancestorPrefixes[i], isInLaw && i == ancestorPrefixes.Length - 1 ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
                if (i < ancestorPrefixes.Length - 1)
                {
                    text += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun11");
                }
            }
            return text;
        }

        public override string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string greats = "";
            for (int i = 1; i < descendant.GetAncestorInfo(ancestor).GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun0");
            }
            return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
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
            string degree = "", greatNxUncleEntryKey = "Destrospean/Genealogy:GreatNx" + (isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
                {
                    if (distantRelationInfo.TimesRemoved > 0)
                    {
                        if (distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
                        {
                            for (int i = 0; i < distantRelationInfo.Degree; i++)
                            {
                                degree += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun0");
                            }
                            for (int i = 1; i < distantRelationInfo.TimesRemoved; i++)
                            {
                                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, greats);
                        }
                        return "";
                    }
                    for (int i = 1; i < distantRelationInfo.Degree; i++)
                    {
                        degree += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun0");
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousin" : "Destrospean/Genealogy:NthCousin", degree);
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
            return Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats);
        }
    }
}