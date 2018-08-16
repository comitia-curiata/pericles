﻿using System.Collections.Generic;
using System.Linq;
using NikCoin.Transactions;

namespace NikCoin.Merkle
{
    public class MerklePathValidator
    {
        private readonly MerkleNodeFactory merkleNodeFactory;

        public MerklePathValidator(MerkleNodeFactory merkleNodeFactory)
        {
            this.merkleNodeFactory = merkleNodeFactory;
        }

        public bool IsTransactionInMerkleTree(Transaction transaction, MerkleTree merkleTree)
        {
            MerkleNode transactionNode;
            if (!merkleTree.LeafNodesDictionary.TryGetValue(transaction.Hash, out transactionNode))
            {
                return false;
            }

            var merklePath = MerklePathFinder.FindMerklePath(merkleTree, transactionNode);
            if (!merklePath.Any())
            {
                return transactionNode.Hash.Equals(merkleTree.Root.Hash);
            }

            var validationRoot = this.ComputeRootUsingMerklePath(merklePath, transactionNode);
            return validationRoot.Hash.Equals(merkleTree.Root.Hash);
        }

        private MerkleNode ComputeRootUsingMerklePath(List<MerkleNode> merklePath, MerkleNode startNode)
        {
            var currNode = startNode;
            foreach (var nextMerklePathNode in merklePath)
            {
                currNode = nextMerklePathNode.IsLeftChild
                    ? this.merkleNodeFactory.BuildInternalNode(nextMerklePathNode, currNode)
                    : this.merkleNodeFactory.BuildInternalNode(currNode, nextMerklePathNode);
            }

            return currNode;
        }
    }
}