namespace EAMS_ACore.Models
{
    public class VoterTurnOutSlotWise
    {
        public string Key { get; set; }
        public int? MasterId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        //public long[] SlotVotes { get; set; }
        public string[] SlotVotes { get; set; }
        public string? TotalVoters { get; set; }
        public string? VotesTillNow { get; set; }
        public List<object> Children { get; set; }
    }


    public class AssemblyVoterTurnOutSlotWise
    {
        public string Key { get; set; }
        public int? MasterId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? AssemblyCode { get; set; }
        public string[] SlotVotes { get; set; }
        public string? VotesTillNow { get; set; }
        public string? TotalVoters { get; set; }
        public List<object> Children { get; set; }
    }
    public class BoothWiseVoterTurnOutSlotWise
    {
        public string Key { get; set; }
        public int? MasterId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? VotesTillNow { get; set; }
        public string[] SlotVotes { get; set; }
        public string? TotalVoters { get; set; }
    }
}
