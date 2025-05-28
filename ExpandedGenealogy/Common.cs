using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

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
            public int AncestorDistance => GenerationalDistance;

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
                if (tempAncestorInfoAndParent[1] == ancestor)
                {
                    ancestorInfoList.Add((AncestorInfo)tempAncestorInfoAndParent[0]);
                }
                else if (tempAncestorInfoAndParent[1] is Genealogy tempParent && tempParent.IsAncestor(ancestor))
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
    }
}