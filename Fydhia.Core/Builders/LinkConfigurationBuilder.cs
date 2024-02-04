using Fydhia.Core.Configurations;
using Microsoft.AspNetCore.Routing;

namespace Fydhia.Core.Builders;

public abstract class LinkConfigurationBuilder
{
    internal abstract LinkConfiguration Build(EndpointDataSource endpointDataSource);
}