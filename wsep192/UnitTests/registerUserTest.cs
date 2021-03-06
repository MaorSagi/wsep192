﻿using System;
using src.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.DataLayer;

namespace UnitTests
{
    [TestClass]
    public class registerUserTest
    {
        private TradingSystem system;
        private User user1;

        public void setUp()
        {
            DBtransactions db = DBtransactions.getInstance(true);
            system = new TradingSystem(null, null);
            user1 = new User(1234, "Seifan", "2457", false, false);
            system.Users.Add(user1.Id, user1);
        }

        [TestMethod]
        public void TestMethod1_success_user_scenario()
        {
            setUp();
            //user1.db.IsTest = true;
            String userName = user1.UserName;
            String password = user1.Password;
            Assert.AreEqual(true, user1.register(userName, password));
        }

        [TestMethod]
        public void TestMethod1_fail_user_scenario()
        {
            setUp();
            String userName = user1.UserName;
            String password = null;
            Assert.AreEqual(false, user1.register(userName, password));
        }

        [TestMethod]
        public void TestMethod1_fail2_user_scenario()
        {
            setUp();
            String userName = null;
            String password = user1.Password;
            Assert.AreEqual(false, user1.register(userName, password));
        }



        [TestMethod]
        public void TestMethod1_success_scenario()
        {
            setUp();
            DBtransactions db = DBtransactions.getInstance(true);
            db.isTest(true);
            StubUser tmpUser = new StubUser(123, "yuval", "4567", false, false, true);
            String userName = tmpUser.UserName;
            String password = tmpUser.Password;
            int userId = tmpUser.Id;
            system.Users.Add(tmpUser.Id, tmpUser);
            Assert.AreEqual(true, system.register(userName, password, userId));
        }

        [TestMethod]
        public void TestMethod1_fail_password_scenario()
        {
            setUp();
            StubUser tmpUser = new StubUser(123, "yuval", "4567", false, false, true);
            String userName = tmpUser.UserName;
            String password = " ";
            int userId = tmpUser.Id;
            system.Users.Add(tmpUser.Id, tmpUser);
            Assert.AreEqual(false, system.register(userName, password, userId));
        }

        [TestMethod]
        public void TestMethod1_fail_userName_scenario()
        {
            setUp();
            StubUser tmpUser = new StubUser(123, "yuval", "4567", false, false, false);
            String userName = "blabla";
            String password = tmpUser.Password;
            int userId = tmpUser.Id;
            system.Users.Add(tmpUser.Id, tmpUser);
            Assert.AreEqual(false, system.register(userName, password, userId));
        }

        [TestMethod]
        public void TestMethod1_fail_userName_password_scenario()
        {
            setUp();
            StubUser tmpUser = new StubUser(123, "yuval", "4567", false, false, false);
            String userName = "blabla";
            String password = "8888";
            int userId = tmpUser.Id;
            system.Users.Add(tmpUser.Id, tmpUser);
            Assert.AreEqual(false, system.register(userName, password, userId));
        }





        /*------------------------stub-classes------------------------------------*/

        class StubUser : User
        {
            bool retVal;
            public StubUser(int id, string userName, string password, bool isAdmin, bool isRegistered, bool ret) : base(id, userName, password, isAdmin, isRegistered)
            {
                this.retVal = ret;
            }

            public override bool register(string userName, string password)
            {
                return retVal;
            }
        }
    }
}
