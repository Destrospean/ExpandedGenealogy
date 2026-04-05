namespace Destrospean.ExpandedGenealogy
{
    public static class Common
    {
        #pragma warning disable 0414
        [System.Obsolete("This remains here because it's a persistable property, and moving it would make players lose data. Use its proxy in `Destrospean.ExpandedGenealogy.GenealogyExtended` instead."), Sims3.SimIFace.PersistableStatic(true)]
        static System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>> sRelationAssignments = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>();
        #pragma warning restore 0414

        public const string kLocalizationPath = "Destrospean/ExpandedGenealogy";

        public static readonly Lang.ExpandedGenealogy.PlayerLanguage PlayerLanguage = System.Activator.CreateInstance(System.Type.GetType("Destrospean.Lang.ExpandedGenealogy." + Sims3.Gameplay.Utilities.Localization.LocalizeString(kLocalizationPath + ":LanguageCode"))) as Lang.ExpandedGenealogy.PlayerLanguage;

        /// <summary>Returns a copy of the string whose first letter, if it exists, is made uppercase.</summary>
        /// <param name="text">The string of which to make the first letter case uppercase</param>
        /// <returns>A copy of the string with the first letter made uppercase</returns>
        public static string Capitalize(this string text)
        {
            return text.Length > 1 ? text.Substring(0, 1).ToUpper() + text.Substring(1) : text.ToUpper();
        }
    }
}
