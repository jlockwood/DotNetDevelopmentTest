using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Manifestarium.GroceryStoreSimulator.StoreModule.Models
{
    /// <summary>
    /// Store simulation state.
    /// </summary>
    public class Store
    {
        /// <summary>
        /// Current store time.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Registers in the store.
        /// </summary>
        public List<Register> Registers { get; protected set; }

        /// <summary>
        /// Customers that haven't yet arrived.
        /// </summary>
        public List<Customer> PendingCustomers { get; protected set; }

        public Store()
        {
            this.Registers = new List<Register>();
            this.PendingCustomers = new List<Customer>();
        }

    }
}
