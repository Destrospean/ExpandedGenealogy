using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
    public abstract class PlayerLanguage
    {
        public virtual bool HasNthUncles
        {
            get
            {
                return false;
            }
        }

        public virtual bool InLawsAndStepRelativesShareTerminology
        {
            get
            {
                return false;
            }
        }

        public string GetAncestorString(Genealogy descendant, Genealogy ancestor, bool isInLaw = false, bool isStepRelative = false)
        {
            return GetAncestorString(ancestor.SimDescription.IsFemale, descendant, ancestor, isInLaw, isStepRelative);
        }

        public virtual string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw = false, bool isStepRelative = false)
        {
            string greats = "";
            for (int i = 1; i < (isStepRelative ? Common.GetStepAncestorInfo(descendant, ancestor) : Common.GetAncestorInfo(descendant, ancestor)).GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
            }
            return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandparent", greats, isInLaw || (isStepRelative && InLawsAndStepRelativesShareTerminology) ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "", isStepRelative && !InLawsAndStepRelativesShareTerminology ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
        }

        public virtual string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling, bool isStepRelative = false)
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
                        return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxNephew", greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
                    }
                    if (Tuning.kAccumulateDistantStepRelatives && descendantOfSibling.IsStepAncestor(stepSibling))
                    {
                        string greats = "";
                        for (int i = 1; i < Common.GetStepAncestorInfo(descendantOfSibling, stepSibling).GenerationalDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                        }
                        return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxNephew", greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
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
                        return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy:GreatNxNephew", greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
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
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy:GreatNxNephew", greats, "");
                }
            }
            return "";
        }

        public string GetDescendantString(Genealogy ancestor, Genealogy descendant, bool isInLaw = false, bool isStepRelative = false)
        {
            return GetDescendantString(descendant.SimDescription.IsFemale, ancestor, descendant, isInLaw, isStepRelative);
        }

        public virtual string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw = false, bool isStepRelative = false)
        {
            string greats = "";
            for (int i = 1; i < (isStepRelative ? Common.GetStepAncestorInfo(descendant, ancestor) : Common.GetAncestorInfo(descendant, ancestor)).GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
            }
            return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxGrandchild", greats, isInLaw || (isStepRelative && InLawsAndStepRelativesShareTerminology) ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:InLaw") : "", isStepRelative && !InLawsAndStepRelativesShareTerminology ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step") : "");
        }

        public abstract string GetDistantRelationString(bool isFemale, Genealogy sim, Common.DistantRelationInfo distantRelationInfo, bool isStepRelative = false);

        public string GetDistantRelationString(Genealogy sim, Common.DistantRelationInfo distantRelationInfo, bool isStepRelative = false)
        {
            return GetDistantRelationString(sim.SimDescription.IsFemale, sim, distantRelationInfo, isStepRelative);
        }

        public virtual string GetOrdinalSuffix(string number)
        {
            if (number.Length > 1)
            {
                switch (number.Substring(number.Length - 2))
                {
                    case "11":
                        return "11";
                    case "12":
                        return "12";
                }
            }
            return number.Substring(number.Length - 1);
        }

        public virtual string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor, bool isStepRelative = false)
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
                        return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxUncle", greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
                    }
                    if (Tuning.kAccumulateDistantStepRelatives && descendantOfSibling.IsStepAncestor(stepSibling))
                    {
                        string greats = "";
                        for (int i = 1; i < Common.GetStepAncestorInfo(descendantOfSibling, stepSibling).GenerationalDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Great");
                        }
                        return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:GreatNxUncle", greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
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
                        return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy:GreatNxUncle", greats, Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy:Step"));
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
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowDistantHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy:GreatNxUncle", greats, "");
                }
            }
            return "";
        }
    }
}