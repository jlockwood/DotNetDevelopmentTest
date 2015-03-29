using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manifestarium.GroceryStoreSimulator.Infrastructure
{
    /// <summary>
    /// NOTE: Yes, I made this up.  Looks legit though, doesn't it?  Like I copy/pasted it.  But I didn't!
    /// Heck yeah.
    /// 
    /// ...Just look at that interface.
    /// 
    /// Contract for a service which runs a simulation process that takes in an input stream and writes to an output stream.
    /// </summary>
    public interface IStreamingSimulatorService
    {
        void RunSimulation(StreamReader inputStream, StreamWriter outputStream);
    }
}
