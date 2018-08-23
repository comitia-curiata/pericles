=========================================================================
FINAL PROJECT FOR MPCS 56600
Authors: Patrick Yukman, Dipanshu Sehjal, Alan Salkanovic, Michael Rice
Project: "Pericles"
=========================================================================

Pericles is a distributed, blockchain-based electronic voting platform. Pericles can support any number of election types (e.g., first-past-the-post, instant-runoff, etc.) with minimal code changes, as ballots are simply stored as serialized JSON strings and arbitrary election algorithms can be written to operate on the set of ballots stored in the blockchain.

Pericles is written in C# (targeting .NET 4.5.2) and was developed and tested using Visual Studio.

--------------------------------------------------------------------------
Configuration:
--------------------------------------------------------------------------

Pericles is controlled by a simple JSON configuration file that is passed to the node as a command-line argument. An example config is given below:

{
    "electionType": "FirstPastThePost",
    "candidates": [
        "Donald Trump",
        "Hillary Clinton",
        "Gary Johnson",
        "Jill Stein"
    ],
    "electionEndTime": "2018-08-23 16:00:00",
    "voterDbFilepath": "../../../VoterDatabase/pericles_voter_db.sqlite",
    "isMiningNode": true,
    "port": 58883
}

Description of config elements:

electionType:         A string indicating the type of election being run, e.g. instant-runoff or first-past-the-post
candidates:           List of valid ballot options
electionEndTime:      A timestamp beyond which votes will no longer be mined into blocks
voterDbFilepath:      Filepath to SQLite db where voter records and encryption information are stored 
isMiningNode:         Boolean for whether or not the node can interact with users to accept votes
port:                 Port on which the node will run

--------------------------------------------------------------------------
Operation:
--------------------------------------------------------------------------

Registrar:
	
	Registrar.exe
	
	Note: the registrar always starts on port 50051.
	
	
PericlesNode:
	
	PericlesNode.exe <path-to-config-file> <port>

	Note: each node should be started on a unique port.
	
--------------------------------------------------------------------------

For more information about key features, design, and motivation, see submitted PowerPoint presentation.

	