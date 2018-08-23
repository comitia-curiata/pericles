using Pericles.Crypto;
using ElectionModels;

namespace Pericles.VoterTerminal
{
    interface IVoterTerminal
    {
        bool Login(out EncryptedKeyPair crypticKeyPair);
        void BallotPrompt(ElectionType electionType);
        string GetResult();
        string GetMyVote(EncryptedKeyPair crypticPair);
    }
}
