using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;

namespace MyApp.Namespace
{
    [Route("api/products")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 

        public ProductApiController(ApplicationDbContext context) 
        {
            _context = context; 
        }

        //GET anrop för att kunna hämta data från db som json
        [HttpGet]
        public async Task<IActionResult> GetProducts() 
        {
            if(_context.Products == null)
            {
                return NotFound(); 
            }

            return Ok(await _context.Products.ToListAsync());
        }

        //GET anrop för en specifik produkt med id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id) 
        {
            var product = await _context.Products.FindAsync(id); 

            if(product == null)
            {
                return NotFound(); 
            }

            return Ok(product); 
        }
    }
}
