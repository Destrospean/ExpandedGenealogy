using Destrospean.Common;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Tuning = Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean.Lang
{
	public abstract class PlayerLanguage
	{
		public virtual bool HasNthUncles
		{
			get
			{
				return false;
			}
		}

		public string GetAncestorString(Genealogy descendant, Genealogy ancestor)
		{
			return GetAncestorString(ancestor.SimDescription.IsFemale, descendant, ancestor, false);
		}

		public virtual string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
		{
			string greats = "";
			for (int i = 1; i < Common.GetAncestorInfo(descendant, ancestor).GenerationalDistance; i++)
			{
				greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
			}
			return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
		}

		public virtual string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
		{
			foreach (Genealogy sibling in siblingOfAncestor.Siblings)
			{
				if (descendantOfSibling.IsAncestor(sibling))
				{
					bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
					if (isHalfRelative && !Tuning.kShowHalfRelatives)
					{
						return "";
					}
					string greats = "";
					for (int i = 1; i < Common.GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
					{
						greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
					}
					return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", greats);
				}
			}
			return "";
		}

		public string GetDescendantString(Genealogy ancestor, Genealogy descendant)
		{
			return GetDescendantString(descendant.SimDescription.IsFemale, ancestor, descendant, false);
		}

		public virtual string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
		{
			string greats = "";
			for (int i = 1; i < Common.GetAncestorInfo(descendant, ancestor).GenerationalDistance; i++)
			{
				greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
			}
			return Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
		}

		public abstract string GetDistantRelationString(bool isFemale, Genealogy sim, Common.DistantRelationInfo distantRelationInfo);

		public string GetDistantRelationString(Genealogy sim, Common.DistantRelationInfo distantRelationInfo)
		{
			return GetDistantRelationString(sim.SimDescription.IsFemale, sim, distantRelationInfo);
		}

		public virtual string GetOrdinalSuffix(string number)
		{
			if (number.Length > 1)
			{
				switch (number.Substring(number.Length - 2))
				{
					case "11":
						return "11";
					case "12":
						return "12";
				}
			}
			return number.Substring(number.Length - 1);
		}

		public virtual string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
		{
			foreach (Genealogy sibling in siblingOfAncestor.Siblings)
			{
				if (descendantOfSibling.IsAncestor(sibling))
				{
					bool isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
					if (isHalfRelative && !Tuning.kShowHalfRelatives)
					{
						return "";
					}
					string greats = "";
					for (int i = 1; i < Common.GetAncestorInfo(descendantOfSibling, sibling).GenerationalDistance; i++)
					{
						greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
					}
					return Localization.LocalizeString(isFemale, isHalfRelative && !Tuning.kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", greats);
				}
			}
			return "";
		}
	}
}