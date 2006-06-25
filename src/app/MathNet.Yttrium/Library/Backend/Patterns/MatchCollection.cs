using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class MatchCollection : KeyedCollection<MathIdentifier,Match>
    {
        public MatchCollection()
            : base()
        {
        }

        protected override MathIdentifier GetKeyForItem(Match item)
        {
            return item.PatternId;
        }

        public bool TryGetValue(MathIdentifier id, out Match match)
        {
            if(Dictionary == null)
            {
                match = null;
                return false;
            }
            return Dictionary.TryGetValue(id, out match);
        }

        public static MatchCollection CombineUnion(List<MatchCollection> matchesList)
        {
            if(matchesList.Count == 0)
                return new MatchCollection();
            if(matchesList.Count == 1)
                return matchesList[0];

            MatchCollection res = matchesList[matchesList.Count - 1];
            matchesList.RemoveAt(matchesList.Count - 1);

            foreach(MatchCollection matches in matchesList)
            {
                foreach(Match match in matches)
                {
                    /*
                     * If a match was already added by another chid,
                     * copy over its captured groups to the existing match.
                     * Otherwise copy over the whole match.
                     */
                    Match m;
                    if(res.TryGetValue(match.PatternId, out m))
                        m.MergeWithMatch(match);
                    else
                        res.Add(match);
                }
            }

            return res;
        }

        ///// <summary>
        ///// Combines Match lists from a list of children (inputs). To match the parent node,
        ///// every match must succeed on every single child (intersection, logical AND).
        ///// Labeled groups of the children are merged.
        ///// </summary>
        public static MatchCollection CombineIntersect(List<MatchCollection> matchesList)
        {
            if(matchesList.Count == 1)
                return matchesList[0];
            MatchCollection res = new MatchCollection();
            if(matchesList.Count == 0)
                return res;
            MatchCollection lastMatches = matchesList[matchesList.Count - 1];
            matchesList.RemoveAt(matchesList.Count - 1);
            foreach(Match lastMatch in lastMatches)
            {
                bool suitable = true;
                foreach(MatchCollection matches in matchesList)
                {
                    // Ensure AND
                    Match match;
                    if(!matches.TryGetValue(lastMatch.PatternId, out match))
                    {
                        suitable = false;
                        break;
                    }
                    // Merge Groups
                    lastMatch.MergeWithMatch(match);
                }
                if(suitable)
                    res.Add(lastMatch);
            }
            return res;
        }

        public static Match CombineIntersectFirst(List<MatchCollection> matchesList)
        {
            if(matchesList.Count == 0)
                return null;
            if(matchesList.Count == 1)
            {
                MatchCollection d = matchesList[0];
                if(d.Count > 0)
                    return d[0];
                else
                    return null;

            }
            MatchCollection lastMatches = matchesList[matchesList.Count - 1];
            matchesList.RemoveAt(matchesList.Count - 1);
            foreach(Match lastMatch in lastMatches)
            {
                bool suitable = true;
                foreach(MatchCollection matches in matchesList)
                {
                    // Ensure AND
                    Match match;
                    if(!matches.TryGetValue(lastMatch.PatternId, out match))
                    {
                        suitable = false;
                        break;
                    }
                    // Merge Groups
                    lastMatch.MergeWithMatch(match);
                }
                if(suitable)
                    return lastMatch;
            }
            return null;
        }
    }
}
