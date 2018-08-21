using System;
using ElectionModels.FirstPastThePost;
using ElectionModels.InstantRunoff;
using ElectionModels.Interfaces;

namespace ElectionModels
{
    public class ElectionAlgorithmFactory
    {
        private readonly IVoteSerializer voteSerializer;

        public ElectionAlgorithmFactory(IVoteSerializer voteSerializer)
        {
            this.voteSerializer = voteSerializer;
        }

        public IElectionAlgorithm Build(ElectionType electionType)
        {
            switch (electionType)
            {
                case ElectionType.FirstPastThePost:
                    return new FirstPastThePostElectionAlgorithm(this.voteSerializer);

                case ElectionType.InstantRunoff:
                    return new InstantRunoffAlgorithm(this.voteSerializer);

                default:
                    throw new Exception($"unrecognized election type: {electionType}");
            }
        }
    }
}