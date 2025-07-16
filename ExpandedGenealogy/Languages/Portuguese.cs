using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang.ExpandedGenealogy
{
    public class POR : PlayerLanguage
    {
        public override bool HasNthUncles
        {
            get
            {
                return true;
            }
        }

        public override string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            string generationalDistance = descendant.GetAncestorInfo(ancestor).GenerationalDistance.ToString();
            if (generationalDistance == "3")
            {
                return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGP", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            }
            else if (int.Parse(generationalDistance) > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            }
            return "";
        }

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            foreach (GenealogyPlaceholder sibling in siblingOfAncestor.GetGenealogyPlaceholder().Siblings)
            {
                if (descendantOfSibling.GetGenealogyPlaceholder().IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling.Genealogy, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowHalfRelatives)
                    {
                        return "";
                    }
                    string descendantTitle, generationalDistance = descendantOfSibling.GetGenealogyPlaceholder().GetAncestorInfo(sibling).GenerationalDistance.ToString();
                    switch (generationalDistance)
                    {
                        case "1":
                            descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandchild");
                            break;
                        case "2":
                            descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGC");
                            break;
                        case "3":
                            descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGC");
                            break;
                        default:
                            descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                            break;
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", descendantTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousinNxRemovedDownward" : "Destrospean/Genealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), descendantTitle.ToLower());
                }
            }
            return "";
        }

        public override string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string generationalDistance = descendant.GetAncestorInfo(ancestor).GenerationalDistance.ToString();
            if (generationalDistance == "3")
            {
                return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGC", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            }
            else if (int.Parse(generationalDistance) > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            }
            return "";
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
            string ancestorTitle = "", degree = (distantRelationInfo.Degree + 1).ToString(), greatNxUncleEntryKey = "Destrospean/Genealogy:GreatNx" + (isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Uncle" : "Nephew"), nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down");
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
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grand" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "parent" : "child"));
                                    break;
                                case 3:
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GG" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "P" : "C"));
                                    break;
                                case 4:
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGG" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "P" : "C"));
                                    break;
                                default:
                                    string generationalDistance = (distantRelationInfo.TimesRemoved - 1).ToString();
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrand" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "parent" : "child"), generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                                    break;
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), ancestorTitle.ToLower()).Replace("  ", " ");
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousin" : "Destrospean/Genealogy:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
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
                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grand" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "parent" : "child"));
                    break;
                case 3:
                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GG" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "P" : "C"));
                    break;
                case 4:
                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGG" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "P" : "C"));
                    break;
                default:
                    string generationalDistance = (distantRelationInfo.TimesRemoved - 1).ToString();
                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrand" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "parent" : "child"), generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                    break;
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), ancestorTitle.ToLower()).Replace("  ", " ");
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            foreach (GenealogyPlaceholder sibling in siblingOfAncestor.GetGenealogyPlaceholder().Siblings)
            {
                if (descendantOfSibling.GetGenealogyPlaceholder().IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling.Genealogy, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowHalfRelatives)
                    {
                        return "";
                    }
                    string ancestorTitle, generationalDistance = descendantOfSibling.GetGenealogyPlaceholder().GetAncestorInfo(sibling).GenerationalDistance.ToString();
                    switch (generationalDistance)
                    {
                        case "1":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandparent");
                            break;
                        case "2":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGP");
                            break;
                        case "3":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGP");
                            break;
                        default:
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                            break;
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousinNxRemovedUpward" : "Destrospean/Genealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), ancestorTitle.ToLower());
                }
            }
            return "";
        }
    }
}