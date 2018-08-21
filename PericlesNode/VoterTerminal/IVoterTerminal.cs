using Pericles.Crypto;
using ElectionModels;

namespace Pericles.VoterTerminal
{
    interface IVoterTerminal
    {
        bool login(out EncryptedKeyPair crypticKeyPair);
        void ballotPrompt(ElectionType electionType);
        string getResult();
    }
}
