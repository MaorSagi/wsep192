﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.Domain;
using src.Domain.Dataclass;

namespace IntegrationTests
{
    [TestClass]
    public class PurchasePolicyTest
    {
        private TradingSystem sys;
        private Store store;
        private User admin;
        private User manager;
        private Role OwnerSotre;
        private Role managerSotre;
        private Product p1;
        private Product p2;
        private ProductInStore ps1;
        private ProductInStore ps2;
        private ProductConditionPolicy pcp;
        private inventoryConditionPolicy icp;
        private BuyConditionPolicy bcp;
        private UserConditionPolicy ucp;
        private IfThenCondition itcp;
        private LogicalConditionPolicy lcp;


        public void setup()
        {
            sys = new TradingSystem(null, null);
            admin = new User(0, "admin", "1234", true, true);
            admin.State = state.signedIn;
            store = new Store(0, "store");
            OwnerSotre = new Owner(store, admin);
            store.RolesDictionary.Add(admin.Id, store.Roles.AddChild(OwnerSotre));
            admin.Roles.Add(store.Id, OwnerSotre);
            manager = new User(1, "manager", "1234", false, true);
            manager.State = state.signedIn;
            List<int> permmision = new List<int>();
            permmision.Add(2);
            managerSotre = new Manager(store, manager, permmision);
            store.RolesDictionary.Add(manager.Id, store.Roles.AddChild(managerSotre));
            manager.Roles.Add(store.Id, managerSotre);

            p1 = new Product(1, "p1", null, null, 1);
            ps1 = new ProductInStore(10, store, p1);
            store.Products.Add(p1.Id, ps1);
            p2 = new Product(2, "p2", null, null, 10);
            ps2 = new ProductInStore(10, store, p2);
            store.Products.Add(p2.Id, ps2);
            pcp = new ProductConditionPolicy(0, 1, 0, 10, LogicalConnections.and);
            icp = new inventoryConditionPolicy(1, 1, 5, LogicalConnections.and);
            bcp = new BuyConditionPolicy(2, 2, 5, 10, 20, LogicalConnections.and);
            ucp = new UserConditionPolicy(3, "Tel Aviv", true, LogicalConnections.and);
            //itcp= if(buy p1 min buy =0 &max buy =10) then (min inventory =5)
            itcp = new IfThenCondition(4, pcp, icp, LogicalConnections.and);
            lcp = new LogicalConditionPolicy(5, LogicalConnections.and, LogicalConnections.and);

            sys.Users.Add(admin.Id, admin);
            sys.Users.Add(manager.Id, manager);
            sys.Stores.Add(store.Id, store);


        }



        //------------------------------@@ test path of check function @@-------------------------------------
        [TestMethod]
        public void ProductConditionPolicy_CheckCondition()
        {
            setup();
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 1));
            Assert.AreEqual(true, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, pcp.CheckCondition(cart, null));
        }

        [TestMethod]
        public void inventoryConditionPolicy_CheckCondition()
        {
            setup();
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, icp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, icp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, icp.CheckCondition(cart, null));

        }

        [TestMethod]
        public void BuyConditionPolicy_CheckCondition()
        {
            setup();
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 1));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null)); // Too few products

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null)); // Too much products

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 2));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null)); // Too cheap

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 3));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null)); // too expensive

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 2));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, bcp.CheckCondition(cart, null)); // proper



        }

        [TestMethod]
        public void UserConditionPolicy_CheckCondition()
        {
            setup();
            UserDetailes user = new UserDetailes("", false);
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "Registeration and Adress check fail");

            user.Isregister = true;
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "The adress check fail");

            user.Isregister = false;
            user.Adress = "Tel Aviv";
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "The registeretion check fail");

            user.Isregister = true;
            Assert.AreEqual(true, ucp.CheckCondition(null, user), "Registeration and Adress check fail");

        }


        //itcp= if(buy p1 min buy =0 &max buy =10) then (min inventory =5)
        [TestMethod]
        public void IfThenCondition_CheckCondition_SimpleOperands()
        {
            setup();
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, itcp.CheckCondition(cart, null), "The empty case - the product is not related to the purchase policy");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "Failure on condition is not satisfactory a");
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "Failure on condition is not satisfactory b");
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "then failure - the then condition are not satisfied");
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "All conditions are to be satisfied");

        }

        [TestMethod]
        public void LogicalConditionPolicy_addChild()
        {
            setup();
            Assert.AreEqual(true, lcp.addChild(icp), "Adding the child is successful");
            Assert.AreEqual(false, lcp.addChild(icp), "Adding a double child is prohibited");
        }

        [TestMethod]
        public void LogicalConditionPolicy_removeChild()
        {
            setup();
            Assert.AreEqual(false, lcp.removeChild(icp), "Deleting a child from an empty list should fail");
            Assert.AreEqual(true, lcp.addChild(icp), "Adding the child is successful");
            Assert.AreEqual(true, lcp.removeChild(icp), "Deleting a child from an existing list should succeed");
            Assert.AreEqual(false, lcp.removeChild(icp), "The child does not exist so the deletion was unsuccessful");
        }

        [TestMethod]
        public void LogicalConditionPolicy_getChild()
        {
            setup();
            lcp.addChild(icp);
            Assert.AreEqual(icp, lcp.getChild(icp.getId()), "The child is there");
            Assert.AreEqual(null, lcp.getChild(bcp.getId()), "The child is not there");
        }


        [TestMethod]
        public void LogicalConditionPolicy_CheckConditionAnd()
        {
            setup();
            //lcp= be satisfy - in cart must be 0 to 10 ps1 or nothing and inventory need minimum 5
            lcp = new LogicalConditionPolicy(5, LogicalConnections.and, LogicalConnections.and);
            lcp.addChild(pcp);
            lcp.addChild(icp);
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, lcp.CheckConditionAnd(cart, null), "The empty case - the product is not related to the purchase policy");
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "The empty case - the product is not related to the purchase policy");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, lcp.CheckConditionAnd(cart, null), "The first condition is not satisfy");
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "The first condition is not satisfy");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, lcp.CheckConditionAnd(cart, null), "Second condition are not satisfy");
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "Second condition are not satisfy");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, lcp.CheckConditionAnd(cart, null), "All conditions are satisfy");
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "All conditions are satisfy");

        }

        [TestMethod]
        public void LogicalConditionPolicy_CheckConditionOr()
        {
            setup();
            //lcp= be satisfy - in cart must be 2 to 10 ps1 or nothing or inventory need minimum 5
            lcp = new LogicalConditionPolicy(5, LogicalConnections.or, LogicalConnections.and);
            pcp = new ProductConditionPolicy(0, 1, 2, 10, LogicalConnections.and);
            lcp.addChild(pcp);
            lcp.addChild(icp);
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, lcp.CheckConditionOr(cart, null), "The empty case - the product is not related to the purchase policy");
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "The empty case - the product is not related to the purchase policy");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 1));
            Assert.AreEqual(true, lcp.CheckConditionOr(cart, null), "The first condition is not satisfy");
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "The first condition is not satisfy");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(true, lcp.CheckConditionOr(cart, null), "Second condition are not satisfy");
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "Second condition are not satisfy");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, lcp.CheckConditionOr(cart, null), "All conditions are satisfy");
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "All conditions are satisfy");


            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, lcp.CheckConditionOr(cart, null), "No condition is satisfy");
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "No condition is satisfy");

        }


        [TestMethod]
        public void LogicalConditionPolicy_CheckCondition_complexCondition()
        {
            setup();
            //lcp= be satisfy - in cart must be 2 to 10 ps1 or nothing or inventory need minimum 5
            LogicalConditionPolicy lcpP1 = new LogicalConditionPolicy(7, LogicalConnections.or, LogicalConnections.and);
            ProductConditionPolicy pcpP1 = new ProductConditionPolicy(8, p1.Id, 2, 10, LogicalConnections.and);
            inventoryConditionPolicy icpP1 = new inventoryConditionPolicy(9, p1.Id, 5, LogicalConnections.and);
            lcpP1.addChild(pcpP1);
            lcpP1.addChild(icpP1);

            LogicalConditionPolicy lcpP2 = new LogicalConditionPolicy(14, LogicalConnections.and, LogicalConnections.and);
            ProductConditionPolicy pcpP2 = new ProductConditionPolicy(15, p2.Id, 2, 10, LogicalConnections.and);
            inventoryConditionPolicy icpP2 = new inventoryConditionPolicy(16, p2.Id, 5, LogicalConnections.and);
            lcpP2.addChild(pcpP2);
            lcpP2.addChild(icpP2);

            lcp.addChild(lcpP1);
            lcp.addChild(lcpP2);

            ProductInStore ps3 = new ProductInStore(10, store, new Product(3, "", "", "", 1));
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps3, 1));
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "The empty case - the product is not related to the purchase policy");


            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 5));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "Sub-tree 1 is not satisfied");


            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "Sub-tree 2 is not satisfied");


            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 5));
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "Both are satisfied");
        }


        //itcp= if(buy p1 min buy =0 &max buy =10) then (min inventory =5)
        [TestMethod]
        public void IfThenCondition_CheckCondition_complexOperands()
        {
            setup();
            LogicalConditionPolicy lcpP1 = new LogicalConditionPolicy(7, LogicalConnections.or, LogicalConnections.and);
            ProductConditionPolicy pcpP1 = new ProductConditionPolicy(8, p1.Id, 2, 10, LogicalConnections.and);
            inventoryConditionPolicy icpP1 = new inventoryConditionPolicy(9, p1.Id, 5, LogicalConnections.and);
            lcpP1.addChild(pcpP1);
            lcpP1.addChild(icpP1);

            LogicalConditionPolicy lcpP2 = new LogicalConditionPolicy(14, LogicalConnections.and, LogicalConnections.and);
            ProductConditionPolicy pcpP2 = new ProductConditionPolicy(15, p2.Id, 2, 10, LogicalConnections.and);
            inventoryConditionPolicy icpP2 = new inventoryConditionPolicy(16, p2.Id, 5, LogicalConnections.and);
            lcpP2.addChild(pcpP2);
            lcpP2.addChild(icpP2);

            itcp = new IfThenCondition(7, lcpP1, lcpP2, LogicalConnections.and);


            ProductInStore ps3 = new ProductInStore(10, store, new Product(3, "", "", "", 1));
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps3, 1));
            Assert.AreEqual(true, itcp.CheckCondition(cart, null), "The empty case - the product is not related to the purchase policy");


            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 5));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "if is not satisfied");


            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "then 2 is not satisfied");


            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 5));
            Assert.AreEqual(true, itcp.CheckCondition(cart, null), "Both are satisfied");


        }
        [TestMethod]
        public void User_searchRoleByStoreIDWithValidatePermmision()
        {
            setup();
            //List<int> per = new List<int>();
            //per.Add(0);
            //manager.Roles.Add(store.Id,new Manager(store, manager,per));
            Assert.AreEqual(admin.Roles[store.Id], admin.searchRoleByStoreIDWithValidatePermmision(store.Id, 1), "Owner validate check");
            Assert.AreEqual(null, admin.searchRoleByStoreIDWithValidatePermmision(store.Id + 1, admin.Id), "A mistake in the identity number of the store");
            Assert.AreEqual(null, this.manager.searchRoleByStoreIDWithValidatePermmision(store.Id, 0), "No permissions are allowed");
            Assert.AreEqual(manager.Roles[store.Id], this.manager.searchRoleByStoreIDWithValidatePermmision(store.Id, 2), "A good case");
        }

        [TestMethod]
        public void User_addSimplePurchasePolicy()
        {
            setup();

            Assert.AreEqual(true, (admin.addSimplePurchasePolicy(new PurchesPolicyData(0,0,p1.Id,0,0,10,-1,-1,0,null,false), store.Id)).GetType()==typeof(ProductConditionPolicy), "Owner validate check");
            Assert.AreEqual(null, admin.addSimplePurchasePolicy(new PurchesPolicyData(0, 0, p1.Id, 0, 0, 10, -1, -1, 0, null, false), store.Id + 1), "A mistake in the identity number of the store");
            Assert.AreEqual(true, ( this.manager.addSimplePurchasePolicy(new PurchesPolicyData(0, 0, p1.Id, 0, 0, 10, -1, -1, 0, null, false), store.Id)).GetType() == typeof(ProductConditionPolicy), "A good case");
        }

        [TestMethod]
        public void TTradingSystem_addSimplePurchasePolicy()
        {
            setup();

            Assert.AreEqual(0, sys.addSimplePurchasePolicy(0, 1, 1, 1, 1, 1, null, false, store.Id, admin.Id), "ProductConditionPolicy check success");
            Assert.AreEqual(-1, sys.addSimplePurchasePolicy(0, -1, 1, 1, -1, -1, null, false, store.Id, admin.Id), "ProductConditionPolicy check fail");
        }




        //------------------------------@@ test path of List<object>->Compelx Purches Policy @@-------------------------------------




        [TestMethod]
        public void Store_factoryProductConditionPolicy()
        {
            setup();
            String details = "";

            details += " ( 0 , " + "p1" + " , 0 , 10 , 0 )";
            PurchasePolicy pcp = store.factoryProductConditionPolicy(1, details.Split(',').Length - 1, 0, details.Split(','), -1);
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, pcp.CheckCondition(cart, null));

        }


        [TestMethod]
        public void Store_addComplexPurchasePolicy_ProductConditionPolicy()
        {
            setup();
            String details = "";

            details += " ( 0 , " + "p1" + " , 0 , 10 , 0 )";
            PurchasePolicy pcp = store.addComplexPurchasePolicy(0, details);
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, pcp.CheckCondition(cart, null));

        }


        [TestMethod]
        public void Store_factoryinventoryConditionPolicy()
        {
            setup();
            String details = " ( 1 , p1, 5 , 0 )";
            PurchasePolicy icp = store.factoryinventoryConditionPolicy(1, details.Split(',').Length - 1, 0, details.Split(','), -1);
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, icp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, icp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, icp.CheckCondition(cart, null));
        }

        [TestMethod]
        public void Store_addComplexPurchasePolicy_inventoryConditionPolicy()
        {
            setup();
            String details = " ( 1 , 1, 5 , 0 )";
            PurchasePolicy icp = store.addComplexPurchasePolicy(0, details);
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, pcp.CheckCondition(cart, null));
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, pcp.CheckCondition(cart, null));

        }


        [TestMethod]
        public void Store_factoryBuyConditionPolicy()
        {
            setup();

            String details = " ( 2 , 2, 5 , 10 , 20 , 0 )";
            PurchasePolicy bcp = store.factoryBuyConditionPolicy(1, details.Split(',').Length - 1, 0, details.Split(','), -1);


            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 1));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null), "Too few products");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null), "Too much products");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 2));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null), "Too cheap");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 3));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null), "too expensive");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 2));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, bcp.CheckCondition(cart, null), "proper");
        }

        [TestMethod]
        public void Store_addComplexPurchasePolicy_addComplexPurchasePolicy_inventoryConditionPolicy()
        {
            setup();
            String details = " ( 2 , 2, 5 , 10 , 20 , 0 )";
            PurchasePolicy bcp = store.addComplexPurchasePolicy(0, details);
            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 1));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null), "Too few products");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null), "Too much products");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 2));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null), "Too cheap");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 3));
            Assert.AreEqual(false, bcp.CheckCondition(cart, null), "too expensive");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 2));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, bcp.CheckCondition(cart, null), "proper");
        }

        [TestMethod]
        public void Store_factoryUserConditionPolicy()
        {
            setup();

            String details = " ( 3 , Tel Aviv , 1, 0 )";
            PurchasePolicy ucp = store.factoryUserConditionPolicy(1, details.Split(',').Length - 1, 0, details.Split(','), -1);

            UserDetailes user = new UserDetailes("", false);
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "Registeration and Adress check fail");

            user.Isregister = true;
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "The adress check fail");

            user.Isregister = false;
            user.Adress = "Tel Aviv";
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "The registeretion check fail");

            user.Isregister = true;
            Assert.AreEqual(true, ucp.CheckCondition(null, user), "Registeration and Adress check fail");

        }


        [TestMethod]
        public void Store_addComplexPurchasePolicy_factoryUserConditionPolicy()
        {
            setup();

            String details = " ( 3 , Tel Aviv , 1, 0 )";
            PurchasePolicy ucp = store.addComplexPurchasePolicy(1, details);

            UserDetailes user = new UserDetailes("", false);
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "Registeration and Adress check fail");

            user.Isregister = true;
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "The adress check fail");

            user.Isregister = false;
            user.Adress = "Tel Aviv";
            Assert.AreEqual(false, ucp.CheckCondition(null, user), "The registeretion check fail");

            user.Isregister = true;
            Assert.AreEqual(true, ucp.CheckCondition(null, user), "Registeration and Adress check fail");

        }



        [TestMethod]
        public void Store_factoryIfThenCondition()
        {

            setup();
            String details = "( 4 , ( 0 , " + "p1" + " , 0 , 10 , 0 ) , ( 1 , p1, 5 , 0 ) , 0 )";
            PurchasePolicy itcp = store.factoryIfThenCondition(1, details.Length - 1, 0, details.Split(','), 0);

            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, itcp.CheckCondition(cart, null), "The empty case - the product is not related to the purchase policy");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "Failure on condition is not satisfactory a");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "Failure on condition is not satisfactory b");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "then failure - the then condition are not satisfied");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, itcp.CheckCondition(cart, null), "All conditions are to be satisfied");



        }


        [TestMethod]
        public void Store_addComplexPurchasePolicy_factoryIfThenCondition()
        {

            setup();
            String details = "( 4 , ( 0 , " + "p1" + " , 0 , 10 , 0 ) , ( 1 , p1, 5 , 0 ) , 0 )";
            PurchasePolicy itcp = store.addComplexPurchasePolicy(0, details);

            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, itcp.CheckCondition(cart, null), "The empty case - the product is not related to the purchase policy");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "Failure on condition is not satisfactory a");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "Failure on condition is not satisfactory b");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, itcp.CheckCondition(cart, null), "then failure - the then condition are not satisfied");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, itcp.CheckCondition(cart, null), "All conditions are to be satisfied");



        }
        [TestMethod]
        public void Store_factoryLogicalCondition_simple()
        {

            setup();
            // true if buy p1 between 0-10 and the min inventory 5
            String details = "( 5 , ( 0 , " + "p1" + " , 0 , 10 , 0 ) , ( 1 ," + "p1" + ", 5 , 0 ) , 0 , 0)";
            PurchasePolicy lcp = store.factoryLogicalCondition(1, details.Split(',').Length - 1, 0, details.Split(','), 0);

            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 1));
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "The empty case - the product is not related to the purchase policy");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "Failure on condition is not satisfactory a");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "Failure on condition is not satisfactory b");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "then failure - the then condition are not satisfied");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "All conditions are to be satisfied");



        }

        [TestMethod]
        public void Store_factoryLogicalCondition_complex()
        {

            setup();
            // true if buy p1 between 0-10 and the min inventory 5 or buy p2 between 0-10 and the min inventory 5
            String details = "(5 ,( 5 , ( 0 , " + "p1" + " , 0 , 10 , 0 ) , ( 1 ," + "p1" + ", 5 , 0 ) , 0 , 0), ( 5 , ( 0 , " + "p2" + " , 0 , 10 , 0 ) , ( 1 ," + "p2" + ", 5 , 0 ) , 0 , 0) , 1 , 0 )";
            PurchasePolicy lcp = store.factoryLogicalCondition(1, details.Split(',').Length - 1, 0, details.Split(','), 0);

            List<KeyValuePair<ProductInStore, int>> cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, -1));

            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "p1-Failure on condition is not satisfactory a");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 11));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, -1));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "p1-Failure on condition is not satisfactory b");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 6));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, -1));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "p1-then failure - the then condition are not satisfied");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, -1));

            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "p1-All conditions are to be satisfied");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, -1));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "p2-Failure on condition is not satisfactory a");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 11));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "p2-Failure on condition is not satisfactory b");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 6));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(false, lcp.CheckCondition(cart, null), "p2-then failure - the then condition are not satisfied");
            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 5));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, -1));
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "p2-All conditions are to be satisfied");

            cart = new List<KeyValuePair<ProductInStore, int>>();
            cart.Add(new KeyValuePair<ProductInStore, int>(ps2, 5));
            cart.Add(new KeyValuePair<ProductInStore, int>(ps1, 5));
            Assert.AreEqual(true, lcp.CheckCondition(cart, null), "Both-All conditions are to be satisfied");



        }
    }
}
