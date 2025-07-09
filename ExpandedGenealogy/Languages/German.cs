using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
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

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling, bool isStepRelative = false)
        {
            if (isStepRelative)
            {
                foreach (Genealogy stepSibling in siblingOfAncestor.GetStepSiblings())
                {
                    if (descendantOfSibling.IsAncestor(stepSibling))
                    {
                        string greats = "";
                        for (int i = 1; i < Common.GetAncestorInfo(descendantOfSibling, stepSibling).GenerationalDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                        }
                        return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
                    }
                    if (Tuning.kAccumulateDistantStepRelatives && descendantOfSibling.IsStepAncestor(stepSibling))
                    {
                        string greats = "";
                        for (int i = 1; i < Common.GetStepAncestorInfo(descendantOfSibling, stepSibling).GenerationalDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                        }
                        return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
                    }
                }
                foreach (Genealogy sibling in siblingOfAncestor.Siblings)
                {
                    if (descendantOfSibling.IsStepAncestor(sibling))
                    {
                        bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                        if (isHalfRelative && !Tuning.kShowDistantHalfRelatives)
                        {
                            return "";
                        }
                        string greats = "";
                        for (int i = 1; i < Common.GetStepAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                        }
                        return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedDownward" : "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
                    }
                }
                return "";
            }
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowDistantHalfRelatives)
                    {
                        return "";
                    }
                    string greats = "";
                    for (int i = 1; i < Common.GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedDownward" : "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), "");
                }
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
            string degree = (distantRelationInfo.Degree + 1).ToString(), greatNxUncleEntryKey = "Destrospean/ExpandedGenealogy:GreatNx" + (isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), greats = "", nthCousinNxRemovedEntryKey = string.Format("Destrospean/ExpandedGenealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down"), step = isStepRelative ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "";
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
                                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats, distantRelationInfo.TimesRemoved > 1 ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand") : "", step);
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:NthHalfCousin" : "Destrospean/ExpandedGenealogy:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())), step);
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
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), step);
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor, bool isStepRelative = false)
        {
            if (isStepRelative)
            {
                foreach (Genealogy stepSibling in siblingOfAncestor.GetStepSiblings())
                {
                    if (descendantOfSibling.IsAncestor(stepSibling))
                    {
                        bool isHalfRelative = Genealogy.IsHalfSibling(stepSibling, siblingOfAncestor);
                        if (isHalfRelative && !Tuning.kShowDistantHalfRelatives)
                        {
                            return "";
                        }
                        string greats = "";
                        for (int i = 1; i < Common.GetAncestorInfo(descendantOfSibling, stepSibling).GenerationalDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                        }
                        return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedUpward" : "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
                    }
                    if (Tuning.kAccumulateDistantStepRelatives && descendantOfSibling.IsStepAncestor(stepSibling))
                    {
                        string greats = "";
                        for (int i = 1; i < Common.GetStepAncestorInfo(descendantOfSibling, stepSibling).GenerationalDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                        }
                        return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
                    }
                }
                foreach (Genealogy sibling in siblingOfAncestor.Siblings)
                {
                    if (descendantOfSibling.IsAncestor(sibling))
                    {
                        bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                        if (isHalfRelative && !Tuning.kShowDistantHalfRelatives)
                        {
                            return "";
                        }
                        string greats = "";
                        for (int i = 1; i < Common.GetStepAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                        }
                        return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedUpward" : "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
                    }
                }
                return "";
            }
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowDistantHalfRelatives)
                    {
                        return "";
                    }
                    string greats = "";
                    for (int i = 1; i < Common.GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedUpward" : "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Grand"), "");
                }
            }
            return "";
        }
    }
}