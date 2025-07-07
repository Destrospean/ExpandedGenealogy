using Destrospean.Lang;
using MonoPatcherLib;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean
{
    [Plugin]
    public class ExpandedGenealogy
    {
        public static bool IsHalfCousin(Genealogy sim1, Genealogy sim2)
        {
            foreach (Genealogy parent1 in sim1.Parents)
            {
                foreach (Genealogy parent2 in sim2.Parents)
                {
                    if (Genealogy.IsHalfSibling(parent1, parent2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsHalfNephew(Genealogy nephew, Genealogy uncle)
        {
            return IsHalfUncle(uncle, nephew);
        }

        public static bool IsHalfSiblingInLaw(Genealogy sim1, Genealogy sim2)
        {
            if (sim1.Spouse != null && sim1.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy sibling in sim1.Spouse.Siblings)
                {
                    if (sibling == sim2 || (sibling.Spouse == sim2 && sibling.PartnerType == PartnerType.Marriage))
                    {
                        return Genealogy.IsHalfSibling(sim1.Spouse, sibling);
                    }
                }
            }
            if (sim2.Spouse != null && sim2.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy sibling in sim2.Spouse.Siblings)
                {
                    if (sibling == sim1 || (sibling.Spouse == sim1 && sibling.PartnerType == PartnerType.Marriage))
                    {
                        return Genealogy.IsHalfSibling(sim2.Spouse, sibling);
                    }
                }
            }
            return false;
        }

        public static bool IsHalfUncle(Genealogy uncle, Genealogy nephew)
        {
            foreach (Genealogy parent in nephew.Parents)
            {
                foreach (Genealogy sibling in parent.Siblings)
                {
                    if (sibling == uncle || (sibling.Spouse == uncle && sibling.PartnerType == PartnerType.Marriage))
                    {
                        return Genealogy.IsHalfSibling(parent, sibling);
                    }
                }
            }
            return false;
        }

        [TypePatch(typeof(Genealogy))]
        public class GenealogyPatch
        {
            public static bool IsSiblingInLaw(Genealogy sim1, Genealogy sim2)
            {
                if (sim1.Spouse != null && sim1.PartnerType == PartnerType.Marriage)
                {
                    foreach (Genealogy sibling in sim1.Spouse.Siblings)
                    {
                        if (sibling == sim2 || (sibling.Spouse == sim2 && sibling.PartnerType == PartnerType.Marriage))
                        {
                            return true;
                        }
                    }
                }
                if (sim2.Spouse != null && sim2.PartnerType == PartnerType.Marriage)
                {
                    foreach (Genealogy sibling in sim2.Spouse.Siblings)
                    {
                        if (sibling == sim1 || (sibling.Spouse == sim1 && sibling.PartnerType == PartnerType.Marriage))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            /* This method is the only method from the vanilla game still used by NRaas Woohooer to check if romance between two Sims is allowed, 
             * whereas other methods from the vanilla game are not referenced, and so replacing this specific method will allow the romance restrictions
             * from this mod to continue to work if NRaas Woohooer is installed.
             */
            public bool IsStepRelated(Genealogy other)
            {
                Genealogy self = (Genealogy)(this as object);
                Common.DistantRelationInfo distantRelationInfo = Common.CalculateDistantRelation(other, self);
                bool isHalfRelative = distantRelationInfo == null ? false : Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0], distantRelationInfo.ThroughWhichChildren[1]);
                /* Check if there is a distant relation, and if so, then check if the Sims are too closely related for romantic interactions depending on whether their degree of cousinage
                 * and/or the generational distance between them are below the minimums that determine that they are not, and if so, then check whether they are half-relatives,
                 * the latter of which matters depending on whether romantic interactions between half-relatives are allowed
                 */
                if (distantRelationInfo != null && distantRelationInfo.Degree < (uint)Tuning.kMinDegreeCousinsToAllowRomance && distantRelationInfo.TimesRemoved < (uint)Tuning.kMinTimesRemovedCousinsToAllowRomance && !(isHalfRelative && Tuning.kAllowRomanceForHalfRelatives))
                {
                    return true;
                }
                foreach (Genealogy parent1 in self.Parents)
                {
                    foreach (Genealogy parent2 in other.Parents)
                    {
                        if (parent1.Spouse == parent2 && parent1.PartnerType == PartnerType.Marriage)
                        {
                            return true;
                        }
                    }
                }
                foreach (Genealogy parent in self.Parents)
                {
                    if (parent.Spouse == other && parent.PartnerType == PartnerType.Marriage)
                    {
                        return true;
                    }
                }
                foreach (Genealogy parent in other.Parents)
                {
                    if (parent.Spouse == self && parent.PartnerType == PartnerType.Marriage)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static bool IsUncle(Genealogy uncle, Genealogy nephew)
            {
                foreach (Genealogy parent in nephew.Parents)
                {
                    foreach (Genealogy sibling in parent.Siblings)
                    {
                        if (sibling == uncle || (sibling.Spouse == uncle && sibling.PartnerType == PartnerType.Marriage))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        [TypePatch(typeof(SimDescription))]
        public class SimDescriptionPatch
        {
            public string GetMyFamilialDescriptionFor(SimDescription other)
            {
                SimDescription self = (SimDescription)(this as object);
                if (other.Genealogy == self.Genealogy)
                {
                    return "";
                }
                if (GameUtils.IsAnyTravelBasedWorld() && GameStates.TravelerIds != null && GameStates.TravelerIds.Contains(self.SimDescriptionId))
                {
                    MiniSimDescription miniSimDescription = MiniSimDescription.Find(self.SimDescriptionId);
                    if (miniSimDescription != null && miniSimDescription.MiniRelationships != null)
                    {
                        foreach (MiniRelationship miniRelationship in miniSimDescription.MiniRelationships)
                        {
                            if (miniRelationship.SimDescriptionId == other.SimDescriptionId)
                            {
                                return miniRelationship.FamilialString;
                            }
                        }
                    }
                }
                PlayerLanguage playerLanguage = Activator.CreateInstance(Type.GetType("Destrospean.Lang." + Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))) as PlayerLanguage;
                string text = "";
                if (Genealogy.IsParent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Parent");
                }
                else if (Genealogy.IsParentInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:ParentInLaw");
                }
                else if (Genealogy.IsChild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Child");
                }
                else if (Genealogy.IsChildInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:ChildInLaw");
                }
                else if (Genealogy.IsHalfSibling(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:HalfSibling");
                }
                else if (Genealogy.IsSibling(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Sibling");
                }
                else if (IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfSiblingInLaw");
                }
                else if (Genealogy.IsSiblingInLaw(other.Genealogy, self.Genealogy) && (!IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:SiblingInLaw");
                }
                else if (Genealogy.IsGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Grandparent");
                }
                else if (Genealogy.IsGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Grandchild");
                }
                else if (Genealogy.IsStepParent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:StepParent");
                }
                else if (Genealogy.IsStepChild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:StepChild");
                }
                else if (Genealogy.IsStepSibling(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:StepSibling");
                }
                else if (IsHalfCousin(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives && Tuning.kShow1stCousinsAsCousins)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfCousin");
                }
                else if (Genealogy.IsCousin(other.Genealogy, self.Genealogy) && Tuning.kShow1stCousinsAsCousins && (!IsHalfCousin(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Cousin");
                }
                else if (IsHalfUncle(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfUncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthHalfCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (Genealogy.IsFatherSideUncle(other.Genealogy, self.Genealogy) && Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!IsHalfUncle(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Uncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (Genealogy.IsMotherSideUncle(other.Genealogy, self.Genealogy) && Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!IsHalfUncle(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:UncleMothersSide") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (IsHalfNephew(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfNephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthHalfCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (Genealogy.IsNephew(other.Genealogy, self.Genealogy) && (!IsHalfNephew(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Nephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:GGP");
                }
                else if (Genealogy.IsGreatGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:GGC");
                }
                else if (self.Genealogy.IsAncestor(other.Genealogy))
                {
                    text = playerLanguage.GetAncestorString(self.Genealogy, other.Genealogy);
                }
                else if (other.Genealogy.IsAncestor(self.Genealogy))
                {
                    text = playerLanguage.GetDescendantString(self.Genealogy, other.Genealogy);
                }
                // Check if the selected Sim is married to one of the target's descendants
                else if (self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.Spouse.IsAncestor(other.Genealogy) && self.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    if (Genealogy.IsGrandparent(other.Genealogy, self.Genealogy.Spouse))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:GrandparentInLaw");
                    }
                    else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy.Spouse))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:GGPInLaw");
                    }
                    else
                    {
                        text = playerLanguage.GetAncestorString(other.IsFemale, self.Genealogy.Spouse, other.Genealogy, true);
                    }
                }
                // Check if the target is married to one of the selected Sim's descendants
                else if (other.Genealogy.Spouse != null && other.Genealogy.Spouse != self.Genealogy && other.Genealogy.Spouse.IsAncestor(self.Genealogy) && other.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    if (Genealogy.IsGrandchild(other.Genealogy.Spouse, self.Genealogy))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:GrandchildInLaw");
                    }
                    else if (Genealogy.IsGreatGrandchild(other.Genealogy.Spouse, self.Genealogy))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:GGCInLaw");
                    }
                    else
                    {
                        text = playerLanguage.GetDescendantString(other.IsFemale, self.Genealogy, other.Genealogy.Spouse, true);
                    }
                }
                else
                {
                    Common.DistantRelationInfo distantRelationInfo = Common.CalculateDistantRelation(other.Genealogy, self.Genealogy);
                    if (distantRelationInfo == null)
                    {
                        // Check if the target is married to someone other than the selected Sim
                        if (other.Genealogy.Spouse != null && other.Genealogy.Spouse != self.Genealogy && other.Genealogy.PartnerType == PartnerType.Marriage)
                        {
                            distantRelationInfo = Common.CalculateDistantRelation(other.Genealogy.Spouse, self.Genealogy);
                        }
                        if (distantRelationInfo == null)
                        {
                            // Check if the selected Sim is married to someone other than the target
                            if (self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.PartnerType == PartnerType.Marriage)
                            {
                                distantRelationInfo = Common.CalculateDistantRelation(other.Genealogy, self.Genealogy.Spouse);
                            }
                            // Check if the selected Sim is married to a sibling of one of the target's ancestors
                            if (distantRelationInfo != null && distantRelationInfo.Degree == 0 && distantRelationInfo.ClosestDescendant == self.Genealogy.Spouse)
                            {
                                text = playerLanguage.GetDistantRelationString(other.Genealogy, distantRelationInfo);
                            }
                        }
                        // Check if the target is married to a sibling of one of the selected Sim's ancestors
                        else if (distantRelationInfo.Degree == 0 && distantRelationInfo.ClosestDescendant == other.Genealogy.Spouse)
                        {
                            text = playerLanguage.GetDistantRelationString(other.IsFemale, other.Genealogy.Spouse, distantRelationInfo);
                        }
                    }
                    else
                    {
                        /* Get a relation name that is valid if the target is a collateral descendant or type of cousin
                         * of the selected Sim or if the selected Sim is a collateral descendant of the target
                         */
                        text = playerLanguage.GetDistantRelationString(other.Genealogy, distantRelationInfo);
                    }
                    // This if-statement block is necessary for when the selected Sim and the target do not share an ancestor in the game but each has an ancestor who is a sibling of the other.
                    if (distantRelationInfo == null)
                    {
                        // Get a relation name that is valid if the target is a sibling of one of the selected Sim's ancestors
                        text = playerLanguage.GetSiblingOfAncestorString(other.IsFemale, self.Genealogy, other.Genealogy);
                        if (string.IsNullOrEmpty(text) && other.Genealogy.Spouse != null && self.Genealogy != other.Genealogy.Spouse && other.Genealogy.PartnerType == PartnerType.Marriage)
                        {
                            // Get a relation name that is valid if the target is married to a sibling of one of the selected Sim's ancestors
                            text = playerLanguage.GetSiblingOfAncestorString(other.IsFemale, self.Genealogy, other.Genealogy.Spouse);
                        }
                        if (string.IsNullOrEmpty(text))
                        {
                            // Get a relation name that is valid if the selected Sim is a sibling of one of the target's ancestors
                            text = playerLanguage.GetDescendantOfSiblingString(other.IsFemale, self.Genealogy, other.Genealogy);
                        }
                        if (string.IsNullOrEmpty(text) && self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.PartnerType == PartnerType.Marriage)
                        {
                            // Get a relation name that is valid if the selected Sim is married to a sibling of one of the target's ancestors
                            text = playerLanguage.GetDescendantOfSiblingString(other.IsFemale, self.Genealogy.Spouse, other.Genealogy);
                        }
                    }
                }
                if (FutureDescendantService.IsAncestorOf(other, self))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Ancestor");
                }
                else if (FutureDescendantService.IsDescendantOf(other, self))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Descendant");
                }
                Relationship relationship = Relationship.Get(self, other, false);
                if (relationship != null)
                {
                    if (relationship.CurrentLTR != LongTermRelationshipTypes.Spouse && relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.AreLitterMates))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Sibling_Pet");
                    }
                    if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.HumanParentPetRel))
                    {
                        if (self.IsHuman)
                        {
                            if (other.IsADogSpecies)
                            {
                                text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Owns_puppy");
                            }
                            if (other.IsCat)
                            {
                                text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Owns_kitten");
                            }
                        }
                        else
                        {
                            text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Parent");
                        }
                    }
                }
                if ((other.IsEP11Bot || self.IsEP11Bot || ((other.IsFrankenstein || self.IsFrankenstein) && Tuning.kReplaceRelationsForSimBots)) && !string.IsNullOrEmpty(text))
                {
                    if (Genealogy.IsParent(other.Genealogy, self.Genealogy) && (self.IsEP11Bot || (self.IsFrankenstein && Tuning.kReplaceRelationsForSimBots)))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Creator");
                    }
                    else if (Genealogy.IsChild(other.Genealogy, self.Genealogy) && (other.IsEP11Bot || (other.IsFrankenstein && Tuning.kReplaceRelationsForSimBots)))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Creation");
                    }
                    else if ((self.IsEP11Bot || (self.IsFrankenstein && Tuning.kReplaceRelationsForSimBots)) && !(other.IsEP11Bot || (other.IsFrankenstein && Tuning.kReplaceRelationsForSimBots)))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:FamilyMember");
                    }
                    else if (other.IsEP11Bot)
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:FamilyBot");
                    }
                    else if (other.IsFrankenstein && Tuning.kReplaceRelationsForSimBots)
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:FamilyBot");
                    }
                }
                return text.Replace("  ", " ").Capitalize();
            }
        }
    }
}