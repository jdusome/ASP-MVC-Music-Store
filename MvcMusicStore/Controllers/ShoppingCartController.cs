using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.Models; //Reference Models

namespace MvcMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        //DB Connection
        private MusicStoreModel db = new MusicStoreModel();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            // Get current cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // set up ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            //Pass the cart to the index view
            return View(viewModel);
        }

        //GET:  AddToCart/300
        public ActionResult AddToCart(int Id)
        {
            //get the current cart if any
            var cart = ShoppingCart.GetCart(this.HttpContext);

            //add the current Album to the cart
            cart.AddToCart(Id);

            //redirect to Index which will display the current cart
            return RedirectToAction("Index");
        }

        //Get: RemoveFromCart/300
        public ActionResult RemoveFromCart(int Id)
        {
            //Get the current cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            //Remove the current Album from the cart
            cart.RemoveFromCart(Id);

            //Redirect to Index to show the updated cart
            return RedirectToAction("Index");
        }

        [Authorize]
        // GET: Checkout
        public ActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] //We validate this any time we post
        //POST: Checkout
        public ActionResult Checkout(FormCollection values) //FormCollection will include all values on the form
        {
            //create a new order and populate the fields from the form
            var order = new Order();

            //This method will automatically map the values coming in to the model
            TryUpdateModel(order); 

            //check the PromoCode for a value of "FREE"
            if(values["PromoCode"] != "FREE")
            {
                ViewBag.Message = "Use Promo Code FREE";

                //Pass the view back, with the order (This will automatically repopulate the view)
                return View(order);
            }

            else
            {
                //Promo Code is good, so save the order

                //auto-fill the username, email, date, and total
                order.Username = User.Identity.Name;
                order.Email = User.Identity.Name;
                order.OrderDate = DateTime.Now;

                var cart = ShoppingCart.GetCart(this.HttpContext);
                order.Total = cart.GetTotal();

                //Save Order to the database
                db.Orders.Add(order);
                db.SaveChanges();

                //Save Order details from cart
                var cartItems = cart.GetCartItems();

                foreach(Cart item in cartItems)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.AlbumId = item.AlbumId;
                    orderDetail.Quantity = item.Count;
                    orderDetail.UnitPrice = item.Album.Price;
                    db.OrderDetails.Add(orderDetail);
                }

                db.SaveChanges();

                //Empty the users cart
                cart.EmptyCart();
            }

            //Redirect to Order Details
            return RedirectToAction("Details", "Orders", new { id = order.OrderId });
        }
    }
}