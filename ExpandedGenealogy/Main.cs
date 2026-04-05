using System;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.SimIFace;

namespace Destrospean.ExpandedGenealogy
{
    [MonoPatcherLib.Plugin]
    public class Main
    {
        static Main()
        {
            EventListener simInstantiatedListener = null;
            World.sOnWorldLoadFinishedEventHandler += (sender, e) =>
                {
                    foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
                    {
                        AddInteractions(sim);
                    }
                    if (Sims3.Gameplay.CAS.Household.ActiveHousehold != null)
                    {
                        GenealogyExtended.RebuildRelationAssignments();
                    }
                    simInstantiatedListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, evt =>
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
                    EventTracker.RemoveListener(simInstantiatedListener);
                    simInstantiatedListener = null;
                };
            Type nraasWoohooerRelationshipsType = Type.GetType("NRaas.CommonSpace.Helpers.Relationships, NRaasWoohooer");
            if (nraasWoohooerRelationshipsType != null)
            {
                MonoPatcherLib.MonoPatcher.ReplaceMethod(nraasWoohooerRelationshipsType.GetMethod("IsCloselyRelated", (System.Reflection.BindingFlags)0x18), Type.GetType("Destrospean.ExpandedGenealogy.Replacements").GetMethod("IsCloselyRelated", (System.Reflection.BindingFlags)0x18));
            }
        }

        static void AddInteractions(Sim sim)
        {
            if (sim != null)
            {
                sim.AddInteraction(Interactions.AssignRelation.Singleton, true);
                sim.AddInteraction(Interactions.ClearRelations.Singleton, true);
            }
        }
    }
}
