using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manifestarium.GroceryStoreSimulator.StoreModule.Models
{
    /// <summary>
    /// Identifies the type of simulated behavior that will be applied to the customer.
    /// Customer Type A always chooses the register with the shortest line (least number of customers in line).
    /// Customer Type B looks at the last customer in each line, and always chooses to be behind the customer with the fewest number of items left to check out, regardless of how many other customers are in the line or how many items they have. Customer Type B will always choose an empty line before a line with any customers in it.
    /// If two or more customers arrive at the same time, those with fewer items choose registers before those with more, and if they have the same number of items then type A's choose before type B's.
    /// </summary>
    public enum CustomerType
    {
        /// <summary>
        /// Customer Type A always chooses the register with the shortest line (least number of customers in line).
        /// If two or more customers arrive at the same time, those with fewer items choose registers before those with more, and if they have the same number of items then type A's choose before type B's.
        /// </summary>
        A,
        /// <summary>
        /// Customer Type B looks at the last customer in each line, and always chooses to be behind the customer with the fewest number of items left to check out, regardless of how many other customers are in the line or how many items they have. Customer Type B will always choose an empty line before a line with any customers in it.
        /// If two or more customers arrive at the same time, those with fewer items choose registers before those with more, and if they have the same number of items then type A's choose before type B's.
        /// </summary>
        B,
    }
}
