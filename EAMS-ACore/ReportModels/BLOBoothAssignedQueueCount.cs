namespace EAMS_ACore.ReportModels
{
    public class BLOBoothAssignedQueueCount
    {

        public string Header { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string AssemblyName { get; set; }
        public Int32 BLOMasterId { get; set; }
        public string BLOName { get; set; }
        public string BLOMobile { get; set; }
        public string BoothName { get; set; }
        public Int32? QueueCount { get; set; }
        public string? LastQueueEnterDateTime { get; set; }
        public string? LastQueueEnterTime { get; set; }
    }
}