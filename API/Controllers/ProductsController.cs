using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IRepository<Product> _productsRepo;
    private readonly IRepository<ProductBrand> _productBrandRepo;
    private readonly IRepository<ProductType> _productTypeRepo;
    private readonly IMapper _mapper;

    public ProductsController(IRepository<Product> productsRepo, IRepository<ProductBrand> productBrandRepo, IRepository<ProductType> productTypeRepo,
     IMapper mapper)
    {
        _productsRepo = productsRepo;
        _productBrandRepo = productBrandRepo;
        _productTypeRepo = productTypeRepo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecificationParams productParams)
    {
        var specifications = new ProductsWithTypesAndBrandsSpecification(productParams);

        var countSpecification = new ProductWithFiltersForCountSpecification(productParams);

        var totalItems = await _productsRepo.CountAsync(countSpecification);

        var products = await _productsRepo.ListAsync(specifications);

        var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

        return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
        var specifications = new ProductsWithTypesAndBrandsSpecification(id);
        var product = await _productsRepo.GetEntityWithSpecification(specifications);

        if (product == null) return NotFound(new ApiResponse(404));

        return _mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
        return Ok(await _productBrandRepo.ListAllAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
    {
        return Ok(await _productTypeRepo.ListAllAsync());
    }
}