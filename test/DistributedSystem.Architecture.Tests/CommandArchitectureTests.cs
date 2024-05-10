using FluentAssertions;
using NetArchTest.Rules;

namespace DistributedSystem.Architecture.Tests;

public class CommandArchitectureTests
{
    private const string DomainNamespace = "DistributedSystem.Domain";
    private const string ApplicationNamespace = "DistributedSystem.Application";
    private const string InfrastructureNamespace = "DistributedSystem.Infrastructure";
    private const string PersistenceNamespace = "DistributedSystem.Persistence";
    private const string PresentationNamespace = "DistributedSystem.Presentation";
    private const string ApiNamespace = "DistributedSystem.API";

    /// <summary>
    /// Domain should not have any dependency on other projects
    /// </summary>
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjetcs()
    {
        // Arrange
        var domain = Domain.AssemblyReference.Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            PersistenceNamespace,
            PresentationNamespace,
            ApiNamespace
        };

        // Act
        var result = Types
            .InAssembly(domain)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjetcs()
    {
        // Arrange
        var application = DistributedSystem.Application.AssemblyReference.Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            PresentationNamespace,
            ApiNamespace
        };

        // Act
        var result = Types
            .InAssembly(application)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_HaveDependencyApplication()
    {
        // Arranges
        var infrastructure = Infrastructure.AssemblyReference.Assembly;

        // Act
        var result = Types
            .InAssembly(infrastructure)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeFalse();
    }
}
