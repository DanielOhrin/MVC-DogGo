using System;
using System.Collections.Generic;
using System.Linq;

using DogGo.Interfaces;
using DogGo.Models;
using DogGo.Models.ViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DogGo.Controllers
{
    public class WalkController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalkRepository _walkRepo;
        private readonly IDogRepository _dogRepo;
        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkController(IWalkerRepository walkerRepository, IWalkRepository walkRepository, IDogRepository dogRepo)
        {
            _walkerRepo = walkerRepository;
            _walkRepo = walkRepository;
            _dogRepo = dogRepo;
        }
        
        //TODO: Learn more about IFormCollection and asp-for. Fix bug when Submitting form in Create.cshtml (this controller)

        // GET: WalkersController
        public ActionResult Index()
        {
            return View();
        }

        // GET: WalkersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {
            List<Dog> dogs = _dogRepo.GetAllDogs();
            List<Walker> walkers = _walkerRepo.GetAllWalkers();

            CreateWalkViewModel vm = new()
            {
                Dogs = dogs,
                Walkers = walkers
            };

            return View(vm);
        }

        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                List<int> dogIds = collection["DogIds"].Select(x => Convert.ToInt32(x)).ToList();

                foreach(int dogId in dogIds)
                {
                    Walk walk = new()
                    {
                        WalkerId = int.Parse(collection["Walk.WalkerId"]),
                        Date = DateTime.Parse(collection["Walker.Date"]),
                        Duration = int.Parse(collection["Walker.Duration"]),
                        DogId = dogId
                    };

                    _walkRepo.CreateWalk(walk);
                }

                return RedirectToAction(nameof(Index), "Walkers");
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}