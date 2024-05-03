using BasketApp.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Primitives;
using Quartz;

namespace BasketApp.Infrastructure.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public class ProcessOutboxMessagesJob : IJob
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPublisher _publisher;

        public ProcessOutboxMessagesJob(ApplicationDbContext dbContext, IPublisher publisher)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Получаем все DomainEvents, которые еще не были отправлены (где ProcessedOnUtc == null)
            var outboxMessages = await _dbContext
                .Set<OutboxMessage>()
                .Where(m => m.ProcessedOnUtc == null)
                .Take(20)
                .ToListAsync(context.CancellationToken);

            // Если такие есть, то перебираем их в цикле
            if (outboxMessages.Any())
            {
                foreach (var outboxMessage in outboxMessages)
                {
                    // Десериализуем в объект
                    var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(outboxMessage.Content,
                        new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.All
                        });

                    // Отправляем, и только если отправка была успшена - ставим дату отправки
                    await _publisher.Publish(domainEvent, context.CancellationToken);
                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                }

                // Сохраняем 
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}