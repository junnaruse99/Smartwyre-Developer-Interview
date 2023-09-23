using System;
using Moq;
using Smartwyre.DeveloperTest.Interfaces;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    private Mock<IRebateDataStore> _rebateDataStoreMock;
    private Mock<IProductDataStore> _productDataStoreMock;
    private CalculateRebateRequest _request;
    private Rebate _rebate;
    private Product _product;

    public RebateServiceTests()
    {
        _rebateDataStoreMock = new Mock<IRebateDataStore>();
        _productDataStoreMock = new Mock<IProductDataStore>();
        _request = new() { RebateIdentifier = "", ProductIdentifier = "", Volume = 100 };
        _rebate = new() { Amount = 100, Identifier = "0", Incentive = IncentiveType.FixedCashAmount, Percentage = 100 };
        _product = new() { Id = 0, Identifier = "0", Price = 100, SupportedIncentives = SupportedIncentiveType.FixedCashAmount, Uom = "0" };
    }

    [Fact]
    public void Calculate_WhenRebateOrProductIsNull_ReturnFailed()
    {
        // Stage
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => null);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => null);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // DO
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsFixedCashAmountAndProductDoesNotSupportIncentive_ReturnFailed()
    {
        // Stage
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _product.SupportedIncentives = SupportedIncentiveType.AmountPerUom;
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsFixedCashAmountAndProductSupportsIncentiveAndRebateAmountIs0_ReturnFailed()
    {
        // Stage
        _rebate.Amount = 0;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsFixedCashAmountAndProductSupportsIncentive_RebateAmountIsStored()
    {
        // Stage
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        _rebateDataStoreMock.Verify(
            x => x.StoreCalculationResult(_rebate, _rebate.Amount),
            Times.Once
        );
    }

    [Fact]
    public void Calculate_WhenRebateIsFixedRateAndProductDoesNotSupportsIncentive_ReturnsFailed()
    {
        // Stage
        _rebate.Incentive = IncentiveType.FixedRateRebate;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsFixedRateAndProductSupportsIncentiveAndPercentageIs0_ReturnsFailed()
    {
        // Stage
        _rebate.Incentive = IncentiveType.FixedRateRebate;
        _rebate.Percentage = 0;
        _product.SupportedIncentives = SupportedIncentiveType.FixedRateRebate;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsFixedRateAndProductSupportsIncentiveAndPriceIs0_ReturnsFailed()
    {
        // Stage
        _rebate.Incentive = IncentiveType.FixedRateRebate;
        _product.SupportedIncentives = SupportedIncentiveType.FixedRateRebate;
        _product.Price = 0;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsFixedRateAndProductSupportsIncentiveAndVolumeIs0_ReturnsFailed()
    {
        // Stage
        _rebate.Incentive = IncentiveType.FixedRateRebate;
        _product.SupportedIncentives = SupportedIncentiveType.FixedRateRebate;
        _request.Volume = 0;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsFixedRateAndProductSupportsIncentiveAndConditionsAreMet_RebateAmountIsStored()
    {
        // Stage
        _rebate.Incentive = IncentiveType.FixedRateRebate;
        _product.SupportedIncentives = SupportedIncentiveType.FixedRateRebate;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        _rebateDataStoreMock.Verify(
            x => x.StoreCalculationResult(_rebate, _rebate.Percentage * _product.Price * _request.Volume),
            Times.Once
        );
    }

    [Fact]
    public void Calculate_WhenRebateIsAmountPerUomAndDoesNotSupportIncentive_ReturnsFailed()
    {
        // Stage
        _rebate.Incentive = IncentiveType.AmountPerUom;
        _product.SupportedIncentives = SupportedIncentiveType.FixedRateRebate;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsAmountPerUomAndSupportsIncentiveAndRebateAmountIs0_ReturnsFailed()
    {
        // Stage
        _rebate.Incentive = IncentiveType.AmountPerUom;
        _product.SupportedIncentives = SupportedIncentiveType.AmountPerUom;
        _rebate.Amount = 0;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsAmountPerUomAndSupportsIncentiveAndVolumeIs0_ReturnsFailed()
    {
        // Stage
        _rebate.Incentive = IncentiveType.AmountPerUom;
        _product.SupportedIncentives = SupportedIncentiveType.AmountPerUom;
        _request.Volume = 0;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_WhenRebateIsAmountPerUomAndSupportsIncentiveAndConditionsAreMet_RebateAmountIsStored()
    {
        // Stage
        _rebate.Incentive = IncentiveType.AmountPerUom;
        _product.SupportedIncentives = SupportedIncentiveType.AmountPerUom;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        _rebateDataStoreMock.Verify(
            x => x.StoreCalculationResult(_rebate, _rebate.Amount * _request.Volume),
            Times.Once
        );
    }

    [Fact]
    public void Calculate_WhenRebateIsFailed_DoNotStore()
    {
        // Stage
        _rebate.Incentive = IncentiveType.AmountPerUom;
        _product.SupportedIncentives = SupportedIncentiveType.FixedCashAmount;
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((string rebateId) => _rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((string productId) => _product);
        RebateService rebateService = new(_rebateDataStoreMock.Object, _productDataStoreMock.Object);

        // Act
        var result = rebateService.Calculate(_request);

        // Assert
        _rebateDataStoreMock.Verify(
            x => x.StoreCalculationResult(_rebate, _rebate.Amount * _request.Volume),
            Times.Never
        );
    }
}

