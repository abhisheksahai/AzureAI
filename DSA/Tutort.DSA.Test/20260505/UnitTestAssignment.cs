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


		[TestCase(18, 21, ExpectedResult = true)]
		[TestCase(10, 21, ExpectedResult = false)]
		[TestCase(15, 5, ExpectedResult = false)]
		[TestCase(20, 25, ExpectedResult = true)]
		public bool CoupleIsEligibleForMarriage_PrintsBoolean(int girlAge, int boyAge)
		{
			bool result = _assignment.CoupleIsEligibleForMarriage(girlAge, boyAge);
			Console.WriteLine($"{girlAge}-{boyAge}-{result}");
			return result;
		}

		[TestCase(500000, ExpectedResult = 5000)]
		[TestCase(100, ExpectedResult = 5)]
		public double PrintTaxAmountReturnsTax(int amount)
		{
			double result = _assignment.PrintTaxAmount(amount);
			Console.WriteLine(result);
			return result;
		}

		[TestCase(2024, ExpectedResult = true)]
		[TestCase(200, ExpectedResult = false)]
		[TestCase(2028, ExpectedResult = true)]
		public bool IsLeapYear_ReturnsBoolean(int year)
		{
			return _assignment.IsLeapYear(year);
		}
	}
}