﻿using Grpc.Core;
using NikCoin.Blocks;
using NikCoin.Mining;
using NikCoin.Protocol;
using NikCoin.Transactions;

namespace NikCoin.Networking
{
    public class NodeServerFactory
    {
        public Server Build(
            NodeConnectionInfo myConnectionInfo, 
            KnownNodeStore knownNodeStore,
            NodeClientFactory nodeClientFactory,
            NodeClientStore nodeClientStore,
            TransactionMemoryPool transactionMemoryPool,
            Blockchain blockchain,
            Miner miner,
            BlockValidator blockValidator,
            BlockchainAdder blockchainAdder)
        {
            var handshakeService = new NodeService(
                knownNodeStore,
                nodeClientFactory,
                nodeClientStore,
                transactionMemoryPool,
                blockchain,
                miner,
                blockValidator,
                blockchainAdder);
            var server = new Server
            {
                Services = { Node.BindService(handshakeService) },
                Ports = { new ServerPort(myConnectionInfo.Ip, myConnectionInfo.Port, ServerCredentials.Insecure) }
            };

            return server;
        }
    }
}