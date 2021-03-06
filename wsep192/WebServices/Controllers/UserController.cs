﻿using src.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;



namespace WebService.Controllers
{

    public class UserController : ApiController
    {
        ServiceLayer service = ServiceLayer.getInstance();
        [Route("api/user/RegisterUser")]
        [HttpGet]
        public string register(String Username, String Password)
        {
            string user = service.initUser();
            bool ans = service.register(Username, Password, user);

            switch (ans)
            {
                case true:
                    return "User successfuly registered";
                case false:
                    return "Error in register";
            }
            return "server error: RegisterUser";
        }



        [Route("api/user/ShoppingCart")]
        [HttpGet]
        public string showCart(String Store, String User)
        {
            List<KeyValuePair<String, int>> cart = service.showCart(Store, User);
            if (cart == null)
                return "null";
            else
                return listToString(cart);
        }
        private string listToString(List<KeyValuePair<string, int>> list)
        {
            string str = "";
            for (int i = 0; i < list.Count; i++)
            {
                str += list[i].Key + "," + list[i].Value.ToString() + ",";
            }
            return str;
        }

        [Route("api/user/RemoveFromCart")]
        [HttpGet]
        public string RemoveFromCart(String list, String store, String user)
        {
            List<string> l = toList(list);
            bool res = service.removeProductsFromCart(l, store, user);
            switch (res)
            {
                case true:
                    return "true";
                case false:
                    return "false";
            }
            return "Server error: removeFromCart";

        }

        [Route("api/user/AddtoCart")]
        [HttpGet]
        public string AddtoCart(String list, String store, String user)
        {
            List<KeyValuePair<string, int>> l = null;
            if (list != null)
                l = toPairList(list);
            bool res = service.addProductsToCart(l, store, user);
            switch (res)
            {
                case true:
                    return "true";
                case false:
                    return "false";
            }
            return "Server error: removeFromCart";

        }

        private List<KeyValuePair<string, int>> toPairList(string list)
        {
            var products = list.Split(',');
            string key = "";
            List<KeyValuePair<string, int>> output = new List<KeyValuePair<string, int>>();
            for (int i = 0; i < products.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    key = products[i];
                }
                else
                {
                    output.Add(new KeyValuePair<string, int>(key, Int32.Parse(products[i])));
                }

            }
            return output;
        }

        private List<String> toList(string products)
        {
            List<String> list;
            if (products != null)
                list = new List<String>(products.Split(','));
            else list = new List<String>();
            list.Remove("");
            return list;
        }

        [Route("api/user/EditCart")]
        [HttpGet]
        public string EditCart(String product, string quantity, String store, String user)
        {
            bool res = service.editProductQuantityInCart(product, Int32.Parse(quantity), store, user);
            switch (res)
            {
                case true:
                    return "true";
                case false:
                    return "false";
            }
            return "Server error: editCart";
        }

        [Route("api/user/LoginUser")]
        [HttpGet]
        public string login(String Username, String Password)
        {
            bool ans = service.signIn(Username, Password);
            switch (ans)
            {
                case true:
                    return "User successfuly logged in";
                case false:
                    return "Error in login";
            }
            return "Server error: LoginUser";
        }

        [Route("api/user/LogOutUser")]
        [HttpGet]
        public string logout(String Username)
        {
            bool ans = service.signOut(Username);
            switch (ans)
            {
                case true:
                    return "true";
                case false:
                    return "false";
            }
            return "Server error: logOutUser";
        }

        [Route("api/user/generateUserID")]
        [HttpGet]
        public Object generateUserID()
        {
            return service.initUser();
        }

        [Route("api/user/OpenStore")]
        [HttpGet]
        public string openStore(String Username, String StoreName)
        {
            bool ans = service.openStore(StoreName, Username);
            switch (ans)
            {
                case true:
                    return "Store created successfully";
                case false:
                    return "Error in creating store";
            }
            return "Server error: openStore";
        }
        [Route("api/user/assignOwner")]
        [HttpGet]
        public string assignOwner(String ownerName, String userToAssign, String storeName)
        {
            bool ans = service.assignOwnerRequest(ownerName, userToAssign, storeName);
            switch (ans)
            {
                case true:
                    return "Requests assigning "+userToAssign+" were successfully sent";
                case false:
                    return "false";
            }
            return "Server error: assignOwner";
        }
        [Route("api/user/removeOwner")]
        [HttpGet]
        public string removeOwner(String ownerToRemove, String storeName, String ownerName)
        {
            bool ans = service.removeOwner(ownerToRemove, storeName, ownerName);
            switch (ans)
            {
                case true:
                    return "Owner successfuly was removed";
                case false:
                    return "Error in removing owner";
            }
            return "Server error: removeOwner";
        }
        [Route("api/user/assignManager")]
        [HttpGet]
        public string assignManager(String ownerName, String userToAssign, String storeName, String permissions)
        {
            List<String> perms = permissions.Split(' ').ToList();
            bool ans = service.assignManager(userToAssign, storeName, perms, ownerName);
            switch (ans)
            {
                case true:
                    return "Manager successfuly was assigned";
                case false:
                    return "Error in assining manager";
            }
            return "Server error: assignManager";
        }

        [Route("api/user/setUp")]
        [HttpGet]
        public string setUp()
        {
            bool ans = service.setUp();
            switch (ans)
            {
                case true:
                    return "System setup completed";
                case false:
                    return "Error in setUp";
            }
            return "Server error: setUp";
        }

        [Route("api/user/SearchProduct")]
        [HttpGet]
        public string searchProduct(String details)
        {
            string ans = service.searchProduct(details);
            return ans;
        }

        [Route("api/user/RemoveUser")]
        [HttpGet]
        public string removeUser(String Username, String RemoveUser)
        {
            bool ans = service.removeUser(RemoveUser, Username);
            switch (ans)
            {
                case true:
                    return "User successfuly removed";
                case false:
                    return "Error in remove user";
            }
            return "Server error: removeUser";
        }

        [Route("api/user/RemoveManager")]
        [HttpGet]
        public string removeManager(String managerToRemove, String storeName, String ownerName)
        {
            bool ans = service.removeManager(managerToRemove, storeName, ownerName);
            switch (ans)
            {
                case true:
                    return "Manager successfuly was removed";
                case false:
                    return "Error in remove user";
            }
            return "Server error: removeUser";
        }

        [Route("api/user/CheckoutBasket")]
        [HttpGet]
        public string checkoutBasket(String address, String username)
        {
            int addressNum;
            try
            {
                addressNum = Int32.Parse(address);
            }
            catch (Exception e)
            {
                addressNum = -1;
            }
            switch (addressNum)
            {
                case 1:
                    address = "telaviv";
                    break;
                case 2:
                    address = "beersheva";
                    break;
                case 3:
                    address = "haifa";
                    break;
                default:
                    address = "";
                    break;
            }
            double ans = service.basketCheckout(address, username);
            return ans + "";
        }

        [Route("api/user/payForBasket")]
        [HttpGet]
        public string payForBasket(String cardNum, String expiredDate, String username)
        {
            List<string[]> res;
            int day, month, year;
            long cardNumber;
            DateTime dateTime;
            String[] date = expiredDate.Split('/');
            try
            {
                day = Int32.Parse(date[0]);
                month = Int32.Parse(date[1]);
                year = Int32.Parse(date[2]);
                cardNumber = Int32.Parse(cardNum);
                dateTime = new DateTime(year, month, day);
            }
            catch (Exception ex)
            {
                return "Date or card number are wrong";
            }

            res = service.payForBasket(cardNumber, dateTime, username);
            return productsToString(res);
        }

        private String productsToString(List<String[]> products)
        {
            String res = "";
            int i = 0;
            int sum = 0;
            foreach (String[] s in products)
            {
                res += "name" + i + "=" + s[0] + "&"
                     + "store" + i + "=" + s[1] + "&"
                    + "quantity" + i + "=" + s[2] + "&"
                    + "price" + i + "=" + s[3] + "&"
                    + "total" + i + "=" + s[4] + "&"
                    ;
                sum += Int32.Parse(s[4]);
                i++;

            }
            res += "sum=" + sum;
            return res;
        }



        [Route("api/user/getVisibility")]
        [HttpGet]
        public bool[] getVisibility(String userName)
        {
            return service.getVisibility(userName);
        }

        [Route("api/user/removeGuestUser")]
        [HttpGet]
        public string removeGuestUser(String guestID)
        {

            bool ans = service.removeUser(guestID, "admin");
            switch (ans)
            {
                case true:
                    return "success";
                case false:
                    return "Error in removeGuestUser";
            }
            return "Error in removeGuestUser";
        }

        [Route("api/user/assignOwnerResult")]
        [HttpGet]
        public string assignOwnerResult(String reqId,String result)
        {

            bool ans = service.assignOwnerResult(Int32.Parse(reqId),Boolean.Parse(result));
            switch (ans)
            {
                case true:
                    return "true";
                case false:
                    return "false";
            }
            return "Error in assignOwnerResult";
        }




    }


}