using Sims3.Gameplay.Socializing;
using Sims3.SimIFace.CustomContent;
using System;
using System.Collections.Generic;

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
        }

        public Dictionary<GenealogyPlaceholder, List<AncestorInfo>> CachedAncestorInfoLists = new Dictionary<GenealogyPlaceholder, List<AncestorInfo>>();

        public Dictionary<GenealogyPlaceholder, List<DistantRelationInfo>> CachedDistantRelationInfoLists = new Dictionary<GenealogyPlaceholder, List<DistantRelationInfo>>();

        public Dictionary<GenealogyPlaceholder, List<SiblingOfAncestorInfo>> CachedSiblingOfAncestorInfoLists = new Dictionary<GenealogyPlaceholder, List<SiblingOfAncestorInfo>>();

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

        public List<GenealogyPlaceholder> mAncestors = null, mParents = null, mParentsRaw = new List<GenealogyPlaceholder>(), mSiblings = null, mSiblingsRaw = new List<GenealogyPlaceholder>();


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

        public void AddChild(Genealogy child)
        {
            AddChild(child.GetGenealogyPlaceholder());
        }

        public void AddChild(GenealogyPlaceholder child)
        {
            child.AddParent(this);
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
}