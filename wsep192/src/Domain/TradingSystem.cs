﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Domain
{
    class TradingSystem
    {
        private Dictionary<int,User> users;
        private Dictionary<int,Store> stores;
        private ProductSupplySystem supplySystem;
        private FinancialSystem financialSystem;
        private int productCounter;
        private int storeCounter;
        private int userCounter;
        private int purchasePolicyCounter;
        private int discountPolicyCounter;
        private Encryption encryption;

        public TradingSystem(ProductSupplySystem supplySystem, FinancialSystem financialSystem)
        {
            this.users = new Dictionary<int, User>();
            this.stores = new Dictionary<int, Store>();
            this.supplySystem = supplySystem;
            this.financialSystem = financialSystem;
            this.productCounter = 0;
            this.storeCounter = 0;
            this.userCounter = 0;
            this.purchasePolicyCounter = 0;
            this.discountPolicyCounter =  0;
            this.encryption = new EncryptionImpl();
        }
        public List<ProductInStore> searchProduct(String details)
        {
            List<ProductInStore> products  = new List<ProductInStore>();
            String[] detailsForFilter = details.Split(' ');
            if (detailsForFilter.Length != 7)
                return products;
            KeyValuePair<int, int> priceRange = new KeyValuePair<int, int>(Int32.Parse(detailsForFilter[3]),
                Int32.Parse(detailsForFilter[4]));
            Filter filter = new Filter(detailsForFilter[0],
                detailsForFilter[1], detailsForFilter[2], priceRange,
                Int32.Parse(detailsForFilter[5]), Int32.Parse(detailsForFilter[6]));
            foreach (Store s in stores.Values)
            {
                s.searchProduct(filter,products);
            }
            return products;
        }
        public User searchUser(int userID)
        {
            foreach (User u in users.Values)
                if (u.Id == userID)
                    return u;
            return null;
        }
        public Store searchStore(int storeID)
        {
            foreach (Store s in stores.Values)
                if (s.Id == storeID)
                    return s;
            return null;
        }
        public int basketCheckout(String address,int userID)
        {
            if (!this.users.ContainsKey(userID))
                return -1;
            else
                return this.users[userID].basketCheckout(address);
        }
        public ShoppingBasket payForBasket(long cardNumber,DateTime date,int userID)
        {
            ShoppingBasket basket = users[userID].Basket;
            foreach(ShoppingCart cart in basket.ShoppingCarts.Values)
            {
                cart.Store.updateCart(cart,"-");
            }

            if(!this.financialSystem.Payment(cardNumber, date, basket.basketCheckout()))
            {
                foreach (ShoppingCart cart in basket.ShoppingCarts.Values)
                {
                    cart.Store.updateCart(cart,"+");
                }
                return null;
            }


            if (!this.supplySystem.deliverToCustomer(this.Users[userID].Address,"Some package Details"))
            {
                return null;
            }

            return basket;
        }
        public int ProductCounter { get => productCounter; set => productCounter = value; }
        public int StoreCounter { get => storeCounter; set => storeCounter = value; }
        public int UserCounter { get => userCounter; set => userCounter = value; }
        public int PurchasePolicyCounter { get => purchasePolicyCounter; set => purchasePolicyCounter = value; }
        public int DiscountPolicyCounter { get => discountPolicyCounter; set => discountPolicyCounter = value; }
        internal Dictionary<int, User> Users { get => users; set => users = value; }
        internal Dictionary<int, Store> Stores { get => stores; set => stores = value; }
        internal ProductSupplySystem SupplySystem { get => supplySystem; set => supplySystem = value; }
        internal FinancialSystem FinancialSystem { get => financialSystem; set => financialSystem = value; }
        public bool init(string adminUserName, string adminPassword)
        {
            User admin = new User(userCounter, adminUserName, adminPassword, true, true);
            users.Add(userCounter, admin);
            userCounter++;
            if (!financialSystem.Connect() || !supplySystem.connect() || !encryption.connect())
                return false;

            return true;
        }

        public bool signOut(int id) {
            if (!users.ContainsKey(id))
                return false;
            return users[id].signOut();

        }

    }
}
