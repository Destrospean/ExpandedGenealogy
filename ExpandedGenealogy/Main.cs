using MonoPatcherLib;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.ExpandedGenealogy
{
    [Plugin]
    public class Main
    {
        static EventListener sSimAddedListener;

        public Main()
        {
            sSimAddedListener = null;
            World.sOnWorldLoadFinishedEventHandler += OnWorldLoadFinished;
            World.sOnWorldQuitEventHandler += OnWorldQuit;
            Type nraasWoohooerType = Type.GetType("NRaas.CommonSpace.Helpers.Relationships, NRaasWoohooer");
            if (nraasWoohooerType != null)
            {
                MonoPatcher.ReplaceMethod(nraasWoohooerType.GetMethod("IsCloselyRelated", BindingFlags.Public | BindingFlags.Static), Type.GetType("Destrospean.ExpandedGenealogy.Replacements").GetMethod("IsCloselyRelated", BindingFlags.Public | BindingFlags.Static));
            }
        }
            
        public class DEBUG_AddCousin : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            public const string LocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddCousin:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddCousin>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, LocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, LocalizationKey + "Path")
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

            public const string LocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddGrandchild:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddGrandchild>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, LocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, LocalizationKey + "Path")
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

            public const string LocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddGrandparent:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddGrandparent>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, LocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, LocalizationKey + "Path")
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

            public const string LocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddNephew:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddNephew>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, LocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, LocalizationKey + "Path")
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

            public const string LocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddUncle:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_AddUncle>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, LocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, LocalizationKey + "Path")
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

            public const string LocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_ClearRelations:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, DEBUG_ClearRelations>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(target.IsFemale, LocalizationKey + "Name", actor.FirstName, target.FirstName);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, LocalizationKey + "Path")
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

        static void AddInteractions(Sim sim)
        {
            List<InteractionDefinition> interactions = new List<InteractionDefinition>()
                {
                    DEBUG_AddCousin.Singleton,
                    DEBUG_AddGrandchild.Singleton,
                    DEBUG_AddGrandparent.Singleton,
                    DEBUG_AddNephew.Singleton,
                    DEBUG_AddUncle.Singleton,
                    DEBUG_ClearRelations.Singleton
                };
            if (!sim.Interactions.Exists(interaction => interaction.InteractionDefinition.GetType() == interactions[0].GetType()))
            {
                interactions.ForEach(sim.AddInteraction);
            }
        }

        static ListenerAction OnSimAdded(Event e)
        {
            try
            {
                Sim sim = e.TargetObject as Sim;
                if (sim != null)
                {
                    AddInteractions(sim);
                    sim.Genealogy.GetGenealogyPlaceholder();
                }
            }
            catch (Exception ex)
            {
                ((IScriptErrorWindow)AppDomain.CurrentDomain.GetData("ScriptErrorWindow")).DisplayScriptError(null, ex);
            }
            return ListenerAction.Keep;
        }

        static void OnWorldLoadFinished(object sender, EventArgs e)
        {
            new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>()).ForEach(AddInteractions);
            if (Household.ActiveHousehold != null)
            {
                Common.RebuildRelationAssignments();
            }
            UpdateListeners();
        }

        static void OnWorldQuit(object sender, EventArgs e)
        {
            EventTracker.RemoveListener(sSimAddedListener);
            sSimAddedListener = null;
        }

        static void UpdateListeners()
        {
            if (sSimAddedListener != null)
            {
                EventTracker.RemoveListener(sSimAddedListener);
                sSimAddedListener = null;
            }
            sSimAddedListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, OnSimAdded);
        }
    }
}