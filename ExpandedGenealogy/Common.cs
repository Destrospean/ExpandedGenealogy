using Sims3.Gameplay.Socializing;
using System.Collections.Generic;

namespace Destrospean
{
    public class Common
    {
        public class AncestorInfo
        {
            public int AncestorDistance;
            public Genealogy ThroughWhichChild;

            public AncestorInfo(int ancestorDistance, Genealogy throughWhichChild)
            {
                AncestorDistance = ancestorDistance;
                ThroughWhichChild = throughWhichChild;
            }
        }

        public class DistantRelationInfo
        {
            public Genealogy ClosestDescendant;
            public int Degree, TimesRemoved;
            public Genealogy[] ThroughWhichChildren;

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
                        if (ancestor1Info.AncestorDistance <= ancestor2Info.AncestorDistance)
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor1Info.AncestorDistance, ancestor2Info.AncestorDistance - ancestor1Info.AncestorDistance, sim1, new Genealogy[]
                            {
                                ancestor1Info.ThroughWhichChild,
                                ancestor2Info.ThroughWhichChild
                            }));
                        }
                        else
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor2Info.AncestorDistance, ancestor1Info.AncestorDistance - ancestor2Info.AncestorDistance, sim2, new Genealogy[]
                            {
                                ancestor1Info.ThroughWhichChild,
                                ancestor2Info.ThroughWhichChild
                            }));
                        }
                    }
                    else if (Genealogy.IsSibling(ancestor1, ancestor2))
                    {
                        AncestorInfo ancestor1Info = GetAncestorInfo(sim1, ancestor1), ancestor2Info = GetAncestorInfo(sim2, ancestor2);
                        if (ancestor1Info.AncestorDistance <= ancestor2Info.AncestorDistance)
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor1Info.AncestorDistance + 1, ancestor2Info.AncestorDistance - ancestor1Info.AncestorDistance, sim1, new Genealogy[]
                            {
                                ancestor1,
                                ancestor2
                            }));
                        }
                        else
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor2Info.AncestorDistance + 1, ancestor1Info.AncestorDistance - ancestor2Info.AncestorDistance, sim2, new Genealogy[]
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
                            new AncestorInfo(((AncestorInfo)tempAncestorInfoAndParent[0]).AncestorDistance + 1, tempParent),
                            parent
                        });
                    }
                }
            }
            int shortestAncestorDistance = int.MaxValue;
            AncestorInfo closestAncestorInfo = null;
            foreach (AncestorInfo ancestorInfo in ancestorInfoList)
            {
                if (shortestAncestorDistance > ancestorInfo.AncestorDistance)
                {
                    shortestAncestorDistance = ancestorInfo.AncestorDistance;
                    closestAncestorInfo = ancestorInfo;
                }
            }
            return closestAncestorInfo;
        }
    }
}