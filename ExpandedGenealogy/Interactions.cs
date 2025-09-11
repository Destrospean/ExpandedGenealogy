using System.Collections.Generic;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.ExpandedGenealogy
{
    public class Interactions
    {
        public class DEBUG_AddCousin : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddCousin:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddCousin>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return Tuning.kShowCheatInteractions && actor != target && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.Genealogy.AddCousin(Target.Genealogy);
                return true;
            }
        }

        public class DEBUG_AddGrandchild : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddGrandchild:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddGrandchild>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return Tuning.kShowCheatInteractions && actor != target && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.Genealogy.AddDescendant(Target.Genealogy);
                return true;
            }
        }

        public class DEBUG_AddGrandparent : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddGrandparent:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddGrandparent>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return Tuning.kShowCheatInteractions && actor != target && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.Genealogy.AddAncestor(Target.Genealogy);
                return true;
            }
        }

        public class DEBUG_AddNephew : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddNephew:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddNephew>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return Tuning.kShowCheatInteractions && actor != target && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.Genealogy.AddDescendantOfSibling(Target.Genealogy);
                return true;
            }
        }

        public class DEBUG_AddUncle : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddUncle:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddUncle>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return Tuning.kShowCheatInteractions && actor != target && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.Genealogy.AddSiblingOfAncestor(Target.Genealogy);
                return true;
            }
        }

        public class DEBUG_AssignRelation : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AssignRelation";

            enum RelationTypes : int
            {
                Ancestor = 0,
                Descendant = 1,
                Sibling = 2,
                SiblingOfAncestor = 3,
                DescendantOfSibling = 4,
                Cousin = 5
            }

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AssignRelation>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, sLocalizationKey + ":Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + ":Path")
                    };
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return Tuning.kShowCheatInteractions && actor != target && !isAutonomous;
                }
            }

            public override bool Run()
            {
                RelationTypes? relationType;
                if (!TryGetRelationType(Target, out relationType))
                {
                    return false;
                }
                if (relationType == RelationTypes.Sibling)
                {
                    Actor.Genealogy.AddSibling(Target.Genealogy);
                    return true;
                }
                bool[] isFemale = relationType == RelationTypes.Descendant || relationType == RelationTypes.DescendantOfSibling ? new bool[]
                    {
                        Target.IsFemale,
                        Actor.IsFemale
                    } : new bool[]
                    {
                        Actor.IsFemale,
                        Target.IsFemale
                    };
                if (relationType == RelationTypes.Cousin)
                {
                    int? degree, timesRemoved;
                    if (!TryGetInteger(out degree, Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/DegreeDialog:Title"), Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/DegreeDialog:Prompt"), 1))
                    {
                        return false;
                    }
                    if (!TryGetInteger(out timesRemoved, Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/TimesRemovedDialog:Title"), Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/TimesRemovedDialog:Prompt"), 0))
                    {
                        return false;
                    }
                    bool? isHigherUpFamilyTree;
                    if (!TryGetDirection(Target, out isHigherUpFamilyTree))
                    {
                        return false;
                    }
                    Actor.Genealogy.AddCousin(Target.Genealogy, degree ?? 0, timesRemoved ?? 0, isHigherUpFamilyTree ?? true);
                    return true;
                }
                int? generationalDistance;
                if (!TryGetInteger(out generationalDistance, Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/GenerationalDistanceDialog:Title"), Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/GenerationalDistanceDialog:Prompt"), 0))
                {
                    return false;
                }
                switch (relationType)
                {
                    case RelationTypes.Ancestor:
                        if (generationalDistance == 0)
                        {
                            Target.Genealogy.AddChild(Actor.Genealogy);
                        }
                        Actor.Genealogy.AddAncestor(Target.Genealogy, generationalDistance ?? 0);
                        break;
                    case RelationTypes.Descendant:
                        if (generationalDistance == 0)
                        {
                            Actor.Genealogy.AddChild(Target.Genealogy);
                        }
                        Actor.Genealogy.AddDescendant(Target.Genealogy, generationalDistance ?? 0);
                        break;
                    case RelationTypes.DescendantOfSibling:
                        Actor.Genealogy.AddDescendantOfSibling(Target.Genealogy, generationalDistance ?? 0);
                        break;
                    case RelationTypes.SiblingOfAncestor:
                        Actor.Genealogy.AddSiblingOfAncestor(Target.Genealogy, generationalDistance ?? 0);
                        break;
                }
                return true;
            }

            static bool TryGetDirection(Sim target, out bool? isHigherUpInFamilyTree)
            {
                string text = ComboSelectionDialog.Show(entries: new Dictionary<string, object>
                    {
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/DirectionDialog/Options:Downward"),
                            true
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/DirectionDialog/Options:Upward"),
                            false
                        }
                    }, titleText: Localization.LocalizeString(sLocalizationKey + "/Dialogs/DirectionDialog:Title"), defaultEntry: true.ToString()) as string;
                if (text == null)
                {
                    isHigherUpInFamilyTree = null;
                    return false;
                }
                isHigherUpInFamilyTree = bool.Parse(text);
                return true;
            }

            static bool TryGetInteger(out int? integer, string titleText, string promptText, int minimum = 0)
            {
                int? result = null;
                Simulator.AddObject(new OneShotFunctionTask(() =>
                    {
                        string text = StringInputDialog.Show(titleText, promptText, minimum.ToString(), true);
                        if (!string.IsNullOrEmpty(text))
                        {
                            result = int.Parse(text);
                            if (result < minimum)
                            {
                                result = null;
                            }
                        }
                    }));
                integer = result;
                return integer != null;
            }

            static bool TryGetRelationType(Sim target, out RelationTypes? relationType)
            {
                string text = ComboSelectionDialog.Show(entries: new Dictionary<string, object>
                    {
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:Ancestor"),
                            RelationTypes.Ancestor
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:Descendant"),
                            RelationTypes.Descendant
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:Sibling"),
                            RelationTypes.Sibling
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:SiblingOfAncestor"),
                            RelationTypes.SiblingOfAncestor
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:DescendantOfSibling"),
                            RelationTypes.DescendantOfSibling
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:Cousin"),
                            RelationTypes.Cousin
                        }
                    }, titleText: Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog:Title"), defaultEntry: RelationTypes.Ancestor.ToString()) as string;
                if (text == null)
                {
                    relationType = null;
                    return false;
                }
                relationType = (RelationTypes)int.Parse(text);
                return true;
            }
        }

        public class DEBUG_ClearRelations : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_ClearRelations:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_ClearRelations>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, sLocalizationKey + "Name", actor.FirstName, target.FirstName);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return Tuning.kShowCheatInteractions && actor != target && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.Genealogy.ClearRelationsWith(Target.Genealogy);
                return true;
            }
        }
    }
}