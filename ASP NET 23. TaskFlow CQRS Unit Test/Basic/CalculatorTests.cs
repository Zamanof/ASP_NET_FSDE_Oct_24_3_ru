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


    public static IEnumerable<object[]> AddData()
    {
        yield return new object[] { 1, 4, 5 };
        yield return new object[] { -1, -8, -9 };
        yield return new object[] { 57, -8, 49 };
        yield return new object[] { 0, 0, 0 };
    }

    [Theory]
    //[InlineData(1, 8, 9)]
    //[InlineData(-1, -8, -9)]
    //[InlineData(57, -8, 49)]
    //[InlineData(0, 0, 0)]
    [MemberData(nameof(AddData))]
    public void Add_ReturnsExpectedResult(int left, int right, int expectedResult)
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(left, right);

        // Assert
        Assert.Equal(expectedResult, result);
    }

}
