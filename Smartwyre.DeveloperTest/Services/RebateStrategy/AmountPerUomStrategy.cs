using System;
using Smartwyre.DeveloperTest.Interfaces;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public class AmountPerUomStrategy : IRebateIncentiveStrategy
    {
        public decimal CalculateRebate(Rebate rebate, Product product, CalculateRebateResult result, CalculateRebateRequest request)
        {
            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom) ||
            rebate.Amount == 0 || request.Volume == 0)
            {
                result.Success = false;
                return 0m;
            }

            result.Success = true;
            return rebate.Amount * request.Volume;
        }
    }
}

