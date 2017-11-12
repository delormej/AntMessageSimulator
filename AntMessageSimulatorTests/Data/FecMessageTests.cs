using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntMessageSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntMessageSimulator.Tests
{
    [TestClass()]
    public class FecMessageTests
    {

        public static T GetDynamicObjectProperty<T>(object o, string property)
        {
            object value = o?.GetType().GetProperty(property)?.GetValue(o, null);
            return (T)value;
        }

    }
}