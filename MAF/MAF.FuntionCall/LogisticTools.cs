using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MAF.FuntionCall
{
	[Description("Retrieves the current status of an enterprise logistic order. Invoke this tool only when user wants to know the current status of an order")]
	public static class LogisticTools
	{
		[Description("Retrieves the current status of a specific logistic order based on the provided order ID. The order must be in the format ORD- followed by 5 digits")]
		public static string GetLogisticStatus(string orderId)
		{
			if (string.IsNullOrEmpty(orderId))
			{
				throw new ArgumentException("Order ID cannot be null or empty.", nameof(orderId));
			}
			return orderId switch
			{
				"ORD-12345" => "Order ID: ORD-12345, Status: Delivered",
				"ORD-67890" => "Order ID: ORD-67890, Status: In Transit",
				"ORD-54321" => "Order ID: ORD-54321, Status: Pending",
				"ORD-98765" => "Order ID: ORD-98765, Status: Cancelled",
				"ORD-11111" => "Order ID: ORD-11111, Status: Out for Delivery",
				_ => $"Order ID '{orderId}' is not recognized."
			};
		}
	}
}