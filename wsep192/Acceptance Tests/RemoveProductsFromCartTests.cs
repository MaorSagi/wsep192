﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.ServiceLayer;

namespace Acceptance_Tests
{
    [TestClass]
    public class RemoveProductsFromCartTests
    {
        ServiceLayer service;
        List<KeyValuePair<string, int>> list;

        public void setUp()
        {
            service = new ServiceLayer();
            service.init("admin", "1234");
            service.initUser();
            service.register("user", "password", "tmpuser");
            service.signIn("user", "password");
            service.openStore("store", "user");
            KeyValuePair<string, int> p1 = new KeyValuePair<string, int>("p1", 1);
            list = new List<KeyValuePair<string, int>>();
            list.Add(p1);
            service.addProductsInStore(list, "store", "user");
            service.addProductsToCart(list, "store", "user");
            service.editProductQuantityInCart("p1", 0, "store", "user");
        }

        [TestMethod]
        public void TestMethod1_empty_failure()
        {
            setUp();
            Assert.AreEqual(false, service.removeProductsFromCart(list, "store", "user"));
        }

        [TestMethod]
        public void TestMethod1_success()
        {
            setUp();
            service.editProductQuantityInCart("p1", 5, "store", "user");
            Assert.AreEqual(true, service.removeProductsFromCart(list, "store", "user"));
        }




    }
}