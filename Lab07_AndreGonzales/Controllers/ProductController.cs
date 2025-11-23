using Lab07_AndreGonzales.DTOs;
using Microsoft.AspNetCore.Mvc;
using Lab07_AndreGonzales.DTOs;
    
namespace Lab07_AndreGonzales.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    [HttpPost("error")]
    public IActionResult CreateProductError()
    {
        throw new Exception("Error de prueba forzado");
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductDto product)
    {
        return Ok(new { message = "Producto creado exitosamente.", product });
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(new[] { new { Id = 1, Name = "Laptop", Price = 2500 } });
    }
}