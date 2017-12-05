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
    [Authorize]
    public class OrdersController : Controller
    {
        private MusicStoreModel db = new MusicStoreModel();

        // GET: Orders
        public ActionResult Index()
        {
            //Filter orders by account that is logged in
            var orders = db.Orders.Where(o => o.Username == User.Identity.Name);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            //Make sure the current user is the owner of the order
            if (order.Username == User.Identity.Name)
            {
                return View(order);
            }

            else
            {
                return HttpNotFound();
            }
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
