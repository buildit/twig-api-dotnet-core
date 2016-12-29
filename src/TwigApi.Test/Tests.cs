using Xunit;
using FluentAssertions;
 
namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            true.Should().Be(true);
        }
    }
}