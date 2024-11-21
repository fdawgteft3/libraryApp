using LibraryApp.Models;
using LibraryApp.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    public class PaymentCartController : Controller
    {
        private readonly S22024Group4ProjectContext _projectContext;

        public PaymentCartController(S22024Group4ProjectContext projectContext)
        {
            _projectContext = projectContext;
        }

        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            return View(cartItems);

        }
        public IActionResult AddToCart(int Id)
        {
            if (_projectContext.BookCopies.Where(bc => bc.BookId == Id) == null)
            {
                return View("Book");
            }
            // Fetch BookCopies into memory
            var books = _projectContext.Books.Where(bc => bc.BookId == Id).ToList();

            // Calculate stock
            int Stock = books.Count;

            
            var book = _projectContext.Books.FirstOrDefault(p => p.BookId == Id);
            

            if (book != null)
            {
                // Check if there's enough stock
                if (Stock > 0)
                {
                    var cartItems = GetCartItems(); // Get cart items from session or elsewhere
                    var cartItem = cartItems.FirstOrDefault(item => item.book.BookId == Id);
                    
                    if (cartItem != null)
                    {
                        // Increase quantity if the product is already in the cart
                        cartItem.Quantity++;
                    }
                    else
                    {
                        // Add new product to the cart
                        cartItems.Add(new CartItem { book = book, Quantity = 1 });
                        
                    }

                    // Save cart items and update stock
                    SaveCartItems(cartItems); // Update your cart in the session
                    Stock--; // Decrease stock by 1
                    _projectContext.SaveChanges(); // Save changes to the database
                }
                else
                {
                    // If no stock, show a message (Optional)
                    TempData["ErrorMessage"] = "Sorry, this product is out of stock!";
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int Id)
        {

            // Fetch BookCopies into memory
            var books = _projectContext.Books.Where(bc => bc.BookId == Id).ToList();

            // Calculate stock
            int Stock = books.Count;

            var cartItems = GetCartItems(); // Get cart items from session or elsewhere
            var cartItem = cartItems.FirstOrDefault(item => item.book.BookId == Id);

            if (cartItem != null)
            {
                // Increase the stock of the product since it's being removed from the cart
                var book = _projectContext.Books.FirstOrDefault(p => p.BookId == Id);
                if (book != null)
                {
                    Stock += cartItem.Quantity; // Add back the quantity to stock
                    _projectContext.SaveChanges(); // Save changes to the database
                }

                // Remove item from cart
                cartItems.Remove(cartItem);
                SaveCartItems(cartItems); // Save the updated cart in the session
            }

            return RedirectToAction("Index"); // Or wherever you want to redirect
        }


        public IActionResult ClearCart()
        {
            SaveCartItems(new List<CartItem>());
            return RedirectToAction("Index");
        }

        private List<CartItem> GetCartItems()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>("CartItems");
            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
            }
            return cartItems;
        }
        private void SaveCartItems(List<CartItem> cartItems)
        {
            HttpContext.Session.Set("CartItems", cartItems); // Update your cart in the session

        }
    }
}
