using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manifestarium.GroceryStoreSimulator.StoreModule.Models
{
    /// <summary>
    /// A checkout register in a store.
    /// </summary>
    public class Register
    {
        public Store Store { get; set; }

        /// <summary>
        /// The customers waiting in line at the register.
        /// </summary>
        public List<Customer> Customers { get; protected set; }

        /// <summary>
        /// A number which uniquely identifies a register within a particular store.
        /// </summary>
        public long RegisterNumber { get; set; }

        /// <summary>
        /// Training register takes 2 minutes per item.  Regular register takes 1 minute per item.
        /// </summary>
        public bool IsTrainingRegister { get; set; }

        /// <summary>
        /// Gets the processing time per item for this register.
        /// </summary>
        public long ProcessingTimePerItem
        {
            get
            {
                return IsTrainingRegister ? 2 : 1; 
            }
        }

        /// <summary>
        /// The time when the current customer started being processed.  
        /// Should set this whenever a customer becomes "the current customer".
        /// </summary>
        public long ItemProcessingStartTime { get; set; }

        public Register()
        {
            this.Customers = new List<Customer>();
        }
    }
}
