using System.Reflection;
using CarService.Application;
using CarService.Domain;
using Xunit;

namespace CarService.UnitTests;

public sealed class ArchitectureTests
{
    [Fact]
    public void DomainDoesNotReferenceApplicationOrInfrastructure()
    {
        var references = typeof(Car).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(references, IsInfrastructureOrPresentation);
        Assert.DoesNotContain(references, reference => reference.Name == "CarService.Application");
    }

    [Fact]
    public void ApplicationDoesNotReferenceInfrastructureOrPresentation()
    {
        var references = typeof(OrderService).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(references, IsInfrastructureOrPresentation);
    }

    private static bool IsInfrastructureOrPresentation(AssemblyName reference) =>
        reference.Name is "CarService.Infrastructure" or "CarService.Api" ||
        reference.Name?.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal) == true ||
        reference.Name?.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal) == true ||
        reference.Name?.StartsWith("Npgsql", StringComparison.Ordinal) == true;
}
