namespace KafkaPubSubSample
{
    public class KafkaSettings
    {
        public string[] Brokers { get; set; }
        public string[] Topics { get; set; }
        public string ClientId { get; set; }
        public string GroupId { get; set; }
        public int ConsumeTimeout { get; set; }
        public int SessionTimeoutMs { get; set; }
        public int MessageMaxBytes { get; set; }
        public bool EnableAutoCommit { get; set; }
    }
}