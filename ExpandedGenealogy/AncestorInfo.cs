namespace Destrospean.ExpandedGenealogy
{
    public class AncestorInfo
    {
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