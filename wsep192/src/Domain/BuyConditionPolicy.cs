﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using src.Domain.Dataclass;

namespace src.Domain
{
    class BuyConditionPolicy : PurchasePolicy
    {
        private int id;
        private int min;
        private int max;
        private int sumMin;
        private int sumMax;
        private LogicalConnections act;


        public BuyConditionPolicy(int id, int min, int max, int sumMin, int sumMax, LogicalConnections act)
        {
            this.id = id;
            this.min = min;
            this.max = max;
            this.sumMin = sumMin;
            this.sumMax = sumMax;
            this.act = act;
        }


        public bool CheckCondition(List<KeyValuePair<ProductInStore, int>> cart, UserDetailes user)
        {
            int sum = 0;
            int totalProducts = 0;

            foreach (KeyValuePair<ProductInStore, int> product in cart)
            {
                totalProducts += product.Value;
                sum += product.Key.Product.Price * product.Value;
            }

            if (min != -1 && totalProducts < min)
                return false;
            if (max != -1 && totalProducts > max)
                return false;
            if (sumMin != -1 && sum < sumMin)
                return false;
            if (sumMax != -1 && sum > sumMax)
                return false;

            return true;
        }

        public int getId()
        {
            return id;
        }
    }
}
