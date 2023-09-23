using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Interfaces;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private IRebateDataStore _rebateDataStore;
    private IProductDataStore _productDataStore;
    private readonly Dictionary<IncentiveType, IRebateIncentiveStrategy> _rebateIncentiveStrategy = new()
    {
        { IncentiveType.FixedCashAmount, new FixedCashAmountStrategy() },
        { IncentiveType.FixedRateRebate, new FixedRateRebateStrategy() },
        { IncentiveType.AmountPerUom, new AmountPerUomStrategy() }
    };

    public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore)
    {
        _rebateDataStore = rebateDataStore ?? throw new ArgumentNullException(nameof(rebateDataStore));
        _productDataStore = productDataStore ?? throw new ArgumentNullException(nameof(productDataStore));
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult() { Success = false };
        var rebateAmount = 0m;

        if (rebate == null || product == null)
            return result;

        if (_rebateIncentiveStrategy.TryGetValue(rebate.Incentive, out IRebateIncentiveStrategy rebateStrategy))
        {
            rebateAmount = rebateStrategy.CalculateRebate(rebate, product, result, request);
        }

        if (result.Success)
        {
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        }

        return result;
    }
}
