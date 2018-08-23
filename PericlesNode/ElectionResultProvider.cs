using System;
using System.Linq;
using ElectionModels.Interfaces;
using Pericles.Votes;

namespace Pericles
{
    public class ElectionResultProvider
    {
        private readonly IElectionAlgorithm electionAlgorithm;
        private readonly DateTime electionEndTime;
        private readonly VoteMemoryPool voteMemoryPool;
        private readonly Blockchain blockchain;

        public ElectionResultProvider(
            IElectionAlgorithm electionAlgorithm,
            DateTime electionEndTime,
            VoteMemoryPool voteMemoryPool,
            Blockchain blockchain)
        {
            this.electionAlgorithm = electionAlgorithm;
            this.electionEndTime = electionEndTime;
            this.voteMemoryPool = voteMemoryPool;
            this.blockchain = blockchain;
        }

        public string GetResults()
        {
            if (DateTime.Now <= this.electionEndTime || this.voteMemoryPool.Count != 0)
            {
                return "election is not finished yet!";
            }

            var ballots = this.blockchain.GetAllBlocks()
                .Skip(1)
                .SelectMany(x => x.MerkleTree.Votes.Select(y => y.Ballot))
                .ToList();
            return this.electionAlgorithm.GetResults(ballots);
        }
    }
}