using MonoPatcherLib;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.ExpandedGenealogy
{
    [Plugin]
    public class Main
    {
        [PersistableStatic]
        static EventListener sSimAddedListener;

        public Main()
        {
            sSimAddedListener = null;
            World.sOnWorldLoadFinishedEventHandler += OnWorldLoadFinished;
            World.sOnWorldQuitEventHandler += OnWorldQuit;
        }

        public class DEBUG_AddCousin : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddCousin:";

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
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && actor != target && !isAutonomous;
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

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddGrandchild:";

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
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && actor != target && !isAutonomous;
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

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddGrandparent:";

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
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && actor != target && !isAutonomous;
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

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddNephew:";

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
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && actor != target && !isAutonomous;
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

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddUncle:";

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
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && actor != target && !isAutonomous;
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

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_ClearRelations:";

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
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && actor != target && !isAutonomous;
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
            if (sim != null && !sim.Interactions.Exists(interaction => interaction.InteractionDefinition.GetType() == DEBUG_AddCousin.Singleton.GetType()))
            {
                sim.AddInteraction(DEBUG_AddCousin.Singleton);
                sim.AddInteraction(DEBUG_AddGrandchild.Singleton);
                sim.AddInteraction(DEBUG_AddGrandparent.Singleton);
                sim.AddInteraction(DEBUG_AddNephew.Singleton);
                sim.AddInteraction(DEBUG_AddUncle.Singleton);
                sim.AddInteraction(DEBUG_ClearRelations.Singleton);
            }
        }

        static void Init()
        {
            UpdateListeners();
        }

        static ListenerAction OnSimAdded(Event e)
        {
            try
            {
                if (e.TargetObject is Sim)
                {
                    AddInteractions((Sim)e.TargetObject);
                    ((Sim)e.TargetObject).Genealogy.GetGenealogyPlaceholder();
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
            Init();
            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                AddInteractions(sim);
            }
            if (Household.ActiveHousehold != null)
            {
                Common.RebuildRelationAssignments();
            }
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