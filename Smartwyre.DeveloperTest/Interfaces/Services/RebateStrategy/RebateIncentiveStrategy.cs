using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Interfaces
{
    public interface IRebateIncentiveStrategy
    {
        decimal CalculateRebate(Rebate rebate, Product product, CalculateRebateResult result, CalculateRebateRequest request);
    }
}

