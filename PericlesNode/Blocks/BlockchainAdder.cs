using System;
using System.Collections.Generic;
using Pericles.Configuration.Console.Interfaces;
using Pericles.Votes;

namespace Pericles.Blocks
{
    public class BlockchainAdder
    {
        private readonly Blockchain blockchain;
        private readonly VoteMemoryPool voteMemoryPool;
        private readonly BlockForwarder blockForwarder;
        private readonly IConsole console;

        public BlockchainAdder(
            Blockchain blockchain,
            VoteMemoryPool voteMemoryPool,
            BlockForwarder blockForwarder,
            IConsole console)
        {
            this.blockchain = blockchain;
            this.voteMemoryPool = voteMemoryPool;
            this.blockForwarder = blockForwarder;
            this.console = console;
        }

        public void AddNewBlock(Block block)
        {
            this.blockchain.AddBlock(block);
            this.RemoveVotesFromMemPool(block.MerkleTree.Votes);

            this.console.WriteLine($"added new block: {block.Hash}");
            this.console.WriteLine($"new blockchain height = {this.blockchain.CurrentHeight}");

            this.blockForwarder.ForwardBlock(block);
        }

        private void RemoveVotesFromMemPool(IEnumerable<Vote> votes)
        {
            foreach (var vote in votes)
            {
                this.voteMemoryPool.DeleteVote(vote.Hash);
            }
        }
    }
}