using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;

namespace projektdotnet.Controllers
{
    //måste vara inloggad för att kunna ändra/lägga till mm
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryModelId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,CategoryModelId,ImageFile")] ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                if(productModel.ImageFile != null)
                {
                    //sparar bilden 
                    var fileName = Path.GetFileNameWithoutExtension(productModel.ImageFile.FileName);
                    var extension = Path.GetExtension(productModel.ImageFile.FileName);
                    productModel.ImageName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productModel.ImageName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await productModel.ImageFile.CopyToAsync(fileStream);
                    }
                }

                _context.Add(productModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryModelId"] = new SelectList(_context.Categories, "Id", "Name", productModel.CategoryModelId);
            return View(productModel);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }
            ViewData["CategoryModelId"] = new SelectList(_context.Categories, "Id", "Name", productModel.CategoryModelId);
            return View(productModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,CategoryModelId,ImageFile,ImageName")] ProductModel productModel)
        {
            if (id != productModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (productModel.ImageFile != null) 
                    {
                        //sparar bild, tar bort gamla ifall att
                        var fileName = Path.GetFileNameWithoutExtension(productModel.ImageFile.FileName);
                        var extension = Path.GetExtension(productModel.ImageFile.FileName);
                        var newImageName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", newImageName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await productModel.ImageFile.CopyToAsync(stream);
                        }

                        if (!string.IsNullOrEmpty(productModel.ImageName))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productModel.ImageName);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        //uppdaterar med nya filen
                        productModel.ImageName = newImageName;
                        
                    }

                    _context.Update(productModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductModelExists(productModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryModelId"] = new SelectList(_context.Categories, "Id", "Name", productModel.CategoryModelId);
            return View(productModel);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productModel = await _context.Products.FindAsync(id);
            if (productModel != null)
            {
                _context.Products.Remove(productModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductModelExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
