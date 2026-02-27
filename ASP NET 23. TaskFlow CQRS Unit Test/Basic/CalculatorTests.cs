namespace ASP_NET_23._TaskFlow_CQRS_Unit_Test.Basic;

public class CalculatorTests
{
    // AAA
    // Arrange
    // Act
    // Assert

    [Fact]
    public void Add_ZeroPlusZero_ReturnsZero()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(0, 0);

        // Assert
        Assert.Equal(0, result);
    }
}
