using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using System.Collections.Generic;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
}

namespace Destrospean.ExpandedGenealogy
{
    public static class Common
    {
        public static Dictionary<ulong, GenealogyPlaceholder> GenealogyPlaceholders
        {
            get
            {
                return sGenealogyPlaceholders;
            }
        }

        static Dictionary<ulong, GenealogyPlaceholder> sGenealogyPlaceholders = new Dictionary<ulong, GenealogyPlaceholder>();

        [PersistableStatic(true)]
        static List<Dictionary<string, object>> sRelationAssignments = new List<Dictionary<string, object>>();

        public static void AddAncestor(this Genealogy descendant, Genealogy ancestor, int generationalDistance = 1, bool addRelationAssignment = true)
        {
            if (addRelationAssignment)
            {
                sRelationAssignments.Add(new Dictionary<string, object>()
                    {
                        {
                            "Relation Type",
                            "Descendant"
                        },
                        {
                            "Generational Distance",
                            generationalDistance
                        },
                        {
                            "Sim A",
                            descendant
                        },
                        {
                            "Sim B",
                            ancestor
                        }
                    });
            }
            List<GenealogyPlaceholder> genealogyPlaceholders = new List<GenealogyPlaceholder>();
            genealogyPlaceholders.Add(descendant.GetGenealogyPlaceholder());
            for (int i = 0; i < generationalDistance; i++)
            {
                GenealogyPlaceholder genealogyPlaceholder = new GenealogyPlaceholder();
                sGenealogyPlaceholders.Add(genealogyPlaceholder.Id, genealogyPlaceholder);
                genealogyPlaceholders.Add(genealogyPlaceholder);
            }
            genealogyPlaceholders.Add(ancestor.GetGenealogyPlaceholder());
            for (int i = 0; i < genealogyPlaceholders.Count - 1; i++)
            {
                sGenealogyPlaceholders[genealogyPlaceholders[i].Id].AddParent(sGenealogyPlaceholders[genealogyPlaceholders[i + 1].Id]);
            }
        }

        public static void AddCousin(this Genealogy self, Genealogy other, int degree = 1, int timesRemoved = 0, bool isHigherUpFamilyTree = true, bool addRelationAssignment = true)
        {
            if (addRelationAssignment)
            {
                sRelationAssignments.Add(new Dictionary<string, object>()
                    {
                        {
                            "Relation Type",
                            "Cousin"
                        },
                        {
                            "Generational Distance",
                            timesRemoved
                        },
                        {
                            "Degree",
                            degree
                        },
                        {
                            "Sim A",
                            self
                        },
                        {
                            "Sim B",
                            other
                        },
                        {
                            "Sim A Is Higher Up Family Tree",
                            isHigherUpFamilyTree
                        }
                    });
            }
            List<GenealogyPlaceholder> sim1GenealogyPlaceholders = new List<GenealogyPlaceholder>(), sim2GenealogyPlaceholders = new List<GenealogyPlaceholder>();
            sim1GenealogyPlaceholders.Add(self.GetGenealogyPlaceholder());
            for (int i = 0; i < degree + (isHigherUpFamilyTree ? 0 : timesRemoved); i++)
            {
                GenealogyPlaceholder genealogyPlaceholder = new GenealogyPlaceholder();
                sGenealogyPlaceholders.Add(genealogyPlaceholder.Id, genealogyPlaceholder);
                sim1GenealogyPlaceholders.Add(genealogyPlaceholder);
            }
            sim2GenealogyPlaceholders.Add(other.GetGenealogyPlaceholder());
            for (int i = 0; i < degree + (isHigherUpFamilyTree ? timesRemoved : 0); i++)
            {
                GenealogyPlaceholder genealogyPlaceholder = new GenealogyPlaceholder();
                sGenealogyPlaceholders.Add(genealogyPlaceholder.Id, genealogyPlaceholder);
                sim2GenealogyPlaceholders.Add(genealogyPlaceholder);
            }
            GenealogyPlaceholder sharedAncestorGenealogyPlaceholder = new GenealogyPlaceholder();
            sGenealogyPlaceholders.Add(sharedAncestorGenealogyPlaceholder.Id, sharedAncestorGenealogyPlaceholder);
            sGenealogyPlaceholders[sim1GenealogyPlaceholders[sim1GenealogyPlaceholders.Count - 1].Id].AddParent(sGenealogyPlaceholders[sharedAncestorGenealogyPlaceholder.Id]);
            sGenealogyPlaceholders[sim2GenealogyPlaceholders[sim2GenealogyPlaceholders.Count - 1].Id].AddParent(sGenealogyPlaceholders[sharedAncestorGenealogyPlaceholder.Id]);
            for (int i = 0; i < sim1GenealogyPlaceholders.Count - 1; i++)
            {
                sGenealogyPlaceholders[sim1GenealogyPlaceholders[i].Id].AddParent(sGenealogyPlaceholders[sim1GenealogyPlaceholders[i + 1].Id]);
            }
            for (int i = 0; i < sim2GenealogyPlaceholders.Count - 1; i++)
            {
                sGenealogyPlaceholders[sim2GenealogyPlaceholders[i].Id].AddParent(sGenealogyPlaceholders[sim2GenealogyPlaceholders[i + 1].Id]);
            }
        }

        public static void AddDescendant(this Genealogy ancestor, Genealogy descendant, int generationalDistance = 1, bool addRelationAssignment = true)
        {
            descendant.AddAncestor(ancestor, generationalDistance, addRelationAssignment);
        }

        public static void AddDescendantOfSibling(this Genealogy siblingOfAncestor, Genealogy descendantOfSibling, int generationalDistance = 0, bool addRelationAssignment = true)
        {
            descendantOfSibling.AddSiblingOfAncestor(siblingOfAncestor, generationalDistance, addRelationAssignment);
        }

        public static void AddSiblingOfAncestor(this Genealogy descendantOfSibling, Genealogy siblingOfAncestor, int generationalDistance = 0, bool addRelationAssignment = true)
        {
            int relationAssignmentIndex = -1;
            if (addRelationAssignment)
            {
                relationAssignmentIndex = sRelationAssignments.FindIndex(relationAssignment => (string)relationAssignment["Relation Type"] == "Descendant Of Sibling" && relationAssignment["Sim B"] == descendantOfSibling);
                sRelationAssignments.Insert(relationAssignmentIndex == -1 ? sRelationAssignments.Count : relationAssignmentIndex, new Dictionary<string, object>()
                    {
                        {
                            "Relation Type",
                            "Descendant Of Sibling"
                        },
                        {
                            "Generational Distance",
                            generationalDistance
                        },
                        {
                            "Sim A",
                            descendantOfSibling
                        },
                        {
                            "Sim B",
                            siblingOfAncestor
                        }
                    });
            }
            siblingOfAncestor.AddCousin(descendantOfSibling, 0, generationalDistance + 1, true, false);
            foreach (GenealogyPlaceholder ancestor in siblingOfAncestor.GetGenealogyPlaceholder().Ancestors)
            {
                if (ancestor.Genealogy != null)
                {
                    descendantOfSibling.AddAncestor(ancestor.Genealogy, siblingOfAncestor.GetGenealogyPlaceholder().GetAncestorInfo(ancestor).GenerationalDistance + generationalDistance + 1, false);
                }
                foreach (GenealogyPlaceholder sibling in ancestor.Siblings)
                {
                    if (sibling.Genealogy != null)
                    {
                        descendantOfSibling.AddCousin(sibling.Genealogy, 0, siblingOfAncestor.GetGenealogyPlaceholder().GetAncestorInfo(ancestor).GenerationalDistance + generationalDistance + 2, false, false);
                    }
                }
            }
            if (relationAssignmentIndex > -1)
            {
                RebuildRelationAssignments();
            }
        }

        public static DistantRelationInfo CalculateDistantRelation(this Genealogy self, Genealogy other)
        {
            return self.GetGenealogyPlaceholder().CalculateDistantRelation(other.GetGenealogyPlaceholder());
        }

        public static DistantRelationInfo CalculateDistantRelation(this GenealogyPlaceholder self, GenealogyPlaceholder other)
        {
            List<DistantRelationInfo> distantRelationInfoList = self.CalculateDistantRelations(other);
            if (distantRelationInfoList.Count == 0)
            {
                return null;
            }
            return distantRelationInfoList[0];
        }

        public static List<DistantRelationInfo> CalculateDistantRelations(this Genealogy self, Genealogy other)
        {
            return self.GetGenealogyPlaceholder().CalculateDistantRelations(other.GetGenealogyPlaceholder());
        }

        public static List<DistantRelationInfo> CalculateDistantRelations(this GenealogyPlaceholder self, GenealogyPlaceholder other)
        {
            List<DistantRelationInfo> cachedDistantRelationInfoList, distantRelationInfoList = new List<DistantRelationInfo>();
            if (self.IsAncestor(other) || other.IsAncestor(self))
            {
                return distantRelationInfoList;
            }
            if (self.CachedDistantRelationInfoLists.TryGetValue(other, out cachedDistantRelationInfoList))
            {
                return cachedDistantRelationInfoList;
            }
            foreach (GenealogyPlaceholder ancestor1 in self.Ancestors)
            {
                foreach (GenealogyPlaceholder ancestor2 in other.Ancestors)
                {
                    AncestorInfo ancestor1Info = self.GetAncestorInfo(ancestor1), ancestor2Info = other.GetAncestorInfo(ancestor2);
                    if (ancestor1.IsSibling(ancestor2))
                    {
                        if (ancestor1Info.GenerationalDistance < ancestor2Info.GenerationalDistance)
                        {
                            distantRelationInfoList.TryAddDistantRelationInfo(ancestor1Info.GenerationalDistance + 1, ancestor2Info.GenerationalDistance - ancestor1Info.GenerationalDistance, ancestor1, ancestor2, self);
                        }
                        else
                        {
                            distantRelationInfoList.TryAddDistantRelationInfo(ancestor2Info.GenerationalDistance + 1, ancestor1Info.GenerationalDistance - ancestor2Info.GenerationalDistance, ancestor1, ancestor2, other);
                        }
                    }
                }
            }
            distantRelationInfoList.Sort((a, b) =>
                {
                    int aCoRelPow = 2 * a.Degree + a.TimesRemoved + (Genealogy.IsHalfSibling(a.ThroughWhichChildren[0].Genealogy, a.ThroughWhichChildren[1].Genealogy) ? 2 : 1), bCoRelPow = 2 * b.Degree + b.TimesRemoved + (Genealogy.IsHalfSibling(b.ThroughWhichChildren[0].Genealogy, b.ThroughWhichChildren[1].Genealogy) ? 2 : 1);
                    if (aCoRelPow == bCoRelPow && a.Degree == b.Degree && a.TimesRemoved == b.TimesRemoved)
                    {
                        return 0;
                    }
                    if (aCoRelPow > bCoRelPow || aCoRelPow == bCoRelPow && (a.Degree > b.Degree || a.Degree == b.Degree && a.TimesRemoved > b.TimesRemoved))
                    {
                        return 1;
                    }
                    return -1;
                });
            self.CachedDistantRelationInfoLists[other] = distantRelationInfoList;
            return distantRelationInfoList;
        }

        public static string Capitalize(this string text)
        {
            return text.Length > 1 ? text.Substring(0, 1).ToUpper() + text.Substring(1) : text.ToUpper();
        }

        public static void ClearCachesInGenealogyPlaceholders()
        {
            foreach (GenealogyPlaceholder genealogyPlaceholder in sGenealogyPlaceholders.Values)
            {
                genealogyPlaceholder.CachedAncestorInfoLists.Clear();
                genealogyPlaceholder.CachedDistantRelationInfoLists.Clear();
                genealogyPlaceholder.CachedSiblingOfAncestorInfoLists.Clear();
                genealogyPlaceholder.Ancestors = null;
                genealogyPlaceholder.Parents = null;
                genealogyPlaceholder.Siblings = null;
            }
        }

        public static void ClearRelationsWith(this Genealogy self, Genealogy other)
        {
            sRelationAssignments.RemoveAll(relationAssignment => relationAssignment["Sim A"] == self && relationAssignment["Sim B"] == other || relationAssignment["Sim A"] == other && relationAssignment["Sim B"] == self);
            RebuildRelationAssignments();
        }

        public static AncestorInfo GetAncestorInfo(this Genealogy descendant, Genealogy ancestor)
        {
            return descendant.GetGenealogyPlaceholder().GetAncestorInfo(ancestor.GetGenealogyPlaceholder());
        }

        public static AncestorInfo GetAncestorInfo(this GenealogyPlaceholder descendant, GenealogyPlaceholder ancestor)
        {
            List<AncestorInfo> ancestorInfoList = descendant.GetAncestorInfoList(ancestor);
            if (ancestorInfoList.Count == 0)
            {
                return null;
            }
            return ancestorInfoList[0];
        }

        public static List<AncestorInfo> GetAncestorInfoList(this Genealogy descendant, Genealogy ancestor)
        {
            return descendant.GetGenealogyPlaceholder().GetAncestorInfoList(ancestor.GetGenealogyPlaceholder());
        }

        public static List<AncestorInfo> GetAncestorInfoList(this GenealogyPlaceholder descendant, GenealogyPlaceholder ancestor)
        {
            List<AncestorInfo> ancestorInfoList = new List<AncestorInfo>(), cachedAncestorInfoList;
            if (descendant.CachedAncestorInfoLists.TryGetValue(ancestor, out cachedAncestorInfoList))
            {
                return cachedAncestorInfoList;
            }
            List<object[]> tempAncestorInfoAndParentList = new List<object[]>();
            foreach (GenealogyPlaceholder parent in descendant.Parents)
            {
                tempAncestorInfoAndParentList.Add(new object[]
                    {
                        new AncestorInfo(0, descendant),
                        parent
                    });
            }
            while (tempAncestorInfoAndParentList.Count > 0)
            {
                object[] tempAncestorInfoAndParent = tempAncestorInfoAndParentList[0];
                tempAncestorInfoAndParentList.RemoveAt(0);
                GenealogyPlaceholder tempParent = (GenealogyPlaceholder)tempAncestorInfoAndParent[1];
                if (tempParent == ancestor)
                {
                    ancestorInfoList.Add((AncestorInfo)tempAncestorInfoAndParent[0]);
                }
                else if (tempParent.IsAncestor(ancestor))
                {
                    foreach (GenealogyPlaceholder parent in tempParent.Parents)
                    {
                        tempAncestorInfoAndParentList.Add(new object[]
                            {
                                new AncestorInfo(((AncestorInfo)tempAncestorInfoAndParent[0]).GenerationalDistance + 1, tempParent),
                                parent
                            });
                    }
                }
            }
            ancestorInfoList.Sort((a, b) => a.GenerationalDistance == b.GenerationalDistance ? 0 : a.GenerationalDistance > b.GenerationalDistance ? 1 : -1);
            descendant.CachedAncestorInfoLists[ancestor] = ancestorInfoList;
            return ancestorInfoList;
        }

        public static GenealogyPlaceholder GetGenealogyPlaceholder(this Genealogy self)
        {
            ulong id = self.SimDescription == null ? self.mMiniSim.SimDescriptionId : self.SimDescription.SimDescriptionId;
            if (!sGenealogyPlaceholders.ContainsKey(id))
            {
                GenealogyPlaceholder genealogyPlaceholder = new GenealogyPlaceholder(self);
                sGenealogyPlaceholders.Add(id, genealogyPlaceholder);
            }
            return sGenealogyPlaceholders[id];
        }

        public static SiblingOfAncestorInfo GetSiblingOfAncestorInfo(this Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            return descendantOfSibling.GetGenealogyPlaceholder().GetSiblingOfAncestorInfo(siblingOfAncestor.GetGenealogyPlaceholder());
        }

        public static SiblingOfAncestorInfo GetSiblingOfAncestorInfo(this GenealogyPlaceholder descendantOfSibling, GenealogyPlaceholder siblingOfAncestor)
        {
            List<SiblingOfAncestorInfo> siblingOfAncestorInfoList = descendantOfSibling.GetSiblingOfAncestorInfoList(siblingOfAncestor);
            if (siblingOfAncestorInfoList.Count == 0)
            {
                return null;
            }
            return siblingOfAncestorInfoList[0];
        }

        public static List<SiblingOfAncestorInfo> GetSiblingOfAncestorInfoList(this Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            return descendantOfSibling.GetGenealogyPlaceholder().GetSiblingOfAncestorInfoList(siblingOfAncestor.GetGenealogyPlaceholder());
        }

        public static List<SiblingOfAncestorInfo> GetSiblingOfAncestorInfoList(this GenealogyPlaceholder descendantOfSibling, GenealogyPlaceholder siblingOfAncestor)
        {
            List<SiblingOfAncestorInfo> cachedSiblingOfAncestorInfoList, siblingOfAncestorInfoList = new List<SiblingOfAncestorInfo>();
            if (descendantOfSibling.CachedSiblingOfAncestorInfoLists.TryGetValue(siblingOfAncestor, out cachedSiblingOfAncestorInfoList))
            {
                return cachedSiblingOfAncestorInfoList;
            }
            foreach (GenealogyPlaceholder sibling in siblingOfAncestor.Siblings)
            {
                AncestorInfo ancestorInfo = descendantOfSibling.GetAncestorInfo(sibling);
                if (ancestorInfo != null)
                {
                    bool isHalfRelative = Genealogy.IsHalfSibling(sibling.Genealogy, siblingOfAncestor.Genealogy);
                    if (!isHalfRelative || Tuning.kShowHalfRelatives)
                    {
                        siblingOfAncestorInfoList.Add(new SiblingOfAncestorInfo(ancestorInfo.GenerationalDistance, isHalfRelative));
                    }
                }
            }
            siblingOfAncestorInfoList.Sort((a, b) =>
                {
                    if (a.GenerationalDistance > b.GenerationalDistance || a.GenerationalDistance == b.GenerationalDistance && a.IsHalfRelative && !b.IsHalfRelative)
                    {
                        return 1;
                    }
                    if (a.GenerationalDistance < b.GenerationalDistance || a.GenerationalDistance == b.GenerationalDistance && !a.IsHalfRelative && b.IsHalfRelative)
                    {
                        return -1;
                    }
                    return 0;
                });
            descendantOfSibling.CachedSiblingOfAncestorInfoLists[siblingOfAncestor] = siblingOfAncestorInfoList;
            return siblingOfAncestorInfoList;
        }

        public static bool IsSimDescriptionIdUnique(ulong simDescriptionId)
        {
            if (simDescriptionId == 0)
            {
                return false;
            }
            if (MiniSimDescription.IsIdUsed(simDescriptionId))
            {
                return false;
            }
            foreach (Household household in Household.sHouseholdList)
            {
                foreach (SimDescription simDescription in household.AllSimDescriptions)
                {
                    if (simDescription.SimDescriptionId == simDescriptionId)
                    {
                        return false;
                    }
                }
            }
            if (Bin.Singleton != null)
            {
                foreach (HouseholdContents household in Bin.Singleton.Households)
                {
                    foreach (SimDescription simDescription in household.Household.AllSimDescriptions)
                    {
                        if (simDescription.SimDescriptionId == simDescriptionId)
                        {
                            return false;
                        }
                    }
                }
            }
            foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
            {
                SimDescription deadSimDescription = urnstone.DeadSimsDescription;
                if (deadSimDescription != null && deadSimDescription.SimDescriptionId == simDescriptionId)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsUniqueIn(this DistantRelationInfo self, List<DistantRelationInfo> list)
        {
            return !list.Exists(distantRelationInfo =>
                {
                    foreach (GenealogyPlaceholder child in distantRelationInfo.ThroughWhichChildren)
                    {
                        if (!new List<GenealogyPlaceholder>(self.ThroughWhichChildren).Contains(child))
                        {
                            return false;
                        }
                    }
                    return distantRelationInfo.ClosestDescendant == self.ClosestDescendant && distantRelationInfo.Degree == self.Degree && distantRelationInfo.TimesRemoved == self.TimesRemoved;
                });
        }

        public static void RebuildRelationAssignments()
        {
            sGenealogyPlaceholders.Clear();
            foreach (MiniSimDescription miniSimDescription in MiniSimDescription.sMiniSims.Values)
            {
                miniSimDescription.Genealogy.GetGenealogyPlaceholder();
            }
            foreach (Household household in Household.sHouseholdList)
            {
                foreach (SimDescription simDescription in household.AllSimDescriptions)
                {
                    simDescription.Genealogy.GetGenealogyPlaceholder();
                }
            }
            if (Bin.Singleton != null)
            {
                foreach (HouseholdContents household in Bin.Singleton.Households)
                {
                    foreach (SimDescription simDescription in household.Household.AllSimDescriptions)
                    {
                        simDescription.Genealogy.GetGenealogyPlaceholder();
                    }
                }
            }
            foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
            {
                if (urnstone.DeadSimsDescription != null)
                {
                    urnstone.DeadSimsDescription.Genealogy.GetGenealogyPlaceholder();
                }
            }
            foreach (Dictionary<string, object> relationAssignment in sRelationAssignments)
            {
                object degree, generationalDistance, isHigherUpFamilyTree, relationType, sim1, sim2;
                relationAssignment.TryGetValue("Relation Type", out relationType);
                relationAssignment.TryGetValue("Generational Distance", out generationalDistance);
                relationAssignment.TryGetValue("Degree", out degree);
                relationAssignment.TryGetValue("Sim A", out sim1);
                relationAssignment.TryGetValue("Sim B", out sim2);
                relationAssignment.TryGetValue("Sim A Is Higher Up Family Tree", out isHigherUpFamilyTree);
                if ((string)relationType == "Cousin")
                {
                    ((Genealogy)sim1).AddCousin((Genealogy)sim2, (int)degree, (int)generationalDistance, (bool)isHigherUpFamilyTree, false);
                }
                if ((string)relationType == "Descendant")
                {
                    ((Genealogy)sim1).AddAncestor((Genealogy)sim2, (int)generationalDistance, false);
                }
                if ((string)relationType == "Descendant Of Sibling")
                {
                    ((Genealogy)sim1).AddSiblingOfAncestor((Genealogy)sim2, (int)generationalDistance, false);
                }
            }
        }

        public static bool TryAddDistantRelationInfo(this List<DistantRelationInfo> distantRelationInfoList, int degree, int timesRemoved, GenealogyPlaceholder throughWhichChild1, GenealogyPlaceholder throughWhichChild2, GenealogyPlaceholder closestDescendant)
        {
            DistantRelationInfo distantRelationInfo = new DistantRelationInfo(degree, timesRemoved, closestDescendant, new GenealogyPlaceholder[]
                {
                    throughWhichChild1,
                    throughWhichChild2
                });
            if (distantRelationInfo.IsUniqueIn(distantRelationInfoList) && distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow && distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
            {
                distantRelationInfoList.Add(distantRelationInfo);
                return true;
            }
            return false;
        }
    }
}