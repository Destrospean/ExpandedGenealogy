using Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.UI.CAS;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang.ExpandedGenealogy
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

        public string GetAncestorString(Genealogy ancestor, Genealogy descendant)
        {
            return GetAncestorString((ancestor.SimDescription ?? ancestor.mMiniSim).IsFemale, ancestor, descendant, false);
        }

        public virtual string GetAncestorString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string greats = "";
            for (int i = 1; i < descendant.GetAncestorInfo(ancestor).GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
            }
            return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrandparent", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
        }

        public virtual string GetDescendantOfSiblingString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string greats = "";
            for (int i = 1; i < siblingOfAncestorInfo.GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
            }
            return Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:GreatNxHalfNephew" : "Destrospean/ExpandedGenealogy/RelationNames:GreatNxNephew", greats);
        }

        public string GetDescendantString(Genealogy descendant, Genealogy ancestor)
        {
            return GetDescendantString((descendant.SimDescription ?? descendant.mMiniSim).IsFemale, descendant, ancestor, false);
        }

        public virtual string GetDescendantString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            string greats = "";
            for (int i = 1; i < descendant.GetAncestorInfo(ancestor).GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
            }
            return Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:GreatNxGrandchild", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:InLaw") : "");
        }

        public abstract string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo);

        public string GetDistantRelationString(Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            return GetDistantRelationString((sim.SimDescription ?? sim.mMiniSim).IsFemale, sim, distantRelationInfo);
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

        public virtual string GetSiblingOfAncestorString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string greats = "";
            for (int i = 1; i < siblingOfAncestorInfo.GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/ExpandedGenealogy/RelationNames:Great");
            }
            return Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/ExpandedGenealogy/RelationNames:GreatNxHalfUncle" : "Destrospean/ExpandedGenealogy/RelationNames:GreatNxUncle", greats);
        }

        public bool TryGetDescendantOfSiblingString(SimDescription descendantOfSibling, SimDescription siblingOfAncestor, out string result)
        {
            string text = GetDescendantOfSiblingString(descendantOfSibling.IsFemale, descendantOfSibling.Genealogy, siblingOfAncestor.Genealogy);
            if (string.IsNullOrEmpty(text) && siblingOfAncestor.Genealogy.Spouse != null && siblingOfAncestor.Genealogy.Spouse != descendantOfSibling.Genealogy && siblingOfAncestor.Genealogy.PartnerType == PartnerType.Marriage)
            {
                text = GetDescendantOfSiblingString(descendantOfSibling.IsFemale, descendantOfSibling.Genealogy, siblingOfAncestor.Genealogy.Spouse);
            }
            result = text;
            return !string.IsNullOrEmpty(result);
        }

        public bool TryGetDistantRelationString(SimDescription simWithRelationName, SimDescription simToGetRelationTo, out string result)
        {
            /* DistantRelationInfo distantRelationInfo = simWithRelationName.Genealogy.CalculateDistantRelation(simToGetRelationTo.Genealogy);
             * if (distantRelationInfo == null)
             * {    
             *     if (simWithRelationName.Genealogy.Spouse != null && simWithRelationName.Genealogy.Spouse != simToGetRelationTo.Genealogy && simWithRelationName.Genealogy.PartnerType == PartnerType.Marriage)
             *     {
             *         distantRelationInfo = simWithRelationName.Genealogy.Spouse.CalculateDistantRelation(simToGetRelationTo.Genealogy);
             *     }
             *     if (distantRelationInfo == null)
             *     {
             *         if (simToGetRelationTo.Genealogy.Spouse != null && simToGetRelationTo.Genealogy.Spouse != simWithRelationName.Genealogy && simToGetRelationTo.Genealogy.PartnerType == PartnerType.Marriage)
             *         {
             *             distantRelationInfo = simWithRelationName.Genealogy.CalculateDistantRelation(simToGetRelationTo.Genealogy.Spouse);
             *         }
             *         if (distantRelationInfo != null)
             *         {
             *             text = GetDistantRelationString(simWithRelationName.Genealogy, distantRelationInfo);
             *         }
             *     }
             *     else
             *     {
             *         text = GetDistantRelationString(simWithRelationName.IsFemale, simWithRelationName.Genealogy.Spouse, distantRelationInfo);
             *     }
             * }
             * else
             * {
             *     text = GetDistantRelationString(simWithRelationName.Genealogy, distantRelationInfo);
             * }
             */
            result = GetDistantRelationString(simWithRelationName.Genealogy, simWithRelationName.Genealogy.GetDistantRelationInfo(simToGetRelationTo.Genealogy));
            return !string.IsNullOrEmpty(result);
        }

        public bool TryGetSiblingOfAncestorString(SimDescription siblingOfAncestor, SimDescription descendantOfSibling, out string result)
        {
            string text = GetSiblingOfAncestorString(siblingOfAncestor.IsFemale, siblingOfAncestor.Genealogy, descendantOfSibling.Genealogy);
            if (string.IsNullOrEmpty(text) && siblingOfAncestor.Genealogy.Spouse != null && descendantOfSibling.Genealogy != siblingOfAncestor.Genealogy.Spouse && siblingOfAncestor.Genealogy.PartnerType == PartnerType.Marriage)
            {
                text = GetSiblingOfAncestorString(siblingOfAncestor.IsFemale, siblingOfAncestor.Genealogy.Spouse, descendantOfSibling.Genealogy);
            }
            result = text;
            return !string.IsNullOrEmpty(result);
        }
    }
}