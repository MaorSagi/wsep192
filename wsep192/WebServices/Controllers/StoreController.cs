﻿using src.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebServices.Controllers
{
    public class StoreController : Controller
    {
        ServiceLayer service = ServiceLayer.getInstance();
        [Route("api/store/AddProductInStore")]
        [HttpGet]

        public string addProductInStore(string userName, string productName, int productQuantity, string storeName)
        {
            List<KeyValuePair<String, int>> productList = new List<KeyValuePair<String, int>>();
            productList.Add(new KeyValuePair<String, int>(productName, productQuantity));

            bool ans = service.addProductsInStore(productList, storeName, userName);
            switch (ans)
            {
                case true:
                    return "Product successfully added to store";
                case false:
                    return "Error in add product in store";
            }
            return "server error: AddProductInStore";
        }
    }
}