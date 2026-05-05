namespace Tutort.DSA.Test
{
	public class UnitTestAssignment
	{
		private Assignment _assignment;

		[SetUp]
		public void Setup()
		{
			_assignment = new Assignment();
		}

		[TestCase(1, 2, 3, ExpectedResult = 1)]
		[TestCase(3, 2, 1, ExpectedResult = 1)]
		[TestCase(2, 1, 3, ExpectedResult = 1)]
		[TestCase(5, 5, 5, ExpectedResult = 5)]
		[TestCase(-1, -2, -3, ExpectedResult = -3)]
		[TestCase(0, -1, 1, ExpectedResult = -1)]
		[TestCase(int.MinValue, 0, int.MaxValue, ExpectedResult = int.MinValue)]
		public int MinimumThreeNumbers_ReturnsMinimum(int x, int y, int z)
		{
			return _assignment.MinimumThreeNumbers(x, y, z);
		}
	}
}