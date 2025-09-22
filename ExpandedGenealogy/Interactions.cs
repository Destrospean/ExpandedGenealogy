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
        public class AssignRelation : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/AssignRelation";

            enum RelationTypes : uint
            {
                Ancestor,
                Descendant,
                Sibling,
                SiblingOfAncestor,
                DescendantOfSibling,
                Cousin,
                CousinOfAncestor,
                DescendantOfCousin
            }

            class DummyComparer : IComparer<string>
            {
                public int Compare(string a, string b)
                {
                    return 1;
                }
            }

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, AssignRelation>
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
                    return Tuning.kShowCheatInteractions && actor != target && (actor.SimDescription.Species == target.SimDescription.Species || actor.IsADogSpecies && target.IsADogSpecies) && !isAutonomous;
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
                bool[] isFemale;
                Sim[] sims;
                if (relationType.ToString().Contains("Descendant"))
                {
                    isFemale = new bool[]
                        {
                            Target.IsFemale,
                            Actor.IsFemale
                        };
                    sims = new Sim[]
                        {
                            Target,
                            Actor
                        };
                }
                else
                {
                    isFemale = new bool[]
                        {
                            Actor.IsFemale,
                            Target.IsFemale
                        };
                    sims = new Sim[]
                        {
                            Actor,
                            Target
                        };
                }
                if (relationType.ToString().Contains("Cousin"))
                {
                    int? degree, timesRemoved = null;
                    if (!TryGetInteger(out degree, Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/DegreeDialog:Title"), Localization.LocalizeString(new bool[]
                        {
                            isFemale[1],
                            isFemale[0]
                        }, sLocalizationKey + "/Dialogs/DegreeDialog:Prompt", sims[1], sims[0]), 1))
                    {
                        return false;
                    }
                    if (relationType != RelationTypes.Cousin && !TryGetInteger(out timesRemoved, Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/TimesRemovedDialog:Title"), Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/TimesRemovedDialog:Prompt", sims[0], sims[1], Common.PlayerLanguage.GetNthUncleDegreeString(degree ?? 0)), 1))
                    {
                        return false;
                    }
                    Actor.Genealogy.AddCousin(Target.Genealogy, degree ?? 0, timesRemoved ?? 0, relationType != RelationTypes.CousinOfAncestor);
                    return true;
                }
                int? generationalDistance;
                if (!TryGetInteger(out generationalDistance, Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/GenerationalDistanceDialog:Title"), Localization.LocalizeString(isFemale, sLocalizationKey + "/Dialogs/GenerationalDistanceDialog:Prompt" + (relationType == RelationTypes.Ancestor || relationType == RelationTypes.Descendant ? "Lineal" : "Collateral"), sims[0], sims[1])))
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

            static bool TryGetInteger(out int? integer, string titleText, string promptText, int minimum = 0)
            {
                integer = null;
                string text = StringInputDialog.Show(titleText, promptText, minimum.ToString(), true);
                if (!string.IsNullOrEmpty(text))
                {
                    integer = int.Parse(text);
                    if (integer < minimum)
                    {
                        integer = null;
                    }
                }
                return integer != null;
            }

            static bool TryGetRelationType(Sim target, out RelationTypes? relationType)
            {
                string text = Dialogs.ComboSelectionDialog.Show(entries: new SortedDictionary<string, object>(new DummyComparer())
                    {
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:Ancestor"),
                            ((uint)RelationTypes.Ancestor).ToString()
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:Descendant"),
                            ((uint)RelationTypes.Descendant).ToString()
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:Sibling"),
                            ((uint)RelationTypes.Sibling).ToString()
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:SiblingOfAncestor"),
                            ((uint)RelationTypes.SiblingOfAncestor).ToString()
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:DescendantOfSibling"),
                            ((uint)RelationTypes.DescendantOfSibling).ToString()
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:Cousin"),
                            ((uint)RelationTypes.Cousin).ToString()
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:CousinOfAncestor"),
                            ((uint)RelationTypes.CousinOfAncestor).ToString()
                        },
                        {
                            Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog/Options:DescendantOfCousin"),
                            ((uint)RelationTypes.DescendantOfCousin).ToString()
                        }
                    }, titleText: Localization.LocalizeString(target.IsFemale, sLocalizationKey + "/Dialogs/RelationTypeDialog:Title"), defaultEntry: ((uint)RelationTypes.Ancestor).ToString()) as string;
                if (text == null)
                {
                    relationType = null;
                    return false;
                }
                relationType = (RelationTypes)uint.Parse(text);
                return true;
            }
        }

        public class ClearRelations : ImmediateInteraction<Sim, Sim>
        {
            public static InteractionDefinition Singleton = new Definition();

            const string sLocalizationKey = "Destrospean/ExpandedGenealogy/Interactions/ClearRelations:";

            [DoesntRequireTuning]
            public class Definition : ImmediateInteractionDefinition<Sim, Sim, ClearRelations>
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
                    return Tuning.kShowCheatInteractions && actor != target && (actor.SimDescription.Species == target.SimDescription.Species || actor.IsADogSpecies && target.IsADogSpecies) && !isAutonomous;
                }
            }

            public override bool Run()
            {
                Actor.Genealogy.ClearRelationsWith(Target.Genealogy);
                Actor.Genealogy.RemoveDirectRelation(Target.Genealogy);
                return true;
            }
        }
    }
}