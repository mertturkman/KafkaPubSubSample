using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KafkaPubSubSample
{
    public class Consumer: BackgroundService
    {
        private readonly KafkaSettings _kafkaSettings;
        private IConsumer<string, string> _consumer;
        
        public Consumer(IOptions<KafkaSettings> kafkaSettings)
        {
            _kafkaSettings = kafkaSettings.Value;

            ConsumerConfig consumerConfig = new ConsumerConfig
            {
                BootstrapServers = String.Join(",", _kafkaSettings.Brokers),
                GroupId = _kafkaSettings.GroupId,
                ClientId = _kafkaSettings.ClientId,
                SessionTimeoutMs = _kafkaSettings.SessionTimeoutMs,
                MessageMaxBytes =  _kafkaSettings.MessageMaxBytes,
                EnableAutoCommit = _kafkaSettings.EnableAutoCommit,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            ConsumerBuilder<string, string> consumerBuilder = new ConsumerBuilder<string, string>(consumerConfig);
            _consumer = consumerBuilder.Build();
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_kafkaSettings.Topics);

            while (true)
            {
                try
                {
                    ConsumeResult<string, string> consumeResult = _consumer.Consume(_kafkaSettings.ConsumeTimeout);
                    if (consumeResult == null || string.IsNullOrEmpty(consumeResult.Message.Value)) continue;

                    Console.WriteLine(
                        $"{consumeResult.Message.Value}{(!string.IsNullOrEmpty(consumeResult.Message.Key) ? " - " + consumeResult.Message.Key : "")} | Topic: {consumeResult.Topic} Partition: {consumeResult.Partition} Offset: {consumeResult.Offset}");

                    _consumer.Commit(consumeResult);
                    
                    if (consumeResult.Message.Value.Contains("exit")) return Task.CompletedTask;
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Consume exception occured: {ex.Error.Reason}");
                }
                catch (KafkaException ex)
                {
                    Console.WriteLine($"Kafka exception occured: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occured: {ex.Source}");
                }
            }
        }
    }
}