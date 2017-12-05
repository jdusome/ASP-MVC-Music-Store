using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcMusicStore.Models
{
    public class ShoppingCart
    {
        //Connect to the DB
        MusicStoreModel db = new MusicStoreModel();

        //String for unique cart Id
        string ShoppingCartId { get; set; }

        //Get current Cart Contents
        //HttpContextBase is used to identify the user and other context of the web
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();

            //Check session to see if we alredy have Cart Id
            if (context.Session["CartId"] == null)
            {
                //If we have no current cart in the session, check it user is logged in
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    //Label CartId as username
                    context.Session["CartId"] = context.User.Identity.Name;
                }
                else
                {
                    //User is anonymous, generate unique cart Id
                    Guid tempCartId = Guid.NewGuid();
                    context.Session["CartId"] = tempCartId;
                }
            }

            cart.ShoppingCartId = context.Session["CartId"].ToString();
            
            return cart;
        }

        /**
         * This method is used to add an item to th cart
         */
        public void AddToCart(int Id)
        {
            //Is this alum already in the cart
            var cartItem = db.Carts.SingleOrDefault(a => a.AlbumId == Id && a.CartId == ShoppingCartId);

            //If item is not already in cart
            if (cartItem == null)
            {
                //Create a new Cart Object
                cartItem = new Cart
                {
                    AlbumId = Id,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };

                db.Carts.Add(cartItem);
            }

            else
            {
                //Add 1 to the current count of this album
                cartItem.Count++;
            }

            //Save the new Cart object to the database
            db.SaveChanges();
        }

        //Remove Item from the cart
        public void RemoveFromCart(int id)
        {
            //Get the selected album from this shopping cart (for this user)
            var item = db.Carts.SingleOrDefault(c => c.AlbumId == id && c.CartId == ShoppingCartId);

            if (item != null)
            {
                //If quantity is 1, delete the item
                if (item.Count == 1)
                {
                    db.Carts.Remove(item);
                }

                //If quantity is > 1, subtract 1 from quantity
                else
                {
                    item.Count--;
                }

                db.SaveChanges();
            }
        }

        //Get Items in the Cart
        public List<Cart> GetCartItems()
        {
            return db.Carts.Where(c => c.CartId == ShoppingCartId).ToList();
        }

        //Get Cart Total
        public decimal GetTotal()
        {
            //Get albums for Cart
            //Calculate Total for Eac (count * price)
            //Sum all lines together for the total
            decimal? total = (from c in db.Carts
                              where c.CartId == ShoppingCartId
                              select (int?)c.Count * c.Album.Price).Sum();

            return total ?? decimal.Zero;
        }

        //Empty Cart
        public void EmptyCart()
        {
            //Get all items in the cart table for the current user
            var cartItems = db.Carts.Where(c => c.CartId == ShoppingCartId);

            //Remove all the items from the database in the cart
            foreach (Cart item in cartItems)
            {
                db.Carts.Remove(item);
            }

            db.SaveChanges();
        }
    }
}