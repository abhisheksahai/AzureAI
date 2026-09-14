using System.ComponentModel;

namespace MAF.HITL.ApproveRequiredFunction
{
	public static class FinanceTools
	{
		public static string IssueRefund([Description("OrderId in the format ORD-12345")] string orderId, [Description("The decimal refund amount to be issued")] double refundAmount)
		{
			Console.WriteLine($"Issuing refund for Order ID: {orderId}, Amount: {refundAmount}");
			return $"Refund of {refundAmount} issued for Order ID: {orderId}";
		}
	}
}