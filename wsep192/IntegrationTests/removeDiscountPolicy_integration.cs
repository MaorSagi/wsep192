﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.Domain;

namespace IntegrationTests
{
    
    [TestClass]
    public class removeDiscountPolicy_integration
    {
        private TradingSystem system;
        private User admin;
        private User ownerUser;
        private User user;
        private ShoppingBasket basket_admin;

        private Owner ownerRole;
        private Store store;

        private Product p1;
        private Product p2;
        private Product p3;
        private Product p4;
        private ProductInStore pis1;
        private ProductInStore pis2;
        private ProductInStore pis3;
        private ProductInStore pis4;
        DuplicatePolicy logic;
        int discountId;

        List<KeyValuePair<String, int>> products;

        public void setUp()
        {
            store = new Store(1111, "adidas");
            logic = DuplicatePolicy.WithMultiplication;

            admin = new User(0, "admin", "1234", true, true);
            basket_admin = admin.Basket;
            user = new User(3434, "luli", "1111", false, true);
            ownerUser = new User(1234, "Seifan", "2457", false, false);
            ownerUser.register(ownerUser.UserName, ownerUser.Password);
            ownerUser.signIn(ownerUser.UserName, ownerUser.Password);
            ownerRole = new Owner(store, ownerUser);
            ownerUser.Roles.Add(store.Id, ownerRole);


            p1 = new Product(0, "first", "", "", 100);
            p2 = new Product(1, "second", "", "", 50);
            p3 = new Product(2, "third", "", "", 200);
            p4 = new Product(3, "fourth", "", "", 300);
            pis1 = new ProductInStore(20, store, p1);
            pis2 = new ProductInStore(20, store, p2);
            pis3 = new ProductInStore(20, store, p3);
            pis4 = new ProductInStore(20, store, p4);
            store.Products.Add(p1.Id, pis1);
            store.Products.Add(p2.Id, pis2);
            store.Products.Add(p3.Id, pis3);
            store.Products.Add(p4.Id, pis4);

            products = new List<KeyValuePair<String, int>>();
            products.Add(new KeyValuePair<String, int>("first", 2));
            products.Add(new KeyValuePair<String, int>("second", 10));
            products.Add(new KeyValuePair<String, int>("third", 5));
            products.Add(new KeyValuePair<String, int>("fourth", 4));


            system = new TradingSystem(null, null);
            system.Stores.Add(store.Id, store);
            system.Users.Add(admin.Id, admin);
            system.Users.Add(ownerUser.Id, ownerUser);
            system.Users.Add(user.Id, user);

            discountId = system.addRevealedDiscountPolicy(products, 20, ownerUser.Id, store.Id, 60, 0);
        }

        [TestMethod]
        public void removeRevealedDiscountPolicy_store_succ()
        {
            setUp();
            int ans = store.removeDiscountPolicy(discountId);
            Assert.AreEqual(0, ans);
        }

        [TestMethod]
        public void removeRevealedDiscountPolicy_role_succ()
        {
            setUp();
            int ans = ownerRole.removeDiscountPolicy(discountId);
            Assert.AreEqual(0, ans);
        }

        [TestMethod]
        public void removeRevealedDiscountPolicy_user_succ()
        {
            setUp();
            int ans = ownerUser.removeDiscountPolicy(discountId, store.Id);
            Assert.AreEqual(0, ans);
        }

        [TestMethod]
        public void removeRevealedDiscountPolicy_tradingSystem_succ()
        {
            setUp();
            int ans = system.removeDiscountPolicy(discountId, store.Id, ownerUser.Id);
            Assert.AreEqual(0, ans);
        }

        [TestMethod]
        public void removeRevealedDiscountPolicy_tradingSystem_fail()
        {
            setUp();
            int ans = system.removeDiscountPolicy(discountId, store.Id, user.Id);
            Assert.AreEqual(-1, ans);
        }
    }
}
