using System;
using System.Reflection;
using MonoPatcherLib;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.SimIFace;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
}

namespace Destrospean.ExpandedGenealogy
{
    [Plugin]
    public class Main
    {
        static EventListener sSimInstantiatedListener;

        public Main()
        {
            sSimInstantiatedListener = null;
            World.sOnWorldLoadFinishedEventHandler += (sender, e) =>
                {
                    foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
                    {
                        AddInteractions(sim);
                    }
                    if (Household.ActiveHousehold != null)
                    {
                        GenealogyExtended.RebuildRelationAssignments();
                    }
                    sSimInstantiatedListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, evt =>
                        {
                            try
                            {
                                Sim sim = evt.TargetObject as Sim;
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
                        });
                };
            World.sOnWorldQuitEventHandler += (sender, e) =>
                {
                    GenealogyPlaceholder.GenealogyPlaceholders.Clear();
                    EventTracker.RemoveListener(sSimInstantiatedListener);
                    sSimInstantiatedListener = null;
                };
            Type nraasWoohooerType = Type.GetType("NRaas.CommonSpace.Helpers.Relationships, NRaasWoohooer");
            if (nraasWoohooerType != null)
            {
                MonoPatcher.ReplaceMethod(nraasWoohooerType.GetMethod("IsCloselyRelated", BindingFlags.Public | BindingFlags.Static), Type.GetType("Destrospean.ExpandedGenealogy.Replacements").GetMethod("IsCloselyRelated", BindingFlags.Public | BindingFlags.Static));
            }
        }

        static void AddInteractions(Sim sim)
        {
            if (!sim.Interactions.Exists(interaction => interaction.InteractionDefinition.GetType() == Interactions.DEBUG_AddCousin.Singleton.GetType()))
            {
                sim.AddInteraction(Interactions.DEBUG_AddCousin.Singleton);
                sim.AddInteraction(Interactions.DEBUG_AddGrandchild.Singleton);
                sim.AddInteraction(Interactions.DEBUG_AddGrandparent.Singleton);
                sim.AddInteraction(Interactions.DEBUG_AddNephew.Singleton);
                sim.AddInteraction(Interactions.DEBUG_AddUncle.Singleton);
                sim.AddInteraction(Interactions.DEBUG_AssignRelation.Singleton);
                sim.AddInteraction(Interactions.DEBUG_ClearRelations.Singleton);
            }
        }
    }
}