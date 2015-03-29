using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Manifestarium.GroceryStoreSimulator.StoreModule.Services;
using Manifestarium.GroceryStoreSimulator.StoreModule.Models;
using System.Diagnostics.Contracts;

namespace Manifestarium.GroceryStoreSimulator.Tests
{
    [TestClass]
    public class CustomerFrameTests
    {
        [TestMethod]
        public void Parse_WithValidString_ReturnsCorrectCustomer()
        {
            // arrange
            string lineString = "A 1 2";
            Customer expected = new Customer()
            {
                ArrivalTime = 1,
                Type = StoreModule.Models.CustomerType.A,
                ItemCount = 2
            };

            // actual
            var actual = Customer.Parse(lineString);

            // assert
            Assert.AreEqual(expected.ArrivalTime, actual.ArrivalTime, "Customer not parsed correctly: ArrivalTime is wrong");
            Assert.AreEqual(expected.Type, actual.Type, "Customer not parsed correctly: Type is wrong");
            Assert.AreEqual(expected.ItemCount, actual.ItemCount, "Customer not parsed correctly: ItemCount is wrong");
        }
    }
}
