using Tutort.DSA.Utils;

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
			var result = _assignment.IsLeapYear(year);
			Console.WriteLine(result);
			return result;
		}

		[TestCase(2, ExpectedResult = NumberType.Even)]
		[TestCase(20, ExpectedResult = NumberType.Even)]
		[TestCase(3, ExpectedResult = NumberType.Odd)]
		[TestCase(33, ExpectedResult = NumberType.Odd)]
		public NumberType GetNumberType_ReturnsType(int number)
		{
			var result = _assignment.GetNumberType(number);
			Console.WriteLine(result);
			return result;
		}

		[TestCase(5)]
		public void PrintTriangleReturnsVoid(int number)
		{
			_assignment.PrintTriangle(number);
		}

		[TestCase(5)]
		public void PrintNumberSeriesReturnsVoid(int number)
		{
			_assignment.PrintNumberSeries(number);
		}

		[TestCase(new int[] { 4, 5, 6 }, new int[] { 1, 2, 3 })]
		[TestCase(new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 })]
		public void MergeTwoArrays_ReturnsVoid(int[] arr1, int[] arr2)
		{
			_assignment.MergeTwoArrays_1(arr1, arr2);
		}
	}
}