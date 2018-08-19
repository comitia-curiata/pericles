﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pericles.Transactions;

namespace Pericles.Blocks
{
    public class BlockchainAdder
    {
        private readonly Blockchain blockchain;
        private readonly TransactionMemoryPool transactionMemoryPool;
        private readonly BlockForwarder blockForwarder;

        public BlockchainAdder(
            Blockchain blockchain,
            TransactionMemoryPool transactionMemoryPool,
            BlockForwarder blockForwarder)
        {
            this.blockchain = blockchain;
            this.transactionMemoryPool = transactionMemoryPool;
            this.blockForwarder = blockForwarder;
        }

        public void AddNewBlock(Block block)
        {
            this.blockchain.AddBlock(block);
            this.RemoveTransactionsFromMemPool(block.MerkleTree.Votes);

            Console.WriteLine($"added new block: {block.Hash}");
            Console.WriteLine($"new blockchain height = {this.blockchain.CurrentHeight}");

            this.blockForwarder.ForwardBlock(block);
        }

        private void RemoveTransactionsFromMemPool(IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions.Skip(1))
            {
                this.transactionMemoryPool.DeleteTransaction(transaction.Hash);
            }
        }
    }
}