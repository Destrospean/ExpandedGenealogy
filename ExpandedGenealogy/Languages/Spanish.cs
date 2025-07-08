using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
    public class SPA : PlayerLanguage
    {
        public override bool HasNthUncles
        {
            get
            {
                return true;
            }
        }
        
        public override string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw = false, bool isStepRelative = false)
        {
            string generationalDistance = (isStepRelative ? Common.GetStepAncestorInfo(descendant, ancestor) : Common.GetAncestorInfo(descendant, ancestor)).GenerationalDistance.ToString();
            if (generationalDistance == "3")
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GGGP", isInLaw || (isStepRelative && InLawsAndStepRelativesShareTerminology) ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "", isStepRelative && !InLawsAndStepRelativesShareTerminology ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
            }
            else if (int.Parse(generationalDistance) > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandparent", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)), isInLaw || (isStepRelative && InLawsAndStepRelativesShareTerminology) ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "", isStepRelative && !InLawsAndStepRelativesShareTerminology ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
            }
            return "";
        }

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling, bool isStepRelative = false)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling) || (isStepRelative && descendantOfSibling.IsStepAncestor(sibling)))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowDistantHalfRelatives)
                    {
                        return "";
                    }
                    string descendantTitle, generationalDistance = (isStepRelative ? Common.GetStepAncestorInfo(descendantOfSibling, sibling) : Common.GetAncestorInfo(descendantOfSibling, sibling)).GenerationalDistance.ToString();
                    switch (generationalDistance)
                    {
                        case "1":
                            descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandchild");
                            break;
                        case "2":
                            descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGC");
                            break;
                        case "3":
                            descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GGGC");
                            break;
                        default:
                            descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandchild", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                            break;
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy:GreatNxNephew", descendantTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:NthHalfCousinNxRemovedDownward" : "Destrospean/ExpandedGenealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), descendantTitle.ToLower(), isStepRelative ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
                }
            }
            return "";
        }

        public override string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw = false, bool isStepRelative = false)
        {
            string generationalDistance = (isStepRelative ? Common.GetStepAncestorInfo(descendant, ancestor) : Common.GetAncestorInfo(descendant, ancestor)).GenerationalDistance.ToString();
            if (generationalDistance == "3")
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GGGC", isInLaw || (isStepRelative && InLawsAndStepRelativesShareTerminology) ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "", isStepRelative && !InLawsAndStepRelativesShareTerminology ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
            }
            else if (int.Parse(generationalDistance) > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandchild", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)), isInLaw || (isStepRelative && InLawsAndStepRelativesShareTerminology) ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "", isStepRelative && !InLawsAndStepRelativesShareTerminology ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
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
            string ancestorTitle = "", degree = (distantRelationInfo.Degree + 1).ToString(), greatNxUncleEntryKey = "Destrospean/ExpandedGenealogy:GreatNx" + (isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), nthCousinNxRemovedEntryKey = string.Format("Destrospean/ExpandedGenealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
                {
                    if (distantRelationInfo.TimesRemoved > 0)
                    {
                        if (distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
                        {
                            switch (distantRelationInfo.TimesRemoved)
                            {
                                case 1:
                                    break;
                                case 2:
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"));
                                    break;
                                case 3:
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                                    break;
                                case 4:
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GGG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                                    break;
                                default:
                                    string generationalDistance = (distantRelationInfo.TimesRemoved - 1).ToString();
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"), generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                                    break;
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), ancestorTitle.ToLower()).Replace("  ", " ");
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:NthHalfCousin" : "Destrospean/ExpandedGenealogy:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
                }
                return "";
            }
            if (distantRelationInfo.TimesRemoved < 1)
            {
                return "";
            }
            switch (distantRelationInfo.TimesRemoved)
            {
                case 1:
                    break;
                case 2:
                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"));
                    break;
                case 3:
                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                    break;
                case 4:
                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GGG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                    break;
                default:
                    string generationalDistance = (distantRelationInfo.TimesRemoved - 1).ToString();
                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"), generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                    break;
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), ancestorTitle.ToLower()).Replace("  ", " ");
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor, bool isStepRelative = false)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling) || (isStepRelative && descendantOfSibling.IsStepAncestor(sibling)))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowDistantHalfRelatives)
                    {
                        return "";
                    }
                    string ancestorTitle, generationalDistance = (isStepRelative ? Common.GetStepAncestorInfo(descendantOfSibling, sibling) : Common.GetAncestorInfo(descendantOfSibling, sibling)).GenerationalDistance.ToString();
                    switch (generationalDistance)
                    {
                        case "1":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandparent");
                            break;
                        case "2":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGP");
                            break;
                        case "3":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GGGP");
                            break;
                        default:
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandparent", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                            break;
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy:GreatNxUncle", ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:NthHalfCousinNxRemovedUpward" : "Destrospean/ExpandedGenealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), ancestorTitle.ToLower(), isStepRelative ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
                }
            }
            return "";
        }
    }
}