﻿using src.DataLayer;
using src.Domain.Dataclass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Domain
{
    public enum state { visitor, signedIn }
    class User
    {
        private int id;
        private String userName;
        private String password;
        private String address;
        private state state;
        private Boolean isAdmin;
        private Boolean isRegistered;
        private ShoppingBasket basket;
        private Dictionary<int, Role> roles;
        private List<String> orderStores;
        private List<String> messages;
        private List<OwnerRequest> requests;
        private state signedIn;
        private state visitor;

        public User(int id, string userName, string password, bool isAdmin, bool isRegistered)
        {
            this.id = id;
            this.userName = userName;
            this.password = password;
            this.address = "";
            this.state = state.visitor;
            this.isAdmin = isAdmin;
            this.isRegistered = isRegistered;
            this.basket = new ShoppingBasket();
            this.roles = new Dictionary<int, Role>();
            this.orderStores = new List<String>();
            this.messages = new List<String>();
            //message , <store,owner>
            this.requests = new List<OwnerRequest>();
        }

        public int Id { get => id; set => id = value; }
        public string UserName { get => userName; set => userName = value; }
        public string Password { get => password; set => password = value; }
        public string Address { get => address; set => address = value; }
        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }
        public bool IsRegistered { get => isRegistered; set => isRegistered = value; }
        internal state State { get => state; set => state = value; }
        internal ShoppingBasket Basket { get => basket; set => basket = value; }
        internal Dictionary<int, Role> Roles { get => roles; set => roles = value; }

        internal bool removeProductsInStore(List<KeyValuePair<int, int>> productsInStore, int storeID)
        {
            Role role = searchRoleByStoreID(storeID, this.Id);
            if (role != null && (role.GetType() == typeof(Owner) || (role.GetType() == typeof(Manager) && ((Manager)role).validatePermission(5))))
                return role.Store.removeProductsInStore(productsInStore, this.id);
            LogManager.Instance.WriteToLog("User-remove product in store fail- the role does not exists or doesn't have permissions\n");
            return false;
        }

        internal bool addProductsInStore(List<KeyValuePair<int, int>> productsInStore, int storeID)
        {
            Role role = searchRoleByStoreID(storeID, this.Id);
            if (role != null && (role.GetType() == typeof(Owner) || (role.GetType() == typeof(Manager) && ((Manager)role).validatePermission(4))))
                return role.Store.addProductsInStore(productsInStore, this.id);
            LogManager.Instance.WriteToLog("User-add products in store fail- the role does not exists or doesn't have permissions\n");
            return false;
        }

        public List<String> getOrderStores()
        {
            return this.orderStores;
        }
        public void setOrderStores()
        {
            this.orderStores = new List<String>();
            foreach(ShoppingCart sc in basket.ShoppingCarts.Values)
            {
                orderStores.Add(sc.Store.Name);
            }
        }

        internal bool editProductsInStore(int productID, string productName, string category, string details, int price, int storeID)
        {
            Role role = searchRoleByStoreID(storeID, this.Id);
            if (role != null && (role.GetType() == typeof(Owner) || (role.GetType() == typeof(Manager) && ((Manager)role).validatePermission(6))))
                return role.Store.editProductsInStore(productID, productName, category, details, price, this.id);
            LogManager.Instance.WriteToLog("User-edit products in store fail- the role does not exists or doesn't have permissions\n");
            return false;
        }

        public bool removeOwner(int userID, int storeID)
        {
            if (this.state != state.signedIn)
            {
                LogManager.Instance.WriteToLog("User-removeOwner " + this.id + " isn't signed in");
                return false;
            }
            Role role = searchRoleByStoreID(storeID, this.Id);
            if (role != null && role.GetType() == typeof(Owner))
            {
                Owner owner = (Owner)role;
                return owner.removeOwner(userID);

            }
            LogManager.Instance.WriteToLog("User-removeOwner " + role.User.Id + " isn't owner");
            return false;

        }

        internal bool createNewProductInStore(string productName, string category, string details, int price, int productID, int storeID)
        {
            Role role = searchRoleByStoreID(storeID, this.Id);
            if (role != null && (role.GetType() == typeof(Owner) || (role.GetType() == typeof(Manager) && ((Manager)role).validatePermission(3))))
                return role.Store.createNewProductInStore(productName, category, details, price, productID, this.Id);
            LogManager.Instance.WriteToLog("User-create new product in store fail- the role does not exists or doesn't have permissions\n");
            return false;
        }

        public virtual Boolean assignManager(User managerUser, int storeId, List<int> permissionToManager)
        {
            if (this.state != state.signedIn || !managerUser.IsRegistered)
            {
                LogManager.Instance.WriteToLog("User - assign manger fail - owner not signed in or manager not registered");
                return false;
            }
            if (this.Roles.ContainsKey(storeId))
            {
                Role role = Roles[storeId];
                if (role != null && role.GetType() == typeof(Owner))
                {
                    Owner owner = (Owner)role;
                    return owner.assignManager(managerUser, permissionToManager);

                }
            }
            LogManager.Instance.WriteToLog("User - assign manger fail -owner not exist in roles");
            return false;
        }

        internal virtual List<KeyValuePair<string, int>> showCart(int storeId)
        {
            return basket.showCart(storeId);
        }


        public virtual Boolean register(string userName, string password)
        {
            if (userName == null || password == null)
            {
                return false;
            }
            this.userName = userName;
            this.password = password;
            this.IsRegistered = true;
            LogManager.Instance.WriteToLog("Register - userName or password null\n");
            return true;
        }
 
        public void addRole(Role role)

        {
            Roles.Add(role.Store.Id, role);
        }

        public Role searchRoleByStoreID(int storeID, int userID)
        {
            foreach (Role role in roles.Values)
                if (role.Store.Id == storeID && role.User.Id == userID)
                    return role;
            return null;
        }


        public virtual double basketCheckout(String address)
        {
            this.address = address;
            if (Basket.ShoppingCarts.Count == 0)
                return 0;
            double basketSum = basket.basketCheckout(new UserDetailes(address, IsRegistered));
            if (basketSum == 0)
                return 0;
            if (basketSum == -1)
                return -1;
            return basketSum + calcAddressFee(address);
        }

        internal bool signOut()
        {
            if (state != state.signedIn)
            {
                LogManager.Instance.WriteToLog("User:signOut failed - user " + UserName + " didn't sign in\n");
                return false;
            }
            state = state.visitor;
            LogManager.Instance.WriteToLog("User:signOut success - " + userName + "\n");
            return true;

        }

        public virtual ShoppingCart addProductsToCart(LinkedList<KeyValuePair<Product, int>> productsToInsert, int storeId)
        {
            return this.basket.addProductsToCart(productsToInsert, storeId,this.id);
        }

        public virtual Boolean signIn(string userName, string password)
        {
            if (userName != null && password != null)
            {
                this.userName = userName;
                this.password = password;
                this.state = state.signedIn;
                return true;
            }
            return false;
        }

        private int calcAddressFee(string address)
        {
            switch (address)
            {
                case "telaviv":
                    return 50;
                case "beersheva":
                    return 10;
                case "haifa":
                    return 60;
                default:
                    return 100;
            }
        }

        internal virtual bool removeProductsFromCart(List<int> productsToRemove, int storeId)
        {
            return basket.removeProductsFromCart(productsToRemove, storeId,this.id);
        }

        internal virtual bool editProductQuantityInCart(int productId, int quantity, int storeId)
        {
            return basket.editProductQuantityInCart(productId, quantity, storeId,this.id);
        }

        public virtual bool removeManager(int userID, int storeID)
        {
            if (this.state != state.signedIn)
            {
                LogManager.Instance.WriteToLog("User-Remove manager fail- User is not logged in\n");
                return false;
            }
            Role role = searchRoleByStoreID(storeID, this.id);
            try
            {
                Owner owner = (Owner)role;
                return owner.removeManager(userID);
            }
            catch (Exception)
            {
                ErrorManager.Instance.WriteToLog("User-remove manager fail- User " + this.id + " does not have appropriate permissions in Store " + storeID + " .\n");
                return false;
            }

        }
        public List<String> getMessages()
        {
            return this.messages;
        }
        public void deleteMessages()
        {
            this.messages = new List<String>();
        }
        public List<OwnerRequest> getRequests()
        {
            return this.requests;
        }
        public void deleteRequests()
        {
            this.requests = new List<OwnerRequest>();
        }


        public bool assignOwner(int storeID, User assigned)
        {
            Role roleOwner = searchRoleByStoreID(storeID, this.Id);
            Role roleAssigned = searchRoleByStoreID(storeID, assigned.Id);
            try
            {
                Owner owner = (Owner)roleOwner;
                if (roleAssigned == null)
                    return owner.assignOwner(assigned);
                else
                {
                    LogManager.Instance.WriteToLog("User-remove manager fail- User " + this.id + " already has role in Store: " + storeID + " .\n");
                    return false;
                }
            }
            catch (Exception)
            {
                ErrorManager.Instance.WriteToLog("User-remove manager fail- User " + this.id + " does not have appropriate permissions in Store " + storeID + " .\n");
                return false;
            }
        }

        public virtual PurchasePolicy addSimplePurchasePolicy(PurchesPolicyData purchesData, int storeID)
        {
            Role role;
            if ((role = searchRoleByStoreIDWithValidatePermmision(storeID, 2)) != null)
                return role.addSimplePurchasePolicy(purchesData);
            return null;
        }

        public virtual PurchasePolicy addComplexPurchasePolicy(int ID, String purchesData, int storeID)
        {

            Role role;
            if ((role = searchRoleByStoreIDWithValidatePermmision(storeID, 2)) != null)
                return role.addComplexPurchasePolicy(ID, purchesData);
            return null;

        }

        internal Role searchRoleByStoreIDWithValidatePermmision(int storeID, int premmision)
        {
            Role role;
            if (this.state != state.signedIn)
            {
                LogManager.Instance.WriteToLog("User-search Role By StoreID With Validate Permmision fail- User is not logged in\n");
                return null;
            }

            if ((role = searchRoleByStoreID(storeID, this.id)) == null)
            {
                LogManager.Instance.WriteToLog("User-search Role By StoreID With Validate Permmision fail- The user " + this.id + " does not have a role in the " + storeID + " store\n");
                return null;
            }

            if (role != null && role.GetType() == typeof(Manager))
            {
                Manager manager = (Manager)role;
                if (!manager.validatePermission(premmision))
                {
                    LogManager.Instance.WriteToLog("User-search Role By StoreID With Validate Permmision fail- Manager does not have permissions\n");
                    return null;
                }
            }
            return role;
        }

        public virtual int addRevealedDiscountPolicy(List<KeyValuePair<String, int>> products, double discountPrecentage, int storeID, int expiredDiscountDate, int discountId, DuplicatePolicy logic)
        {
            Role role;
            DateTime currentTime = DateTime.Now;
            DateTime expiredDate = currentTime.AddDays(expiredDiscountDate);
            if ((role = searchRoleByStoreIDWithValidatePermmision(storeID, 1)) != null)
            {
                return role.addRevealedDiscountPolicy(products, discountPrecentage, expiredDate, discountId, logic);
            }
            return -1;
        }

        public virtual int addConditionalDiscuntPolicy(List<String> products, String condition, double discountPrecentage, int expiredDiscountDate, DuplicatePolicy duplicate, LogicalConnections logic, int discountId, int storeId)
        {
            Role role;
            DateTime currentTime = DateTime.Now;
            DateTime expiredDate = currentTime.AddDays(expiredDiscountDate);
            if ((role = searchRoleByStoreIDWithValidatePermmision(storeId, 1)) != null)
            {
                return role.addConditionalDiscuntPolicy(products, condition, discountPrecentage, expiredDate, discountId, duplicate, logic);
            }
            return -1;
        }


        public virtual int removeDiscountPolicy(int discountId, int storeId)
        {
            Role role;
            if ((role = searchRoleByStoreIDWithValidatePermmision(storeId, 1)) != null)
            {
                return role.removeDiscountPolicy(discountId);
            }
            return -1;
        }

        public virtual int removePurchasePolicy(int purchaseId, int storeId)
        {
            Role role;
            if ((role = searchRoleByStoreIDWithValidatePermmision(storeId, 1)) != null)
            {
                return role.removePurchasePolicy(purchaseId);
            }
            return -1;
        }

        internal bool isLoggedIn()
        {
            return this.state == state.signedIn;
        }
    }
}
