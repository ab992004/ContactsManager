namespace CRUDTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            var class1 = new Class1();
            int a = 5, b = 10, expected = 15;
            //Act
            int actual = class1.Add(a, b);
            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
