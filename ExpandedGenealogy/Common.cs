using System;
using System.Collections.Generic;
using Sims3.Gameplay.CAS;
using Sims3.UI;

namespace Destrospean.ExpandedGenealogy
{
    public static class Common
    {
#pragma warning disable 0414
        [Obsolete("This remains here because it's a persistable property, and moving it would make players lose data. Use its proxy in `Destrospean.ExpandedGenealogy.GenealogyExtended` instead.", true), Sims3.SimIFace.PersistableStatic(true)]
        static List<Dictionary<string, object>> sRelationAssignments = new List<Dictionary<string, object>>();
#pragma warning restore 0414

        internal const string kLocalizationPath = "Destrospean/ExpandedGenealogy";

        public static readonly Lang.ExpandedGenealogy.PlayerLanguage PlayerLanguage = Activator.CreateInstance(Type.GetType("Destrospean.Lang.ExpandedGenealogy." + Sims3.Gameplay.Utilities.Localization.LocalizeString(kLocalizationPath + ":LanguageCode"))) as Lang.ExpandedGenealogy.PlayerLanguage;

        /// <summary>Returns a copy of the string whose first letter, if it exists, is made uppercase.</summary>
        /// <param name="text">The string of which to make the first letter case uppercase</param>
        /// <returns>A copy of the string with the first letter made uppercase</returns>
        public static string Capitalize(this string text)
        {
            return text.Length > 1 ? text.Substring(0, 1).ToUpper() + text.Substring(1) : text.ToUpper();
        }

        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
        }

        public static void Notify(string message, SimDescription simDescription, StyledNotification.NotificationStyle style)
        {
            Notify(message, simDescription, style, true);
        }

        public static void Notify(string message, SimDescription fakeSimDescription, StyledNotification.NotificationStyle style, bool checkForFake)
        {
            SimDescription simDescription = fakeSimDescription;
            if (simDescription == null)
            {
                StyledNotification.Show(new StyledNotification.Format(message, style));
                return;
            }
            if (checkForFake)
            {
                simDescription = SimDescription.Find(fakeSimDescription.SimDescriptionId);
                if (simDescription == null)
                {
                    StyledNotification.Show(new StyledNotification.Format(message, style));
                    return;
                }
            }
            StyledNotification.Show(simDescription.CreatedSim == null ? new StyledNotification.Format(message, style) : new StyledNotification.Format(message, Sims3.SimIFace.ObjectGuid.InvalidObjectGuid, simDescription.CreatedSim.ObjectId, style));
        }

        /// <summary>This method was borrowed from Lazy Duchess' Mono Patcher</summary>
        public static void ReplaceMethod(System.Reflection.MethodInfo oldMethod, System.Reflection.MethodInfo newMethod)
        {
            unsafe
            {
                byte[] replacementByteArray = new byte[40];
                System.Runtime.InteropServices.Marshal.Copy(newMethod.MethodHandle.Value, replacementByteArray, 0, 40);
                System.Runtime.InteropServices.Marshal.Copy(replacementByteArray, 0, oldMethod.MethodHandle.Value, 24);
                System.Runtime.InteropServices.Marshal.Copy(replacementByteArray, 28, new IntPtr(oldMethod.MethodHandle.Value.ToInt32() + 28), 12);
            }
        }
    }
}
