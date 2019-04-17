﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.ServiceLayer;
namespace Acceptance_Tests
{
    [TestClass]
    public class UnitTest1
    {
        private ServiceLayer service;
        private string user;

        public void setUp()
        {
            service = new ServiceLayer();
            service.init("admin", "1234");
            user=service.initUser();
        }

        [TestMethod]
        //an unregisterd user tries to preform open store.
        public void testOpenStore1()
        {
            setUp();
            bool x = service.openStore("Bershka", "Yonit");
            Assert.IsFalse(x);
        }

        [TestMethod]
        //a registerd user tries to preform open store.
        public void testOpenStore2()
        {
            setUp();
            bool x=service.register("Yonit", "23&As2", user);
            service.signIn("Yonit", "23&As2");
            service.openStore("Bershka", "Yonit");
            Assert.IsTrue(x);
        }
    }
}
