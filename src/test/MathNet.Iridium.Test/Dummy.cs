using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Iridium.Test
{
    [TestFixture]
    public class Dummy
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual("foo","foo");
        }
    }
}
