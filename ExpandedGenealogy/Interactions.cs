using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
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
                Actor.SimDescription.Genealogy.AddCousin(Target.SimDescription.Genealogy);
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
                Actor.SimDescription.Genealogy.AddDescendant(Target.SimDescription.Genealogy);
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
                Actor.SimDescription.Genealogy.AddAncestor(Target.SimDescription.Genealogy);
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
                Actor.SimDescription.Genealogy.AddDescendantOfSibling(Target.SimDescription.Genealogy);
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
                Actor.SimDescription.Genealogy.AddSiblingOfAncestor(Target.SimDescription.Genealogy);
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
                Actor.SimDescription.Genealogy.ClearRelationsWith(Target.SimDescription.Genealogy);
                return true;
            }
        }
    }
}