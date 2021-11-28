using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KafkaPubSubSample.Producer
{
    public class Producer : BackgroundService
    {
        private readonly KafkaSettings _kafkaSettings;
        private readonly IProducer<string, string> _producer;
        
        public Producer(IOptions<KafkaSettings> kafkaSettings)
        {
            _kafkaSettings = kafkaSettings.Value;

            ProducerConfig producerConfig = new ProducerConfig
            {
                BootstrapServers = String.Join(",", _kafkaSettings.Brokers),
                MessageMaxBytes = _kafkaSettings.MessageMaxBytes,
                MessageTimeoutMs = _kafkaSettings.MessageTimeoutMs,
                Acks = (Acks) Enum.Parse(typeof(Acks), _kafkaSettings.Acks, true),
            };
            
            ProducerBuilder<string, string> producerBuilder = new ProducerBuilder<string, string>(producerConfig);
            _producer = producerBuilder.Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string value = null;
            while (value != "exit")
            {
                try
                {
                    Console.Write("Enter Key: ");
                    string key = Console.ReadLine();

                    Console.Write("Enter Message: ");
                    value = Console.ReadLine();
                    
                    _producer.Produce(_kafkaSettings.Topics[0], new Message<string, string> { Key = key, Value = value}, DeliveryHandler);
                }
                catch (KafkaException e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
            return Task.CompletedTask;
        }
        
        private void DeliveryHandler(DeliveryReport<string, string> r)
        {
            Console.WriteLine(!r.Error.IsError
                ? $"Delivered message Key: '{r.Key}', Value: '{r.Value}' to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");
        }
    }
}