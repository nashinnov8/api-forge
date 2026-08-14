
namespace ApiForge.Core.Architecture
{
    public class ArchitectureOptions
    {
        public ArchitectureStyle Style { get; init; } = ArchitectureStyle.VerticalSlice;

        public bool UseDdd { get; init; }

        public bool UseCqrs { get; init; }

        public bool UseDomainEvents { get; init; }
    }

}