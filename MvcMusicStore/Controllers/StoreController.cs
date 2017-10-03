using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        // db connection
        MusicStoreModel db = new MusicStoreModel();

        // GET: Store
        public ActionResult Index()
        {

            //var genres = new List<Genre>();

            ////Loop through the 10 genres ex) Genre 1
            //for(int i = 1; i <= 10; i++)
            //{
            //    genres.Add(new Genre { Name = "Genre " + i.ToString() });
            //}

            // give the list to the view with ViewBag
            //ViewBag.genres = genres;

            //Get Genres from Database with Genre Model, Sorted by Name
            var genres = db.Genres.ToList().OrderBy(g => g.Name);


            //pass genre list as a parameter to the view
            return View(genres);
        }

        //GET: Browse
        public ActionResult Browse(string genre)
        {
            //Grab Selected Genre from DB, and include the related Albums where gn.name == genre
            var g = db.Genres.Include("Albums").SingleOrDefault(gn => gn.Name == genre);

            //add selected genre to the viewbag, so we can display in browse view
            //ViewBag.genre = genre;

            return View(g);
        }
    }
}