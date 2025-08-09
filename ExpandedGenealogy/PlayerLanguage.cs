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

        public string GetAncestorString(Genealogy descendant, Genealogy ancestor)
        {
            return GetAncestorString(ancestor.SimDescription.IsFemale, descendant, ancestor, false);
        }

        public virtual string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            string greats = "";
            for (int i = 1; i < descendant.GetAncestorInfo(ancestor).GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
        }

        public virtual string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string greats = "";
            for (int i = 1; i < siblingOfAncestorInfo.GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            return Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", greats);
        }

        public string GetDescendantString(Genealogy ancestor, Genealogy descendant)
        {
            return GetDescendantString(descendant.SimDescription.IsFemale, ancestor, descendant, false);
        }

        public virtual string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string greats = "";
            for (int i = 1; i < descendant.GetAncestorInfo(ancestor).GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
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
            SiblingOfAncestorInfo siblingOfAncestorInfo = descendantOfSibling.GetSiblingOfAncestorInfo(siblingOfAncestor);
            if (siblingOfAncestorInfo == null)
            {
                return "";
            }
            string greats = "";
            for (int i = 1; i < siblingOfAncestorInfo.GenerationalDistance; i++)
            {
                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
            }
            return Localization.LocalizeString(isFemale, siblingOfAncestorInfo.IsHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", greats);
        }

        public bool TryGetDistantRelationString(SimDescription sim1, SimDescription sim2, out string result)
        {
            string text = "";
            // Get a relation name that is valid if Sim 2 is a sibling of one of Sim 1's ancestors
            text = GetSiblingOfAncestorString(sim2.IsFemale, sim1.Genealogy, sim2.Genealogy);
            if (string.IsNullOrEmpty(text) && sim2.Genealogy.Spouse != null && sim1.Genealogy != sim2.Genealogy.Spouse && sim2.Genealogy.PartnerType == PartnerType.Marriage)
            {
                // Get a relation name that is valid if Sim 2 is married to a sibling of one of Sim 1's ancestors
                text = GetSiblingOfAncestorString(sim2.IsFemale, sim1.Genealogy, sim2.Genealogy.Spouse);
            }
            if (string.IsNullOrEmpty(text))
            {
                // Get a relation name that is valid if Sim 1 is a sibling of one of Sim 2's ancestors
                text = GetDescendantOfSiblingString(sim2.IsFemale, sim1.Genealogy, sim2.Genealogy);
            }
            if (string.IsNullOrEmpty(text) && sim1.Genealogy.Spouse != null && sim1.Genealogy.Spouse != sim2.Genealogy && sim1.Genealogy.PartnerType == PartnerType.Marriage)
            {
                // Get a relation name that is valid if Sim 1 is married to a sibling of one of Sim 2's ancestors
                text = GetDescendantOfSiblingString(sim2.IsFemale, sim1.Genealogy.Spouse, sim2.Genealogy);
            }
            if (string.IsNullOrEmpty(text))
            {
                text = GetDistantRelationString(sim2.Genealogy, sim2.Genealogy.GetGenealogyPlaceholder().CalculateDistantRelation(sim1.Genealogy.GetGenealogyPlaceholder()));
                /* DistantRelationInfo distantRelationInfo = sim2.Genealogy.GetGenealogyPlaceholder().CalculateDistantRelation(sim1.Genealogy.GetGenealogyPlaceholder());
                 * if (distantRelationInfo == null)
                 * {    
                 *     // Check if Sim 2 is married to someone other than Sim 1
                 *     if (sim2.Genealogy.Spouse != null && sim2.Genealogy.Spouse != sim1.Genealogy && sim2.Genealogy.PartnerType == PartnerType.Marriage)
                 *     {
                 *         distantRelationInfo = sim2.Genealogy.Spouse.GetGenealogyPlaceholder().CalculateDistantRelation(sim1.Genealogy.GetGenealogyPlaceholder());
                 *     }
                 *     if (distantRelationInfo == null)
                 *     {
                 *         // Check if Sim 1 is married to someone other than Sim 2
                 *         if (sim1.Genealogy.Spouse != null && sim1.Genealogy.Spouse != sim2.Genealogy && sim1.Genealogy.PartnerType == PartnerType.Marriage)
                 *         {
                 *             distantRelationInfo = sim2.Genealogy.GetGenealogyPlaceholder().CalculateDistantRelation(sim1.Genealogy.Spouse.GetGenealogyPlaceholder());
                 *         }
                 *         // Check if Sim 1 is married to a sibling of one of Sim 2's ancestors
                 *         if (distantRelationInfo != null && distantRelationInfo.Degree == 0 && distantRelationInfo.ClosestDescendant.Genealogy == sim1.Genealogy.Spouse)
                 *         {
                 *             text = GetDistantRelationString(sim2.Genealogy, distantRelationInfo);
                 *         }
                 *     }
                 *     // Check if Sim 2 is married a cousin of Sim 1
                 *     else if (distantRelationInfo.ClosestDescendant.Genealogy == sim2.Genealogy.Spouse)
                 *     {
                 *         text = GetDistantRelationString(sim2.IsFemale, sim2.Genealogy.Spouse, distantRelationInfo);
                 *     }
                 * }
                 * else
                 * {
                 *     // Get a relation name that is valid if Sim 2 is a collateral descendant or type of cousin of Sim 1 or if Sim 1 is a collateral descendant of Sim 2
                 *     text = GetDistantRelationString(sim2.Genealogy, distantRelationInfo);
                 * }
                 */
            }
            result = text;
            return !string.IsNullOrEmpty(result);
        }
    }
}