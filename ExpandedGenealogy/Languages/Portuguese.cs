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

        public override string GetAncestorString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string generationalDistance = descendant.GetAncestorInfo(ancestor).GenerationalDistance.ToString();
            if (generationalDistance == "3")
            {
                return Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GGGP", isInLaw ? Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:InLaw") : "");
            }
            else if (int.Parse(generationalDistance) > 3)
            {
                return Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GreatNxGrandparent", generationalDistance, Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)), isInLaw ? Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:InLaw") : "");
            }
            return "";
        }

        public override string GetDescendantOfSiblingString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string descendantTitle, generationalDistance = siblingOfAncestorInfo.GenerationalDistance.ToString();
            switch (generationalDistance)
            {
                case "1":
                    descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandchild");
                    break;
                case "2":
                    descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGC");
                    break;
                case "3":
                    descendantTitle = Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GGGC");
                    break;
                default:
                    descendantTitle = Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GreatNxGrandchild", generationalDistance, Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                    break;
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? Common.kLocalizationPath + "/RelationNames:GreatNxHalfNephew" : Common.kLocalizationPath + "/RelationNames:GreatNxNephew", descendantTitle.ToLower()) : Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? Common.kLocalizationPath + "/RelationNames:NthHalfCousinNxRemovedDownward" : Common.kLocalizationPath + "/RelationNames:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixNoun1"), descendantTitle.ToLower());
        }

        public override string GetDescendantString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            string generationalDistance = descendant.GetAncestorInfo(ancestor).GenerationalDistance.ToString();
            if (generationalDistance == "3")
            {
                return Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GGGC", isInLaw ? Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:InLaw") : "");
            }
            else if (int.Parse(generationalDistance) > 3)
            {
                return Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GreatNxGrandchild", generationalDistance, Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)), isInLaw ? Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:InLaw") : "");
            }
            return "";
        }

        public override string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null || distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelatives)
            {
                return "";
            }
            string ancestorTitle = "", degree = (distantRelationInfo.Degree + 1).ToString();
            if (distantRelationInfo.Degree > 0 && distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow)
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
                                ancestorTitle = Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GGG" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "P" : "C"));
                                break;
                            default:
                                string generationalDistance = (distantRelationInfo.TimesRemoved - 1).ToString();
                                ancestorTitle = Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GreatNxGrand" + (distantRelationInfo.ClosestDescendant.Genealogy == sim ? "parent" : "child"), generationalDistance, Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                                break;
                        }
                        return Localization.LocalizeString(isFemale, string.Format(Common.kLocalizationPath + "/RelationNames:Nth{0}CousinNxRemoved{1}ward", distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant.Genealogy == sim ? "Up" : "Down"), degree, Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), ancestorTitle.ToLower()).Replace("  ", " ").Replace("- ", " ");
                    }
                    return "";
                }
                return Localization.LocalizeString(isFemale, distantRelationInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? Common.kLocalizationPath + "/RelationNames:NthHalfCousin" : Common.kLocalizationPath + "/RelationNames:NthCousin", distantRelationInfo.Degree.ToString(), Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixNoun" + GetOrdinalSuffix(distantRelationInfo.Degree.ToString())));
            }
            return "";
        }

        public override string GetSiblingOfAncestorString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string ancestorTitle, generationalDistance = siblingOfAncestorInfo.GenerationalDistance.ToString();
            switch (generationalDistance)
            {
                case "1":
                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandparent");
                    break;
                case "2":
                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGP");
                    break;
                case "3":
                    ancestorTitle = Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GGGP");
                    break;
                default:
                    ancestorTitle = Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:GreatNxGrandparent", generationalDistance, Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixAdj" + GetOrdinalSuffix(generationalDistance)));
                    break;
            }
            return Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? Common.kLocalizationPath + "/RelationNames:GreatNxHalfUncle" : Common.kLocalizationPath + "/RelationNames:GreatNxUncle", ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? Common.kLocalizationPath + "/RelationNames:NthHalfCousinNxRemovedUpward" : Common.kLocalizationPath + "/RelationNames:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, Common.kLocalizationPath + "/RelationNames:OrdinalSuffixNoun1"), ancestorTitle.ToLower());
        }
    }
}