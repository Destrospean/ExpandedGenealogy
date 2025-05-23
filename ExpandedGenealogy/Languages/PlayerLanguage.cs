using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using static Destrospean.Common;
using static Sims3.Gameplay.Destrospean.ExpandedGenealogy;

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

        public string GetAncestorString(Genealogy descendant, Genealogy ancestor)
        {
            return GetAncestorString(ancestor.SimDescription.IsFemale, descendant, ancestor, false);
        }

        public virtual string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            string greats = "";
            for (int i = 1; i < GetAncestorInfo(descendant, ancestor).AncestorDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            string text = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        public virtual string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
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
                    string greats = "";
                    for (int i = 1; i < GetAncestorInfo(descendantOfSibling, sibling).AncestorDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                    }
                    string text = Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", greats);
                    return text.Substring(0, 1).ToUpper() + text.Substring(1);
                }
            }
            return "";
        }

        public string GetDescendantString(Genealogy ancestor, Genealogy descendant)
        {
            return GetDescendantString(descendant.SimDescription.IsFemale, ancestor, descendant, false);
        }

        public virtual string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string greats = "";
            for (int i = 1; i < GetAncestorInfo(descendant, ancestor).AncestorDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            string text = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        public abstract string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo);

        public string GetDistantRelationString(Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            return GetDistantRelationString(sim.SimDescription.IsFemale, sim, distantRelationInfo);
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

        public virtual string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
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
                    string greats = "";
                    for (int i = 1; i < GetAncestorInfo(descendantOfSibling, sibling).AncestorDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                    }
                    string text = Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", greats);
                    return text.Substring(0, 1).ToUpper() + text.Substring(1);
                }
            }
            return "";
        }
    }
}