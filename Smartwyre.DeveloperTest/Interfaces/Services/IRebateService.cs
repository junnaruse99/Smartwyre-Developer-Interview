using Smartwyre.DeveloperTest.Interfaces;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public interface IRebateService
{
    CalculateRebateResult Calculate(CalculateRebateRequest request);
}
