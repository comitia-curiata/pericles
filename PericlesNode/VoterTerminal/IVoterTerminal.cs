using Pericles.Crypto;
using ElectionModels;

namespace Pericles.VoterTerminal
{
    interface IVoterTerminal
    {
        bool login(string pw, out EncryptedKeyPair crypticKeyPair);
        void ballotPrompt(ElectionType electionType);
        string getResult();
    }
}
