using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using System;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
}

namespace Destrospean.ExpandedGenealogy
{
    public class AncestorInfo
    {
        [Obsolete("Please use `GenerationalDistance` as it has a better name.")]
        public int AncestorDistance
        {
            get
            {
                return GenerationalDistance;
            }
        }

        public int GenerationalDistance
        {
            get;
            private set;
        }

        public GenealogyPlaceholder ThroughWhichChild
        {
            get;
            private set;
        }

        public AncestorInfo(int generationalDistance, GenealogyPlaceholder throughWhichChild)
        {
            GenerationalDistance = generationalDistance;
            ThroughWhichChild = throughWhichChild;
        }
    }

    public class DistantRelationInfo
    {
        public GenealogyPlaceholder ClosestDescendant
        {
            get;
            private set;
        }

        public int Degree
        {
            get;
            private set;
        }

        public GenealogyPlaceholder[] ThroughWhichChildren
        {
            get;
            private set;
        }

        public int TimesRemoved
        {
            get;
            private set;
        }

        public DistantRelationInfo(int degree, int timesRemoved, GenealogyPlaceholder closestDescendant, GenealogyPlaceholder[] throughWhichChildren)
        {
            ClosestDescendant = closestDescendant;
            Degree = degree;
            ThroughWhichChildren = throughWhichChildren;
            TimesRemoved = timesRemoved;
        }
    }

    public class GenealogyPlaceholder
    {
        public Dictionary<GenealogyPlaceholder, AncestorInfo> CachedAncestorInfo = new Dictionary<GenealogyPlaceholder, AncestorInfo>();

        public Dictionary<GenealogyPlaceholder, List<DistantRelationInfo>> CachedDistantRelationInfoLists = new Dictionary<GenealogyPlaceholder, List<DistantRelationInfo>>();

        public List<GenealogyPlaceholder> mAncestors = null, mParents = null, mParentsRaw = new List<GenealogyPlaceholder>(), mSiblings = null, mSiblingsRaw = new List<GenealogyPlaceholder>();

        public List<GenealogyPlaceholder> Ancestors
        {
            get
            {
                if (mAncestors == null)
                {
                    List<GenealogyPlaceholder> ancestors = new List<GenealogyPlaceholder>(), tempAncestors = new List<GenealogyPlaceholder>();
                    tempAncestors.AddRange(Parents);
                    while (tempAncestors.Count > 0)
                    {
                        GenealogyPlaceholder tempAncestor = tempAncestors[0];
                        tempAncestors.RemoveAt(0);
                        ancestors.Add(tempAncestor);
                        tempAncestors.AddRange(tempAncestor.Parents);
                    }
                    mAncestors = ancestors;
                }
                return mAncestors;
            }
        }

        public Genealogy Genealogy
        {
            get;
            private set;
        }

        public ulong Id
        {
            get;
            private set;
        }

        public List<GenealogyPlaceholder> Parents
        {
            get
            {
                if (mParents == null)
                {
                    List<GenealogyPlaceholder> parents = new List<GenealogyPlaceholder>();
                    parents.AddRange(mParentsRaw);
                    if (Genealogy != null)
                    {
                        parents.AddRange(Genealogy.Parents.ConvertAll(new Converter<Genealogy, GenealogyPlaceholder>(parent => parent.GetGenealogyPlaceholder())));
                    }
                    mParents = parents;
                }
                return mParents;
            }
        }

        public List<GenealogyPlaceholder> Siblings
        {
            get
            {
                if (mSiblings == null)
                {
                    List<GenealogyPlaceholder> siblings = new List<GenealogyPlaceholder>();
                    siblings.AddRange(mSiblingsRaw);
                    if (Genealogy != null)
                    {
                        siblings.AddRange(Genealogy.Siblings.ConvertAll(new Converter<Genealogy, GenealogyPlaceholder>(sibling => sibling.GetGenealogyPlaceholder())));
                    }
                    mSiblings = siblings;
                }
                return mSiblings;
            }
        }

        public void AddParent(Genealogy parent)
        {
            AddParent(parent.GetGenealogyPlaceholder());
        }

        public void AddParent(GenealogyPlaceholder parent)
        {
            mParentsRaw.Add(parent);
            Common.ClearCachesInGenealogyPlaceholders();
        }

        public void AddChild(Genealogy child)
        {
            AddChild(child.GetGenealogyPlaceholder());
        }

        public void AddChild(GenealogyPlaceholder child)
        {
            child.AddParent(this);
        }

        public void AddSibling(Genealogy sibling)
        {
            AddSibling(sibling.GetGenealogyPlaceholder());
        }

        public void AddSibling(GenealogyPlaceholder sibling)
        {
            mSiblingsRaw.Add(sibling);
            Common.ClearCachesInGenealogyPlaceholders();
        }

        public bool IsAncestor(Genealogy ancestor)
        {
            return IsAncestor(ancestor.GetGenealogyPlaceholder());
        }

        public bool IsAncestor(GenealogyPlaceholder ancestor)
        {
            return this.GetAncestorInfo(ancestor) != null;
        }

        public bool IsChild(Genealogy child)
        {
            return IsChild(child.GetGenealogyPlaceholder());
        }

        public bool IsChild(GenealogyPlaceholder child)
        {
            return child.Parents.Contains(this);
        }

        public bool IsDescendant(Genealogy descendant)
        {
            return IsDescendant(descendant.GetGenealogyPlaceholder());
        }

        public bool IsDescendant(GenealogyPlaceholder descendant)
        {
            return descendant.GetAncestorInfo(this) != null;
        }

        public bool IsParent(Genealogy parent)
        {
            return IsParent(parent.GetGenealogyPlaceholder());
        }

        public bool IsParent(GenealogyPlaceholder parent)
        {
            return Parents.Contains(parent);
        }

        public bool IsSibling(Genealogy sibling)
        {
            return IsSibling(sibling.GetGenealogyPlaceholder());
        }

        public bool IsSibling(GenealogyPlaceholder sibling)
        {
            return Siblings.Contains(sibling);
        }

        public GenealogyPlaceholder(Genealogy genealogy = null)
        {
            Genealogy = genealogy;
            while (genealogy == null)
            {
                Id = DownloadContent.GenerateGUID();
                if (Common.IsSimDescriptionIdUnique(Id) && !Common.GenealogyPlaceholders.ContainsKey(Id))
                {
                    return;
                }
            }
            Id = genealogy.SimDescription == null ? genealogy.mMiniSim.SimDescriptionId : genealogy.SimDescription.SimDescriptionId;
        }
    }

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

        [PersistableStatic]
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
            sGenealogyPlaceholders[sim1GenealogyPlaceholders[sim1GenealogyPlaceholders.Count - 1].Id].AddSibling(sGenealogyPlaceholders[sim2GenealogyPlaceholders[sim2GenealogyPlaceholders.Count - 1].Id]);
            sGenealogyPlaceholders[sim2GenealogyPlaceholders[sim2GenealogyPlaceholders.Count - 1].Id].AddSibling(sGenealogyPlaceholders[sim1GenealogyPlaceholders[sim1GenealogyPlaceholders.Count - 1].Id]);
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

        public static DistantRelationInfo CalculateDistantRelation(this GenealogyPlaceholder self, GenealogyPlaceholder other)
        {
            int lowestDegree = int.MaxValue;
            DistantRelationInfo closestDistantRelationInfo = null;
            foreach (DistantRelationInfo distantRelationInfo in self.CalculateDistantRelations(other))
            {
                if (lowestDegree > distantRelationInfo.Degree || (lowestDegree == distantRelationInfo.Degree && closestDistantRelationInfo != null && closestDistantRelationInfo.TimesRemoved > distantRelationInfo.TimesRemoved))
                {
                    lowestDegree = distantRelationInfo.Degree;
                    closestDistantRelationInfo = distantRelationInfo;
                }
            }
            return closestDistantRelationInfo;
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
                    if (ancestor1 == ancestor2)
                    {
                        AncestorInfo ancestor1Info = self.GetAncestorInfo(ancestor1), ancestor2Info = other.GetAncestorInfo(ancestor2);
                        if (ancestor1Info.GenerationalDistance <= ancestor2Info.GenerationalDistance)
                        {
                            DistantRelationInfo distantRelationInfo = new DistantRelationInfo(ancestor1Info.GenerationalDistance, ancestor2Info.GenerationalDistance - ancestor1Info.GenerationalDistance, self, new GenealogyPlaceholder[]
                                {
                                    ancestor1Info.ThroughWhichChild,
                                    ancestor2Info.ThroughWhichChild
                                });
                            distantRelationInfoList.Add(distantRelationInfo);

                        }
                        else
                        {
                            DistantRelationInfo distantRelationInfo = new DistantRelationInfo(ancestor2Info.GenerationalDistance, ancestor1Info.GenerationalDistance - ancestor2Info.GenerationalDistance, other, new GenealogyPlaceholder[]
                                {
                                    ancestor1Info.ThroughWhichChild,
                                    ancestor2Info.ThroughWhichChild
                                });
                            distantRelationInfoList.Add(distantRelationInfo);
                        }
                    }
                    else if (ancestor1.IsSibling(ancestor2))
                    {
                        AncestorInfo ancestor1Info = self.GetAncestorInfo(ancestor1), ancestor2Info = other.GetAncestorInfo(ancestor2);
                        if (ancestor1Info.GenerationalDistance <= ancestor2Info.GenerationalDistance)
                        {
                            DistantRelationInfo distantRelationInfo = new DistantRelationInfo(ancestor1Info.GenerationalDistance + 1, ancestor2Info.GenerationalDistance - ancestor1Info.GenerationalDistance, self, new GenealogyPlaceholder[]
                                {
                                    ancestor1,
                                    ancestor2
                                });
                            distantRelationInfoList.Add(distantRelationInfo);
                        }
                        else
                        {
                            DistantRelationInfo distantRelationInfo = new DistantRelationInfo(ancestor2Info.GenerationalDistance + 1, ancestor1Info.GenerationalDistance - ancestor2Info.GenerationalDistance, other, new GenealogyPlaceholder[]
                                {
                                    ancestor1,
                                    ancestor2
                                });
                            distantRelationInfoList.Add(distantRelationInfo);
                        }
                    }
                }
            }
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
                genealogyPlaceholder.CachedAncestorInfo.Clear();
                genealogyPlaceholder.CachedDistantRelationInfoLists.Clear();
                genealogyPlaceholder.mAncestors = null;
                genealogyPlaceholder.mParents = null;
                genealogyPlaceholder.mSiblings = null;
            }
        }

        public static void ClearRelationsWith(this Genealogy self, Genealogy other)
        {
            sRelationAssignments.RemoveAll(relationAssignment => (relationAssignment["Sim A"] == self && relationAssignment["Sim B"] == other) || (relationAssignment["Sim A"] == other && relationAssignment["Sim B"] == self));
            RebuildRelationAssignments();
        }

        public static AncestorInfo GetAncestorInfo(this Genealogy descendant, Genealogy ancestor)
        {
            return descendant.GetGenealogyPlaceholder().GetAncestorInfo(ancestor.GetGenealogyPlaceholder());
        }

        public static AncestorInfo GetAncestorInfo(this GenealogyPlaceholder descendant, GenealogyPlaceholder ancestor)
        {
            AncestorInfo cachedAncestorInfo;
            if (descendant.CachedAncestorInfo.TryGetValue(ancestor, out cachedAncestorInfo))
            {
                return cachedAncestorInfo;
            }
            List<AncestorInfo> ancestorInfoList = new List<AncestorInfo>();
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
            int shortestGenerationalDistance = int.MaxValue;
            AncestorInfo closestAncestorInfo = null;
            foreach (AncestorInfo ancestorInfo in ancestorInfoList)
            {
                if (shortestGenerationalDistance > ancestorInfo.GenerationalDistance)
                {
                    shortestGenerationalDistance = ancestorInfo.GenerationalDistance;
                    closestAncestorInfo = ancestorInfo;
                }
            }
            descendant.CachedAncestorInfo[ancestor] = closestAncestorInfo;
            return closestAncestorInfo;
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
    }
}