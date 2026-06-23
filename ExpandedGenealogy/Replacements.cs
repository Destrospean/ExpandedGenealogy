using System;
using System.Collections.Generic;
using System.Reflection;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.ExpandedGenealogy
{
    public class Replacements
    {
        public void AddChild(IGenealogy iChild)
        {
            Genealogy other = (Genealogy)iChild,
            self = (Genealogy)(object)this;
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
            List<Genealogy> descendants = new List<Genealogy>
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

        public string GetMyFamilialDescriptionFor(SimDescription other)
        {
            string localizationKey = Common.kLocalizationPath + "/RelationNames",
            text = "";
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
                text = Common.PlayerLanguage.GetAncestorString(other.Genealogy, self.Genealogy);
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
                text = Common.PlayerLanguage.GetDescendantString(other.Genealogy, self.Genealogy);
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
                text = !Common.PlayerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, localizationKey + ":HalfUncle") : Localization.LocalizeString(other.IsFemale, localizationKey + ":NthHalfCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, localizationKey + ":OrdinalSuffixNoun1"), "", "");
            }
            else if (Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!GenealogyExtended.IsHalfUncle(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
            {
                text = !Common.PlayerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Uncle" + (Genealogy.IsMotherSideUncle(other.Genealogy, self.Genealogy) ? "MothersSide" : "")) : Localization.LocalizeString(other.IsFemale, localizationKey + ":NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, localizationKey + ":OrdinalSuffixNoun1"), "", "");
            }
            else if (Common.PlayerLanguage.TryGetSiblingOfAncestorString(other, self, out text))
            {
            }
            else if (GenealogyExtended.IsHalfNephew(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
            {
                text = !Common.PlayerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, localizationKey + ":HalfNephew") : Localization.LocalizeString(other.IsFemale, localizationKey + ":NthHalfCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, localizationKey + ":OrdinalSuffixNoun1"), "", "");
            }
            else if (Genealogy.IsNephew(other.Genealogy, self.Genealogy) && (!GenealogyExtended.IsHalfNephew(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
            {
                text = !Common.PlayerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Nephew") : Localization.LocalizeString(other.IsFemale, localizationKey + ":NthCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, localizationKey + ":OrdinalSuffixNoun1"), "", "");
            }
            else if (Common.PlayerLanguage.TryGetDescendantOfSiblingString(other, self, out text))
            {
            }
            else if (GenealogyExtended.IsHalfCousin(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives && Tuning.kShow1stCousinsAsCousins)
            {
                text = Localization.LocalizeString(other.IsFemale, localizationKey + ":HalfCousin");
            }
            else if (Genealogy.IsCousin(other.Genealogy, self.Genealogy) && Tuning.kShow1stCousinsAsCousins && (!GenealogyExtended.IsHalfCousin(other.Genealogy, self.Genealogy) || Tuning.kShowHalfRelativesAsFullRelatives))
            {
                text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Cousin");
            }
            else if (Common.PlayerLanguage.TryGetDistantRelationString(other, self, out text))
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
                    text = Localization.LocalizeString(other.IsFemale, localizationKey + ":GrandparentInLaw");
                }
                else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy.Spouse))
                {
                    text = Localization.LocalizeString(other.IsFemale, localizationKey + ":GGPInLaw");
                }
                else
                {
                    text = Common.PlayerLanguage.GetAncestorString(other.IsFemale, other.Genealogy, self.Genealogy.Spouse, true);
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
                    text = Localization.LocalizeString(other.IsFemale, localizationKey + ":GrandchildInLaw");
                }
                else if (Genealogy.IsGreatGrandchild(other.Genealogy.Spouse, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, localizationKey + ":GGCInLaw");
                }
                else
                {
                    text = Common.PlayerLanguage.GetDescendantString(other.IsFemale, other.Genealogy.Spouse, self.Genealogy, true);
                }
            }
            else if (GenealogyExtended.IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) && Tuning.kShowHalfRelatives && !Tuning.kShowHalfRelativesAsFullRelatives)
            {
                text = Localization.LocalizeString(other.IsFemale, localizationKey + ":HalfSiblingInLaw");
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
                    text = Localization.LocalizeString(other.IsFemale, localizationKey + ":FamilyBot");
                }
            }
            return text.Capitalize();
        }

        public bool IsBloodRelated(Genealogy other)
        {
            bool isSufficientlyRelatedToRuleOutRomance;
            ((Genealogy)(object)this).GetCoefficientOfRelationship(other, out isSufficientlyRelatedToRuleOutRomance, true);
            return isSufficientlyRelatedToRuleOutRomance;
        }

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

        public Rect RecurseLayoutChildrenMasterController(int x, int y, SimTreeInfo parent1, SimTreeInfo parent2, int recurseLevel, bool growXPositive, bool notWithParent2)
        {
            FamilyTreeDialog self = (FamilyTreeDialog)(object)this;
            MethodInfo exceptionMethod = Type.GetType("NRaas.Common, NRaasMasterController").GetMethod("Exception", new Type[]
                {
                    typeof(object),
                    typeof(object),
                    typeof(string),
                    typeof(Exception)
                }),
            getSimTreeInfoMethod = GetType().GetMethod("GetSimTreeInfo", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[]
                {
                    typeof(IMiniSimDescription)
                }, null);
            Rect result = new Rect(x, y, x, y);
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("RecurseLayoutChildren" + System.Environment.NewLine);
            try
            {
                if (recurseLevel == 0 || parent1 == null)
                {
                    return result;
                }
                stringBuilder.Append("A");
                GenealogyPlaceholder parent1GenealogyPlaceholder = ((Genealogy)parent1.mSimDescription.CASGenealogy).GetGenealogyPlaceholder(),
                parent2GenealogyPlaceholder = parent2 == null ? null : ((Genealogy)parent2.mSimDescription.CASGenealogy).GetGenealogyPlaceholder();
                List<GenealogyPlaceholder> descendants = new List<GenealogyPlaceholder>(GenealogyPlaceholder.GenealogyPlaceholders.Values).FindAll(a => a.Genealogy != null && parent1GenealogyPlaceholder.IsDescendant(a) && (parent2 == null || !notWithParent2 && parent2GenealogyPlaceholder.IsDescendant(a)));
                descendants.Sort((a, b) => (a.GetAncestorInfo(parent1GenealogyPlaceholder).GenerationalDistance - b.GetAncestorInfo(parent1GenealogyPlaceholder).GenerationalDistance).Clamp(-1, 1));
                List<IMiniSimDescription> childList = descendants.FindAll(a => !descendants.Exists(a.IsAncestor)).ConvertAll(a => a.Genealogy.IMiniSimDescription);
                if (childList.Count == 0)
                {
                    return result;
                }
                stringBuilder.Append("B");
                SimTreeInfo firstChild = null,
                lastChild = null;
                int newX = x,
                newY = y + (int)FamilyTreeDialog.Y_DIST_BETWEEN_THUMBS;
                Rect rect = new Rect(x, newY, x, newY);
                foreach (IMiniSimDescription child in childList)
                {
                    try
                    {
                        if (child != null)
                        {
                            lastChild = (SimTreeInfo)getSimTreeInfoMethod.Invoke(this, new object[]
                                {
                                    child
                                });
                            if (firstChild == null)
                            {
                                firstChild = lastChild;
                            }
                            newX = (int)(growXPositive ? rect.BottomRight.x : rect.TopLeft.x - FamilyTreeThumb.kRegularArea.x);
                            int generationalDistance = ((Genealogy)child.CASGenealogy).GetAncestorInfo(parent1GenealogyPlaceholder.Genealogy).GenerationalDistance;
                            lastChild.mWin = (FamilyTreeThumb)GetType().GetMethod("CreateFamilyTreeThumb", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[]
                                {
                                    lastChild.mSimDescription,
                                    new Vector2(newX, y + (int)FamilyTreeDialog.Y_DIST_BETWEEN_THUMBS * (generationalDistance + 1) + FamilyTreeThumb.kRegularArea.y * generationalDistance)
                                });
                            lastChild.mBottomBounds = lastChild.mWin.Area;
                            rect = Rect.Union(rect, lastChild.mWin.Area);
                            rect = Rect.Union(rect, RecurseLayoutChildrenMasterController(newX, (int)lastChild.mWin.Area.BottomRight.y, lastChild, null, recurseLevel - 1, growXPositive, false));
                            rect = growXPositive ? new Rect(rect.TopLeft, new Vector2(rect.BottomRight.x + FamilyTreeDialog.X_DIST_BETWEEN_THUMBS, rect.BottomRight.y)) : new Rect(new Vector2(rect.TopLeft.x - FamilyTreeDialog.X_DIST_BETWEEN_THUMBS, rect.TopLeft.y), rect.BottomRight);
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptionMethod.Invoke(null, new object[]
                            {
                                child,
                                null,
                                stringBuilder.ToString(),
                                ex
                            });
                    }
                }
                stringBuilder.Append("C");
                if (firstChild != null)
                {
                    if (parent1 != null && parent2 != null && !notWithParent2)
                    {
                        Rect area = firstChild.mWin.Area;
                        area.TopLeft = new Vector2(area.TopLeft.x, parent1.mBottomBounds.BottomRight.y);
                        area.BottomRight = new Vector2(area.BottomRight.x, parent1.mBottomBounds.BottomRight.y);
                        self.ConnectBoundsV(area, firstChild.mWin.Area, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    }
                    else
                    {
                        ConnectSims(parent1, firstChild, false, parent2 == null, notWithParent2 || parent2 == null, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    }
                }
                stringBuilder.Append("D");
                if (firstChild != lastChild)
                {
                    Rect childrenArea = ConnectSims(firstChild, lastChild, true, true, true, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    foreach (IMiniSimDescription child in childList)
                    {
                        SimTreeInfo childSimTreeInfo = (SimTreeInfo)getSimTreeInfoMethod.Invoke(this, new object[]
                            {
                                child
                            });
                        if (childSimTreeInfo != firstChild && childSimTreeInfo != lastChild)
                        {
                            Rect area = childSimTreeInfo.mWin.Area;
                            area.TopLeft = new Vector2(area.TopLeft.x, childrenArea.TopLeft.y);
                            area.BottomRight = new Vector2(area.BottomRight.x, childrenArea.TopLeft.y);
                            //Common.Notify(area.TopLeft.ToString() + " " + area.BottomRight.ToString() + " " + childSimTreeInfo.mWin.Area.TopLeft.ToString() + " " + childSimTreeInfo.mWin.Area.BottomRight.ToString(), null, StyledNotification.NotificationStyle.kSystemMessage);
                            self.ConnectBoundsV(area, childSimTreeInfo.mWin.Area, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                        }
                    }
                }
                return rect;
            }
            catch (Exception ex)
            {
                exceptionMethod.Invoke(null, new object[]
                    {
                        parent1 == null ? null : parent1.mSimDescription,
                        parent2 == null ? null : parent2.mSimDescription,
                        stringBuilder.ToString(),
                        ex
                    });
                return result;
            }
        }

        public Rect RecurseLayoutParents(int x, int y, SimTreeInfo child, int recurseLevel)
        {
            FamilyTreeDialog self = (FamilyTreeDialog)(object)this;
            GenealogyPlaceholder childGenealogyPlaceholder;
            IMiniSimDescription parent1 = null,
            parent2 = null;
            Rect rect = new Rect(x, y, x, y);
            if (self.GetParents(child.mSimDescription, ref parent1, ref parent2))
            {
                int num = (int)((float)y - FamilyTreeThumb.kRegularArea.y - FamilyTreeDialog.Y_DIST_BETWEEN_THUMBS);
                if (recurseLevel == 1)
                {
                    rect = self.GenericLayoutParents(x, num, self.GetSimTreeInfo(parent1), self.GetSimTreeInfo(parent2));
                }
                else
                {
                    Rect tempRect = new Rect(x, num, x, num);
                    if (parent1 != null)
                    {
                        tempRect = RecurseLayoutParents(x, num, self.GetSimTreeInfo(parent1), recurseLevel - 1);
                    }
                    if (parent2 != null)
                    {
                        RecurseLayoutParents((int)(tempRect.BottomRight.x + FamilyTreeDialog.X_DIST_BETWEEN_THUMBS), num, self.GetSimTreeInfo(parent2), recurseLevel - 1);
                    }
                    rect = self.GenericLayoutParents(0, 0, self.GetSimTreeInfo(parent1), self.GetSimTreeInfo(parent2));
                }
            }
            else if (((Genealogy)child.mSimDescription.CASGenealogy).IMiniSimDescription != null && (childGenealogyPlaceholder = ((Genealogy)child.mSimDescription.CASGenealogy).GetGenealogyPlaceholder()).Ancestors.Exists(a => a.Genealogy != null))
            {
                List<GenealogyPlaceholder> ancestors = new List<GenealogyPlaceholder>(childGenealogyPlaceholder.Ancestors.FindAll(a => a.Genealogy != null));
                ancestors.Sort((a, b) => (childGenealogyPlaceholder.GetAncestorInfo(a).GenerationalDistance - childGenealogyPlaceholder.GetAncestorInfo(b).GenerationalDistance).Clamp(-1, 1));
                parent1 = ancestors[0].Genealogy.IMiniSimDescription;
                if (ancestors.Count > 1 && childGenealogyPlaceholder.GetAncestorInfo(ancestors[0]).GenerationalDistance == childGenealogyPlaceholder.GetAncestorInfo(ancestors[1]).GenerationalDistance)
                {
                    parent2 = ancestors[1].Genealogy.IMiniSimDescription;
                }
                int num = (int)((float)y - FamilyTreeThumb.kRegularArea.y - FamilyTreeDialog.Y_DIST_BETWEEN_THUMBS);
                if (recurseLevel == 1)
                {
                    rect = self.GenericLayoutParents(x, num, self.GetSimTreeInfo(parent1), self.GetSimTreeInfo(parent2));
                }
                else
                {
                    Rect tempRect = new Rect(x, num, x, num);
                    if (parent1 != null)
                    {
                        tempRect = RecurseLayoutParents(x, num, self.GetSimTreeInfo(parent1), recurseLevel - 1);
                    }
                    if (parent2 != null)
                    {
                        RecurseLayoutParents((int)(tempRect.BottomRight.x + FamilyTreeDialog.X_DIST_BETWEEN_THUMBS), num, self.GetSimTreeInfo(parent2), recurseLevel - 1);
                    }
                    rect = self.GenericLayoutParents(0, 0, self.GetSimTreeInfo(parent1), self.GetSimTreeInfo(parent2));
                }
            }
            child.mWin = self.CreateFamilyTreeThumb(child.mSimDescription, new Vector2((int)(parent1 != null && parent2 != null ? rect.TopLeft.x + (rect.Width - FamilyTreeThumb.kRegularArea.x) / 2 : rect.TopLeft.x), y));
            child.mBottomBounds = child.mWin.Area;
            if (parent1 != null || parent2 != null)
            {
                if (parent1 != null && parent2 != null && self.AreMarried(parent1, parent2))
                {
                    self.ConnectBoundsV(rect, child.mWin.Area, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                }
                else
                {
                    if (parent1 != null)
                    {
                        self.ConnectSims(self.GetSimTreeInfo(parent1), child, false, true, false, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    }
                    if (parent2 != null)
                    {
                        self.ConnectSims(self.GetSimTreeInfo(parent2), child, false, true, false, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    }
                }
            }
            return Rect.Union(rect, child.mWin.Area);
        }

        public Rect RecurseLayoutParentsMasterController(int x, int y, SimTreeInfo child, Dictionary<IMiniSimDescription, SimTreeInfo> usedInfo, int recurseLevel)
        {
            if (((Genealogy)child.mSimDescription.CASGenealogy).IMiniSimDescription != null && ((Genealogy)child.mSimDescription.CASGenealogy).GetGenealogyPlaceholder().Ancestors.Exists(a => a.Genealogy == null))
            {
                return RecurseLayoutAssignedAncestorsMasterController(x, y, child, usedInfo, recurseLevel);
            }
            FamilyTreeDialog self = (FamilyTreeDialog)(object)this;
            IMiniSimDescription parent1 = null,
            parent2 = null;
            BindingFlags nonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo genericLayoutParentsMethod = GetType().GetMethod("GenericLayoutParents", nonPublicInstance, null, new Type[]
                {
                    typeof(int),
                    typeof(int),
                    typeof(SimTreeInfo),
                    typeof(SimTreeInfo)
                }, null),
            getSimTreeInfoMethod = GetType().GetMethod("GetSimTreeInfo", nonPublicInstance, null, new Type[]
                {
                    typeof(IMiniSimDescription)
                }, null);
            try
            {
                SimTreeInfo parent1SimTreeInfo = null,
                parent2SimTreeInfo = null;
                Rect rect = new Rect(x, y, x, y);
                if (self.GetParents(child.mSimDescription, ref parent1, ref parent2))
                {
                    if (parent1 != null && !usedInfo.ContainsKey(parent1))
                    {
                        parent1SimTreeInfo = (SimTreeInfo)getSimTreeInfoMethod.Invoke(this, new object[]
                            {
                                parent1
                            });
                        usedInfo.Add(parent1, parent1SimTreeInfo);
                    }
                    if (parent2 != null && !usedInfo.ContainsKey(parent2))
                    {
                        parent2SimTreeInfo = (SimTreeInfo)getSimTreeInfoMethod.Invoke(this, new object[]
                            {
                                parent2
                            });
                        usedInfo.Add(parent2, parent2SimTreeInfo);
                    }
                    int num = (int)((float)y - FamilyTreeThumb.kRegularArea.y - FamilyTreeDialog.Y_DIST_BETWEEN_THUMBS);
                    if (recurseLevel == 1)
                    {
                        rect = (Rect)genericLayoutParentsMethod.Invoke(this, new object[]
                            {
                                x,
                                num,
                                parent1SimTreeInfo,
                                parent2SimTreeInfo
                            });
                    }
                    else
                    {
                        Rect tempRect = new Rect(x, num, x, num);
                        if (parent1SimTreeInfo != null)
                        {
                            tempRect = RecurseLayoutParentsMasterController(x, num, parent1SimTreeInfo, usedInfo, recurseLevel - 1);
                        }
                        if (parent2SimTreeInfo != null)
                        {
                            RecurseLayoutParentsMasterController((int)(tempRect.BottomRight.x + FamilyTreeDialog.X_DIST_BETWEEN_THUMBS), num, parent2SimTreeInfo, usedInfo, recurseLevel - 1);
                        }
                        rect = (Rect)genericLayoutParentsMethod.Invoke(this, new object[]
                            {
                                0,
                                0,
                                parent1SimTreeInfo,
                                parent2SimTreeInfo
                            });
                    }
                }
                x = (int)rect.TopLeft.x;
                if (parent1SimTreeInfo != null && parent2SimTreeInfo != null)
                {
                    x = (int)(rect.TopLeft.x + (rect.Width - FamilyTreeThumb.kRegularArea.x) / 2);
                }
                child.mWin = (FamilyTreeThumb)GetType().GetMethod("CreateFamilyTreeThumb", nonPublicInstance).Invoke(this, new object[]
                    {
                        child.mSimDescription,
                        new Vector2(x, y)
                    });
                child.mBottomBounds = child.mWin.Area;
                if (parent1SimTreeInfo != null || parent2SimTreeInfo != null)
                {
                    if (parent1SimTreeInfo != null && parent2SimTreeInfo != null && (PartnerType)GetType().GetMethod("GetPartnerType", nonPublicInstance, null, new Type[]
                        {
                            typeof(IMiniSimDescription),
                            typeof(IMiniSimDescription)
                        }, null).Invoke(this, new object[]
                            {
                                parent1,
                                parent2
                            }) != 0)
                    {
                        self.ConnectBoundsV(rect, child.mWin.Area, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    }
                    else
                    {
                        if (parent1SimTreeInfo != null)
                        {
                            self.ConnectSims(parent1SimTreeInfo, child, false, true, false, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                        }
                        if (parent2SimTreeInfo != null)
                        {
                            self.ConnectSims(parent2SimTreeInfo, child, false, true, false, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                        }
                    }
                }
                return Rect.Union(rect, child.mWin.Area);
            }
            catch (Exception ex)
            {
                string text = "";
                if (child.mSimDescription != null)
                {
                    text += System.Environment.NewLine + "Child 1: " + child.mSimDescription.FullName;
                }
                if (parent1 != null)
                {
                    text += System.Environment.NewLine + "Parent 1: " + parent1.FullName;
                }
                if (parent2 != null)
                {
                    text += System.Environment.NewLine + "Parent 2: " + parent2.FullName;
                }
                Type.GetType("NRaas.Common, NRaasMasterController").GetMethod("Exception", new Type[]
                    {
                        typeof(string),
                        typeof(Exception)
                    }).Invoke(null, new object[]
                        {
                            text,
                            ex
                        });
                return default(Rect);
            }
        }

        public Rect RecurseLayoutAssignedAncestorsMasterController(int x, int y, SimTreeInfo child, Dictionary<IMiniSimDescription, SimTreeInfo> usedInfo, int recurseLevel)
        {
            FamilyTreeDialog self = (FamilyTreeDialog)(object)this;
            MethodInfo exceptionMethod = Type.GetType("NRaas.Common, NRaasMasterController").GetMethod("Exception", new Type[]
                {
                    typeof(object),
                    typeof(object),
                    typeof(string),
                    typeof(Exception)
                }),
            getSimTreeInfoMethod = GetType().GetMethod("GetSimTreeInfo", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[]
                {
                    typeof(IMiniSimDescription)
                }, null);
            Rect result = new Rect(x, y, x, y);
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("RecurseLayoutParents" + System.Environment.NewLine);
            try
            {
                if (recurseLevel == 0 || child == null)
                {
                    return result;
                }
                stringBuilder.Append("A");
                GenealogyPlaceholder childGenealogyPlaceholder = ((Genealogy)child.mSimDescription.CASGenealogy).GetGenealogyPlaceholder();
                List<GenealogyPlaceholder> ancestors = new List<GenealogyPlaceholder>(GenealogyPlaceholder.GenealogyPlaceholders.Values).FindAll(a => a.Genealogy != null && childGenealogyPlaceholder.IsAncestor(a));
                ancestors.Sort((a, b) => (childGenealogyPlaceholder.GetAncestorInfo(a).GenerationalDistance - childGenealogyPlaceholder.GetAncestorInfo(b).GenerationalDistance).Clamp(-1, 1));
                List<IMiniSimDescription> parentList = ancestors.FindAll(a => !ancestors.Exists(a.IsDescendant)).ConvertAll(a => a.Genealogy.IMiniSimDescription);
                if (parentList.Count == 0)
                {
                    return result;
                }
                stringBuilder.Append("B");
                SimTreeInfo firstParent = null,
                lastParent = null;
                int newX = x,
                newY = (int)((float)y - FamilyTreeThumb.kRegularArea.y - FamilyTreeDialog.Y_DIST_BETWEEN_THUMBS);
                Rect rect = new Rect(x, newY, x, newY);
                foreach (IMiniSimDescription parent in parentList)
                {
                    try
                    {
                        if (parent != null)
                        {
                            lastParent = (SimTreeInfo)getSimTreeInfoMethod.Invoke(this, new object[]
                                {
                                    parent
                                });
                            if (firstParent == null)
                            {
                                firstParent = lastParent;
                            }
                            newX = (int)rect.BottomRight.x;
                            int generationalDistance = childGenealogyPlaceholder.Genealogy.GetAncestorInfo((Genealogy)parent.CASGenealogy).GenerationalDistance;
                            lastParent.mWin = (FamilyTreeThumb)GetType().GetMethod("CreateFamilyTreeThumb", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[]
                                {
                                    lastParent.mSimDescription,
                                    new Vector2(newX, (int)((float)y - (FamilyTreeThumb.kRegularArea.y + FamilyTreeDialog.Y_DIST_BETWEEN_THUMBS) * (generationalDistance + 1)))
                                });
                            lastParent.mBottomBounds = lastParent.mWin.Area;
                            rect = Rect.Union(rect, lastParent.mWin.Area);
                            rect = Rect.Union(rect, RecurseLayoutAssignedAncestorsMasterController(newX, (int)lastParent.mWin.Area.TopLeft.y, lastParent, usedInfo, recurseLevel - 1));
                            rect = new Rect(rect.TopLeft, new Vector2(rect.BottomRight.x + FamilyTreeDialog.X_DIST_BETWEEN_THUMBS, rect.BottomRight.y));
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptionMethod.Invoke(null, new object[]
                            {
                                parent,
                                null,
                                stringBuilder.ToString(),
                                ex
                            });
                    }
                }
                stringBuilder.Append("C");
                if (firstParent != null)
                {
                    if (child != null)
                    {
                        Rect area = firstParent.mWin.Area;
                        area.TopLeft = new Vector2(area.TopLeft.x, child.mBottomBounds.BottomRight.y);
                        area.BottomRight = new Vector2(area.BottomRight.x, child.mBottomBounds.BottomRight.y);
                        self.ConnectBoundsV(area, firstParent.mWin.Area, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    }
                    else
                    {
                        ConnectSims(child, firstParent, true, true, true, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    }
                }
                stringBuilder.Append("D");
                if (firstParent != lastParent)
                {
                    Rect parentsArea = ConnectSims(firstParent, lastParent, true, true, true, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                    foreach (IMiniSimDescription parent in parentList)
                    {
                        SimTreeInfo parentSimTreeInfo = (SimTreeInfo)getSimTreeInfoMethod.Invoke(this, new object[]
                            {
                                parent
                            });
                        if (parentSimTreeInfo != firstParent && parentSimTreeInfo != lastParent)
                        {
                            Rect area = parentSimTreeInfo.mWin.Area;
                            area.TopLeft = new Vector2(area.TopLeft.x, parentsArea.TopLeft.y);
                            area.BottomRight = new Vector2(area.BottomRight.x, parentsArea.TopLeft.y);
                            self.ConnectBoundsV(area, parentSimTreeInfo.mWin.Area, new Color(FamilyTreeDialog.DEFAULT_RELATIONSHIP_COLOR));
                        }
                    }
                }
                return rect;
            }
            catch (Exception ex)
            {
                exceptionMethod.Invoke(null, new object[]
                    {
                        child,
                        null,
                        stringBuilder.ToString(),
                        ex
                    });
                return result;
            }
        }

        public Rect ConnectSims(SimTreeInfo simA, SimTreeInfo simB, bool above, bool fromHalf, bool toHalf, Color barColor)
        {
            FamilyTreeDialog self = (FamilyTreeDialog)(object)this;
            if (simA.mWin != null && simB.mWin != null && self.mThumbParentWin != null)
            {
                Rect simARect = simA.mWin.Area,
                simBRect = simB.mWin.Area;
                if (simARect.TopLeft.y > simBRect.TopLeft.y)
                {
                    return ConnectSims(simB, simA, above, toHalf, fromHalf, barColor);
                }
                if (simARect.TopLeft.y < simBRect.TopLeft.y)
                {
                    UIImage leftCornerImage = null,
                    rightCornerImage = null;
                    int leftCornerX = 0,
                    rightCornerX = 0;
                    if (simARect.TopLeft.x < simBRect.TopLeft.x)
                    {
                        leftCornerImage = above ? self.CONNECTOR_IMAGE_CORNER_TOP_LEFT : self.CONNECTOR_IMAGE_CORNER_BOTTOM_LEFT;
                        rightCornerImage = self.CONNECTOR_IMAGE_CORNER_TOP_RIGHT;
                        leftCornerX = (int)(fromHalf ? simA.mWin.Position.x + simARect.Width / 2 : simA.mWin.Position.x + 2 * simARect.Width / 3);
                        rightCornerX = (int)(toHalf ? simB.mWin.Position.x + simBRect.Width / 2 : simB.mWin.Position.x + simBRect.Width / 3);
                    }
                    else if (simARect.TopLeft.x > simBRect.TopLeft.x)
                    {
                        leftCornerImage = above ? self.CONNECTOR_IMAGE_CORNER_TOP_RIGHT : self.CONNECTOR_IMAGE_CORNER_BOTTOM_RIGHT;
                        rightCornerImage = self.CONNECTOR_IMAGE_CORNER_TOP_LEFT;
                        leftCornerX = (int)(fromHalf ? simA.mWin.Position.x + simARect.Width / 2 : simA.mWin.Position.x + simARect.Width / 3);
                        rightCornerX = (int)(toHalf ? simB.mWin.Position.x + simBRect.Width / 2 : simB.mWin.Position.x + 2 * simBRect.Width / 3);
                    }
                    if (simARect.TopLeft.x != simBRect.TopLeft.x)
                    {
                        Window leftCornerWindow = self.LoadCornerPiece(leftCornerImage),
                        rightCornerWindow = self.LoadCornerPiece(rightCornerImage);
                        leftCornerWindow.ShadeColor = barColor;
                        rightCornerWindow.ShadeColor = barColor;
                        leftCornerWindow.Position = new Vector2((float)leftCornerX - leftCornerWindow.Area.Width / 2, (int)(above ? simARect.TopLeft.y - leftCornerWindow.Area.Height + 1 : simARect.BottomRight.y - 1));
                        rightCornerWindow.Position = new Vector2((float)rightCornerX - rightCornerWindow.Area.Width / 2, leftCornerWindow.Position.y);
                        self.mThumbParentWin.AddChild(rightCornerWindow);
                        self.mThumbParentWin.AddChild(leftCornerWindow);
                        self.ConnectBoundsV(rightCornerWindow.Area, simB.mWin.Area, simARect.TopLeft.x > simBRect.TopLeft.x, barColor);
                        Rect leftCornerBounds = leftCornerWindow.Area,
                        rightCornerBounds = rightCornerWindow.Area;
                        leftCornerBounds.TopLeft = new Vector2(leftCornerBounds.TopLeft.x, leftCornerBounds.TopLeft.y - 9);
                        leftCornerBounds.BottomRight = new Vector2(leftCornerBounds.BottomRight.x, leftCornerBounds.BottomRight.y - 9);
                        rightCornerBounds.TopLeft = new Vector2(rightCornerBounds.TopLeft.x, rightCornerBounds.TopLeft.y - 9);
                        rightCornerBounds.BottomRight = new Vector2(rightCornerBounds.BottomRight.x, rightCornerBounds.BottomRight.y - 9);
                        return self.ConnectBoundsH(leftCornerBounds, rightCornerBounds, simARect.TopLeft.x > simBRect.TopLeft.x, barColor);
                    }
                    return self.ConnectBoundsV(simARect, simBRect, barColor);
                }
                if (simARect.TopLeft.x > simBRect.TopLeft.x)
                {
                    return ConnectSims(simB, simA, above, toHalf, fromHalf, barColor);
                }
                Window leftCorner = above ? self.LoadCornerPiece(self.CONNECTOR_IMAGE_CORNER_TOP_LEFT) : self.LoadCornerPiece(self.CONNECTOR_IMAGE_CORNER_BOTTOM_LEFT),
                rightCorner = above ? self.LoadCornerPiece(self.CONNECTOR_IMAGE_CORNER_TOP_RIGHT) : self.LoadCornerPiece(self.CONNECTOR_IMAGE_CORNER_BOTTOM_RIGHT);
                leftCorner.ShadeColor = barColor;
                rightCorner.ShadeColor = barColor;
                leftCorner.Position = new Vector2((int)(fromHalf ? simARect.TopLeft.x + (simARect.Width - leftCorner.Area.Width) / 2 + 4 : simARect.TopLeft.x + 2 * simARect.Width / 3 - leftCorner.Area.Width / 2), (int)(above ? simARect.TopLeft.y - leftCorner.Area.Height + 1 : simARect.BottomRight.y - 1));
                rightCorner.Position = new Vector2((int)(toHalf ? simBRect.TopLeft.x + (simBRect.Width - rightCorner.Area.Width) / 2 - 4 : simBRect.TopLeft.x + simBRect.Width / 3 - rightCorner.Area.Width / 2), leftCorner.Position.y);
                self.mThumbParentWin.AddChild(leftCorner);
                self.mThumbParentWin.AddChild(rightCorner);
                return self.ConnectBoundsH(leftCorner.Area, rightCorner.Area, above, barColor);
            }
            return Rect.Empty;
        }
    }
}
