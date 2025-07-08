using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
}

namespace Destrospean
{
    public static class Common
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

            public Genealogy ThroughWhichChild
            {
                get;
                private set;
            }

            public AncestorInfo(int generationalDistance, Genealogy throughWhichChild)
            {
                GenerationalDistance = generationalDistance;
                ThroughWhichChild = throughWhichChild;
            }
        }

        public class DistantRelationInfo
        {
            public Genealogy ClosestDescendant
            {
                get;
                private set;
            }

            public int Degree
            {
                get;
                private set;
            }

            public Genealogy[] ThroughWhichChildren
            {
                get;
                private set;
            }

            public int TimesRemoved
            {
                get;
                private set;
            }

            public DistantRelationInfo(int degree, int timesRemoved, Genealogy closestDescendant, Genealogy[] throughWhichChildren)
            {
                ClosestDescendant = closestDescendant;
                Degree = degree;
                ThroughWhichChildren = throughWhichChildren;
                TimesRemoved = timesRemoved;
            }
        }

        public static DistantRelationInfo CalculateDistantRelation(Genealogy sim1, Genealogy sim2)
        {
            List<DistantRelationInfo> distantRelationInfoList = new List<DistantRelationInfo>();
            foreach (Genealogy ancestor1 in sim1.Ancestors)
            {
                foreach (Genealogy ancestor2 in sim2.Ancestors)
                {
                    if (ancestor1 == ancestor2)
                    {
                        AncestorInfo ancestor1Info = GetAncestorInfo(sim1, ancestor1), ancestor2Info = GetAncestorInfo(sim2, ancestor2);
                        if (ancestor1Info.GenerationalDistance <= ancestor2Info.GenerationalDistance)
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor1Info.GenerationalDistance, ancestor2Info.GenerationalDistance - ancestor1Info.GenerationalDistance, sim1, new Genealogy[]
                                    {
                                        ancestor1Info.ThroughWhichChild,
                                        ancestor2Info.ThroughWhichChild
                                    }));
                        }
                        else
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor2Info.GenerationalDistance, ancestor1Info.GenerationalDistance - ancestor2Info.GenerationalDistance, sim2, new Genealogy[]
                                    {
                                        ancestor1Info.ThroughWhichChild,
                                        ancestor2Info.ThroughWhichChild
                                    }));
                        }
                    }
                    else if (Genealogy.IsSibling(ancestor1, ancestor2))
                    {
                        AncestorInfo ancestor1Info = GetAncestorInfo(sim1, ancestor1), ancestor2Info = GetAncestorInfo(sim2, ancestor2);
                        if (ancestor1Info.GenerationalDistance <= ancestor2Info.GenerationalDistance)
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor1Info.GenerationalDistance + 1, ancestor2Info.GenerationalDistance - ancestor1Info.GenerationalDistance, sim1, new Genealogy[]
                                    {
                                        ancestor1,
                                        ancestor2
                                    }));
                        }
                        else
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor2Info.GenerationalDistance + 1, ancestor1Info.GenerationalDistance - ancestor2Info.GenerationalDistance, sim2, new Genealogy[]
                                    {
                                        ancestor1,
                                        ancestor2
                                    }));
                        }
                    }
                }
            }
            int lowestDegree = int.MaxValue;
            DistantRelationInfo closestDistantRelationInfo = null;
            foreach (DistantRelationInfo distantRelationInfo in distantRelationInfoList)
            {
                if (lowestDegree > distantRelationInfo.Degree || (lowestDegree == distantRelationInfo.Degree && closestDistantRelationInfo != null && closestDistantRelationInfo.TimesRemoved > distantRelationInfo.TimesRemoved))
                {
                    lowestDegree = distantRelationInfo.Degree;
                    closestDistantRelationInfo = distantRelationInfo;
                }
            }
            return closestDistantRelationInfo;
        }

        public static DistantRelationInfo CalculateDistantStepRelation(Genealogy sim1, Genealogy sim2)
        {
            List<DistantRelationInfo> distantRelationInfoList = new List<DistantRelationInfo>();
            List<Genealogy> sim1Ancestors = new List<Genealogy>(), sim2Ancestors = new List<Genealogy>();
            sim1Ancestors.AddRange(sim1.Ancestors);
            sim1Ancestors.AddRange(sim1.GetStepAncestors());
            sim2Ancestors.AddRange(sim2.Ancestors);
            sim2Ancestors.AddRange(sim2.GetStepAncestors());
            foreach (Genealogy ancestor1 in sim1Ancestors)
            {
                foreach (Genealogy ancestor2 in sim2Ancestors)
                {
                    if (ancestor1 == ancestor2)
                    {
                        int stepCount = Convert.ToInt32(!sim1.IsAncestor(ancestor1)) + Convert.ToInt32(!sim2.IsAncestor(ancestor2));
                        AncestorInfo ancestor1Info = sim1.IsAncestor(ancestor1) ? GetAncestorInfo(sim1, ancestor1) : GetStepAncestorInfo(sim1, ancestor1), ancestor2Info = sim2.IsAncestor(ancestor2) ? GetAncestorInfo(sim2, ancestor2) : GetStepAncestorInfo(sim2, ancestor2);
                        if (stepCount != 0 && (stepCount != 2 || Tuning.kAccumulateDistantStepRelatives))
                        {
                            if (ancestor1Info.GenerationalDistance <= ancestor2Info.GenerationalDistance)
                            {
                                distantRelationInfoList.Add(new DistantRelationInfo(ancestor1Info.GenerationalDistance, ancestor2Info.GenerationalDistance - ancestor1Info.GenerationalDistance, sim1, new Genealogy[]
                                        {
                                            ancestor1Info.ThroughWhichChild,
                                            ancestor2Info.ThroughWhichChild
                                        }));
                            }
                            else
                            {
                                distantRelationInfoList.Add(new DistantRelationInfo(ancestor2Info.GenerationalDistance, ancestor1Info.GenerationalDistance - ancestor2Info.GenerationalDistance, sim2, new Genealogy[]
                                        {
                                            ancestor1Info.ThroughWhichChild,
                                            ancestor2Info.ThroughWhichChild
                                        }));
                            }
                        }
                    }
                }
            }
            int lowestDegree = int.MaxValue;
            DistantRelationInfo closestDistantRelationInfo = null;
            foreach (DistantRelationInfo distantRelationInfo in distantRelationInfoList)
            {
                if (lowestDegree > distantRelationInfo.Degree || (lowestDegree == distantRelationInfo.Degree && closestDistantRelationInfo != null && closestDistantRelationInfo.TimesRemoved > distantRelationInfo.TimesRemoved))
                {
                    lowestDegree = distantRelationInfo.Degree;
                    closestDistantRelationInfo = distantRelationInfo;
                }
            }
            return closestDistantRelationInfo;
        }

        public static string Capitalize(this string text)
        {
            return text.Length > 1 ? text.Substring(0, 1).ToUpper() + text.Substring(1) : text.ToUpper();
        }

        public static AncestorInfo GetAncestorInfo(Genealogy descendant, Genealogy ancestor)
        {
            List<AncestorInfo> ancestorInfoList = new List<AncestorInfo>();
            List<object[]> tempAncestorInfoAndParentList = new List<object[]>();
            foreach (Genealogy parent in descendant.Parents)
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
                Genealogy tempParent = (Genealogy)tempAncestorInfoAndParent[1];
                if (tempParent == ancestor)
                {
                    ancestorInfoList.Add((AncestorInfo)tempAncestorInfoAndParent[0]);
                }
                else if (tempParent.IsAncestor(ancestor))
                {
                    foreach (Genealogy parent in tempParent.Parents)
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
            return closestAncestorInfo;
        }

        public static AncestorInfo GetStepAncestorInfo(Genealogy stepDescendant, Genealogy stepAncestor)
        {
            List<AncestorInfo> ancestorInfoList = new List<AncestorInfo>();
            List<object[]> ancestorInfoAndAncestorList = new List<object[]>(), tempAncestorInfoAndParentList = new List<object[]>();
            List<Genealogy> parents = new List<Genealogy>();
            parents.AddRange(stepDescendant.GetStepParents());
            parents.AddRange(stepDescendant.Parents);
            foreach (Genealogy parent in parents)
            {
                tempAncestorInfoAndParentList.Add(new object[]
                {
                    new AncestorInfo(0, stepDescendant),
                    parent
                });
            }
            while (tempAncestorInfoAndParentList.Count > 0)
            {
                object[] tempAncestorInfoAndParent = tempAncestorInfoAndParentList[0];
                tempAncestorInfoAndParentList.RemoveAt(0);
                Genealogy tempParent = (Genealogy)tempAncestorInfoAndParent[1];
                if (tempAncestorInfoAndParent[1] == stepAncestor)
                {
                    ancestorInfoAndAncestorList.Add(tempAncestorInfoAndParent);
                }
                else if (tempParent.IsAncestor(stepAncestor) || tempParent.IsStepAncestor(stepAncestor))
                {
                    parents = new List<Genealogy>();
                    if (Tuning.kAccumulateDistantStepRelatives || stepDescendant.IsAncestor(tempParent))
                    {
                        parents.AddRange(tempParent.GetStepParents());
                    }
                    parents.AddRange(tempParent.Parents);
                    foreach (Genealogy parent in parents)
                    {
                        tempAncestorInfoAndParentList.Add(new object[]
                            {
                                new AncestorInfo(((AncestorInfo)tempAncestorInfoAndParent[0]).GenerationalDistance + 1, tempParent),
                                parent,
                            });
                    }
                }
            }
            foreach (object[] ancestorInfoAndAncestor in ancestorInfoAndAncestorList)
            {
                if (!stepDescendant.IsAncestor((Genealogy)ancestorInfoAndAncestor[1]))
                {
                    ancestorInfoList.Add((AncestorInfo)ancestorInfoAndAncestor[0]);
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
            return closestAncestorInfo;
        }

        public static List<Genealogy> GetStepAncestors(this Genealogy sim)
        {
            List<Genealogy> ancestors = new List<Genealogy>(), stepAncestors = new List<Genealogy>(), tempAncestors = new List<Genealogy>();
            tempAncestors.AddRange(sim.GetStepParents());
            tempAncestors.AddRange(sim.Parents);
            while (tempAncestors.Count > 0)
            {
                Genealogy ancestor = tempAncestors[0];
                tempAncestors.RemoveAt(0);
                ancestors.Add(ancestor);
                if (Tuning.kAccumulateDistantStepRelatives || sim.IsAncestor(ancestor)) 
                {
                    tempAncestors.AddRange(ancestor.GetStepParents());
                }
                tempAncestors.AddRange(ancestor.Parents);
            }
            foreach (Genealogy ancestor in ancestors)
            {
                if (!ancestor.IsAncestor(sim))
                {
                    stepAncestors.Add(ancestor);
                }
            }
            return stepAncestors;
        }

        public static List<Genealogy> GetStepParents(this Genealogy sim)
        {
            List<Genealogy> stepParents = new List<Genealogy>();
            foreach (Genealogy parent in sim.Parents)
            {
                if (parent.Spouse != null && !Genealogy.IsParent(parent.Spouse, sim))
                {
                    stepParents.Add(parent.Spouse);
                }
            }
            return stepParents;
        }

        public static List<Genealogy> GetStepSiblings(this Genealogy sim)
        {
            List<Genealogy> stepSiblings = new List<Genealogy>();
            foreach (Genealogy stepParent in sim.GetStepParents())
            {
                foreach (Genealogy child in stepParent.Children)
                {
                    if (!Genealogy.IsSibling(sim, child))
                    {
                        stepSiblings.Add(child);
                    }
                }
            }
            return stepSiblings;
        }

        public static bool IsStepAncestor(this Genealogy self, Genealogy other)
        {
            return self.GetStepAncestors().Contains(other);
        }
    }
}