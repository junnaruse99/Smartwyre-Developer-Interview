using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Interfaces
{
    public interface IProductDataStore
    {
        Product GetProduct(string productIdentifier);
    }
}

