namespace {{ProjectName}}.SharedKernel;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
