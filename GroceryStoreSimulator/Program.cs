using Manifestarium.GroceryStoreSimulator.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace Manifestarium.GroceryStoreSimulator
{
    /// <summary>
    /// This program simulates a grocery store / cashier line.  
    /// It reads input from a file, and prints the resulting score to the console.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Simulation service to run (dependency-injected)
        /// </summary>
        [Import]
        IStreamingSimulatorService simulatorService;

        /// <summary>
        /// Starts a simulation (dependency-injected) that processes an input file (specified in the command line arg) 
        /// and writes output to the console.
        /// </summary>
        public static int Main(string[] args)
        {
            /// Since this is a static method, we need to create an instance of Program so that we can play with MEF. 
            var p = new Program();
            return p.Run(args);
        }

        /// <summary>
        /// Called from Main() so we can continue in an instance context, rather than a static context.
        /// </summary>
        public int Run(string[] args)
        {
            // Handle unexpected exceptions gracefully.  (Nobody's perfect.)
            try
            {
                // Initialize dependency injection to get the simulator service we are going to run.
                Compose();
                // Require exactly one command line argument.
                if (!(args != null && args.Length == 1))
                    throw new ArgumentOutOfRangeException("The number of command line arguments must be exactly 1.  If your file path has spaces in it, enclose it in quotes.");
                string filename = args[0];
                // Open the input file, specified in the command line argument.
                using (StreamReader inputReader = new StreamReader(filename))
                {
                    // Open a stream writer that outputs to the console.
                    var outputWriter = new StreamWriter(Console.OpenStandardOutput());
                    outputWriter.AutoFlush = true;
                    Console.SetOut(outputWriter);
                    // Enter The Matrix!
                    this.simulatorService.RunSimulation(inputReader, outputWriter); ;
                }
                return 0; // Return a status code indicating success.
            }
            catch (Exception ex)
            {
                // Welcome to the last-ditch catch-all handler!  Let's report our failure with a popular meme, since this is just a dev test.
                Console.WriteLine("\"I need about Tree Fiddy.\"\r\n" +
                                    "You suddenly realize that the software developer you are evaluating " +
                                    "is 8 stories tall and a crustacean from the paleozoic era. " +
                                    "Looks like the Loch Ness monster got you again!\r\n" +
                                    "Exception:\r\n" +
                                    "{0}", ex.ToString());
                return -1; // Return a status code indicating failure.
            }
        }

        /// <summary>
        /// Initialize DI / IoC using MEF 
        /// </summary>
        private void Compose()
        {
            // An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            // Add all the parts found in this assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(this.GetType().Assembly));
            // Add all the parts found in all DLL assemblies in the same directory as the executing program
            catalog.Catalogs.Add(new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
            // Create the CompositionContainer with the parts in the catalog.
            CompositionContainer container = new CompositionContainer(catalog);
            // Fill the imports of this object
            container.ComposeParts(this);
        }
    }
}
