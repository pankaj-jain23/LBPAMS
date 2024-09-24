﻿using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.Models
{
    public class ElectionInfoMaster
    {
        [Key]
        public int ElectionInfoMasterId { get; set; }
        public int StateMasterId { get; set; }

        public int DistrictMasterId { get; set; }
        public int ElectionTypeMasterId { get; set; }
        public int AssemblyMasterId { get; set; }

        public int? FourthLevelMasterId { get; set; }
        public int? PSZoneMasterId { get; set; }

        public int BoothMasterId { get; set; }

        public int EventMasterId { get; set; }
        public int EventSequence { get; set; }
        public string EventName { get; set; }
        public string EventABBR { get; set; }
        public bool ElectionInfoStatus { get; set; }
        public string? FOUserId { get; set; }
        public string? AROUserId { get; set; }
        public bool IsPartyDispatched { get; set; }
        public DateTime? PartyDispatchedLastUpdate { get; set; }

        public bool IsPartyReached { get; set; }
        public DateTime? PartyReachedLastUpdate { get; set; }

        public bool IsSetupOfPolling { get; set; }
        public DateTime? SetupOfPollingLastUpdate { get; set; }

        public bool IsMockPollDone { get; set; }
        public DateTime? MockPollDoneLastUpdate { get; set; }
        public int? NoOfPollingAgents { get; set; }

        public bool IsPollStarted { get; set; }
        public DateTime? PollStartedLastUpdate { get; set; }

        public int? FinalTVote { get; set; }

        public bool FinalTVoteStatus { get; set; }

        public DateTime? VotingLastUpdate { get; set; }

        public int? VoterInQueue { get; set; }
        public DateTime? VoterInQueueLastUpdate { get; set; }

        public bool IsVoterTurnOut { get; set; }
        public DateTime? VotingTurnOutLastUpdate { get; set; }

        public bool IsPollEnded { get; set; }
        public DateTime? IsPollEndedLastUpdate { get; set; }
        public bool IsMCESwitchOff { get; set; }
        public DateTime? MCESwitchOffLastUpdate { get; set; }
        public bool IsPartyDeparted { get; set; }
        public DateTime? PartyDepartedLastUpdate { get; set; }
        public bool IsPartyReachedCollectionCenter { get; set; }
        public DateTime? PartyReachedCollectionCenterLastUpdate { get; set; }

        public bool IsEVMDeposited { get; set; }
        public DateTime? EVMDepositedLastUpdate { get; set; }

        /**Male Female Transgender**/
        public int? Male { get; set; }
        public int? Female { get; set; }
        public int? Transgender { get; set; }

        public int? EDC { get; set; }

        public bool? IsQueueUndo { get; set; }
       
        
    }

}
