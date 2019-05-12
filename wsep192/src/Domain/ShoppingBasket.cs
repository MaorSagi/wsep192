﻿using src.Domain.Dataclass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Domain
{
    class ShoppingBasket
    {
        private Dictionary<int, ShoppingCart> shoppingCarts;

        public ShoppingBasket()
        {
            this.ShoppingCarts = new Dictionary<int, ShoppingCart>();
        }

        internal Dictionary<int, ShoppingCart> ShoppingCarts { get => shoppingCarts; set => shoppingCarts = value; }


        public virtual double basketCheckout(UserDetailes user)
        {
            double sum = 0;
            double tmp = 0;
            foreach (ShoppingCart c in shoppingCarts.Values)
            {
                tmp = c.cartCheckout(user);
                if (tmp == -1)
                    return -1;
                sum += tmp;
            }
            return sum;

        }

        internal List<KeyValuePair<string, int>> showCart(int storeId)
        {
            if (!shoppingCarts.ContainsKey(storeId))
            {
                LogManager.Instance.WriteToLog("ShoppingBasket:showCart failed - Shopping basket does not contain the store\n");
                return null;
            }

            return shoppingCarts[storeId].showCart();
        }
        public ShoppingCart addProductsToCart(LinkedList<KeyValuePair<Product, int>> productsToInsert, int storeID)
        {
            bool exist = true;
            if (productsToInsert.Count == 0)
                return null;
            if (!this.shoppingCarts.ContainsKey(storeID))
            {
                exist = false;
                shoppingCarts.Add(storeID, new ShoppingCart(storeID, null));

            }
            shoppingCarts[storeID].addProducts(productsToInsert);
            if (!exist)
                return shoppingCarts[storeID];
            return null;
        }

        internal bool removeProductsFromCart(List<int> productsToRemove, int storeId)
        {
            if (!shoppingCarts.ContainsKey(storeId))
            {
                LogManager.Instance.WriteToLog("ShoppingBasket:removeProductsFromCart failed - Shopping cart does not exist\n");
                return false;
            }
            return shoppingCarts[storeId].removeProductsFromCart(productsToRemove);
        }
        internal bool editProductQuantityInCart(int productId, int quantity, int storeId)
        {
            if (!shoppingCarts.ContainsKey(storeId))
            {
                LogManager.Instance.WriteToLog("ShoppingBasket:editProductQuantityInCart failed - Shopping cart does not exist\n");
                return false;

            }
            return shoppingCarts[storeId].editProductQuantityInCart(productId, quantity);
        }
    }
}
