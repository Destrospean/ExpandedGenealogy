namespace Destrospean.ExpandedGenealogy
{
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

        public bool IsHalfRelative
        {
            get
            {
                return Sims3.Gameplay.Socializing.Genealogy.IsHalfSibling(ThroughWhichChildren[0].Genealogy, ThroughWhichChildren[1].Genealogy);
            }
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
}
