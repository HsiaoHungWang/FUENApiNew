using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FUENApi.Models;
using FUENApi.Models.DTO;

namespace FUENApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductsController : ControllerBase
    {
        private readonly iSpan202301Context _context;

        public ProductsController(iSpan202301Context context)
        {
            _context = context;
        }

        //public ProductsController()
        //{
        //    _context = new iSpan202301Context();
        //}


        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<ProductsDTO>> GetProducts(string? keyword, int page = 1, int pageSize = 3)
        {
          if (_context.Products == null)
          {
              return NotFound();
          }

            //讀出所有資料
            var products = _context.Products.AsQueryable();

            //產品名稱搜尋
            if(!string.IsNullOrEmpty(keyword)) {
                products = products.Where(p => p.Name.Contains(keyword));
            }

            //分頁
            int totalCount = products.Count(); //總共幾筆 10
            //int pageSize = 3;  //每頁3筆資料
            int totalPages =(int) Math.Ceiling(totalCount / (double)pageSize); //計算出共幾頁 4
            //int page = 2; //顯示第幾頁

            products = products.Skip(pageSize * (page-1)).Take(pageSize);  
            //page = 0 * 3  take 1,2,3
            //page=1 * 3 take 4,5,6
            //page=2 * 3 take 7,8,9


            ProductsDTO productsDTO = new ProductsDTO();
            productsDTO.Products = await products.ToListAsync();
            productsDTO.TotalPages = totalPages;


            return productsDTO;

           // return await products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProducts(int id)
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            var products = await _context.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            return products;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducts(int id, Products products)
        {
            if (id != products.Id)
            {
                return BadRequest();
            }

            _context.Entry(products).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Products>> PostProducts(Products products)
        {
          if (_context.Products == null)
          {
              return Problem("Entity set 'iSpan202301Context.Products'  is null.");
          }
            _context.Products.Add(products);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts", new { id = products.Id }, products);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducts(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductsExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
