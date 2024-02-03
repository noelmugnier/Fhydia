using System.Runtime.CompilerServices;
using Fydhia.Core.Configurations;

[assembly: InternalsVisibleTo("Fhydia.Controllers")]
[assembly: InternalsVisibleTo("Fhydia.MinimalApi")]

namespace Fydhia.Core.Builders;

public abstract class LinkConfigurationBuilder
{
    internal abstract LinkConfiguration Build();
}