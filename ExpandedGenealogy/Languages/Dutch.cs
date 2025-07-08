using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
    public class DUT : PlayerLanguage
    {
        public override string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw = false, bool isStepRelative = false)
        {
            string text = "";
            List<string> binaryGroups = new List<string>();
            int generationalDistance = (isStepRelative ? Common.GetStepAncestorInfo(descendant, ancestor) : Common.GetAncestorInfo(descendant, ancestor)).GenerationalDistance;
            while (generationalDistance > 511)
            {
                binaryGroups.Insert(0, "111111111");
                generationalDistance -= 511;
            }
            binaryGroups.Insert(0, Convert.ToString(generationalDistance - Convert.ToInt32(binaryGroups.Count != 0), 2));
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
                        ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + (9 - j).ToString());
                    }
                }
                switch (binaryGroups[i].Substring(binaryGroups[i].Length - 2))
                {
                    case "01":
                        ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand");
                        break;
                    case "10":
                        ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1");
                        break;
                    case "11":
                        ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun2");
                        break;
                }
            }
            for (int i = 0; i < ancestorPrefixes.Length; i++)
            {
                text += Localization.LocalizeString(isFemale, i == 0 ? "Destrospean/ExpandedGenealogy:GreatNxGrandparent" : "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun12", ancestorPrefixes[i], isInLaw && i == ancestorPrefixes.Length - 1 ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "", isStepRelative ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
                if (i < ancestorPrefixes.Length - 1)
                {
                    text += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun11");
                }
            }
            return text;
        }

        public override string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw = false, bool isStepRelative = false)
        {
            string greats = "";
            for (int i = 1; i < (isStepRelative ? Common.GetStepAncestorInfo(descendant, ancestor) : Common.GetAncestorInfo(descendant, ancestor)).GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun0");
            }
            return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandchild", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "", isStepRelative ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
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
            string degree = "", greatNxUncleEntryKey = "Destrospean/ExpandedGenealogy:GreatNx" + (isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/ExpandedGenealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down"), step = isStepRelative ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "";
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
                                degree += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun0");
                            }
                            for (int i = 1; i < distantRelationInfo.TimesRemoved; i++)
                            {
                                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, greats, step);
                        }
                        return "";
                    }
                    for (int i = 1; i < distantRelationInfo.Degree; i++)
                    {
                        degree += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun0");
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:NthHalfCousin" : "Destrospean/ExpandedGenealogy:NthCousin", degree, step);
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
    }
}