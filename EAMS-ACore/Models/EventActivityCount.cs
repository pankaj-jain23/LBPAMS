﻿namespace EAMS_ACore.Models
{
    public class EventActivityCount
    {
        public string Key { get; set; }
        public int? MasterId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? PartyDispatch { get; set; }
        public string? PartyArrived { get; set; }
        public string? SetupPollingStation { get; set; }
        public string? MockPollDone { get; set; }
        public string? PollStarted { get; set; }
        // public int? VoterTurnedOut { get; set; }
        public string? VoterTurnOutValue { get; set; }
        public string? FinalVotesValue { get; set; }
        public string? QueueValue { get; set; }
        public string? PollEnded { get; set; }
        public string? MCEVMOff { get; set; }
        public string? PartyDeparted { get; set; }
        public string? PartyReachedAtCollection { get; set; }
        public string? EVMDeposited { get; set; }
        public int? TotalSo { get; set; }
        public List<object> Children { get; set; }
    }
    public class AssemblyEventActivityCount
    {
        public string Key { get; set; }
        public int? MasterId { get; set; }
        public int? StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? TotalSoCount { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? AssemblyCode { get; set; }
        public string? PartyDispatch { get; set; }
        public string? PartyArrived { get; set; }
        public string? SetupPollingStation { get; set; }
        public string? MockPollDone { get; set; }
        public string? PollStarted { get; set; }
        //  public int? VoterTurnedOut { get; set; }
        public string? VoterTurnOutValue { get; set; }
        public string? FinalVotesValue { get; set; }
        public string? QueueValue { get; set; }
        public string? PollEnded { get; set; }
        public string? MCEVMOff { get; set; }
        public string? PartyDeparted { get; set; }
        public string? PartyReachedAtCollection { get; set; }
        public string? EVMDeposited { get; set; }
        public int? TotalSo { get; set; }
        public List<object> Children { get; set; }
    }
    public class FourthLevelEventActivityCount
    {
        public string Key { get; set; }
        public int? MasterId { get; set; }
        public int? StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int AssemblyMasterId { get; set; }
        public int? TotalSoCount { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? HierarchyCode { get; set; }
        public string? PartyDispatch { get; set; }
        public string? PartyArrived { get; set; }
        public string? SetupPollingStation { get; set; }
        public string? MockPollDone { get; set; }
        public string? PollStarted { get; set; }
        //  public int? VoterTurnedOut { get; set; }
        public string? VoterTurnOutValue { get; set; }
        public string? FinalVotesValue { get; set; }
        public string? QueueValue { get; set; }
        public string? PollEnded { get; set; }
        public string? MCEVMOff { get; set; }
        public string? PartyDeparted { get; set; }
        public string? PartyReachedAtCollection { get; set; }
        public string? EVMDeposited { get; set; }
        public int? TotalSo { get; set; }
        public List<object> Children { get; set; }
    }
    public class AssemblyEventActivityCountPCWise
    {
        public string Key { get; set; }
        public int? MasterId { get; set; }
        public int? StateMasterId { get; set; }
        public int? PCMasterId { get; set; }
        public int? TotalSoCount { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? AssemblyCode { get; set; }
        public string? PartyDispatch { get; set; }
        public string? PartyArrived { get; set; }
        public string? SetupPollingStation { get; set; }
        public string? MockPollDone { get; set; }
        public string? PollStarted { get; set; }
        //  public int? VoterTurnedOut { get; set; }
        public string? VoterTurnOutValue { get; set; }
        public string? FinalVotesValue { get; set; }
        public string? QueueValue { get; set; }
        public string? PollEnded { get; set; }
        public string? MCEVMOff { get; set; }
        public string? PartyDeparted { get; set; }
        public string? PartyReachedAtCollection { get; set; }
        public string? EVMDeposited { get; set; }
        public int? TotalSo { get; set; }
        public List<object> Children { get; set; }
    }
    public class EventActivityBoothWise
    {
        public string Key { get; set; }
        public int? MasterId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? PartyDispatch { get; set; }
        public int? PartyArrived { get; set; }
        public int? SetupPollingStation { get; set; }
        public int? MockPollDone { get; set; }
        public int? PollStarted { get; set; }
        public int? VoterTurnedOut { get; set; }
        public int? VoterTurnOutValue { get; set; }
        public int? FinalVotesValue { get; set; }
        public int? QueueValue { get; set; }
        public int? PollEnded { get; set; }
        public int? MCEVMOff { get; set; }
        public int? PartyDeparted { get; set; }
        public int? PartyReachedAtCollection { get; set; }
        public int? EVMDeposited { get; set; }
        public int? AssignedSOId { get; set; }
        public string? AssignedSOName { get; set; }
        public string? AssignedSOMobile { get; set; }

    }
}
