using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{

    [Authorize(Roles = "Administrator")] //Authorize will make sure users that are not logged in cannot be accessed unless logged in. Roles specfies which user roles can enter.
    public class StoreManagerController : Controller
    {
        private MusicStoreModel db = new MusicStoreModel();
        // GET: StoreManager
        public ActionResult Index()
        {
            var albums = db.Albums.Include(a => a.Artist).Include(a => a.Genre);
            ViewBag.AlbumCount = albums.Count();
            return View(albums.ToList().OrderBy(a => a.Artist.Name).ThenBy(a => a.Title));
        }

        // POST: StoreManager - search by title
        [HttpPost] //This specifies a POST over GET request
        [ValidateAntiForgeryToken] //Validate Anti-forgery token, to prevent cross-site scripting
        public ActionResult Index(string Title)
        {
            //Return only albums that contain the keyword(s) in the title
            var albums = from a in db.Albums
                         where a.Title.Contains(Title)
                         orderby a.Artist.Name, a.Title
                         select a;

            ViewBag.AlbumCount = albums.Count();
            ViewBag.SearchTerm = Title;

            return View(albums);
        }

        // GET: StoreManager/Details/5
        [AllowAnonymous] //This allows anonymous users to access just this one
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // GET: StoreManager/Create
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(db.Artists.OrderBy(a => a.Name), "ArtistId", "Name");
            ViewBag.GenreId = new SelectList(db.Genres.OrderBy(g => g.Name), "GenreId", "Name");
            return View();
        }

        // POST: StoreManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AlbumId,GenreId,ArtistId,Title,Price,AlbumArtUrl")] Album album)
        {
            if (ModelState.IsValid)
            {
                //Save album cover if there is one
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    //If this is a legit file, save it
                    if (file.FileName != null && file.ContentLength > 0 && file.ContentLength <= 2048000)
                    {
                        //This is the dynamic path that we want to save the image file
                        string path = Server.MapPath("/Content/Images/") + file.FileName;

                        //Save file into directory on server
                        file.SaveAs(path);

                        //Add new path to the database
                        album.AlbumArtUrl = "/Content/Images/" + file.FileName;
                    }
                }

                db.Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            return View(album);
        }

        // GET: StoreManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtistId = new SelectList(db.Artists.OrderBy(a => a.Name), "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(db.Genres.OrderBy(g => g.Name), "GenreId", "Name", album.GenreId);
            return View(album);
        }

        // POST: StoreManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        //We are binding our form inputs to the model
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AlbumId,GenreId,ArtistId,Title,Price,AlbumArtUrl")] Album album)
        {
            if (ModelState.IsValid)
            {
                //Save album cover if there is one
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    //If this is a legit file, save it
                    if (file.FileName != null && file.ContentLength > 0)
                    {
                        //This is the dynamic path that we want to save the image file
                        string path = Server.MapPath("/Content/Images/") + file.FileName;

                        //Save file into directory on server
                        file.SaveAs(path);

                        //Add new path to the database
                        album.AlbumArtUrl = "/Content/Images/" + file.FileName;
                    }
                }

                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            return View(album);
        }

        // GET: StoreManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: StoreManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
            db.Albums.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
