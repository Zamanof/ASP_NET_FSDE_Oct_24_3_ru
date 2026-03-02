using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using FluentAssertions;

namespace ASP_NET_23._TaskFlow_CQRS_Unit_Test.Parametrized;

public class TaskItemQueryParamsTests
{
    [Theory]
    [InlineData(0, 14, 1, 14)]
    [InlineData(98, 0, 98, 10)]
    [InlineData(98, 214, 98, 100)]
    [InlineData(2, 3, 2, 3)]
    public void Validate_NormalizesPageAndSize(
        int page,
        int size,
        int expectedPage,
        int expectedSize
        )
    {
        // Arrange
        var param = new TaskItemQueryParams
        {
            Page = page,
            Size = size
        };

        // Act
        param.Validate();

        // Assert
        //Assert.Equal(expectedPage, param.Page);
        //Assert.Equal(expectedSize, param.Size);
        param.Page.Should().Be(expectedPage);
        param.Size.Should().Be(expectedSize);

    }
}
