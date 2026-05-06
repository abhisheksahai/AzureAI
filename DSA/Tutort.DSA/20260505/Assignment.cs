using Tutort.DSA.Utils;

namespace Tutort.DSA
{
	public class Assignment
	{
		public int MinimumThreeNumbers(int x, int y, int z)
		{
			int result = z;
			if (x < y && x < z)
			{
				result = x;
			}
			else if (y < x && y < z)
			{
				result = y;
			}
			return result;
		}

		public bool CoupleIsEligibleForMarriage(int girlAge, int boyAge)
		{
			return (girlAge >= 18 && boyAge >= 21) ? true : false;
		}

		public double PrintTaxAmount(double amount)
		{
			return (amount >= 500000) ? amount * .01 : amount * 0.05;
		}

		public bool IsLeapYear(int year)
		{
			return (year % 4 == 0) && (year % 100 != 0 || year % 400 == 0);
		}

		public NumberType GetNumberType(int number)
		{
			return (number % 2) == 0 ? NumberType.Even : NumberType.Odd;
		}

		public void PrintTriangle(int size)
		{
			for (int i = 1; i <= size; i++)
			{
				for (int j = 1; j <= i; j++)
				{
					Console.Write("*");
				}
				Console.WriteLine("");
			}
		}

		public int PrintLargestOf3Numbers_1(int a, int b, int c)
		{
			return Math.Max(Math.Max(a, b), c);
		}

		public int PrintLargestOf3Numbers_2(int a, int b, int c)
		{
			int max = int.MinValue;
			if (a > max)
			{
				max = a;
			}
			if (b > max)
			{
				max = b;
			}
			if (c > max)
			{
				max = c;
			}
			return max;
		}

		public int PrintLargestOf3Numbers_3(int a, int b, int c)
		{
			return (a > b && a > c) ? a : (b > a && b > c) ? b : c;
		}

		public void PrintNumberSeries(int size)
		{
			int num = 1;
			for (int i = 1; i <= size; i++)
			{
				for (int j = 1; j <= i; j++)
				{
					Console.Write($"{num} ");
					num++;
				}
				Console.WriteLine("");
			}
		}
	}
}