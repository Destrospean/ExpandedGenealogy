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

        public static bool IsStepCousin(Genealogy sim1, Genealogy sim2)
        {
            foreach (Genealogy parent in sim1.Parents)
            {
                foreach (Genealogy stepParent in sim2.GetStepParents())
                {
                    if (Genealogy.IsSibling(parent, stepParent) || (Tuning.kAccumulateDistantStepRelatives && Genealogy.IsStepSibling(parent, stepParent) && parent != stepParent))
                    {
                        return true;
                    }
                }
            }
            foreach (Genealogy stepParent1 in sim1.GetStepParents())
            {
                foreach (Genealogy parent in sim2.Parents)
                {
                    if (Genealogy.IsSibling(stepParent1, parent) || (Tuning.kAccumulateDistantStepRelatives && Genealogy.IsStepSibling(stepParent1, parent) && stepParent1 != parent))
                    {
                        return true;
                    }
                }
                if (Tuning.kAccumulateDistantStepRelatives)
                {
                    foreach (Genealogy stepParent2 in sim2.GetStepParents())
                    {
                        if (Genealogy.IsSibling(stepParent1, stepParent2) || (Genealogy.IsStepSibling(stepParent1, stepParent2) && stepParent1 != stepParent2))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsStepGrandchild(Genealogy stepGrandchild, Genealogy stepGrandparent)
        {
            return IsStepGrandparent(stepGrandparent, stepGrandchild);
        }

        public static bool IsStepGrandparent(Genealogy stepGrandparent, Genealogy stepGrandchild)
        {
            Common.AncestorInfo stepAncestorInfo = Common.GetStepAncestorInfo(stepGrandchild, stepGrandparent);
            return stepAncestorInfo != null && stepAncestorInfo.GenerationalDistance == 1;
        }

        public static bool IsStepGreatGrandchild(Genealogy stepGreatGrandchild, Genealogy stepGreatGrandparent)
        {
            return IsStepGreatGrandparent(stepGreatGrandparent, stepGreatGrandchild);
        }

        public static bool IsStepGreatGrandparent(Genealogy stepGreatGrandparent, Genealogy stepGreatGrandchild)
        {
            Common.AncestorInfo stepAncestorInfo = Common.GetStepAncestorInfo(stepGreatGrandchild, stepGreatGrandparent);
            return stepAncestorInfo != null && stepAncestorInfo.GenerationalDistance == 2;
        }

        public static bool IsStepHalfCousin(Genealogy sim1, Genealogy sim2)
        {
            foreach (Genealogy parent in sim1.Parents)
            {
                foreach (Genealogy stepParent in sim2.GetStepParents())
                {
                    if (Genealogy.IsHalfSibling(parent, stepParent))
                    {
                        return true;
                    }
                }
            }
            foreach (Genealogy stepParent1 in sim1.GetStepParents())
            {
                foreach (Genealogy parent in sim2.Parents)
                {
                    if (Genealogy.IsHalfSibling(stepParent1, parent))
                    {
                        return true;
                    }
                }
                if (Tuning.kAccumulateDistantStepRelatives)
                {
                    foreach (Genealogy stepParent2 in sim2.GetStepParents())
                    {
                        if (Genealogy.IsHalfSibling(stepParent1, stepParent2))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsStepHalfNephew(Genealogy stepNephew, Genealogy stepUncle)
        {
            return IsStepHalfUncle(stepUncle, stepNephew);
        }

        public static bool IsStepHalfUncle(Genealogy stepUncle, Genealogy stepNephew)
        {
            foreach (Genealogy stepParent in stepNephew.GetStepParents())
            {
                foreach (Genealogy sibling in stepParent.Siblings)
                {
                    if (sibling == stepUncle || (sibling.Spouse == stepUncle && sibling.PartnerType == PartnerType.Marriage))
                    {
                        return Genealogy.IsHalfSibling(stepParent, sibling);
                    }
                }
            }
            return false;
        }

        public static bool IsStepNephew(Genealogy stepNephew, Genealogy stepUncle)
        {
            return IsStepUncle(stepUncle, stepNephew);
        }

        public static bool IsStepSiblingInLaw(Genealogy sim1, Genealogy sim2)
        {
            if (sim1.Spouse != null && sim1.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy stepSibling in sim1.Spouse.GetStepSiblings())
                {
                    if (stepSibling == sim2 || (stepSibling.Spouse == sim2 && stepSibling.PartnerType == PartnerType.Marriage))
                    {
                        return true;
                    }
                }
            }
            if (sim2.Spouse != null && sim2.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy stepSibling in sim2.Spouse.GetStepSiblings())
                {
                    if (stepSibling == sim1 || (stepSibling.Spouse == sim1 && stepSibling.PartnerType == PartnerType.Marriage))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsStepUncle(Genealogy stepUncle, Genealogy stepNephew)
        {
            foreach (Genealogy parent in stepNephew.Parents)
            {
                foreach (Genealogy stepSibling in parent.GetStepSiblings())
                {
                    if (stepSibling == stepUncle || (stepSibling.Spouse == stepUncle && stepSibling.PartnerType == PartnerType.Marriage))
                    {
                        return true;
                    }
                }
            }
            foreach (Genealogy stepParent in stepNephew.GetStepParents())
            {
                foreach (Genealogy sibling in stepParent.Siblings)
                {
                    if (sibling == stepUncle || (sibling.Spouse == stepUncle && sibling.PartnerType == PartnerType.Marriage))
                    {
                        return true;
                    }
                }
                if (Tuning.kAccumulateDistantStepRelatives)
                {
                    foreach (Genealogy stepSibling in stepParent.GetStepSiblings())
                    {
                        if (stepSibling == stepUncle || (stepSibling.Spouse == stepUncle && stepSibling.PartnerType == PartnerType.Marriage))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        [TypePatch(typeof(Genealogy))]
        public class GenealogyPatch
        {
            public static bool IsHalfSibling(Genealogy sim1, Genealogy sim2)
            {
                int sharedParentCount = 0, totalParentCount = sim1.Parents.Count + sim2.Parents.Count;
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
                return Genealogy.IsSibling(sim1, sim2) && sharedParentCount * 2 != totalParentCount;
            }

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
                if (distantRelationInfo != null && distantRelationInfo.Degree < (uint)Tuning.kMinDegreeCousinsToAllowRomance && distantRelationInfo.TimesRemoved < (uint)Tuning.kMinTimesRemovedCousinsToAllowRomance && !(isHalfRelative && Tuning.kAllowRomanceForDistantHalfRelatives))
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
                PlayerLanguage playerLanguage = Activator.CreateInstance(Type.GetType("Destrospean.Lang." + Localization.LocalizeString("Destrospean/ExpandedGenealogy:LanguageCode"))) as PlayerLanguage;
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
                else if (IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) && Tuning.kShowDistantHalfRelatives && !Tuning.kShowDistantHalfRelativesAsFullRelatives)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:HalfSiblingInLaw");
                }
                else if (Genealogy.IsSiblingInLaw(other.Genealogy, self.Genealogy) && (!IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) || Tuning.kShowDistantHalfRelativesAsFullRelatives))
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
                else if (IsHalfCousin(other.Genealogy, self.Genealogy) && Tuning.kShowDistantHalfRelatives && !Tuning.kShowDistantHalfRelativesAsFullRelatives && Tuning.kShow1stCousinsAsCousins)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:HalfCousin");
                }
                else if (Genealogy.IsCousin(other.Genealogy, self.Genealogy) && Tuning.kShow1stCousinsAsCousins && (!IsHalfCousin(other.Genealogy, self.Genealogy) || Tuning.kShowDistantHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Cousin");
                }
                else if (IsHalfUncle(other.Genealogy, self.Genealogy) && Tuning.kShowDistantHalfRelatives && !Tuning.kShowDistantHalfRelativesAsFullRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:HalfUncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthHalfCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", "");
                }
                else if (Genealogy.IsFatherSideUncle(other.Genealogy, self.Genealogy) && Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!IsHalfUncle(other.Genealogy, self.Genealogy) || Tuning.kShowDistantHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Uncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", "");
                }
                else if (Genealogy.IsMotherSideUncle(other.Genealogy, self.Genealogy) && Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!IsHalfUncle(other.Genealogy, self.Genealogy) || Tuning.kShowDistantHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:UncleMothersSide") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", "");
                }
                else if (IsHalfNephew(other.Genealogy, self.Genealogy) && Tuning.kShowDistantHalfRelatives && !Tuning.kShowDistantHalfRelativesAsFullRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:HalfNephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthHalfCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", "");
                }
                else if (Genealogy.IsNephew(other.Genealogy, self.Genealogy) && (!IsHalfNephew(other.Genealogy, self.Genealogy) || Tuning.kShowDistantHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Nephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", "");
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
                else if (IsStepSiblingInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepSiblingInLaw");
                }
                else if (IsStepHalfCousin(other.Genealogy, self.Genealogy) && Tuning.kShowDistantHalfRelatives && !Tuning.kShowDistantHalfRelativesAsFullRelatives && Tuning.kShowDistantStepRelatives && Tuning.kShow1stCousinsAsCousins)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepHalfCousin");
                }
                else if (IsStepCousin(other.Genealogy, self.Genealogy) && Tuning.kShowDistantStepRelatives && Tuning.kShow1stCousinsAsCousins && (!IsStepHalfCousin(other.Genealogy, self.Genealogy) || Tuning.kShowDistantHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepCousin");
                }
                else if (IsStepHalfUncle(other.Genealogy, self.Genealogy) && Tuning.kShowDistantHalfRelatives && !Tuning.kShowDistantHalfRelativesAsFullRelatives && Tuning.kShowDistantStepRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepHalfUncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthHalfCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:Step"));
                }
                else if (IsStepUncle(other.Genealogy, self.Genealogy) && Tuning.kShowDistantStepRelatives && (!IsStepHalfUncle(other.Genealogy, self.Genealogy) || Tuning.kShowDistantHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepUncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:Step"));
                }
                else if (IsStepHalfNephew(other.Genealogy, self.Genealogy) && Tuning.kShowDistantHalfRelatives && !Tuning.kShowDistantHalfRelativesAsFullRelatives && Tuning.kShowDistantStepRelatives)
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepHalfNephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthHalfCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:Step"));
                }
                else if (IsStepNephew(other.Genealogy, self.Genealogy) && Tuning.kShowDistantStepRelatives && (!IsStepHalfNephew(other.Genealogy, self.Genealogy) || Tuning.kShowDistantHalfRelativesAsFullRelatives))
                {
                    text = !playerLanguage.HasNthUncles || Tuning.kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepNephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:OrdinalSuffixNoun1"), "", "", Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:Step"));
                }
                else if (IsStepGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepGrandparent");
                }
                else if (IsStepGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepGrandchild");
                }
                else if (IsStepGreatGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepGGP");
                }
                else if (IsStepGreatGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:StepGGC");
                }
                // Check if the selected Sim is married to one of the target's descendants
                else if (self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.Spouse.IsAncestor(other.Genealogy) && self.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    if (Genealogy.IsGrandparent(other.Genealogy, self.Genealogy.Spouse))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:GrandparentInLaw");
                    }
                    else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy.Spouse))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:GGPInLaw");
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
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:GrandchildInLaw");
                    }
                    else if (Genealogy.IsGreatGrandchild(other.Genealogy.Spouse, self.Genealogy))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:GGCInLaw");
                    }
                    else
                    {
                        text = playerLanguage.GetDescendantString(other.IsFemale, self.Genealogy, other.Genealogy.Spouse, true);
                    }
                }
                else
                {
                    if (Tuning.kShowDistantStepRelatives)
                    {
                        Common.DistantRelationInfo distantStepRelationInfo = Common.CalculateDistantStepRelation(other.Genealogy, self.Genealogy);
                        if (distantStepRelationInfo == null)
                        {
                            // Check if the target is married to someone other than the selected Sim
                            if (other.Genealogy.Spouse != null && other.Genealogy.Spouse != self.Genealogy && other.Genealogy.PartnerType == PartnerType.Marriage)
                            {
                                distantStepRelationInfo = Common.CalculateDistantStepRelation(other.Genealogy.Spouse, self.Genealogy);
                            }
                            if (distantStepRelationInfo == null)
                            {
                                // Check if the selected Sim is married to someone other than the target
                                if (self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.PartnerType == PartnerType.Marriage)
                                {
                                    distantStepRelationInfo = Common.CalculateDistantStepRelation(other.Genealogy, self.Genealogy.Spouse);
                                }
                                // Check if the selected Sim is married to a (step)sibling of one of the target's (step-)ancestors
                                if (distantStepRelationInfo != null && distantStepRelationInfo.Degree == 0 && distantStepRelationInfo.ClosestDescendant == self.Genealogy.Spouse)
                                {
                                    text = playerLanguage.GetDistantRelationString(other.Genealogy, distantStepRelationInfo, true);
                                }
                            }
                            // Check if the target is married to a (step)sibling of one of the selected Sim's (step-)ancestors
                            else if (distantStepRelationInfo.Degree == 0 && distantStepRelationInfo.ClosestDescendant == other.Genealogy.Spouse)
                            {
                                text = playerLanguage.GetDistantRelationString(other.IsFemale, other.Genealogy.Spouse, distantStepRelationInfo, true);
                            }
                        }
                        else
                        {
                            /* Get a relation name that is valid if the target is a collateral step-descendant or type of step-cousin
                             * of the selected Sim or if the selected Sim is a collateral step-descendant of the target
                             */
                            text = playerLanguage.GetDistantRelationString(other.Genealogy, distantStepRelationInfo, true);
                        }
                        // This if-statement block is necessary for when the selected Sim and the target do not share an ancestor in the game but each has an ancestor who is a sibling of the other.
                        if (distantStepRelationInfo == null)
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
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/ExpandedGenealogy:FamilyBot");
                    }
                }
                return text.Replace("  ", " ").Capitalize();
            }
        }
    }
}