using System;
using Smartwyre.DeveloperTest.Interfaces;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public class FixedRateRebateStrategy : IRebateIncentiveStrategy
    {
        public decimal CalculateRebate(Rebate rebate, Product product, CalculateRebateResult result, CalculateRebateRequest request)
        {
            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate) ||
                rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
            {
                result.Success = false;
                return 0m;
            }

            result.Success = true;
            return product.Price * rebate.Percentage * request.Volume;
        }
    }
}

