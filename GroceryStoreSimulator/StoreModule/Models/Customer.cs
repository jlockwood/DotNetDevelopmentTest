using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manifestarium.GroceryStoreSimulator.StoreModule.Models
{
    /// <summary>
    /// A simulated customer.
    /// </summary>
    public class Customer
    {
        public Store Store { get; set; }

        public Register Register { get; set; }

        /// <summary>
        /// An ID for the customer.  Customers will be numbered by the order they appear in the input stream.
        /// </summary>
        public long CustomerNumber { get; set; }
        
        /// <summary>
        /// The number of items the customer has, that have not yet been processed by a cashier.  It can be a non-whole number,
        /// depending on the granularity of the simulation steps and the rate at which a cashier processes items.
        /// </summary>
        public long ItemCount { get; set; }

        /// <summary>
        /// Identifies the type of simulated behavior to apply to the customer.
        /// </summary>
        public CustomerType Type { get; set; }

        /// <summary>
        /// The time that the customer arrives at the registers.
        /// </summary>
        public long ArrivalTime { get; set; }

        /// <summary>
        /// Parses a customer instance from a string
        /// </summary>
        public static Customer Parse(string customerString)
        {
            CustomerType type;
            long arrivalTime;
            long itemCount;
            // Split the line into space-delimited segments, ignoring any extra spacing.
            string[] values = customerString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            // Parse the values
            // NOTE: I'd rather use Contract.Requires() here for validation...
            if (!Enum.TryParse<CustomerType>(values[0], out type))
                throw new ArgumentException("Invalid Type value.");
            if (!long.TryParse(values[1], out arrivalTime))
                throw new ArgumentException("Invalid ArrivalTime value.");
            if (!long.TryParse(values[2], out itemCount))
                throw new ArgumentException("Invalid ItemCount value.");
            if (itemCount < 0)
                throw new ArgumentOutOfRangeException("ItemCount", "Item count cannot be negative.  Validation errors are the least of your worries if people are trying to buy antimatter items in your store.");
            return new Customer()
            {
                Type = type, 
                ArrivalTime = arrivalTime ,
                ItemCount = itemCount
            };
        }
    }
}
