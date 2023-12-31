﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Helper;
using WorldLibrary.Web.Models;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers
{
    public class PhysicalLibrariesController : Controller
    {
        private readonly IPhysicalLibraryRepository _physicalLibraryRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        public PhysicalLibrariesController(IPhysicalLibraryRepository physicalLibraryRepository,
            IUserHelper userHelper,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper)
        {

            _physicalLibraryRepository = physicalLibraryRepository;
            _userHelper = userHelper;
           _blobHelper = blobHelper;
            _converterHelper=converterHelper;
        }

        // GET: PhysicalLibraries
        public IActionResult Index()
        {
            return View(_physicalLibraryRepository.GetAll().OrderBy(p => p.Name));
        }

        // GET: PhysicalLibraries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("LibraryNotFound");
            }

            var physicalLibrary = await _physicalLibraryRepository.GetByIdAsync(id.Value);
            if (physicalLibrary == null)
            {
                return new NotFoundViewResult("LibraryNotFound");
            }

            return View(physicalLibrary);
        }

        // GET: PhysicalLibraries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PhysicalLibraries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhysicalLibraryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageid = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageid =  await _blobHelper.UploadBlobAsync(model.ImageFile, "libraries");
                }

                var physicalLibrary = _converterHelper.ToLibrary(model, imageid, true);

                physicalLibrary.User = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel.pt");
                await _physicalLibraryRepository.CreateAsync(physicalLibrary);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

       
        // GET: PhysicalLibraries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("LibraryNotFound");
            }

            var physicalLibrary = await _physicalLibraryRepository.GetByIdAsync(id.Value);
            if (physicalLibrary == null)
            {
                return new NotFoundViewResult("LibraryNotFound");
            }

            var model = _converterHelper.ToPhysicalLibraryViewModel(physicalLibrary);
            return View(model);
        }
                

        // POST: PhysicalLibraries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PhysicalLibraryViewModel model)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageid = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageid =  await _blobHelper.UploadBlobAsync(model.ImageFile, "libraries");
                    }

                    var physicalLibrary = _converterHelper.ToLibrary(model, imageid, false);

                    physicalLibrary.User = await _userHelper.GetUserByEmailAsync("evelyn.nunes@cinel.pt");
                    await _physicalLibraryRepository.UpdateAsync(physicalLibrary);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _physicalLibraryRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("LibraryNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: PhysicalLibraries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("LibraryNotFound");
            }

            var physicalLibrary = await _physicalLibraryRepository.GetByIdAsync(id.Value);
            if (physicalLibrary == null)
            {
                return new NotFoundViewResult("LibraryNotFound");
            }

            return View(physicalLibrary);
        }

        // POST: PhysicalLibraries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var physicalLibrary = await _physicalLibraryRepository.GetByIdAsync(id);
            try
            {
                await _physicalLibraryRepository.DeleteAsync(physicalLibrary);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{physicalLibrary.Name} probably in been used!!!";
                    ViewBag.ErrorMessage = $"{physicalLibrary.Name} can not be deleted because there are reserves in this library.</br></br>" +
                        $"First delete all the reserves in this library" +
                        $" and please try again delete it!";

                }
                return View("Error");
            }
        }

        public IActionResult LibraryNotFound()
        {
            return View();
        }
    }
}
