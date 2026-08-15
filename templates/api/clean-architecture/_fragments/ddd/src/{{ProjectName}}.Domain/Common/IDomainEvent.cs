namespace {{ProjectName}}.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
