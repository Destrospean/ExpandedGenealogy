using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using static Destrospean.Common;
using static Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
    public class SPA : PlayerLanguage
    {
        public override bool HasNthUncles => true;

        public override string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            string nthGrandparent = GetAncestorInfo(descendant, ancestor).AncestorDistance.ToString();
            if (nthGrandparent == "3")
            {
                return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGP", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            }
            else if (int.Parse(nthGrandparent) > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", nthGrandparent, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandparent)), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
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
                    if (isHalfRelative && !kShowHalfRelatives)
                    {
                        return "";
                    }
                    string descendantTitle, nthGrandchild = GetAncestorInfo(descendantOfSibling, sibling).AncestorDistance.ToString();
                    switch (nthGrandchild)
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
                            descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", nthGrandchild, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandchild)));
                            break;
                    }
                    return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", descendantTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousinNxRemovedDownward" : "Destrospean/Genealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), descendantTitle.ToLower());
                }
            }
            return "";
        }

        public override string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string nthGrandchild = GetAncestorInfo(descendant, ancestor).AncestorDistance.ToString();
            if (nthGrandchild == "3")
            {
                return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGC", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            }
            else if (int.Parse(nthGrandchild) > 3)
            {
                return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", nthGrandchild, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandchild)), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            }
            return "";
        }

        public override string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null)
            {
                return "";
            }
            bool isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0], distantRelationInfo.ThroughWhichChildren[1]);
            if (isHalfRelative && !kShowHalfRelatives)
            {
                return "";
            }
            string ancestorTitle = "", degree = (distantRelationInfo.Degree + 1).ToString(), greatNxUncleEntryKey = "Destrospean/Genealogy:GreatNx" + (isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)kMaxDegreeCousinsToShow)
                {
                    if (distantRelationInfo.TimesRemoved > 0)
                    {
                        if (distantRelationInfo.TimesRemoved <= (uint)kMaxTimesRemovedCousinsToShow)
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
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                                    break;
                                default:
                                    string nthGrandparent = (distantRelationInfo.TimesRemoved - 1).ToString();
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"), nthGrandparent, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandparent)));
                                    break;
                            }
                            return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), ancestorTitle.ToLower()).Replace("  ", " ");
                        }
                        return "";
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousin" : "Destrospean/Genealogy:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
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
                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                    break;
                default:
                    string nthGrandparent = (distantRelationInfo.TimesRemoved - 1).ToString();
                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"), nthGrandparent, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandparent)));
                    break;
            }
            return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), ancestorTitle.ToLower()).Replace("  ", " ");
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !kShowHalfRelatives)
                    {
                        return "";
                    }
                    string ancestorTitle, nthGrandparent = GetAncestorInfo(descendantOfSibling, sibling).AncestorDistance.ToString();
                    switch (nthGrandparent)
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
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", nthGrandparent, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandparent)));
                            break;
                    }
                    return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousinNxRemovedUpward" : "Destrospean/Genealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), ancestorTitle.ToLower());
                }
            }
            return "";
        }
    }
}