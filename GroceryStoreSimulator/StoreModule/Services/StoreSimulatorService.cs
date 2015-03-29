using Manifestarium.GroceryStoreSimulator.Infrastructure;
using Manifestarium.GroceryStoreSimulator.StoreModule.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manifestarium.GroceryStoreSimulator.StoreModule.Services
{
    /// <summary>
    /// A streaming simulation service for grocery stores.
    /// </summary>
    [Export(typeof(IStreamingSimulatorService))] // Inject this type for IStreamingSimulatorService imports
    public class StoreSimulatorService : IStreamingSimulatorService
    {
        /// <summary>
        /// Processes the input stream and writes the simulation results to the output stream.
        /// This method is thread-safe.
        /// </summary>
        public void RunSimulation(StreamReader input, StreamWriter output)
        {
            Store store = new Store();
            // Load the sim data
            Load(store, input);
            Debug.WriteLine("t={0}: Started with {1} registers; #{1} is a training register.", store.Time, store.Registers.Count);
            // Run the simulation as long as there are customers at registers or pending.
            while ((store.Registers.Where(r => r.Customers.Count > 0).Count() > 0) | (store.PendingCustomers.Count > 0))
            {
                // Advance the simulation by one time step.
                SimulateTimeStep(store);
            }
            Debug.WriteLine("Finished at: t={0} minutes", store.Time);
            // Write the output message.
            output.Write("Finished at: t={0} minutes", store.Time);
        }

        /// <summary>
        /// Reads a line from the input string and increments lineNumber to keep track of the current line.
        /// </summary>
        private static string ReadLine(StreamReader input, ref long lineNumber)
        {
            string line = input.ReadLine();
            lineNumber++;
            return line;
        }

        /// <summary>
        /// Loads the simulation file
        /// </summary>
        private static void Load(Store store, StreamReader input)
        {
            // Tracks the current input line number, so we can include it along with any exceptions while processing the data.
            long lineNumber = 0;
            // Read the register count line and initialize the registers
            LoadRegisters(store, input, ref lineNumber);
            // Read the customer event frames.  We will do this all at once instead of in a streaming fashion, because there is no requirement that the lines will be in order anyway.
            LoadCustomers(store, input, ref lineNumber);
        }

        /// <summary>
        /// Read the register count, validate it, initialize the registers.
        /// </summary>
        private static void LoadRegisters(Store store, StreamReader input, ref long lineNumber)
        {
            try
            {
                // Read & validate the register count
                long registerCount = long.Parse(ReadLine(input, ref lineNumber));
                if (registerCount <= 0)
                    throw new InvalidDataException("Register count must be greater than 0.");
                // Create the registers, add them to the store's register list.
                for (int i = 0; i < registerCount; i++)
                {
                    store.Registers.Add(new Register()
                    {
                        Store = store,
                        RegisterNumber = i + 1
                    });
                }
                // The last register is always used for Training, so set its type to Training.
                store.Registers.Last().IsTrainingRegister = true;
            }
            catch (Exception ex)
            {
                // Wrap the exception with line number info
                throw new InvalidDataException(string.Format("Unable to parse line {0}", lineNumber.ToString()), ex);
            }
        }

        /// <summary>
        // Read the customer lines
        /// </summary>
        private static void LoadCustomers(Store store, StreamReader input, ref long lineNumber)
        {
            try
            {
                while (!input.EndOfStream)
                {
                    var customer = Customer.Parse(ReadLine(input, ref lineNumber));
                    customer.Store = store;
                    customer.CustomerNumber = store.PendingCustomers.Count() + 1;
                    store.PendingCustomers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                // Wrap the exception with line number info
                throw new InvalidDataException(string.Format("Unable to parse line {0}", lineNumber.ToString()), ex);
            }
        }

        /// <summary>
        /// Performs a simulation time step.
        /// </summary>
        private static void SimulateTimeStep(Store store)
        {
            // Advance the simulation time by 1 minute.
            store.Time += 1;
            // Process customer items, remove finished customers from the register line
            ProcessRegisters(store);
            // Customers with arrival times that are due need to be turned into customers and assigned to a register.
            ProcessPendingCustomers(store);
        }

        private static void ProcessRegisters(Store store)
        {
            // Process registers that have customers in line.
            foreach (var register in store.Registers.Where(r => r.Customers.Count > 0))
            {
                // Get current customer
                var currentCustomer = register.Customers.First();
                // Finish processing the current item if enough time has elapsed.
                long elapsedItemTime = store.Time - register.ItemProcessingStartTime;
                if (elapsedItemTime == register.ProcessingTimePerItem)
                {
                    // Remove an item from the customer
                    currentCustomer.ItemCount = currentCustomer.ItemCount - 1;
                    // Reset the processing start time for the next item
                    register.ItemProcessingStartTime = store.Time;
                }
                // If the customer has no more items, let them leave.
                if (currentCustomer.ItemCount == 0)
                {
                    // Remove the current customer when we're done processing its items
                    Debug.WriteLine("t={0}: Customer #{1} left register #{2}.", store.Time, currentCustomer.CustomerNumber, register.RegisterNumber);
                    register.Customers.Remove(currentCustomer);
                    currentCustomer.Register = null;
                    // Skip past customers with 0 items.
                    while (register.Customers.Count > 0 && register.Customers.First().ItemCount == 0)
                    {
                        Debug.WriteLine("t={0}: Customer #{1} left register #{2}.  It had 0 items and was skipped.", store.Time, register.Customers.First().CustomerNumber, register.RegisterNumber);
                        register.Customers.RemoveAt(0);
                    }
                    // Start proccessing the next customer's items if there are more customers waiting, or clear the start time value if not.
                    if (register.Customers.Count > 0)
                    {
                        currentCustomer = register.Customers.First();
                        Debug.WriteLine("t={0}: Register #{1} has started serving Customer #{2}. He has {3} items, which will take until t={4} to process.", store.Time, register.RegisterNumber, currentCustomer.CustomerNumber, currentCustomer.ItemCount, store.Time + (currentCustomer.ItemCount * register.ProcessingTimePerItem));
                        register.ItemProcessingStartTime = store.Time;
                    }
                }
            }
        }

        /// <summary>
        /// When the arrival time of a pending customer is reached, we add the customer to the appropriate register line. 
        /// </summary>
        private static void ProcessPendingCustomers(Store store)
        {
            // If two or more customers arrive at the same time, those with fewer items choose registers before those 
            // with more, and if they have the same number of items then type A's choose before type B's.
            var arrivingCustomers = (from c in store.PendingCustomers
                                     where c.ArrivalTime <= store.Time
                                     orderby c.ArrivalTime, c.ItemCount, c.Type
                                     select c).ToArray();
            foreach (var customer in arrivingCustomers)
            {
                // Choose a register for the customer.
                Register register = null;
                switch (customer.Type)
                {
                    case CustomerType.A:
                        // Customer Type A always chooses the register with the shortest line (least number of customers in line).
                        register = store.Registers.OrderBy(r => r.Customers.Count).FirstOrDefault();
                        break;
                    case CustomerType.B:
                        // Customer Type B looks at the last customer in each line, and always chooses to be behind the customer with 
                        // the fewest number of items left to check out, regardless of how many other customers are in the line or how
                        // many items they have. Customer Type B will always choose an empty line before a line with any customers in it.
                        register = store.Registers.Where(r => r.Customers.Count == 0)
                            .Concat(store.Registers.Where(r => r.Customers.Count > 0).OrderBy(r => r.Customers.Last().ItemCount))
                            .FirstOrDefault();
                        break;
                }
                // Remove the customer from the pending list & add it to the register.
                store.PendingCustomers.Remove(customer);
                customer.Register = register;
                register.Customers.Add(customer);
                Debug.Write(string.Format("t={0}: Customer #{1} (type {2}) arrives with {3} items, which will take until t={4} to process, and goes to register #{5}", store.Time, customer.CustomerNumber, customer.Type.ToString(), customer.ItemCount, store.Time + (register.Customers.First().ItemCount * register.ProcessingTimePerItem), register.RegisterNumber));
                // If the customer is first in line, set the item processing start time.
                if (register.Customers.Count == 1)
                {
                    register.ItemProcessingStartTime = store.Time;
                    Debug.WriteLine(" which starts servicing him.");
                }
                else
                {
                    long endTime = store.Time;
                    // Add time remaining for item being processed right now
                    endTime += ((register.Customers[0].ItemCount - 1) * register.ProcessingTimePerItem) + (register.ProcessingTimePerItem - (store.Time - register.ItemProcessingStartTime));
                    // Add times for each item after the first one.
                    for (int i = 1; i < register.Customers.Count; i++)
                        endTime += ((register.Customers[i].ItemCount) * register.ProcessingTimePerItem);
                    Debug.WriteLine(string.Format(", behind Customer #{0}.", register.Customers[register.Customers.Count - 2].CustomerNumber));
                }
            }
        }
    }
}
