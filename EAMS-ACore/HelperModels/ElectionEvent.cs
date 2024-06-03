using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.HelperModels
{
    public enum ElectionEvent
    {
        PartyDispatch = 1,
        PartyReached = 2,
        SetupPollingStation = 3,
        MockPollDone = 4,
        PollStarted = 5,
        VoterTurnOut = 6,
        VoterInQueue = 7,
        FinalVotes = 8,
        PollEnded = 9,
        MCEVM = 10,
        PartyDeparted = 11,
        PartyReachedCollectionCentre = 12,
        EVMDeposited = 13
    }

}
