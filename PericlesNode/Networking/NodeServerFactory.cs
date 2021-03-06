﻿using Grpc.Core;
using Pericles.Blocks;
using Pericles.Configuration.Console.Interfaces;
using Pericles.Mining;
using Pericles.Protocol;
using Pericles.Votes;

namespace Pericles.Networking
{
    public class NodeServerFactory
    {
        public Server Build(
            NodeConnectionInfo myConnectionInfo, 
            KnownNodeStore knownNodeStore,
            NodeClientFactory nodeClientFactory,
            NodeClientStore nodeClientStore,
            VoteMemoryPool voteMemoryPool,
            Blockchain blockchain,
            Miner miner,
            VoteValidator voteValidator,
            BlockValidator blockValidator,
            BlockchainAdder blockchainAdder,
            IConsole console)
        {
            var handshakeService = new NodeService(
                knownNodeStore,
                nodeClientFactory,
                nodeClientStore,
                voteMemoryPool,
                blockchain,
                miner,
                voteValidator,
                blockValidator,
                blockchainAdder,
                console);
            var server = new Server
            {
                Services = { Node.BindService(handshakeService) },
                Ports = { new ServerPort(myConnectionInfo.Ip, myConnectionInfo.Port, ServerCredentials.Insecure) }
            };

            return server;
        }
    }
}