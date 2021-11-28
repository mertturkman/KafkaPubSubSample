namespace KafkaPubSubSample.Producer
{
    public class KafkaSettings
    {
        public string[] Brokers { get; set; }
        public int MessageMaxBytes { get; set; }
        public int MessageTimeoutMs { get; set; }
        public string Acks { get; set; }
        public string[] Topics { get; set; }
    }
}