﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.ServiceLayer;

namespace Acceptance_Tests
{
    [TestClass]
    public class InitTests
    {
        ServiceLayer service;

        public void setUp()
        {
            service = ServiceLayer.getInstance(false);
        }

        [TestMethod]
        public void TestMethod1_success()
        {
            setUp();
            Assert.AreEqual(true, service.init("Admin", "SecretPassword1D4F6Yt7"));
            service.shutDown();
        }
    }
}