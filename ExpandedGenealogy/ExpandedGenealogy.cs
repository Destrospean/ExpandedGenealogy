using MonoPatcherLib;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using static Sims3.Gameplay.Destrospean.ExpandedGenealogy;

namespace Destrospean
{
    [Plugin]
    public class ExpandedGenealogy
    {
        public class AncestorInfo
        {
            public int AncestorDistance;
            public Genealogy ThroughWhichChild;

            public AncestorInfo(int ancestorDistance, Genealogy throughWhichChild)
            {
                AncestorDistance = ancestorDistance;
                ThroughWhichChild = throughWhichChild;
            }
        }

        public class DistantRelationInfo
        {
            public Genealogy ClosestDescendant;
            public int Degree, TimesRemoved;
            public Genealogy[] ThroughWhichChildren;

            public DistantRelationInfo(int degree, int timesRemoved, Genealogy closestDescendant, Genealogy[] throughWhichChildren)
            {
                ClosestDescendant = closestDescendant;
                Degree = degree;
                ThroughWhichChildren = throughWhichChildren;
                TimesRemoved = timesRemoved;
            }
        }

        public static DistantRelationInfo CalculateDistantRelation(Genealogy sim1, Genealogy sim2)
        {
            List<DistantRelationInfo> distantRelationInfoList = new List<DistantRelationInfo>();
            foreach (Genealogy ancestor1 in sim1.Ancestors)
            {
                foreach (Genealogy ancestor2 in sim2.Ancestors)
                {
                    if (ancestor1 == ancestor2)
                    {
                        AncestorInfo ancestor1Info = GetAncestorInfo(sim1, ancestor1), ancestor2Info = GetAncestorInfo(sim2, ancestor2);
                        if (ancestor1Info.AncestorDistance <= ancestor2Info.AncestorDistance)
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor1Info.AncestorDistance, ancestor2Info.AncestorDistance - ancestor1Info.AncestorDistance, sim1, new Genealogy[]
                            {
                                ancestor1Info.ThroughWhichChild,
                                ancestor2Info.ThroughWhichChild
                            }));
                        }
                        else
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor2Info.AncestorDistance, ancestor1Info.AncestorDistance - ancestor2Info.AncestorDistance, sim2, new Genealogy[]
                            {
                                ancestor1Info.ThroughWhichChild,
                                ancestor2Info.ThroughWhichChild
                            }));
                        }
                    }
                    else if (Genealogy.IsSibling(ancestor1, ancestor2))
                    {
                        AncestorInfo ancestor1Info = GetAncestorInfo(sim1, ancestor1), ancestor2Info = GetAncestorInfo(sim2, ancestor2);
                        if (ancestor1Info.AncestorDistance <= ancestor2Info.AncestorDistance)
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor1Info.AncestorDistance + 1, ancestor2Info.AncestorDistance - ancestor1Info.AncestorDistance, sim1, new Genealogy[]
                            {
                                ancestor1,
                                ancestor2
                            }));
                        }
                        else
                        {
                            distantRelationInfoList.Add(new DistantRelationInfo(ancestor2Info.AncestorDistance + 1, ancestor1Info.AncestorDistance - ancestor2Info.AncestorDistance, sim2, new Genealogy[]
                            {
                                ancestor1,
                                ancestor2
                            }));
                        }
                    }
                }
            }
            int lowestDegree = int.MaxValue;
            DistantRelationInfo closestDistantRelationInfo = null;
            foreach (DistantRelationInfo distantRelationInfo in distantRelationInfoList)
            {
                if (lowestDegree > distantRelationInfo.Degree || (lowestDegree == distantRelationInfo.Degree && closestDistantRelationInfo != null && closestDistantRelationInfo.TimesRemoved > distantRelationInfo.TimesRemoved))
                {
                    lowestDegree = distantRelationInfo.Degree;
                    closestDistantRelationInfo = distantRelationInfo;
                }
            }
            return closestDistantRelationInfo;
        }

        public static AncestorInfo GetAncestorInfo(Genealogy descendant, Genealogy ancestor)
        {
            List<AncestorInfo> ancestorInfoList = new List<AncestorInfo>();
            List<object[]> tempAncestorInfoAndParentList = new List<object[]>();
            foreach (Genealogy parent in descendant.Parents)
            {
                tempAncestorInfoAndParentList.Add(new object[]
                {
                    new AncestorInfo(0, descendant),
                    parent
                });
            }
            while (tempAncestorInfoAndParentList.Count > 0)
            {
                object[] tempAncestorInfoAndParent = tempAncestorInfoAndParentList[0];
                tempAncestorInfoAndParentList.RemoveAt(0);
                if (tempAncestorInfoAndParent[1] == ancestor)
                {
                    ancestorInfoList.Add((AncestorInfo)tempAncestorInfoAndParent[0]);
                }
                else if (tempAncestorInfoAndParent[1] is Genealogy tempParent && tempParent.IsAncestor(ancestor))
                {
                    foreach (Genealogy parent in tempParent.Parents)
                    {
                        tempAncestorInfoAndParentList.Add(new object[]
                        {
                            new AncestorInfo(((AncestorInfo)tempAncestorInfoAndParent[0]).AncestorDistance + 1, tempParent),
                            parent
                        });
                    }
                }
            }
            int shortestAncestorDistance = int.MaxValue;
            AncestorInfo closestAncestorInfo = null;
            foreach (AncestorInfo ancestorInfo in ancestorInfoList)
            {
                if (shortestAncestorDistance > ancestorInfo.AncestorDistance)
                {
                    shortestAncestorDistance = ancestorInfo.AncestorDistance;
                    closestAncestorInfo = ancestorInfo;
                }
            }
            return closestAncestorInfo;
        }

        public static string GetAncestorString(Genealogy descendant, Genealogy ancestor)
        {
            return GetAncestorString(ancestor.SimDescription.IsFemale, descendant, ancestor, false);
        }

        public static string GetAncestorString(bool isFemale, Genealogy descendant, Genealogy ancestor, bool isInLaw)
        {
            string text = "";
            switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
            {
                case "CHS":
                case "CHT":
                case "CZE":
                case "DAN":
                case "FIN":
                case "GRE":
                case "HUN":
                case "JPN":
                case "KOR":
                case "NOR":
                case "POL":
                case "RUS":
                case "SWE":
                case "THA":
                case "ENG":
                case "FRE":
                case "GER":
                    string greats = "";
                    for (int i = 1; i < GetAncestorInfo(descendant, ancestor).AncestorDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                    }
                    text = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
                    text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                    break;
                case "DUT":
                    List<string> binaryGroups = new List<string>();
                    int ancestorDistance = GetAncestorInfo(descendant, ancestor).AncestorDistance;
                    while (ancestorDistance > 511)
                    {
                        binaryGroups.Insert(0, "111111111");
                        ancestorDistance -= 511;
                    }
                    binaryGroups.Insert(0, Convert.ToString(ancestorDistance - (binaryGroups.Count == 0 ? 0 : 1), 2));
                    while (binaryGroups[0].Length < 9)
                    {
                        binaryGroups[0] = "0" + binaryGroups[0];
                    }
                    string[] ancestorPrefixes = new string[binaryGroups.Count];
                    for (int i = 0; i < ancestorPrefixes.Length; i++)
                    {
                        ancestorPrefixes[i] = "";
                        for (int j = 0; j < 7; j++)
                        {
                            if (binaryGroups[i][j] == '1')
                            {
                                ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + (9 - j).ToString());
                            }
                        }
                        switch (binaryGroups[i].Substring(binaryGroups[i].Length - 2))
                        {
                            case "01":
                                ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand");
                                break;
                            case "10":
                                ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1");
                                break;
                            case "11":
                                ancestorPrefixes[i] += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun2");
                                break;
                        }
                    }
                    for (int i = 0; i < ancestorPrefixes.Length; i++)
                    {
                        text += Localization.LocalizeString(isFemale, i == 0 ? "Destrospean/Genealogy:GreatNxGrandparent" : "Destrospean/Genealogy:OrdinalSuffixNoun12", ancestorPrefixes[i], isInLaw && i == ancestorPrefixes.Length - 1 ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
                        if (i < ancestorPrefixes.Length - 1)
                        {
                            text += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun11");
                        }
                    }
                    text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                    break;
                case "ITA":
                case "POR":
                case "SPA":
                    string nthGrandparent = GetAncestorInfo(descendant, ancestor).AncestorDistance.ToString();
                    if (nthGrandparent == "3")
                    {
                        text = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGP", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
                    }
                    else if (int.Parse(nthGrandparent) > 3)
                    {
                        text = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", nthGrandparent, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandparent)), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
                    }
                    break;
            }
            return text;
        }

        public static string GetDescendantOfSiblingString(bool isFemale, Genealogy siblingOfAncestor, Genealogy descendantOfSibling)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isGerman = true, isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !kShowHalfRelatives)
                    {
                        return "";
                    }
                    switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                    {
                        case "CHS":
                        case "CHT":
                        case "CZE":
                        case "DAN":
                        case "FIN":
                        case "GRE":
                        case "HUN":
                        case "JPN":
                        case "KOR":
                        case "NOR":
                        case "POL":
                        case "RUS":
                        case "SWE":
                        case "THA":
                        case "DUT":
                        case "ENG":
                        case "FRE":
                        case "ITA":
                            isGerman = false;
                            goto case "GER";
                        case "GER":
                            string greats = "";
                            for (int i = 1; i < GetAncestorInfo(descendantOfSibling, sibling).AncestorDistance; i++)
                            {
                                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                            }
                            string text = kShow1stCousinsAsCousins || !isGerman ? Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedDownward" : "NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
                            return text.Substring(0, 1).ToUpper() + text.Substring(1);
                        case "POR":
                        case "SPA":
                            string descendantTitle, nthGrandchild = GetAncestorInfo(descendantOfSibling, sibling).AncestorDistance.ToString();
                            switch (nthGrandchild)
                            {
                                case "1":
                                    descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandchild");
                                    break;
                                case "2":
                                    descendantTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGC");
                                    break;
                                case "3":
                                    descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGC");
                                    break;
                                default:
                                    descendantTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", nthGrandchild, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandchild)));
                                    break;
                            }
                            return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfNephew" : "Destrospean/Genealogy:GreatNxNephew", descendantTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousinNxRemovedDownward" : "Destrospean/Genealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), descendantTitle.ToLower());
                    }
                }
            }
            return "";
        }


        public static string GetDescendantString(Genealogy ancestor, Genealogy descendant)
        {
            return GetDescendantString(descendant.SimDescription.IsFemale, ancestor, descendant, false);
        }

        public static string GetDescendantString(bool isFemale, Genealogy ancestor, Genealogy descendant, bool isInLaw)
        {
            string greats = "", text = "";
            switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
            {
                case "CHS":
                case "CHT":
                case "CZE":
                case "DAN":
                case "FIN":
                case "GRE":
                case "HUN":
                case "JPN":
                case "KOR":
                case "NOR":
                case "POL":
                case "RUS":
                case "SWE":
                case "THA":
                case "ENG":
                case "FRE":
                case "GER":
                    if (string.IsNullOrEmpty(greats))
                    {
                        for (int i = 1; i < GetAncestorInfo(descendant, ancestor).AncestorDistance; i++)
                        {
                            greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                        }
                    }
                    text = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", greats, isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
                    text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                    break;
                case "DUT":
                    for (int i = 1; i < GetAncestorInfo(descendant, ancestor).AncestorDistance; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun0");
                    }
                    goto case "GER";
                case "ITA":
                case "POR":
                case "SPA":
                    string nthGrandchild = GetAncestorInfo(descendant, ancestor).AncestorDistance.ToString();
                    if (nthGrandchild == "3")
                    {
                        text = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGC", isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
                    }
                    else if (int.Parse(nthGrandchild) > 3)
                    {
                        text = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandchild", nthGrandchild, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandchild)), isInLaw ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:InLaw") : "");
                    }
                    break;
            }
            return text;
        }

        public static string GetDistantRelationString(bool isFemale, Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            if (distantRelationInfo == null)
            {
                return "";
            }
            bool isGerman = true, isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0], distantRelationInfo.ThroughWhichChildren[1]);
            if (isHalfRelative && !kShowHalfRelatives)
            {
                return "";
            }
            string degree = distantRelationInfo.Degree.ToString(), greatNxUncleEntryKey = "Destrospean/Genealogy:GreatNx" + (isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Half" : "") + (distantRelationInfo.ClosestDescendant == sim ? "Uncle" : "Nephew"), nthCousinNxRemovedEntryKey = string.Format("Destrospean/Genealogy:Nth{0}CousinNxRemoved{1}ward", isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Half" : "", distantRelationInfo.ClosestDescendant == sim ? "Up" : "Down");
            if (distantRelationInfo.Degree > 0)
            {
                if (distantRelationInfo.Degree <= (uint)kMaxDegreeCousinsToShow)
                {
                    if (distantRelationInfo.TimesRemoved > 0)
                    {
                        if (distantRelationInfo.TimesRemoved <= (uint)kMaxTimesRemovedCousinsToShow)
                        {
                            switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                            {
                                case "CHS":
                                case "CHT":
                                case "CZE":
                                case "DAN":
                                case "FIN":
                                case "GRE":
                                case "HUN":
                                case "JPN":
                                case "KOR":
                                case "NOR":
                                case "POL":
                                case "RUS":
                                case "SWE":
                                case "THA":
                                case "ENG":
                                    return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), distantRelationInfo.TimesRemoved.ToString());
                                case "DUT":
                                    degree = "";
                                    for (int i = 0; i < distantRelationInfo.Degree; i++)
                                    {
                                        degree += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun0");
                                    }
                                    goto case "ITA";
                                case "GER":
                                    degree = (distantRelationInfo.Degree + 1).ToString();
                                    distantRelationInfo.TimesRemoved--;
                                    goto case "ITA";
                                case "FRE":
                                case "ITA":
                                    string greats = "";
                                    for (int i = 1; i < distantRelationInfo.TimesRemoved; i++)
                                    {
                                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                                    }
                                    return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), greats, distantRelationInfo.TimesRemoved > 0 ? Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand") : "");
                                case "POR":
                                case "SPA":
                                    string ancestorTitle = "";
                                    degree = (distantRelationInfo.Degree + 1).ToString();
                                    switch (distantRelationInfo.TimesRemoved)
                                    {
                                        case 1:
                                            break;
                                        case 2:
                                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"));
                                            break;
                                        case 3:
                                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                                            break;
                                        case 4:
                                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                                            break;
                                        default:
                                            string nthGrandparent = (distantRelationInfo.TimesRemoved - 1).ToString();
                                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"), nthGrandparent, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandparent)));
                                            break;
                                    }
                                    return Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)), ancestorTitle.ToLower()).Replace("  ", " ");
                            }
                        }
                        return "";
                    }
                    if (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode") == "DUT")
                    {
                        degree = "";
                        for (int i = 1; i < distantRelationInfo.Degree; i++)
                        {
                            degree += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun0");
                        }
                    }
                    return Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousin" : "Destrospean/Genealogy:NthCousin", degree, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun" + GetOrdinalSuffix(degree)));
                }
                return "";
            }
            if (distantRelationInfo.TimesRemoved < 1)
            {
                return "";
            }
            switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
            {
                case "CHS":
                case "CHT":
                case "CZE":
                case "DAN":
                case "FIN":
                case "GRE":
                case "HUN":
                case "JPN":
                case "KOR":
                case "NOR":
                case "POL":
                case "RUS":
                case "SWE":
                case "THA":
                case "DUT":
                case "ENG":
                case "FRE":
                case "ITA":
                    isGerman = false;
                    goto case "GER";
                case "GER":
                    string greats = "";
                    for (int i = 2; i < distantRelationInfo.TimesRemoved; i++)
                    {
                        greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                    }
                    return kShow1stCousinsAsCousins || !isGerman ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, greats) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
                case "POR":
                case "SPA":
                    string ancestorTitle = "";
                    switch (distantRelationInfo.TimesRemoved)
                    {
                        case 1:
                            break;
                        case 2:
                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"));
                            break;
                        case 3:
                            ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                            break;
                        case 4:
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGG" + (distantRelationInfo.ClosestDescendant == sim ? "P" : "C"));
                            break;
                        default:
                            string nthGrandparent = (distantRelationInfo.TimesRemoved - 1).ToString();
                            ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrand" + (distantRelationInfo.ClosestDescendant == sim ? "parent" : "child"), nthGrandparent, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandparent)));
                            break;
                    }
                    return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, greatNxUncleEntryKey, ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, nthCousinNxRemovedEntryKey, "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), ancestorTitle.ToLower()).Replace("  ", " ");
                default:
                    return "";
            }
        }

        public static string GetDistantRelationString(Genealogy sim, DistantRelationInfo distantRelationInfo)
        {
            return GetDistantRelationString(sim.SimDescription.IsFemale, sim, distantRelationInfo);
        }

        public static string GetOrdinalSuffix(string number)
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

        public static string GetSiblingOfAncestorString(bool isFemale, Genealogy descendantOfSibling, Genealogy siblingOfAncestor)
        {
            foreach (Genealogy sibling in siblingOfAncestor.Siblings)
            {
                if (descendantOfSibling.IsAncestor(sibling))
                {
                    bool isGerman = true, isHalfRelative = Genealogy.IsHalfSibling(sibling, siblingOfAncestor);
                    if (isHalfRelative && !kShowHalfRelatives)
                    {
                        return "";
                    }
                    switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                    {
                        case "CHS":
                        case "CHT":
                        case "CZE":
                        case "DAN":
                        case "FIN":
                        case "GRE":
                        case "HUN":
                        case "JPN":
                        case "KOR":
                        case "NOR":
                        case "POL":
                        case "RUS":
                        case "SWE":
                        case "THA":
                        case "DUT":
                        case "ENG":
                        case "FRE":
                        case "ITA":
                            isGerman = false;
                            goto case "GER";
                        case "GER":
                            string greats = "";
                            for (int i = 1; i < GetAncestorInfo(descendantOfSibling, sibling).AncestorDistance; i++)
                            {
                                greats += Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Great");
                            }
                            string text = kShow1stCousinsAsCousins || !isGerman ? Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", greats) : Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "NthHalfCousinNxRemovedUpward" : "NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), greats, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:Grand"));
                            return text.Substring(0, 1).ToUpper() + text.Substring(1);
                        case "POR":
                        case "SPA":
                            string ancestorTitle, nthGrandparent = GetAncestorInfo(descendantOfSibling, sibling).AncestorDistance.ToString();
                            switch (nthGrandparent)
                            {
                                case "1":
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:Grandparent");
                                    break;
                                case "2":
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Gameplay/Socializing:GGP");
                                    break;
                                case "3":
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GGGP");
                                    break;
                                default:
                                    ancestorTitle = Localization.LocalizeString(isFemale, "Destrospean/Genealogy:GreatNxGrandparent", nthGrandparent, Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixAdj" + GetOrdinalSuffix(nthGrandparent)));
                                    break;
                            }
                            return kShow1stCousinsAsCousins ? Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:GreatNxHalfUncle" : "Destrospean/Genealogy:GreatNxUncle", ancestorTitle.ToLower()) : Localization.LocalizeString(isFemale, isHalfRelative && !kShowHalfRelativesAsFullRelatives ? "Destrospean/Genealogy:NthHalfCousinNxRemovedUpward" : "Destrospean/Genealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(isFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), ancestorTitle.ToLower());
                    }
                }
            }
            return "";
        }

        public static bool IsHalfCousin(Genealogy sim1, Genealogy sim2)
        {
            foreach (Genealogy parent1 in sim1.Parents)
            {
                foreach (Genealogy parent2 in sim2.Parents)
                {
                    if (Genealogy.IsHalfSibling(parent1, parent2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsHalfNephew(Genealogy nephew, Genealogy uncle)
        {
            return IsHalfUncle(uncle, nephew);
        }

        public static bool IsHalfSiblingInLaw(Genealogy sim1, Genealogy sim2)
        {
            if (sim1.Spouse != null && sim1.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy sibling in sim1.Spouse.Siblings)
                {
                    if (sibling == sim2 || (sibling.Spouse == sim2 && sibling.PartnerType == PartnerType.Marriage))
                    {
                        return Genealogy.IsHalfSibling(sim1.Spouse, sibling);
                    }
                }
            }
            if (sim2.Spouse != null && sim2.PartnerType == PartnerType.Marriage)
            {
                foreach (Genealogy sibling in sim2.Spouse.Siblings)
                {
                    if (sibling == sim1 || (sibling.Spouse == sim1 && sibling.PartnerType == PartnerType.Marriage))
                    {
                        return Genealogy.IsHalfSibling(sim2.Spouse, sibling);
                    }
                }
            }
            return false;
        }

        public static bool IsHalfUncle(Genealogy uncle, Genealogy nephew)
        {
            foreach (Genealogy parent in nephew.Parents)
            {
                foreach (Genealogy sibling in parent.Siblings)
                {
                    if (sibling == uncle || sibling.Spouse == uncle)
                    {
                        return Genealogy.IsHalfSibling(parent, uncle);
                    }
                }
            }
            return false;
        }

        [TypePatch(typeof(Genealogy))]
        public class GenealogyPatch
        {
            public static bool IsSiblingInLaw(Genealogy sim1, Genealogy sim2)
            {
                if (sim1.Spouse != null && sim1.PartnerType == PartnerType.Marriage)
                {
                    foreach (Genealogy sibling in sim1.Spouse.Siblings)
                    {
                        if (sibling == sim2 || (sibling.Spouse == sim2 && sibling.PartnerType == PartnerType.Marriage))
                        {
                            return true;
                        }
                    }
                }
                if (sim2.Spouse != null && sim2.PartnerType == PartnerType.Marriage)
                {
                    foreach (Genealogy sibling in sim2.Spouse.Siblings)
                    {
                        if (sibling == sim1 || (sibling.Spouse == sim1 && sibling.PartnerType == PartnerType.Marriage))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool IsStepRelated(Genealogy other)
            {
                Genealogy self = (Genealogy)(this as object);
                DistantRelationInfo distantRelationInfo = CalculateDistantRelation(other, self);
                bool isHalfRelative = false;
                if (distantRelationInfo != null)
                {
                    isHalfRelative = Genealogy.IsHalfSibling(distantRelationInfo.ThroughWhichChildren[0], distantRelationInfo.ThroughWhichChildren[1]);
                }
                if (!(distantRelationInfo == null || distantRelationInfo.Degree >= (uint)kMinDegreeCousinsToAllowRomance || (isHalfRelative && kAllowRomanceForHalfRelatives) || distantRelationInfo.TimesRemoved >= (uint)kMinTimesRemovedCousinsToAllowRomance))
                {
                    return true;
                }
                foreach (Genealogy parent1 in self.Parents)
                {
                    foreach (Genealogy parent2 in other.Parents)
                    {
                        if (parent1.Spouse == parent2 && parent1.PartnerType == PartnerType.Marriage)
                        {
                            return true;
                        }
                    }
                }
                foreach (Genealogy parent in self.Parents)
                {
                    if (parent.Spouse == other && parent.PartnerType == PartnerType.Marriage)
                    {
                        return true;
                    }
                }
                foreach (Genealogy parent in other.Parents)
                {
                    if (parent.Spouse == self && parent.PartnerType == PartnerType.Marriage)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static bool IsUncle(Genealogy uncle, Genealogy nephew)
            {
                foreach (Genealogy parent in nephew.Parents)
                {
                    foreach (Genealogy sibling in parent.Siblings)
                    {
                        if (sibling == uncle || (sibling.Spouse == uncle && sibling.PartnerType == PartnerType.Marriage))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        [TypePatch(typeof(SimDescription))]
        public class SimDescriptionPatch
        {
            public string GetMyFamilialDescriptionFor(SimDescription other)
            {
                SimDescription self = (SimDescription)(this as object);
                if (other.Genealogy == self.Genealogy)
                {
                    return "";
                }
                if (GameUtils.IsAnyTravelBasedWorld() && GameStates.TravelerIds != null && GameStates.TravelerIds.Contains(self.SimDescriptionId))
                {
                    MiniSimDescription miniSimDescription = MiniSimDescription.Find(self.SimDescriptionId);
                    if (miniSimDescription != null && miniSimDescription.MiniRelationships != null)
                    {
                        foreach (MiniRelationship miniRelationship in miniSimDescription.MiniRelationships)
                        {
                            if (miniRelationship.SimDescriptionId == other.SimDescriptionId)
                            {
                                return miniRelationship.FamilialString;
                            }
                        }
                    }
                }
                string text = "";
                if (Genealogy.IsParent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Parent");
                }
                else if (Genealogy.IsParentInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:ParentInLaw");
                }
                else if (Genealogy.IsChild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Child");
                }
                else if (Genealogy.IsChildInLaw(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:ChildInLaw");
                }
                else if (Genealogy.IsHalfSibling(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:HalfSibling");
                }
                else if (Genealogy.IsSibling(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Sibling");
                }
                else if (IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) && kShowHalfRelatives && !kShowHalfRelativesAsFullRelatives)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfSiblingInLaw");
                }
                else if (Genealogy.IsSiblingInLaw(other.Genealogy, self.Genealogy) && (!IsHalfSiblingInLaw(other.Genealogy, self.Genealogy) || kShowHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:SiblingInLaw");
                }
                else if (Genealogy.IsGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Grandparent");
                }
                else if (Genealogy.IsGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Grandchild");
                }
                else if (Genealogy.IsStepParent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:StepParent");
                }
                else if (Genealogy.IsStepChild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:StepChild");
                }
                else if (Genealogy.IsStepSibling(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:StepSibling");
                }
                else if (IsHalfCousin(other.Genealogy, self.Genealogy) && kShowHalfRelatives && !kShowHalfRelativesAsFullRelatives && kShow1stCousinsAsCousins)
                {
                    text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfCousin");
                }
                else if (Genealogy.IsCousin(other.Genealogy, self.Genealogy) && kShow1stCousinsAsCousins && (!IsHalfCousin(other.Genealogy, self.Genealogy) || kShowHalfRelativesAsFullRelatives))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Cousin");
                }
                else if (IsHalfUncle(other.Genealogy, self.Genealogy) && kShowHalfRelatives && !kShowHalfRelativesAsFullRelatives)
                {
                    switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                    {
                        case "GER":
                        case "POR":
                        case "SPA":
                            text = kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfUncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthHalfCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "").Replace("  ", " ");
                            text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                            break;
                        default:
                            text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfUncle");
                            break;
                    }
                }
                else if (Genealogy.IsFatherSideUncle(other.Genealogy, self.Genealogy) && Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!IsHalfUncle(other.Genealogy, self.Genealogy) || kShowHalfRelativesAsFullRelatives))
                {
                    switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                    {
                        case "GER":
                        case "POR":
                        case "SPA":
                            text = kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Uncle") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "").Replace("  ", " ");
                            text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                            break;
                        default:
                            text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Uncle");
                            break;
                    }
                }
                else if (Genealogy.IsMotherSideUncle(other.Genealogy, self.Genealogy) && Genealogy.IsUncle(other.Genealogy, self.Genealogy) && (!IsHalfUncle(other.Genealogy, self.Genealogy) || kShowHalfRelativesAsFullRelatives))
                {
                    switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                    {
                        case "GER":
                        case "POR":
                        case "SPA":
                            text = kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:UncleMothersSide") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthCousinNxRemovedUpward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "").Replace("  ", " ");
                            text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                            break;
                        default:
                            text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:UncleMothersSide");
                            break;
                    }
                }
                else if (IsHalfNephew(other.Genealogy, self.Genealogy) && kShowHalfRelatives && !kShowHalfRelativesAsFullRelatives)
                {
                    switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                    {
                        case "GER":
                        case "POR":
                        case "SPA":
                            text = kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfNephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthHalfCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "").Replace("  ", " ");
                            text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                            break;
                        default:
                            text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:HalfNephew");
                            break;
                    }
                }
                else if (Genealogy.IsNephew(other.Genealogy, self.Genealogy) && (!IsHalfNephew(other.Genealogy, self.Genealogy) || kShowHalfRelativesAsFullRelatives))
                {
                    switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                    {
                        case "GER":
                        case "POR":
                        case "SPA":
                            text = kShow1stCousinsAsCousins ? Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Nephew") : Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:NthCousinNxRemovedDownward", "1", Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:OrdinalSuffixNoun1"), "", "").Replace("  ", " ");
                            text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                            break;
                        default:
                            text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Nephew");
                            break;
                    }
                }
                else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:GGP");
                }
                else if (Genealogy.IsGreatGrandchild(other.Genealogy, self.Genealogy))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:GGC");
                }
                else if (self.Genealogy.IsAncestor(other.Genealogy))
                {
                    text = GetAncestorString(self.Genealogy, other.Genealogy);
                }
                else if (other.Genealogy.IsAncestor(self.Genealogy))
                {
                    text = GetDescendantString(self.Genealogy, other.Genealogy);
                }
                else if (self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.Spouse.IsAncestor(other.Genealogy) && self.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    if (Genealogy.IsGrandparent(other.Genealogy, self.Genealogy.Spouse))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:GrandparentInLaw");
                    }
                    else if (Genealogy.IsGreatGrandparent(other.Genealogy, self.Genealogy.Spouse))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:GGPInLaw");
                    }
                    else
                    {
                        text = GetAncestorString(other.IsFemale, self.Genealogy.Spouse, other.Genealogy, true);
                    }
                }
                else if (other.Genealogy.Spouse != null && other.Genealogy.Spouse != self.Genealogy && other.Genealogy.Spouse.IsAncestor(self.Genealogy) && other.Genealogy.PartnerType == PartnerType.Marriage)
                {
                    if (Genealogy.IsGrandchild(other.Genealogy.Spouse, self.Genealogy))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:GrandchildInLaw");
                    }
                    else if (Genealogy.IsGreatGrandchild(other.Genealogy.Spouse, self.Genealogy))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:GGCInLaw");
                    }
                    else
                    {
                        text = GetDescendantString(other.IsFemale, self.Genealogy, other.Genealogy.Spouse, true);
                    }
                }
                else
                {
                    DistantRelationInfo distantRelationInfo = CalculateDistantRelation(other.Genealogy, self.Genealogy);
                    if (distantRelationInfo == null)
                    {
                        if (!(other.Genealogy.Spouse == null || other.Genealogy.Spouse == self.Genealogy || other.Genealogy.PartnerType != PartnerType.Marriage))
                        {
                            distantRelationInfo = CalculateDistantRelation(other.Genealogy.Spouse, self.Genealogy);
                        }
                        if (distantRelationInfo == null)
                        {
                            if (!(self.Genealogy.Spouse == null || self.Genealogy.Spouse == other.Genealogy || self.Genealogy.PartnerType != PartnerType.Marriage))
                            {
                                distantRelationInfo = CalculateDistantRelation(other.Genealogy, self.Genealogy.Spouse);
                            }
                            if (distantRelationInfo != null && distantRelationInfo.Degree == 0 && distantRelationInfo.ClosestDescendant == self.Genealogy.Spouse)
                            {
                                text = GetDistantRelationString(other.Genealogy, distantRelationInfo);
                            }
                        }
                        else if (distantRelationInfo.Degree == 0 && distantRelationInfo.ClosestDescendant == other.Genealogy.Spouse)
                        {
                            text = GetDistantRelationString(other.IsFemale, other.Genealogy.Spouse, distantRelationInfo);
                        }
                    }
                    else
                    {
                        text = GetDistantRelationString(other.Genealogy, distantRelationInfo);
                    }
                    if (distantRelationInfo == null)
                    {
                        text = GetSiblingOfAncestorString(other.IsFemale, self.Genealogy, other.Genealogy);
                        if (string.IsNullOrEmpty(text) && other.Genealogy.Spouse != null && self.Genealogy != other.Genealogy.Spouse && other.Genealogy.PartnerType == PartnerType.Marriage)
                        {
                            text = GetSiblingOfAncestorString(other.IsFemale, self.Genealogy, other.Genealogy.Spouse);
                        }
                        if (string.IsNullOrEmpty(text))
                        {
                            text = GetDescendantOfSiblingString(other.IsFemale, self.Genealogy, other.Genealogy);
                        }
                        if (string.IsNullOrEmpty(text) && self.Genealogy.Spouse != null && self.Genealogy.Spouse != other.Genealogy && self.Genealogy.PartnerType == PartnerType.Marriage)
                        {
                            text = GetDescendantOfSiblingString(other.IsFemale, self.Genealogy.Spouse, other.Genealogy);
                        }
                    }
                    if (!string.IsNullOrEmpty(text))
                    {
                        switch (Localization.LocalizeString("Destrospean/Genealogy:LanguageCode"))
                        {
                            case "DUT":
                            case "FRE":
                            case "GER":
                            case "ITA":
                                text = text.Substring(0, 1).ToUpper() + text.Substring(1);
                                break;
                        }
                    }
                }
                if (FutureDescendantService.IsAncestorOf(other, self))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Ancestor");
                }
                else if (FutureDescendantService.IsDescendantOf(other, self))
                {
                    text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Descendant");
                }
                Relationship relationship = Relationship.Get(self, other, false);
                if (relationship != null)
                {
                    if (relationship.CurrentLTR != LongTermRelationshipTypes.Spouse && relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.AreLitterMates))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Sibling_Pet");
                    }
                    if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.HumanParentPetRel))
                    {
                        if (self.IsHuman)
                        {
                            if (other.IsADogSpecies)
                            {
                                text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Owns_puppy");
                            }
                            if (other.IsCat)
                            {
                                text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Owns_kitten");
                            }
                        }
                        else
                        {
                            text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Parent");
                        }
                    }
                }
                if ((other.IsEP11Bot || self.IsEP11Bot || ((other.IsFrankenstein || self.IsFrankenstein) && kReplaceRelationsForSimBots)) && !string.IsNullOrEmpty(text))
                {
                    if (Genealogy.IsParent(other.Genealogy, self.Genealogy) && (self.IsEP11Bot || (self.IsFrankenstein && kReplaceRelationsForSimBots)))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Creator");
                    }
                    else if (Genealogy.IsChild(other.Genealogy, self.Genealogy) && (other.IsEP11Bot || (other.IsFrankenstein && kReplaceRelationsForSimBots)))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:Creation");
                    }
                    else if ((self.IsEP11Bot || (self.IsFrankenstein && kReplaceRelationsForSimBots)) && !(other.IsEP11Bot || (other.IsFrankenstein && kReplaceRelationsForSimBots)))
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:FamilyMember");
                    }
                    else if (other.IsEP11Bot)
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Gameplay/Socializing:FamilyBot");
                    }
                    else if (other.IsFrankenstein && kReplaceRelationsForSimBots)
                    {
                        text = Localization.LocalizeString(other.IsFemale, "Destrospean/Genealogy:FamilyBot");
                    }
                }
                return text;
            }
        }
    }
}