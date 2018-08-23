using System;
using System.Collections.Generic;
using System.Linq;
using Pericles.Crypto;
using VoterDatabase;
using ElectionModels;
using ElectionModels.FirstPastThePost;
using ElectionModels.InstantRunoff;
using Pericles.Votes;

namespace Pericles.VoterTerminal
{
    public class VoterTerminal: IVoterTerminal
    {
        private readonly VoterDatabaseFacade voterDb;
        private readonly string[] candidateArr;
        private readonly IVoteSerializer voteSerializer;
        private readonly VoteMemoryPool voteMemoryPool;
        private readonly ElectionResultProvider electionResultProvider;
        private readonly Blockchain blockchain;

        private EncryptedKeyPair keyPair;
        private string password;


        public VoterTerminal(
            VoterDatabaseFacade voterDb,
            string[] candidateArr,
            IVoteSerializer voteSerializer,
            VoteMemoryPool voteMemoryPool,
            ElectionResultProvider electionResultProvider,
            Blockchain blockchain)
        {
            this.voterDb = voterDb;
            this.candidateArr = candidateArr;
            this.voteSerializer = voteSerializer;
            this.voteMemoryPool = voteMemoryPool;
            this.electionResultProvider = electionResultProvider;
            this.blockchain = blockchain;
        }

        public bool Login(out EncryptedKeyPair crypticKeyPair)
        {
            Console.WriteLine("Please enter your password:\n");
            string userSuppliedPassword = Console.ReadLine();
            crypticKeyPair = null;
            var success = this.voterDb.TryGetVoterEncryptedKeyPair(userSuppliedPassword, out var encryptedKeyPair);
            if (!success)
            {
                return false;
            }

            crypticKeyPair = encryptedKeyPair;
            this.keyPair = encryptedKeyPair;
            this.password = userSuppliedPassword;
            return true;
        }

        public void BallotPrompt(ElectionType electionType) {
            
            if (electionType == ElectionType.FirstPastThePost)
            {
                var i = 1;

                Console.WriteLine("Welcome to the election! This is a 'First Past The Post' election model.\nThese are the following candidates to select from:\n" +
                    string.Join("\n", this.candidateArr.Select(x => $"{i++}: {x}"))
                    );
                int counter = 1;
                int choice = 0;
                while (counter < 4) {
                    Console.WriteLine("Please select your choice of candidate e.g. 1.");
                    choice = Convert.ToInt32(Console.ReadLine());
                    if (choice < 1 || choice > candidateArr.Length)
                    {
                        Console.WriteLine("Please enter a valid choice.");
                        counter += 1;
                    }
                    else
                    {
                        Console.WriteLine("Thank you for voting!");
                        break;
                    }
                }

                if (counter == 4)
                {
                    Console.WriteLine("You've exhausted your tries. Bye Bye");
                    return;
                }

                FirstPastThePostVote irVote = new FirstPastThePostVote(this.candidateArr[choice - 1]);
                var jsonVote = this.voteSerializer.Serialize(irVote);
                var signature = SignatureProvider.Sign(this.password, this.keyPair, jsonVote.GetBytes());
                Vote vote = new Vote(this.keyPair.PublicKey.GetBase64String(), jsonVote, signature.GetBase64String());
                this.voteMemoryPool.AddVote(vote);
            }
            else if (electionType == ElectionType.InstantRunoff)
            {
                var i = 1;
                Console.WriteLine("Welcome to the election! This is a 'Instant Runoff' election model.\nThese are the following candidates to select from:\n" +
                    string.Join("\n", this.candidateArr.Select(x => $"{i++}: {x}"))
                    );
                // We need to consider when a candidate is not wishing NONE
                int counter = 1;
                var prefs = "";
                List<string> tokens = null;
                char[] charSeparators = new char[] { ',' };
                while (counter < 4)
                {
                    Console.WriteLine("Please type a list in order of candidate preference e.g. 2,1,3.");
                    prefs= Console.ReadLine();
                    tokens = prefs.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (tokens.Count > this.candidateArr.Length)
                    {
                        Console.WriteLine("Please enter valid choices in the correct format.");
                        counter += 1;
                    }
                    else
                    {
                        Console.WriteLine("You've exhausted your tries. Bye Bye");
                        break;
                    }
                }

                if (counter == 4)
                {
                    Console.WriteLine("You've exhausted your tries. Bye Bye");
                    return;
                }

                List<string> rankedOrderedCandidates = new List<string>();

                for (int j=0; j < tokens.Count; j++)
                {
                    rankedOrderedCandidates.Add(this.candidateArr[Convert.ToInt32(tokens[j]) - 1]);
                }


                InstantRunoffVote iroVote = new InstantRunoffVote(rankedOrderedCandidates); // make the IR vote
                var jsonVote = this.voteSerializer.Serialize(iroVote);
                var signature = SignatureProvider.Sign(this.password, this.keyPair, jsonVote.GetBytes());
                Vote vote = new Vote(this.keyPair.PublicKey.GetBase64String(), jsonVote, signature.GetBase64String());
                this.voteMemoryPool.AddVote(vote);
            }
        }

        public string GetResult()
        {
            return this.electionResultProvider.GetResults();
        }

        public string GetMyVote(EncryptedKeyPair crypticKeyPair)
        {
            this.blockchain.TryGetVoteByVoter(Convert.ToBase64String(crypticKeyPair.PublicKey), out Vote vote);
            return vote.Ballot;
        }

    }
}
