using System.Collections.Generic;

namespace Pericles.Votes
{
    public static class GenesisVote
    {
        public static readonly List<Vote> Instance =
            new List<Vote>
            {
                // all quotes by Pericles
                new Vote("Freedom is the sure possession of those alone who have the courage to defend it.",
                    "Just because you do not take an interest in politics doesn't mean politics won't take an interest in you.",
                    "Wait for that wisest of all counselors, time.")
            };
    }
}