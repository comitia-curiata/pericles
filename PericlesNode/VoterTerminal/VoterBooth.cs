using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectionModels;

namespace Pericles.VoterTerminal
{
    class VoterBooth
    {
        IVoterTerminal voterTerminal;
        ElectionType electionType;
        public VoterBooth(IVoterTerminal voterTerminal, ElectionType electionType)
        {
            this.voterTerminal = voterTerminal;
            this.electionType = electionType;
        }

        public void LaunchBooth()
        {
            while (true)
            {
                Console.WriteLine("Welcome to the Voting Booth. Please select an option:\n1.Login to vote\n2.Get Results");
                int choice;
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("unrecognized option");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        var isLogin = this.voterTerminal.login(out var encryptedkeyPair);
                        if (isLogin)
                        {
                            this.voterTerminal.ballotPrompt(this.electionType);
                        }

                        else
                        {
                            Console.WriteLine("Incorrect password. Please login again!");
                        }
                        break;
                    case 2:
                        Console.WriteLine(this.voterTerminal.getResult());
                        break;
                    default:
                        break;
                }

                
            }
        }
    }
}
