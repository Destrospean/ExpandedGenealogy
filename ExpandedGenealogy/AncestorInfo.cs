using System;

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
}