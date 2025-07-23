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
            foreach (GenealogyPlaceholder sibling in siblingOfAncestor.GetGenealogyPlaceholder().Siblings)
            {
                if (descendantOfSibling.GetGenealogyPlaceholder().IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling.Genealogy, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowHalfRelatives)
                    {
                        return "";
                    }
                    string greats = "";
                    for (int i = 1; i < descendantOfSibling.GetGenealogyPlaceholder().GetAncestorInfo(sibling).GenerationalDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", greats);
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
            foreach (GenealogyPlaceholder sibling in siblingOfAncestor.GetGenealogyPlaceholder().Siblings)
            {
                if (descendantOfSibling.GetGenealogyPlaceholder().IsAncestor(sibling))
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling.Genealogy, siblingOfAncestor);
                    if (isHalfRelative && !Tuning.kShowHalfRelatives)
                    {
                        return "";
                    }
                    string greats = "";
                    for (int i = 1; i < descendantOfSibling.GetAncestorInfo(sibling.Genealogy).GenerationalDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", greats);
                }
            }
            return "";
        }

        public bool TryGetDistantRelationString(SimDescription sim1, SimDescription sim2, out string result)
        {
            string text = "";
            DistantRelationInfo distantRelationInfo = sim2.Genealogy.GetGenealogyPlaceholder().CalculateDistantRelation(sim1.Genealogy.GetGenealogyPlaceholder());
            if (distantRelationInfo == null)
            {
                // Check if the target is married to someone other than the selected Sim
                if (sim2.Genealogy.Spouse != null && sim2.Genealogy.Spouse != sim1.Genealogy && sim2.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    distantRelationInfo = sim2.Genealogy.Spouse.GetGenealogyPlaceholder().CalculateDistantRelation(sim1.Genealogy.GetGenealogyPlaceholder());
                }
                if (distantRelationInfo == null)
                {
                    // Check if the selected Sim is married to someone other than the target
                    if (sim1.Genealogy.Spouse != null && sim1.Genealogy.Spouse != sim2.Genealogy && sim1.Genealogy.PartnerType == PartnerType.Marriage)
                    {
                        distantRelationInfo = sim2.Genealogy.GetGenealogyPlaceholder().CalculateDistantRelation(sim1.Genealogy.Spouse.GetGenealogyPlaceholder());
                    }
                    // Check if the selected Sim is married to a sibling of one of the target's ancestors
                    if (distantRelationInfo != null && distantRelationInfo.Degree == 0 && distantRelationInfo.ClosestDescendant.Genealogy == sim1.Genealogy.Spouse)
                    {
                        text = GetDistantRelationString(sim2.Genealogy, distantRelationInfo);
                    }
                }
                // Check if the target is married to a sibling of one of the selected Sim's ancestors
                else if (distantRelationInfo.Degree == 0 && distantRelationInfo.ClosestDescendant.Genealogy == sim2.Genealogy.Spouse)
                {
                    text = GetDistantRelationString(sim2.IsFemale, sim2.Genealogy.Spouse, distantRelationInfo);
                }
            }
            else
            {
                /* Get a relation name that is valid if the target is a collateral descendant or type of cousin
                 * of the selected Sim or if the selected Sim is a collateral descendant of the target
                 */
                text = GetDistantRelationString(sim2.Genealogy, distantRelationInfo);
            }
            // This if-statement block is necessary for when the selected Sim and the target do not share an ancestor in the game but each has an ancestor who is a sibling of the other.
            if (distantRelationInfo == null)
            {
                // Get a relation name that is valid if the target is a sibling of one of the selected Sim's ancestors
                text = GetSiblingOfAncestorString(sim2.IsFemale, sim1.Genealogy, sim2.Genealogy);
                if (string.IsNullOrEmpty(text) && sim2.Genealogy.Spouse != null && sim1.Genealogy != sim2.Genealogy.Spouse && sim2.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    // Get a relation name that is valid if the target is married to a sibling of one of the selected Sim's ancestors
                    text = GetSiblingOfAncestorString(sim2.IsFemale, sim1.Genealogy, sim2.Genealogy.Spouse);
                }
                if (string.IsNullOrEmpty(text))
                {
                    // Get a relation name that is valid if the selected Sim is a sibling of one of the target's ancestors
                    text = GetDescendantOfSiblingString(sim2.IsFemale, sim1.Genealogy, sim2.Genealogy);
                }
                if (string.IsNullOrEmpty(text) && sim1.Genealogy.Spouse != null && sim1.Genealogy.Spouse != sim2.Genealogy && sim1.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    // Get a relation name that is valid if the selected Sim is married to a sibling of one of the target's ancestors
                    text = GetDescendantOfSiblingString(sim2.IsFemale, sim1.Genealogy.Spouse, sim2.Genealogy);
                }
            }
            result = text;
            return !string.IsNullOrEmpty(result);
        }
    }
}