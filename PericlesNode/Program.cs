using System;
using ElectionModels;
using Pericles.Blocks;
using Pericles.CommonUtils;
using Pericles.Configuration;
using Pericles.Configuration.Console;
using Pericles.Crypto;
using Pericles.Merkle;
using Pericles.Mining;
using Pericles.Networking;
using Pericles.Votes;
using VoterDatabase;

namespace Pericles
{
    public static class Program
    {
        private const int RegistrarPort = 50083;
        private const int MinNetworkSize = 3;

        public static void Main(string[] args)
        {
            var configFilepath = args[0];
            var password = args[1];

            // config
            var nodeConfig = ConfigDeserializer.Deserialize<NodeConfig>(configFilepath);
            var console = ConsoleFactory.Build(nodeConfig.IsMiningNode);
            var ipAddress = IpAddressProvider.GetLocalIpAddress();

            // password check
            var voterDb = new VoterDatabaseFacade(nodeConfig.VoterDbFilepath);
            var foundMiner = voterDb.TryGetVoterEncryptedKeyPair(password, out var encryptedKeyPair);
            if (!foundMiner)
            {
                Console.WriteLine("incorrect password: you may not mine!");
                return;
            }

            // blockchain
            var blockchain = new Blockchain();

            // networking
            var registrarClientFactory = new RegistrarClientFactory();
            var registrarClient = registrarClientFactory.Build(ipAddress, RegistrarPort);
            var registrationRequestFactory = new RegistrationRequestFactory();
            var myConnectionInfo = new NodeConnectionInfo(ipAddress, nodeConfig.Port);
            var knownNodeStore = new KnownNodeStore();
            var nodeClientFactory = new NodeClientFactory();
            var handshakeRequestFactory = new HandshakeRequestFactory(blockchain);
            var nodeClientStore = new NodeClientStore();
            var nodeServerFactory = new NodeServerFactory();

            // votes
            var protoVoteFactory = new ProtoVoteFactory();
            var voteForwarder = new VoteForwarder(nodeClientStore, protoVoteFactory);
            var voteMemoryPool = new VoteMemoryPool(voteForwarder, console);

            // blocks
            var merkleNodeFactory = new MerkleNodeFactory();
            var merkleTreeFactory = new MerkleTreeFactory(merkleNodeFactory);
            var minerId = encryptedKeyPair.PublicKey.GetBase64String();
            var blockFactory = new BlockFactory(merkleTreeFactory, minerId);
            var protoBlockFactory = new ProtoBlockFactory(protoVoteFactory);
            var blockForwarder = new BlockForwarder(nodeClientStore, protoBlockFactory);
            var voteValidator = new VoteValidator(blockchain, voterDb, nodeConfig.ElectionEndTime);
            var blockValidator = new BlockValidator(blockFactory, voteValidator);
            var blockchainAdder = new BlockchainAdder(blockchain, voteMemoryPool, blockForwarder, console);

            // mining
            var difficultyTarget = TargetFactory.Build(BlockHeader.DefaultBits);
            var miner = new Miner(
                blockchain,
                voteMemoryPool,
                difficultyTarget,
                blockFactory,
                blockchainAdder,
                console);

            // interaction
            var voteSerializer = new VoteSerializer();
            var electionAlgorithmFactory = new ElectionAlgorithmFactory(voteSerializer);
            var electionAlgorithm = electionAlgorithmFactory.Build(nodeConfig.ElectionType);
            var electionResultProvider = new ElectionResultProvider(
                electionAlgorithm,
                nodeConfig.ElectionEndTime,
                voteMemoryPool,
                blockchain);
            var voterTerminal = new VoterTerminal.VoterTerminal(
                voterDb,
                nodeConfig.Candidates.ToArray(),
                voteSerializer,
                voteMemoryPool,
                electionResultProvider);
            var votingBooth = new VoterTerminal.VoterBooth(voterTerminal, nodeConfig.ElectionType);

            // startup
            var nodeServer = nodeServerFactory.Build(
                myConnectionInfo,
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
            var boostrapper = new Bootstrapper(
                MinNetworkSize,
                knownNodeStore,
                nodeClientFactory,
                handshakeRequestFactory,
                nodeClientStore,
                registrarClient,
                registrationRequestFactory,
                nodeServer);

            Console.WriteLine("bootstrapping node network...");
            boostrapper.Bootstrap(myConnectionInfo);
            Console.WriteLine($"{MinNetworkSize} nodes in network! bootstrapping complete");

            if (nodeConfig.IsMiningNode)
            {
                Console.WriteLine("starting miner...");
                miner.Start();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
            }
            else
            {
                votingBooth.LaunchBooth();
            }
        }
    }
}
