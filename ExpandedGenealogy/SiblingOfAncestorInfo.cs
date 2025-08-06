namespace Destrospean.ExpandedGenealogy
{
    public class SiblingOfAncestorInfo
    {
        public int GenerationalDistance
        {
            get;
            private set;
        }

        public bool IsHalfRelative
        {
            get;
            private set;
        }

        public SiblingOfAncestorInfo(int generationalDistance, bool isHalfRelative)
        {
            GenerationalDistance = generationalDistance;
            IsHalfRelative = isHalfRelative;
        }
    }
}