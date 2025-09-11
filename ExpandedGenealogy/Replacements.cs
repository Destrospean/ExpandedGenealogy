using System;
using System.Collections.Generic;
using Destrospean.Lang.ExpandedGenealogy;
using MonoPatcherLib;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.ExpandedGenealogy
{
    public class Replacements
    {
        /// <summary>Replacement method for NRaas Woohooer's `IsCloselyRelated` method</summary>
        /// <param name="thoroughCheck">Parameter made obsolete by this mod but is still required to replace the original method</param>
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

        [TypePatch(typeof(Genealogy))]
        public class GenealogyPatch
        {
            public void AddChild(IGenealogy iChild)
            {
                Genealogy other = (Genealogy)iChild, self = (Genealogy)(object)this;
                if (other.mNaturalParents.Count == 2)
                {
                    return;
                }
                List<Genealogy> siblings = new List<Genealogy>();
                if (other.mNaturalParents.Count == 0)
                {
                    siblings.AddRange(other.Siblings);
                }
                siblings.Add(other);
                foreach (Genealogy sibling in siblings)
                {
                    if (self.mChildren.Contains(sibling))
                    {
                        continue;
                    }
                    self.ClearDerivedData();
                    sibling.ClearDerivedData();
                    foreach (Genealogy child in self.mChildren)
                    {
                        child.ClearDerivedData();
                    }
                    List<Genealogy> tempAncestors = new List<Genealogy>();
                    tempAncestors.AddRange(self.mNaturalParents);
                    while (tempAncestors.Count > 0)
                    {
                        Genealogy tempAncestor = tempAncestors[0];
                        tempAncestors.RemoveAt(0);
                        tempAncestor.ClearDerivedData();
                        tempAncestors.AddRange(tempAncestor.mNaturalParents);
                    }
                    self.mChildren.Add(sibling);
                    sibling.mNaturalParents.Add(self);
                    if (sibling.mSim != null && !sibling.IMiniSimDescription.IsEP11Bot && self.mSim != null && self.mSim.CreatedSim != null)
                    {
                        EventTracker.SendEvent(new GotChildAndAgeTransitionEvent(self.mSim.CreatedSim, sibling.mSim.CreatedSim, false));
                        EventTracker.SendEvent(EventTypeId.kChildBornOrAdopted, null, sibling.mSim.CreatedSim);
                    }
                }
                GenealogyExtended.RebuildRelationAssignments();
            }

            public void ClearDerivedData()
            {
                List<Genealogy> descendants = new List<Genealogy>()
                    {
                        (Genealogy)(object)this
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
                GenealogyPlaceholder.ClearCaches();
            }

            public bool IsBloodRelated(Genealogy other)
            {
                Genealogy self = (Genealogy)(object)this;
                float relationshipCoefficient = 0f;
                // Check if the target is an ancestor of the selected Sim.
                foreach (AncestorInfo ancestorInfo in self.GetAncestorInfoList(other))
                {
                    if (Tuning.kDenyRomanceWithAncestors)
                    {
                        return true;
                    }
                    relationshipCoefficient += (float)Math.Pow(2, -ancestorInfo.GenerationalDistance - 1);
                }
                // Check if the selected Sim is an ancestor of the target.
                foreach (AncestorInfo ancestorInfo in other.GetAncestorInfoList(self))
                {
                    if (Tuning.kDenyRomanceWithAncestors)
                    {
                        return true;
                    }
                    relationshipCoefficient += (float)Math.Pow(2, -ancestorInfo.GenerationalDistance - 1);
                }
                // Check if the Sims are siblings.
                if (Genealogy.IsSibling(self, other))
                {
                    bool isHalfSibling = Genealogy.IsHalfSibling(self, other);
                    if (Tuning.kDenyRomanceWithSiblings && !(isHalfSibling && Tuning.kAllowRomanceForHalfRelatives))
                    {
                        return true;
                    }
                    relationshipCoefficient += isHalfSibling ? .25f : .5f;
                }
                // Check if the target is a sibling of one of the selected Sim's ancestors.
                foreach (SiblingOfAncestorInfo siblingOfAncestorInfo in self.GetSiblingOfAncestorInfoList(other))
                {
                    if (Tuning.kDenyRomanceWithSiblingsOfAncestors && !(siblingOfAncestorInfo.IsHalfRelative && Tuning.kAllowRomanceForHalfRelatives))
                    {
                        return true;
                    }
                    relationshipCoefficient += (float)Math.Pow(2, -siblingOfAncestorInfo.GenerationalDistance - (siblingOfAncestorInfo.IsHalfRelative ? 3 : 2));
                }
                // Check if the selected Sim is a sibling of one of the target's ancestors
                foreach (SiblingOfAncestorInfo siblingOfAncestorInfo in other.GetSiblingOfAncestorInfoList(self))
                {
                    if (Tuning.kDenyRomanceWithSiblingsOfAncestors && !(siblingOfAncestorInfo.IsHalfRelative && Tuning.kAllowRomanceForHalfRelatives))
                    {
                        return true;
                    }
                    relationshipCoefficient += (float)Math.Pow(2, -siblingOfAncestorInfo.GenerationalDistance - (siblingOfAncestorInfo.IsHalfRelative ? 3 : 2));
                }
                foreach (DistantRelationInfo distantRelationInfo in other.GetDistantRelationInfoList(self))
                {
                    /* Check if the Sims are too closely related for romantic interactions depending on whether their degree of cousinage
                     * and the generational distance between them are below the minimums that determine that they are not, and if so, then check whether they are half-relatives,
                     * the latter of which matters depending on whether romantic interactions between distant half-relatives are allowed.
                     */
                    if (distantRelationInfo.Degree < (uint)Tuning.kMinDegreeCousinsToAllowRomance && distantRelationInfo.TimesRemoved < (uint)Tuning.kMinTimesRemovedCousinsToAllowRomance && !(distantRelationInfo.IsHalfRelative && Tuning.kAllowRomanceForHalfRelatives))
                    {
                        return true;
                    }
                    relationshipCoefficient += (float)Math.Pow(2, -2 * distantRelationInfo.Degree - distantRelationInfo.TimesRemoved - (distantRelationInfo.IsHalfRelative ? 2 : 1));
                }
                /* Check if the coefficient of relationship for the two Sims is higher than the minimum to disallow romance.
                 * If the minimum value is less than 0, then the coefficient of relationship does not determine whether romance between two Sims is allowed.
                 */
                return relationshipCoefficient >= Tuning.kMinRelationshipCoefficientToDenyRomance && Tuning.kMinRelationshipCoefficientToDenyRomance >= 0f;
            }

            public static bool IsCousin(Genealogy sim1, Genealogy sim2)
            {
                return GenealogyExtended.IsCousin(sim1, sim2);
            }

            public bool IsFutureBloodRelated(Genealogy other)
            {
                Genealogy self = (Genealogy)(object)this;
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
                Genealogy self = (Genealogy)(object)this;
                foreach (Genealogy parent1 in self.Parents)
                {
                    foreach (Genealogy parent2 in other.Parents)
                    {
                        if (parent1.Spouse == parent2 && parent1.PartnerType == PartnerType.Marriage && Tuning.kDenyRomanceWithStepSiblings)
                        {
                            return true;
                        }
                    }
                }
                foreach (Genealogy parent in self.Parents)
                {
                    if (parent.Spouse == other && parent.PartnerType == PartnerType.Marriage && Tuning.kDenyRomanceWithStepParents)
                    {
                        return true;
                    }
                }
                foreach (Genealogy parent in other.Parents)
                {
                    if (parent.Spouse == self && parent.PartnerType == PartnerType.Marriage && Tuning.kDenyRomanceWithStepParents)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static bool IsUncle(Genealogy uncle, Genealogy nephew)
            {
                SiblingOfAncestorInfo siblingOfAncestorInfo = nephew.GetSiblingOfAncestorInfo(uncle);
                if (siblingOfAncestorInfo != null && siblingOfAncestorInfo.GenerationalDistance == 0)
                {
                    return true;
                }
                if (uncle.Spouse == null || uncle.PartnerType != PartnerType.Marriage)
                {
                    return false;
                }
                siblingOfAncestorInfo = nephew.GetSiblingOfAncestorInfo(uncle.Spouse);
                return siblingOfAncestorInfo != null && siblingOfAncestorInfo.GenerationalDistance == 0;
            }
        }

        [TypePatch(typeof(SimDescription))]
        public class SimDescriptionPatch
        {
            public string GetMyFamilialDescriptionFor(SimDescription other)
            {
                string text = "";
                SimDescription self = (SimDescription)(object)this;
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
                PlayerLanguage playerLanguage = Activator.CreateInstance(Type.GetType("Destrospean.Lang.ExpandedGenealogy." + Localization.LocalizeString("Destrospean/ExpandedGenealogy:LanguageCode"))) as PlayerLanguage;
                if (Genealogy.IsParent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Parent");
                }
                else if (Genealogy.IsGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Grandparent");
                }
                else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:GGP");
                }
                else if (self.Genealogy.GetGenealogyPlaceholder().IsAncestor(other.Genealogy))
                {
                    text = playerLanguage.GetAncestorString(other.Genealogy, self.Genealogy);
                }
                else if (Genealogy.IsChild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Child");
                }
                else if (Genealogy.IsGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Grandchild");
                }
                else if (Genealogy.IsGreatGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:GGC");
                }
                else if (self.Genealogy.GetGenealogyPlaceholder().IsDescendant(other.Genealogy))
                {
                    text = playerLanguage.GetDescendantString(other.Genealogy, self.Genealogy);
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
                else if (GenealogyExtended.IsHalfUncle(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:HalfUncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), "", "");
                }
                else if (Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!GenealogyExtended.IsHalfUncle(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Uncle" + (Genealogy.IsMotherSideUncle(other.Genealogy, self.Genealogy) ? "MothersSide" : "")) : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), "", "");
                }
                else if (playerLanguage.TryGetSiblingOfAncestorString(other, self, out text))
                {
                }
                else if (GenealogyExtended.IsHalfNephew(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:HalfNephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:NthHalfCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), "", "");
                }
                else if (Genealogy.IsNephew(other.Genealogy, self.Genealogy) && (!GenealogyExtended.IsHalfNephew(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Nephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:OrdinalSuffixNoun1"), "", "");
                }
                else if (playerLanguage.TryGetDescendantOfSiblingString(other, self, out text))
                {
                }
                else if (GenealogyExtended.IsHalfCousin(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives && Tuning.kShow1stCousinsAsCousins)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:HalfCousin");
                }
                else if (Genealogy.IsCousin(other.Genealogy, self.Genealogy) && Tuning.kShow1stCousinsAsCousins && (!GenealogyExtended.IsHalfCousin(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Cousin");
                }
                else if (playerLanguage.TryGetDistantRelationString(other, self, out text))
                {
                }
                else if (Genealogy.IsParentInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:ParentInLaw");
                }
                // Check if the selected Sim is married to one of the target's descendants.
                else if (self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.Spouse.GetGenealogyPlaceholder().IsAncestor(other.Genealogy) && self.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    if (Genealogy.IsGrandparent(other.Genealogy, self.Genealogy.Spouse))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:GrandparentInLaw");
                    }
                    else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy.Spouse))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGPInLaw");
                    }
                    else
                    {
                        text = playerLanguage.GetAncestorString(other.IsFemale, other.Genealogy, self.Genealogy.Spouse, true);
                    }
                }
                else if (Genealogy.IsChildInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:ChildInLaw");
                }
                // Check if the target is married to one of the selected Sim's descendants.
                else if (other.Genealogy.Spouse != null && other.Genealogy.Spouse != self.Genealogy && other.Genealogy.Spouse.GetGenealogyPlaceholder().IsAncestor(self.Genealogy) && other.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    if (Genealogy.IsGrandchild(other.Genealogy.Spouse, self.Genealogy))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:GrandchildInLaw");
                    }
                    else if (Genealogy.IsGreatGrandchild(other.Genealogy.Spouse, self.Genealogy))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:GGCInLaw");
                    }
                    else
                    {
                        text = playerLanguage.GetDescendantString(other.IsFemale, other.Genealogy.Spouse, self.Genealogy, true);
                    }
                }
                else if (GenealogyExtended.IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:HalfSiblingInLaw");
                }
                else if (Genealogy.IsSiblingInLaw(other.Genealogy, self.Genealogy) && (!GenealogyExtended.IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
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
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy/RelationNames:FamilyBot");
                    }
                }
                return text.Capitalize();
            }

            public ulong MakeUniqueId()
            {
                SimDescription self = (SimDescription)(object)this;
                ulong simDescriptionId = self.mSimDescriptionId;
                while (!self.IsSimDescriptionIdUnique(simDescriptionId) || GenealogyPlaceholder.GenealogyPlaceholders.ContainsKey(simDescriptionId))
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