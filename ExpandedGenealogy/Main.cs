using MonoPatcherLib;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;
using Sims3.Gameplay.Core;

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

        public class DEBUG_AddCousin : ImmediateInteraction<Sim, GameObject>
        {
            public static InteractionDefinition Singleton = new Definition();

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddCousin:";

            public class Definition : ImmediateInteractionDefinition<Sim, GameObject, DEBUG_AddCousin>
            {
                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(((Sim)target).IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.SimDescription.Genealogy.AddCousin(((Sim)Target).SimDescription.Genealogy);
                return true;
            }
        }

        public class DEBUG_AddGrandchild : ImmediateInteraction<Sim, GameObject>
        {
            public static InteractionDefinition Singleton = new Definition();

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddGrandchild:";

            public class Definition : ImmediateInteractionDefinition<Sim, GameObject, DEBUG_AddGrandchild>
            {
                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(((Sim)target).IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.SimDescription.Genealogy.AddDescendant(((Sim)Target).SimDescription.Genealogy);
                return true;
            }
        }

        public class DEBUG_AddGrandparent : ImmediateInteraction<Sim, GameObject>
        {
            public static InteractionDefinition Singleton = new Definition();

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddGrandparent:";

            public class Definition : ImmediateInteractionDefinition<Sim, GameObject, DEBUG_AddGrandparent>
            {
                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(((Sim)target).IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.SimDescription.Genealogy.AddAncestor(((Sim)Target).SimDescription.Genealogy);
                return true;
            }
        }

        public class DEBUG_AddNephew : ImmediateInteraction<Sim, GameObject>
        {
            public static InteractionDefinition Singleton = new Definition();

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddNephew:";

            public class Definition : ImmediateInteractionDefinition<Sim, GameObject, DEBUG_AddNephew>
            {
                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(((Sim)target).IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.SimDescription.Genealogy.AddDescendantOfSibling(((Sim)Target).SimDescription.Genealogy);
                return true;
            }
        }

        public class DEBUG_AddUncle : ImmediateInteraction<Sim, GameObject>
        {
            public static InteractionDefinition Singleton = new Definition();

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_AddUncle:";

            public class Definition : ImmediateInteractionDefinition<Sim, GameObject, DEBUG_AddUncle>
            {
                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(((Sim)target).IsFemale, sLocalizationKey + "Name");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.SimDescription.Genealogy.AddSiblingOfAncestor(((Sim)Target).SimDescription.Genealogy);
                return true;
            }
        }

        public class DEBUG_ClearRelations : ImmediateInteraction<Sim, GameObject>
        {
            public static InteractionDefinition Singleton = new Definition();

            public const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/DEBUG_ClearRelations:";

            public class Definition : ImmediateInteractionDefinition<Sim, GameObject, DEBUG_ClearRelations>
            {
                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString(((Sim)target).IsFemale, sLocalizationKey + "Name", actor.FirstName, ((Sim)target).FirstName);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Localization.LocalizeString(isFemale, sLocalizationKey + "Path")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (Cheats.sTestingCheatsEnabled || Tuning.kDebug) && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.SimDescription.Genealogy.ClearRelationsWith(((Sim)Target).SimDescription.Genealogy);
                return true;
            }
        }

        static void AddInteractions(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            foreach (InteractionObjectPair interaction in gameObject.Interactions)
            {
                if (interaction.InteractionDefinition.GetType() == DEBUG_AddCousin.Singleton.GetType())
                {
                    return;
                }
            }
            gameObject.AddInteraction(DEBUG_AddCousin.Singleton);
            gameObject.AddInteraction(DEBUG_AddGrandchild.Singleton);
            gameObject.AddInteraction(DEBUG_AddGrandparent.Singleton);
            gameObject.AddInteraction(DEBUG_AddNephew.Singleton);
            gameObject.AddInteraction(DEBUG_AddUncle.Singleton);
            gameObject.AddInteraction(DEBUG_ClearRelations.Singleton);
        }

        static void Init()
        {
            UpdateListeners();
        }

        static ListenerAction OnSimAdded(Event e)
        {
            try
            {
                if (e.Actor is Sim)
                {
                    AddInteractions((Sim)e.Actor);
                    ((Sim)e.Actor).Genealogy.GetGenealogyPlaceholder();
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
            Common.RebuildRelationAssignments();
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