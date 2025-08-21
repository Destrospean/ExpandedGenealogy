using System;
using System.Collections.Generic;
using Sims3.SimIFace;

namespace Destrospean.ExpandedGenealogy
{
    public static class Common
    {
        [Obsolete("This remains here because it's a persistable property, and moving it would make players lose data. Use its proxy in `Destrospean.ExpandedGenealogy.GenealogyExtended` instead.")]
        internal static List<Dictionary<string, object>> RelationAssignments
        {
            get
            {
                return sRelationAssignments;
            }
        }


        [PersistableStatic(true)]
        static List<Dictionary<string, object>> sRelationAssignments = new List<Dictionary<string, object>>();

        /// <summary>Returns a copy of the string whose first letter, if it exists, is made uppercase.</summary>
        /// <param name="text">The string of which to make the first letter case uppercase</param>
        /// <returns>A copy of the string with the first letter made uppercase</returns>
        public static string Capitalize(this string text)
        {
            return text.Length > 1 ? text.Substring(0, 1).ToUpper() + text.Substring(1) : text.ToUpper();
        }
    }
}