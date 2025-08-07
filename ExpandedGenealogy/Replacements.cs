using Destrospean.Lang.ExpandedGenealogy;
using MonoPatcherLib;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.ExpandedGenealogy
{
    public class Replacements
    {
        public static bool IsCloselyRelated(SimDescription sim1, SimDescription sim2, bool thoroughCheck)
        {
            if (sim1 == sim2)
            {
                return true;
            }
            if (sim1 == null || sim2 == null || !(sim1.Species == sim2.Species || sim1.IsADogSpecies && sim2.IsADogSpecies) || sim1.IsRobot || sim2.IsRobot)
            {
                return false;
            }
            if ((FutureDescendantService.IsAncestorOf(sim1, sim2) || FutureDescendantService.IsAncestorOf(sim2, sim1)) && Tuning.kDenyRomanceWithAncestors)
            {
                return true;
            }
            return sim1.Genealogy.IsBloodRelated(sim2.Genealogy) || sim1.Genealogy.IsStepRelated(sim2.Genealogy);
        }

        public static bool IsCousin(Genealogy sim1, Genealogy sim2, int degree = 1, int timesRemoved = 0, Genealogy closestDescendant = null)
        {
            foreach (DistantRelationInfo distantRelationInfo in sim1.GetGenealogyPlaceholder().CalculateDistantRelations(sim2.GetGenealogyPlaceholder()))
            {
                if (distantRelationInfo.Degree == degree && distantRelationInfo.TimesRemoved == timesRemoved && (closestDescendant == null || distantRelationInfo.ClosestDescendant.Genealogy == closestDescendant))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsHalfCousin(Genealogy sim1, Genealogy sim2, int degree = 1, int timesRemoved = 0, Genealogy closestDescendant = null)
        {
            bool isOnlyHalf = false;
            foreach (DistantRelationInfo distantRelationInfo in sim1.GetGenealogyPlaceholder().CalculateDistantRelations(sim2.GetGenealogyPlaceholder()))
            {
                if (distantRelationInfo.Degree == degree && distantRelationInfo.TimesRemoved == timesRemoved && (closestDescendant == null || distantRelationInfo.ClosestDescendant.Genealogy == closestDescendant))
                {
                    if (Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0].Genealogy, distantRelationInfo.ThroughWhichChildren[1].Genealogy))
                    {
                        isOnlyHalf = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return isOnlyHalf;
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
                    if (sibling == sim2 || sibling.Spouse == sim2 && sibling.PartnerType == PartnerType.Marriage)
                    {
                        return Genealogy.IsHalfSibling(sim1.Spouse, sibling);
                    }
                }
            }
            if (sim2.Spouse != null && sim2.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy sibling in sim2.Spouse.Siblings)
                {
                    if (sibling == sim1 || sibling.Spouse == sim1 && sibling.PartnerType == PartnerType.Marriage)
                    {
                        return Genealogy.IsHalfSibling(sim2.Spouse, sibling);
                    }
                }
            }
            return false;
        }

        public static bool IsHalfUncle(Genealogy uncle, Genealogy nephew)
        {
            if (IsHalfCousin(uncle, nephew, 0, 1, uncle))
            {
                return true;
            }
            else if (IsCousin(uncle, nephew, 0, 1, uncle))
            {
                return false;
            }
            else if (uncle.Spouse != null && uncle.PartnerType == PartnerType.Marriage && IsHalfCousin(uncle.Spouse, nephew, 0, 1, uncle.Spouse))
            {
                return true;
            }
            bool isOnlyHalf = false;
            foreach (GenealogyPlaceholder genealogyPlaceholder in uncle.GetGenealogyPlaceholder().Siblings)
            {
                foreach (GenealogyPlaceholder sibling in uncle.GetGenealogyPlaceholder().Siblings)
                {
                    AncestorInfo ancestorInfo = nephew.GetGenealogyPlaceholder().GetAncestorInfo(sibling);
                    if (ancestorInfo != null && ancestorInfo.GenerationalDistance == 0)
                    {
                        if (Genealogy.IsHalfSibling(uncle, sibling.Genealogy))
                        {
                            isOnlyHalf = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            if (isOnlyHalf)
            {
                return true;
            }    
            if (uncle.Spouse != null && uncle.PartnerType == PartnerType.Marriage)
            {
                foreach (GenealogyPlaceholder sibling in uncle.Spouse.GetGenealogyPlaceholder().Siblings)
                {
                    AncestorInfo ancestorInfo = nephew.GetGenealogyPlaceholder().GetAncestorInfo(sibling);
                    if (ancestorInfo != null && ancestorInfo.GenerationalDistance == 0)
                    {
                        if (Genealogy.IsHalfSibling(uncle.Spouse, sibling.Genealogy))
                        {
                            isOnlyHalf = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return isOnlyHalf;
        }

        [TypePatch(typeof(Genealogy))]
        public class GenealogyPatch
        {
            public void ClearDerivedData()
            {
                List<Genealogy> descendants = new List<Genealogy>()
                    {
                        (Genealogy)(this as object)
                    };
                while (descendants.Count > 0)
                {
                    Genealogy descendant = descendants[0];
                    descendants.RemoveAt(0);
                    descendant.mAncestors = null;
                    if (descendant.mSiblings != null && descendant.mNaturalParents.Count > 0)
                    {
                        descendant.mSiblings = null;
                    }
                    if (descendant.mChildren != null)
                    {
                        descendants.AddRange(descendant.mChildren);
                    }
                }
                Common.RebuildRelationAssignments();
            }

            public bool IsBloodRelated(Genealogy other)
            {
                Genealogy self = (Genealogy)(this as object);
                float relationshipCoefficient = 0f;
                // Check if the target is an ancestor of the selected Sim
                foreach (AncestorInfo ancestorInfo in self.GetAncestorInfoList(other))
                {
                    relationshipCoefficient += (float)Math.Pow(2, -ancestorInfo.GenerationalDistance - 1);
                    if (Tuning.kDenyRomanceWithAncestors)
                    {
                        return true;
                    }
                }
                // Check if the selected Sim is an ancestor of the target
                foreach (AncestorInfo ancestorInfo in other.GetAncestorInfoList(self))
                {
                    relationshipCoefficient += (float)Math.Pow(2, -ancestorInfo.GenerationalDistance - 1);
                    if (Tuning.kDenyRomanceWithAncestors)
                    {
                        return true;
                    }
                }
                // Check if the Sims are siblings
                if (Genealogy.IsSibling(self, other))
                {
                    bool isHalfSibling = Genealogy.IsHalfSibling(self, other);
                    relationshipCoefficient += isHalfSibling ? .25f : .5f;
                    if (Tuning.kDenyRomanceWithSiblings && !(isHalfSibling && Tuning.kAllowRomanceForHalfRelatives))
                    {
                        return true;
                    }
                }
                // Check if the target is a sibling of one of the selected Sim's ancestors
                foreach (GenealogyPlaceholder sibling in other.GetGenealogyPlaceholder().Siblings)
                {
                    foreach (AncestorInfo tempAncestorInfo in self.GetGenealogyPlaceholder().GetAncestorInfoList(sibling))
                    {
                        relationshipCoefficient += (float)Math.Pow(2, -tempAncestorInfo.GenerationalDistance - (Genealogy.IsHalfSibling(other, sibling.Genealogy) ? 3 : 2));
                        if (Tuning.kDenyRomanceWithSiblingsOfAncestors)
                        {
                            return true;
                        }
                    }
                }
                // Check if the selected Sim is a sibling of one of the target's ancestors
                foreach (GenealogyPlaceholder sibling in self.GetGenealogyPlaceholder().Siblings)
                {
                    foreach (AncestorInfo tempAncestorInfo in other.GetGenealogyPlaceholder().GetAncestorInfoList(sibling))
                    {
                        relationshipCoefficient += (float)Math.Pow(2, -tempAncestorInfo.GenerationalDistance - (Genealogy.IsHalfSibling(self, sibling.Genealogy) ? 3 : 2));
                        if (Tuning.kDenyRomanceWithSiblingsOfAncestors)
                        {
                            return true;
                        }
                    }
                }
                foreach (DistantRelationInfo distantRelationInfo in other.GetGenealogyPlaceholder().CalculateDistantRelations(self.GetGenealogyPlaceholder()))
                {
                    if (distantRelationInfo.Degree == 0 && new List<GenealogyPlaceholder>(distantRelationInfo.ThroughWhichChildren).Exists(sibling => self.GetGenealogyPlaceholder().IsAncestor(sibling) || other.GetGenealogyPlaceholder().IsAncestor(sibling)) && (other.GetGenealogyPlaceholder().Siblings.Exists(sibling => self.GetGenealogyPlaceholder().IsAncestor(sibling) && self.GetGenealogyPlaceholder().GetAncestorInfo(sibling).GenerationalDistance == distantRelationInfo.TimesRemoved - 1) || self.GetGenealogyPlaceholder().Siblings.Exists(sibling => other.GetGenealogyPlaceholder().IsAncestor(sibling) && other.GetGenealogyPlaceholder().GetAncestorInfo(sibling).GenerationalDistance == distantRelationInfo.TimesRemoved - 1)))
                    {
                        continue;
                    }
                    bool isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0].Genealogy, distantRelationInfo.ThroughWhichChildren[1].Genealogy);
                    if (distantRelationInfo.Degree == 0 && Tuning.kDenyRomanceWithSiblingsOfAncestors && !(isHalfRelative && Tuning.kAllowRomanceForHalfRelatives))
                    {
                        return true;
                    }
                    relationshipCoefficient += (float)Math.Pow(2, -2 * distantRelationInfo.Degree - distantRelationInfo.TimesRemoved - (isHalfRelative ? 2 : 1));
                    /* Check if the Sims are too closely related for romantic interactions depending on whether their degree of cousinage
                     * and the generational distance between them are below the minimums that determine that they are not, and if so, then check whether they are half-relatives,
                     * the latter of which matters depending on whether romantic interactions between distant half-relatives are allowed
                     */
                    if (distantRelationInfo.Degree < (uint)Tuning.kMinDegreeCousinsToAllowRomance && distantRelationInfo.TimesRemoved < (uint)Tuning.kMinTimesRemovedCousinsToAllowRomance && !(isHalfRelative && Tuning.kAllowRomanceForHalfRelatives))
                    {
                        return true;
                    }
                }
                /* Check if the coefficient of relationship for the two Sims is higher the minimum to disallow it.
                 * If the minimum value is less than 0, then the coefficient of relationship does not determine whether romance between two Sims is allowed.
                 */
                return relationshipCoefficient >= Tuning.kMinRelationshipCoefficientToDenyRomance && Tuning.kMinRelationshipCoefficientToDenyRomance >= 0f;
            }

            public static bool IsCousin(Genealogy sim1, Genealogy sim2)
            {
                return Replacements.IsCousin(sim1, sim2);
            }

            public bool IsFutureBloodRelated(Genealogy other)
            {
                Genealogy self = (Genealogy)(this as object);
                if (other.SimDescription == null)
                {
                    return false;
                }
                if ((FutureDescendantService.IsAncestorOf(self.SimDescription, other.SimDescription) || FutureDescendantService.IsAncestorOf(other.SimDescription, self.SimDescription)) && Tuning.kDenyRomanceWithAncestors)
                {
                    return true;
                }
                return false;
            }

            public static bool IsGrandparent(Genealogy grandparent, Genealogy grandchild)
            {
                AncestorInfo ancestorInfo = grandchild.GetAncestorInfo(grandparent);
                return ancestorInfo != null && ancestorInfo.GenerationalDistance == 1;
            }

            public static bool IsGreatGrandparent(Genealogy greatGrandparent, Genealogy greatGrandchild)
            {
                AncestorInfo ancestorInfo = greatGrandchild.GetAncestorInfo(greatGrandparent);
                return ancestorInfo != null && ancestorInfo.GenerationalDistance == 2;
            }

            public static bool IsHalfSibling(Genealogy sim1, Genealogy sim2)
            {
                if (sim1 == null || sim2 == null)
                {
                    return false;
                }
                int sharedParentCount = 0;
                foreach (Genealogy parent1 in sim1.Parents)
                {
                    foreach (Genealogy parent2 in sim2.Parents)
                    {
                        if (parent1 == parent2)
                        {
                            sharedParentCount++;
                        }
                    }
                }
                return Genealogy.IsSibling(sim1, sim2) && sharedParentCount * 2 != sim1.Parents.Count + sim2.Parents.Count;
            }

            public static bool IsSiblingInLaw(Genealogy sim1, Genealogy sim2)
            {
                if (sim1.Spouse != null && sim1.PartnerType == PartnerType.Marriage)
                {
                    foreach (Genealogy sibling in sim1.Spouse.Siblings)
                    {
                        if (sibling == sim2 || sibling.Spouse == sim2 && sibling.PartnerType == PartnerType.Marriage)
                        {
                            return true;
                        }
                    }
                }
                if (sim2.Spouse != null && sim2.PartnerType == PartnerType.Marriage)
                {
                    foreach (Genealogy sibling in sim2.Spouse.Siblings)
                    {
                        if (sibling == sim1 || sibling.Spouse == sim1 && sibling.PartnerType == PartnerType.Marriage)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool IsStepRelated(Genealogy other)
            {
                if (Tuning.kAllowRomanceForStepRelatives)
                {
                    return false;
                }
                Genealogy self = (Genealogy)(this as object);
                foreach (Genealogy parent1 in self.Parents)
                {
                    foreach (Genealogy parent2 in other.Parents)
                    {
                        if (parent1.Spouse == parent2 && parent1.PartnerType == PartnerType.Marriage && Tuning.kDenyRomanceWithSiblings)
                        {
                            return true;
                        }
                    }
                }
                foreach (Genealogy parent in self.Parents)
                {
                    if (parent.Spouse == other && parent.PartnerType == PartnerType.Marriage && Tuning.kDenyRomanceWithAncestors)
                    {
                        return true;
                    }
                }
                foreach (Genealogy parent in other.Parents)
                {
                    if (parent.Spouse == self && parent.PartnerType == PartnerType.Marriage && Tuning.kDenyRomanceWithAncestors)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static bool IsUncle(Genealogy uncle, Genealogy nephew)
            {
                if (Replacements.IsCousin(uncle, nephew, 0, 1, uncle) || uncle.Spouse != null && uncle.PartnerType == PartnerType.Marriage && Replacements.IsCousin(uncle.Spouse, nephew, 0, 1, uncle.Spouse))
                {
                    return true;
                }
                foreach (GenealogyPlaceholder genealogyPlaceholder in uncle.GetGenealogyPlaceholder().Siblings)
                {
                    foreach (GenealogyPlaceholder sibling in uncle.GetGenealogyPlaceholder().Siblings)
                    {
                        AncestorInfo ancestorInfo = nephew.GetGenealogyPlaceholder().GetAncestorInfo(sibling);
                        if (ancestorInfo != null && ancestorInfo.GenerationalDistance == 0)
                        {
                            return true;
                        }
                    }
                }
                if (uncle.Spouse != null && uncle.PartnerType == PartnerType.Marriage)
                {
                    foreach (GenealogyPlaceholder sibling in uncle.Spouse.GetGenealogyPlaceholder().Siblings)
                    {
                        AncestorInfo ancestorInfo = nephew.GetGenealogyPlaceholder().GetAncestorInfo(sibling);
                        if (ancestorInfo != null && ancestorInfo.GenerationalDistance == 0)
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
                string text = "";
                SimDescription self = (SimDescription)(this as object);
                if (other.Genealogy == self.Genealogy)
                {
                    return text;
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
                PlayerLanguage playerLanguage = Activator.CreateInstance(Type.GetType("Destrospean.Lang.ExpandedGenealogy." + Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))) as PlayerLanguage;
                if (Genealogy.IsParent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Parent");
                }
                else if (Genealogy.IsChild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Child");
                }
                else if (Genealogy.IsGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Grandparent");
                }
                else if (Genealogy.IsGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Grandchild");
                }
                else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:GGP");
                }
                else if (Genealogy.IsGreatGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:GGC");
                }
                else if (self.Genealogy.GetGenealogyPlaceholder().IsAncestor(other.Genealogy))
                {
                    text = playerLanguage.GetAncestorString(self.Genealogy, other.Genealogy);
                }
                else if (other.Genealogy.GetGenealogyPlaceholder().IsAncestor(self.Genealogy))
                {
                    text = playerLanguage.GetDescendantString(self.Genealogy, other.Genealogy);
                }
                else if (Genealogy.IsHalfSibling(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:HalfSibling");
                }
                else if (Genealogy.IsSibling(other.Genealogy, self.Genealogy) && (!Genealogy.IsHalfSibling(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Sibling");
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
                else if (IsHalfUncle(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfUncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthHalfCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!IsHalfUncle(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Uncle" + (Genealogy.IsMotherSideUncle(other.Genealogy, self.Genealogy) ? "MothersSide" : "")) : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (IsHalfNephew(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfNephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthHalfCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (Genealogy.IsNephew(other.Genealogy, self.Genealogy) && (!IsHalfNephew(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Nephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "");
                }
                else if (IsHalfCousin(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives && Tuning.kShow1stCousinsAsCousins)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfCousin");
                }
                else if (Genealogy.IsCousin(other.Genealogy, self.Genealogy) && Tuning.kShow1stCousinsAsCousins && (!IsHalfCousin(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Cousin");
                }
                else if (playerLanguage.TryGetDistantRelationString(self, other, out text))
                {
                }
                else if (Genealogy.IsParentInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:ParentInLaw");
                }
                else if (Genealogy.IsChildInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:ChildInLaw");
                }
                // Check if the selected Sim is married to one of the target's descendants
                else if (self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.Spouse.GetGenealogyPlaceholder().IsAncestor(other.Genealogy) && self.Genealogy.PartnerType == PartnerType.Marriage)
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
                else if (other.Genealogy.Spouse != null && other.Genealogy.Spouse != self.Genealogy && other.Genealogy.Spouse.GetGenealogyPlaceholder().IsAncestor(self.Genealogy) && other.Genealogy.PartnerType == PartnerType.Marriage)
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
                else if (IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfSiblingInLaw");
                }
                else if (Genealogy.IsSiblingInLaw(other.Genealogy, self.Genealogy) && (!IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:SiblingInLaw");
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
                if ((other.IsEP11Bot || self.IsEP11Bot || (other.IsFrankenstein || self.IsFrankenstein) && Tuning.kReplaceRelationsForSimBots) && !string.IsNullOrEmpty(text))
                {
                    if (Genealogy.IsParent(other.Genealogy, self.Genealogy) && (self.IsEP11Bot || self.IsFrankenstein && Tuning.kReplaceRelationsForSimBots))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Creator");
                    }
                    else if (Genealogy.IsChild(other.Genealogy, self.Genealogy) && (other.IsEP11Bot || other.IsFrankenstein && Tuning.kReplaceRelationsForSimBots))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Creation");
                    }
                    else if ((self.IsEP11Bot || self.IsFrankenstein && Tuning.kReplaceRelationsForSimBots) && !(other.IsEP11Bot || other.IsFrankenstein && Tuning.kReplaceRelationsForSimBots))
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

            public ulong MakeUniqueId()
            {
                SimDescription self = (SimDescription)(this as object);
                ulong simDescriptionId = self.mSimDescriptionId;
                while (!self.IsSimDescriptionIdUnique(simDescriptionId) || Common.GenealogyPlaceholders.ContainsKey(simDescriptionId))
                {
                    simDescriptionId = DownloadContent.GenerateGUID();
                }
                if (simDescriptionId != self.mSimDescriptionId)
                {
                    self.mOldSimDescriptionId = self.mSimDescriptionId;
                }
                self.mSimDescriptionId = simDescriptionId;
                if (self.CelebrityManager != null)
                {
                    self.CelebrityManager.ResetOwnerSimDescription(self.mSimDescriptionId);
                }
                if (self.PetManager != null)
                {
                    self.PetManager.ResetOwnerSimDescription(self.mSimDescriptionId);
                }
                if (self.TraitChipManager != null)
                {
                    self.TraitChipManager.ResetOwnerSimDescription(self.mSimDescriptionId);
                }
                return simDescriptionId;
            }
        }
    }
}