﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_page.Data;

namespace Restaurant_page.Pages
{
    [Authorize(Roles = "Admin")]
    public class EditMenuItemModel : PageModel
    {

        [BindProperty]
        public MenuItem MenuItem { get; set; }

        private readonly AppDbContext _db;
        private readonly IHostingEnvironment _he;
        public IList<MenuItem> MenuItems;
        public string CurrentImage;
        [BindProperty]
        public IFormFile ItemImage { get; set; }
        public EditMenuItemModel(AppDbContext db, IHostingEnvironment he) {
            _db = db;
            _he = he;
                }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            MenuItem = await _db.MenuItems.FindAsync(id);

            if (MenuItem == null)
            {
                return RedirectToPage("/Admin/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ItemImage != null)
            {
                var filename = Path.Combine(_he.WebRootPath, "images", Path.GetFileName(ItemImage.FileName));
                MenuItem.Image = Path.Combine("images", Path.GetFileName(ItemImage.FileName));
                ItemImage.CopyTo(new FileStream(filename, FileMode.Create));

                //the rootpath used for images uses backslash, this logic swaps those to forward slashes for HTML validation purposes.
                string imagePath = "";
                foreach (var letter in MenuItem.Image)
                {
                    var letterToUse = 'a';
                    if (letter == '\\')
                    {
                        letterToUse = '/';
                    }
                    else
                    {
                        letterToUse = letter;
                    }
                    imagePath += letterToUse;

                }
                MenuItem.Image = imagePath;
            } else
            {
                //logic to ensure editing menu item doesn't delete picture when no picture selected.
                MenuItems = _db.MenuItems.AsNoTracking().FromSql("SELECT top 1 * FROM MenuItems WHERE MenuID = {0}", MenuItem.MenuID).ToList();
                MenuItem.Image = MenuItems[0].Image;
                
            }

            _db.Attach(MenuItem).State = EntityState.Modified;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new Exception($"MenuItem {MenuItem.MenuID} not found!", e);
            }
            return RedirectToPage("/Admin/Index");
        }
    }
}