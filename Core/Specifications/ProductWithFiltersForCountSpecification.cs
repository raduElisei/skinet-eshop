using Core.Entities;

namespace Core.Specifications;

public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
{
    public ProductWithFiltersForCountSpecification(ProductSpecificationParams productParams)
        : base(x =>
            (!productParams.BrandId.HasValue || x.ProductBrandId == productParams.BrandId) &&
            (!productParams.TypeId.HasValue || x.ProductTypeId == productParams.TypeId)
        )
    {
    }
}