using FluentAssertions;
using NetArchTest.Rules;

namespace DistributedSystem.Architecture.Tests;

public class QueryArchitectureTests
{
    private const string DomainNamespace = "Query.Domain";
    private const string ApplicationNamespace = "Query.Application";
    private const string InfrastructureNamespace = "Query.Infrastructure";
    private const string PersistenceNamespace = "Query.Persistence";
    private const string PresentationNamespace = "Query.Presentation";
    private const string ApiNamespace = "Query.API";

    /// <summary>
    /// Domain should not have any dependency on other projects
    /// </summary>
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjetcs()
    {
        // Arrange
        var domain = Query.Domain.AssemblyReference.Assembly;

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
        var application = Query.Application.AssemblyReference.Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            PersistenceNamespace,
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
        var infrastructure = Query.Infrastructure.AssemblyReference.Assembly;

        // Act
        var result = Types
            .InAssembly(infrastructure)
            .Should()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeFalse();
    }
}
