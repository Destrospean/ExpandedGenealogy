using System.Collections.Generic;
using Sims3.Gameplay.Socializing;
using Sims3.UI.CAS;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.ExpandedGenealogy
{
    public static class GenealogyExtended
    {
        static List<Dictionary<string, object>> RelationAssignments
        {
            get
            {
#pragma warning disable 0618
                return Common.RelationAssignments;
#pragma warning restore 0618
            }
        }

        /// <summary>Assigns an ancestor to a Sim without knowing the Sims between the Sim and said ancestor.</summary>
        /// <param name="descendant">The descendant within the ancestor–descendant relationship</param>
        /// <param name="ancestor">The ancestor within the ancestor–descendant relationship</param>
        /// <param name="generationalDistance">The generational distance between the ancestor and descendant, with 0 for parents and children, 1 for grandparents and grandchildren, etc.</param>
        /// <param name="addRelationAssignment">Determine whether to save the assignment as an instruction for rebuilding the relations later</param>
        public static void AddAncestor(this Genealogy descendant, Genealogy ancestor, int generationalDistance = 1, bool addRelationAssignment = true)
        {
            if (addRelationAssignment)
            {
                RelationAssignments.Add(new Dictionary<string, object>()
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
            List<GenealogyPlaceholder> ancestry = new List<GenealogyPlaceholder>()
                {
                    descendant.GetGenealogyPlaceholder()
                };
            for (int i = 0; i < generationalDistance; i++)
            {
                GenealogyPlaceholder fakeAncestor = new GenealogyPlaceholder();
                GenealogyPlaceholder.GenealogyPlaceholders.Add(fakeAncestor.Id, fakeAncestor);
                ancestry.Add(fakeAncestor);
            }
            ancestry.Add(ancestor.GetGenealogyPlaceholder());
            for (int i = 0; i < ancestry.Count - 1; i++)
            {
                ancestry[i].AddParent(ancestry[i + 1]);
            }
        }

        /// <summary>Assigns a cousin to a Sim without knowing the Sims that bridge the Sim and said cousin.</summary>
        /// <param name="self">The Sim to be given a relative</param>
        /// <param name="other">The Sim to be assigned as a relative</param>
        /// <param name="degree">The degree of cousinage between the two Sims</param>
        /// <param name="timesRemoved">The number of removals of generations between the two Sims</param>
        /// <param name="isHigherUpFamilyTree">Determine whether the Sim to be given a relative is higher up in the family tree</param>
        /// <param name="addRelationAssignment">Determine whether to save the assignment as an instruction for rebuilding the relations later</param>
        public static void AddCousin(this Genealogy self, Genealogy other, int degree = 1, int timesRemoved = 0, bool isHigherUpFamilyTree = true, bool addRelationAssignment = true)
        {
            if (addRelationAssignment)
            {
                RelationAssignments.Add(new Dictionary<string, object>()
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
            List<GenealogyPlaceholder>[] ancestries = new List<GenealogyPlaceholder>[]
            {
                new List<GenealogyPlaceholder>()
                {
                    self.GetGenealogyPlaceholder()
                },
                new List<GenealogyPlaceholder>()
                {
                    other.GetGenealogyPlaceholder()
                }
            };
            GenealogyPlaceholder sharedFakeAncestor = new GenealogyPlaceholder();
            GenealogyPlaceholder.GenealogyPlaceholders.Add(sharedFakeAncestor.Id, sharedFakeAncestor);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < degree + (isHigherUpFamilyTree ^ i == 0 ? timesRemoved : 0); j++)
                {
                    GenealogyPlaceholder fakeAncestor = new GenealogyPlaceholder();
                    GenealogyPlaceholder.GenealogyPlaceholders.Add(fakeAncestor.Id, fakeAncestor);
                    ancestries[i].Add(fakeAncestor);
                }
                ancestries[i].FindLast(x => true).AddParent(sharedFakeAncestor);
                for (int j = 0; j < ancestries[i].Count - 1; j++)
                {
                    ancestries[i][j].AddParent(ancestries[i][j + 1]);
                }
            }
        }

        /// <summary>Assigns a descendant to a Sim without knowing the Sims between the Sim and said descendant.</summary>
        /// <param name="ancestor">The ancestor within the ancestor–descendant relationship</param>
        /// <param name="descendant">The descendant within the ancestor–descendant relationship</param>
        /// <param name="generationalDistance">The generational distance between the ancestor and descendant, with 0 for parents and children, 1 for grandparents and grandchildren, etc.</param>
        /// <param name="addRelationAssignment">Determine whether to save the assignment as an instruction for rebuilding the relations later</param>
        public static void AddDescendant(this Genealogy ancestor, Genealogy descendant, int generationalDistance = 1, bool addRelationAssignment = true)
        {
            descendant.AddAncestor(ancestor, generationalDistance, addRelationAssignment);
        }

        /// <summary>Assigns a Sim as a "sibling's descendant" to another Sim without knowing the ancestors of said "sibling's descendant."</summary>
        /// <param name="siblingOfAncestor">The alleged ancestor's sibling</param>
        /// <param name="descendantOfSibling">The alleged sibling's descendant</param>
        /// <param name="generationalDistance">The generational distance between the ancestor's sibling and said sibling's descendant, with 0 for aunts/uncles and nieces/nephews, 1 for great-aunts/uncles and great-nieces/nephews, etc.</param>
        /// <param name="addRelationAssignment">Determine whether to save the assignment as an instruction for rebuilding the relations later</param>
        public static void AddDescendantOfSibling(this Genealogy siblingOfAncestor, Genealogy descendantOfSibling, int generationalDistance = 0, bool addRelationAssignment = true)
        {
            descendantOfSibling.AddSiblingOfAncestor(siblingOfAncestor, generationalDistance, addRelationAssignment);
        }

        /// <summary>Assigns a Sim as an "ancestor's sibling" to another Sim without knowing the ancestors of the other Sim.</summary>
        /// <param name="descendantOfSibling">The alleged sibling's descendant</param>
        /// <param name="siblingOfAncestor">The alleged ancestor's sibling</param>
        /// <param name="generationalDistance">The generational distance between the ancestor's sibling and said sibling's descendant, with 0 for aunts/uncles and nieces/nephews, 1 for great-aunts/uncles and great-nieces/nephews, etc.</param>
        /// <param name="addRelationAssignment">Determine whether to save the assignment as an instruction for rebuilding the relations later</param>
        public static void AddSiblingOfAncestor(this Genealogy descendantOfSibling, Genealogy siblingOfAncestor, int generationalDistance = 0, bool addRelationAssignment = true)
        {
            int relationAssignmentIndex = -1;
            if (addRelationAssignment)
            {
                relationAssignmentIndex = RelationAssignments.FindIndex(relationAssignment => (string)relationAssignment["Relation Type"] == "Descendant Of Sibling" && relationAssignment["Sim B"] == descendantOfSibling);
                RelationAssignments.Insert(relationAssignmentIndex == -1 ? RelationAssignments.Count : relationAssignmentIndex, new Dictionary<string, object>()
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

        public static void ClearRelationsWith(this Genealogy self, Genealogy other)
        {
            RelationAssignments.RemoveAll(relationAssignment => relationAssignment["Sim A"] == self && relationAssignment["Sim B"] == other || relationAssignment["Sim A"] == other && relationAssignment["Sim B"] == self);
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

        public static DistantRelationInfo GetDistantRelationInfo(this Genealogy self, Genealogy other)
        {
            return self.GetGenealogyPlaceholder().GetDistantRelationInfo(other.GetGenealogyPlaceholder());
        }

        public static DistantRelationInfo GetDistantRelationInfo(this GenealogyPlaceholder self, GenealogyPlaceholder other)
        {
            List<DistantRelationInfo> distantRelationInfoList = self.GetDistantRelationInfoList(other);
            if (distantRelationInfoList.Count == 0)
            {
                return null;
            }
            return distantRelationInfoList[0];
        }

        public static List<DistantRelationInfo> GetDistantRelationInfoList(this Genealogy self, Genealogy other)
        {
            return self.GetGenealogyPlaceholder().GetDistantRelationInfoList(other.GetGenealogyPlaceholder());
        }

        public static List<DistantRelationInfo> GetDistantRelationInfoList(this GenealogyPlaceholder self, GenealogyPlaceholder other)
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
                    int aCoRelPowAbs = 2 * a.Degree + a.TimesRemoved + (a.IsHalfRelative ? 2 : 1), bCoRelPowAbs = 2 * b.Degree + b.TimesRemoved + (b.IsHalfRelative ? 2 : 1);
                    if (aCoRelPowAbs == bCoRelPowAbs && a.Degree == b.Degree && a.TimesRemoved == b.TimesRemoved)
                    {
                        return 0;
                    }
                    if (aCoRelPowAbs > bCoRelPowAbs || aCoRelPowAbs == bCoRelPowAbs && (a.Degree > b.Degree || a.Degree == b.Degree && a.TimesRemoved > b.TimesRemoved))
                    {
                        return 1;
                    }
                    return -1;
                });
            self.CachedDistantRelationInfoLists[other] = distantRelationInfoList;
            return distantRelationInfoList;
        }

        public static GenealogyPlaceholder GetGenealogyPlaceholder(this Genealogy self)
        {
            return GenealogyPlaceholder.GetGenealogyPlaceholder(self);
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

        public static bool IsCousin(this Genealogy sim1, Genealogy sim2, int degree = 1, int timesRemoved = 0, Genealogy closestDescendant = null)
        {
            foreach (DistantRelationInfo distantRelationInfo in sim1.GetDistantRelationInfoList(sim2))
            {
                if (distantRelationInfo.Degree == degree && distantRelationInfo.TimesRemoved == timesRemoved && (closestDescendant == null || distantRelationInfo.ClosestDescendant.Genealogy == closestDescendant))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsHalfCousin(this Genealogy sim1, Genealogy sim2, int degree = 1, int timesRemoved = 0, Genealogy closestDescendant = null)
        {
            bool isOnlyHalf = false;
            foreach (DistantRelationInfo distantRelationInfo in sim1.GetDistantRelationInfoList(sim2))
            {
                if (distantRelationInfo.Degree == degree && distantRelationInfo.TimesRemoved == timesRemoved && (closestDescendant == null || distantRelationInfo.ClosestDescendant.Genealogy == closestDescendant))
                {
                    if (distantRelationInfo.IsHalfRelative)
                    {
                        isOnlyHalf = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return isOnlyHalf;
        }

        public static bool IsHalfNephew(this Genealogy nephew, Genealogy uncle)
        {
            return IsHalfUncle(uncle, nephew);
        }

        public static bool IsHalfSiblingInLaw(this Genealogy sim1, Genealogy sim2)
        {
            if (sim1.Spouse != null && sim1.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy sibling in sim1.Spouse.Siblings)
                {
                    if (sibling == sim2 || sibling.Spouse == sim2 && sibling.PartnerType == PartnerType.Marriage)
                    {
                        return Genealogy.IsHalfSibling(sim1.Spouse, sibling);
                    }
                }
            }
            if (sim2.Spouse != null && sim2.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy sibling in sim2.Spouse.Siblings)
                {
                    if (sibling == sim1 || sibling.Spouse == sim1 && sibling.PartnerType == PartnerType.Marriage)
                    {
                        return Genealogy.IsHalfSibling(sim2.Spouse, sibling);
                    }
                }
            }
            return false;
        }

        public static bool IsHalfUncle(this Genealogy uncle, Genealogy nephew)
        {
            SiblingOfAncestorInfo siblingOfAncestorInfo = nephew.GetSiblingOfAncestorInfo(uncle);
            if (siblingOfAncestorInfo != null && siblingOfAncestorInfo.GenerationalDistance == 0 && siblingOfAncestorInfo.IsHalfRelative)
            {
                return true;
            }
            if (uncle.Spouse == null || uncle.PartnerType != PartnerType.Marriage)
            {
                return false;
            }
            siblingOfAncestorInfo = nephew.GetSiblingOfAncestorInfo(uncle.Spouse);
            return siblingOfAncestorInfo != null && siblingOfAncestorInfo.GenerationalDistance == 0 && siblingOfAncestorInfo.IsHalfRelative;
        }

        public static void RebuildRelationAssignments()
        {
            GenealogyPlaceholder.GenealogyPlaceholders.Clear();
            foreach (Dictionary<string, object> relationAssignment in RelationAssignments)
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

        static bool TryAddDistantRelationInfo(this List<DistantRelationInfo> distantRelationInfoList, int degree, int timesRemoved, GenealogyPlaceholder throughWhichChild1, GenealogyPlaceholder throughWhichChild2, GenealogyPlaceholder closestDescendant)
        {
            DistantRelationInfo distantRelationInfo = new DistantRelationInfo(degree, timesRemoved, closestDescendant, new GenealogyPlaceholder[]
                {
                    throughWhichChild1,
                    throughWhichChild2
                });
            if (!distantRelationInfoList.Exists(tempDistantRelationInfo => !new List<GenealogyPlaceholder>(tempDistantRelationInfo.ThroughWhichChildren).Exists(child => !new List<GenealogyPlaceholder>(distantRelationInfo.ThroughWhichChildren).Contains(child)) && tempDistantRelationInfo.ClosestDescendant == distantRelationInfo.ClosestDescendant && tempDistantRelationInfo.Degree == distantRelationInfo.Degree && tempDistantRelationInfo.TimesRemoved == distantRelationInfo.TimesRemoved) && distantRelationInfo.Degree <= (uint)Tuning.kMaxDegreeCousinsToShow && distantRelationInfo.TimesRemoved <= (uint)Tuning.kMaxTimesRemovedCousinsToShow)
            {
                distantRelationInfoList.Add(distantRelationInfo);
                return true;
            }
            return false;
        }
    }
}