using System;
using System.Collections.Generic;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace.CustomContent;

namespace Destrospean.ExpandedGenealogy
{
    public class GenealogyPlaceholder
    {
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
            private set
            {
                mAncestors = value;
            }
        }

        public Dictionary<GenealogyPlaceholder, List<AncestorInfo>> CachedAncestorInfoLists
        {
            get
            {
                return mCachedAncestorInfoLists;
            }
        }

        public Dictionary<GenealogyPlaceholder, List<DistantRelationInfo>> CachedDistantRelationInfoLists
        {
            get
            {
                return mCachedDistantRelationInfoLists;
            }
        }

        public Dictionary<GenealogyPlaceholder, List<SiblingOfAncestorInfo>> CachedSiblingOfAncestorInfoLists
        {
            get
            {
                return mCachedSiblingOfAncestorInfoLists;
            }
        }

        public static Dictionary<ulong, GenealogyPlaceholder> GenealogyPlaceholders
        {
            get
            {
                return sGenealogyPlaceholders;
            }
        }

        public Genealogy Genealogy
        {
            get;
            private set;
        }

        public bool HasUniqueId
        {
            get
            {
                if (Id == 0 || MiniSimDescription.IsIdUsed(Id) || GenealogyPlaceholders.ContainsKey(Id))
                {
                    return false;
                }
                foreach (Household household in Household.sHouseholdList)
                {
                    foreach (SimDescription simDescription in household.AllSimDescriptions)
                    {
                        if (simDescription.SimDescriptionId == Id)
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
                            if (simDescription.SimDescriptionId == Id)
                            {
                                return false;
                            }
                        }
                    }
                }
                foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
                {
                    SimDescription deadSimDescription = urnstone.DeadSimsDescription;
                    if (deadSimDescription != null && deadSimDescription.SimDescriptionId == Id)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public ulong Id
        {
            get;
            private set;
        }

        List<GenealogyPlaceholder> mAncestors = null, mParents = null, mParentsRaw = new List<GenealogyPlaceholder>(), mSiblings = null;

        Dictionary<GenealogyPlaceholder, List<AncestorInfo>> mCachedAncestorInfoLists = new Dictionary<GenealogyPlaceholder, List<AncestorInfo>>();

        Dictionary<GenealogyPlaceholder, List<DistantRelationInfo>> mCachedDistantRelationInfoLists = new Dictionary<GenealogyPlaceholder, List<DistantRelationInfo>>();

        Dictionary<GenealogyPlaceholder, List<SiblingOfAncestorInfo>> mCachedSiblingOfAncestorInfoLists = new Dictionary<GenealogyPlaceholder, List<SiblingOfAncestorInfo>>();

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
                        parents.AddRange(Genealogy.Parents.ConvertAll(new Converter<Genealogy, GenealogyPlaceholder>(parent => GetGenealogyPlaceholder(parent))));
                    }
                    mParents = parents;
                }
                return mParents;
            }
            private set
            {
                mParents = value;
            }
        }

        static Dictionary<ulong, GenealogyPlaceholder> sGenealogyPlaceholders = new Dictionary<ulong, GenealogyPlaceholder>();

        public List<GenealogyPlaceholder> Siblings
        {
            get
            {
                if (mSiblings == null)
                {
                    List<GenealogyPlaceholder> siblings = new List<GenealogyPlaceholder>();
                    if (Genealogy != null)
                    {
                        siblings.AddRange(Genealogy.Siblings.ConvertAll(new Converter<Genealogy, GenealogyPlaceholder>(sibling => GetGenealogyPlaceholder(sibling))));
                    }
                    foreach (GenealogyPlaceholder parent in Parents)
                    {
                        siblings.AddRange(new List<GenealogyPlaceholder>(GenealogyPlaceholders.Values).FindAll(child => child.IsParent(parent) && child != this && !siblings.Contains(child)));
                    }
                    mSiblings = siblings;
                }
                return mSiblings;
            }
            private set
            {
                mSiblings = value;
            }
        }

        public void AddChild(Genealogy child)
        {
            AddChild(GetGenealogyPlaceholder(child));
        }

        public void AddChild(GenealogyPlaceholder child)
        {
            child.AddParent(this);
        }

        public void AddParent(Genealogy parent)
        {
            AddParent(GetGenealogyPlaceholder(parent));
        }

        public void AddParent(GenealogyPlaceholder parent)
        {
            mParentsRaw.Add(parent);
            foreach (GenealogyPlaceholder sibling in Siblings)
            {
                sibling.mParentsRaw.Add(parent);
            }
            ClearCaches();
        }

        public static void ClearCaches()
        {
            foreach (GenealogyPlaceholder genealogyPlaceholder in GenealogyPlaceholders.Values)
            {
                genealogyPlaceholder.CachedAncestorInfoLists.Clear();
                genealogyPlaceholder.CachedDistantRelationInfoLists.Clear();
                genealogyPlaceholder.CachedSiblingOfAncestorInfoLists.Clear();
                genealogyPlaceholder.Ancestors = null;
                genealogyPlaceholder.Parents = null;
                genealogyPlaceholder.Siblings = null;
            }
        }

        public static GenealogyPlaceholder GetGenealogyPlaceholder(Genealogy sim)
        {
            ulong id = sim.SimDescription == null ? sim.mMiniSim.SimDescriptionId : sim.SimDescription.SimDescriptionId;
            if (!GenealogyPlaceholder.GenealogyPlaceholders.ContainsKey(id))
            {
                GenealogyPlaceholder genealogyPlaceholder = new GenealogyPlaceholder(sim);
                GenealogyPlaceholder.GenealogyPlaceholders.Add(id, genealogyPlaceholder);
            }
            return GenealogyPlaceholder.GenealogyPlaceholders[id];
        }

        public bool IsAncestor(Genealogy ancestor)
        {
            return IsAncestor(GetGenealogyPlaceholder(ancestor));
        }

        public bool IsAncestor(GenealogyPlaceholder ancestor)
        {
            return Ancestors.Contains(ancestor);
        }

        public bool IsChild(Genealogy child)
        {
            return IsChild(GetGenealogyPlaceholder(child));
        }

        public bool IsChild(GenealogyPlaceholder child)
        {
            return child.IsParent(this);
        }

        public bool IsDescendant(Genealogy descendant)
        {
            return IsDescendant(GetGenealogyPlaceholder(descendant));
        }

        public bool IsDescendant(GenealogyPlaceholder descendant)
        {
            return descendant.IsAncestor(this);
        }

        public bool IsParent(Genealogy parent)
        {
            return IsParent(GetGenealogyPlaceholder(parent));
        }

        public bool IsParent(GenealogyPlaceholder parent)
        {
            return Parents.Contains(parent);
        }

        public bool IsSibling(Genealogy sibling)
        {
            return IsSibling(GetGenealogyPlaceholder(sibling));
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
                if (HasUniqueId)
                {
                    return;
                }
            }
            Id = genealogy.SimDescription == null ? genealogy.mMiniSim.SimDescriptionId : genealogy.SimDescription.SimDescriptionId;
        }
    }
}