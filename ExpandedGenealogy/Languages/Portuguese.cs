using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
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
            string generationalDistance = Common.GetAncestorInfo(descendant, ancestor).GenerationalDistance.ToString();
            if (generationalDistance == "3")
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGGP", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
            }
            else if (int.Parse(generationalDistance) > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrandparent", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
            }
            return "";
        }

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowHalfRelatives)
                    {
                        return "";
                    }
                    string descendantTitle, generationalDistance = Common.GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance.ToString();
                    switch (generationalDistance)
                    {
                        case "1":
                            descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandchild");
                            break;
                        case "2":
                            descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGC");
                            break;
                        case "3":
                            descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGGC");
                            break;
                        default:
                            descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrandchild", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                            break;
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy/RelationNames:GreatNxNephew", descendantTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousinNxRemovedDownward" : "Destrospean/ExpandedGenealogy/RelationNames:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), descendantTitle.ToLower());
                }
            }
            return "";
        }

        public override string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string generationalDistance = Common.GetAncestorInfo(descendant, ancestor).GenerationalDistance.ToString();
            if (generationalDistance == "3")
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGGC", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
            }
            else if (int.Parse(generationalDistance) > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrandchild", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
            }
            return "";
        }

        public override string GetDistantRelationString(bool isFemale, Genealogy sim, Common.DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null)
            {
                return "";
            }
            bool isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0], distantRelationInfo.ThroughWhichChildren[1]);
            if (isHalfRelative && !Tuning.kShowHalfRelatives)
            {
                return "";
            }
            string ancestorTitle = "", degree = (distantRelationInfo.Degree + 1).ToString(), greatNxUncleEntryKey = "Destrospean/ExpandedGenealogy/RelationNames:GreatNx" + (isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), nthCousinNxRemovedEntryKey = string.Format("Destrospean/ExpandedGenealogy/RelationNames:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down");
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
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                                    break;
                                default:
                                    string generationalDistance = (distantRelationInfo.TimesRemoved - 1).ToString();
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"), generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                                    break;
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), ancestorTitle.ToLower()).Replace("  ", " ").Replace("- ", " ");
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousin" : "Destrospean/ExpandedGenealogy/RelationNames:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
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
                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                    break;
                default:
                    string generationalDistance = (distantRelationInfo.TimesRemoved - 1).ToString();
                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"), generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                    break;
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), ancestorTitle.ToLower()).Replace("  ", " ");
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowHalfRelatives)
                    {
                        return "";
                    }
                    string ancestorTitle, generationalDistance = Common.GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance.ToString();
                    switch (generationalDistance)
                    {
                        case "1":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandparent");
                            break;
                        case "2":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGP");
                            break;
                        case "3":
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGGP");
                            break;
                        default:
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrandparent", generationalDistance, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                            break;
                    }
                    return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy/RelationNames:GreatNxUncle", ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousinNxRemovedUpward" : "Destrospean/ExpandedGenealogy/RelationNames:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), ancestorTitle.ToLower());
                }
            }
            return "";
        }
    }
}