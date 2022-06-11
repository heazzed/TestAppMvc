using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppMvc.Models;
using AppMvc.Util;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using AppMvc.Filters;
using AppMvc.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace AppMvc.Controllers
{
    public class HomeController : Controller
    {
        // DI Logger

        //private readonly ILogger<HomeController> _logger;
        //
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        // DI WebHost to test work with files

        private readonly WebRepository webRepository;

        private WebContext _context;

        public void DoDb()
        {
            //_context.
        }

        private readonly IWebHostEnvironment host;

        public HomeController(IWebHostEnvironment webHost, WebContext context, WebRepository webRepository)
        {
            host = webHost;
            _context = context;
            this.webRepository = webRepository;
        }

        public async Task<IActionResult> GetUsersAsync()
        {
            IEnumerable<User> model = await webRepository.GetUsersAsync();
            return View("GetUsers", model);
        }

        public IActionResult GetUserById(int id)
        {
            User user = webRepository.GetUserById(id);

            if (user.Password == null && user.Company == null)
            { 
                user.Password = "1234";
                user.Company = "Google";
            }
            user.Id = 3;

            _context.Users.Update(user);
            _context.SaveChanges();

            return Content($"Ok! user {user.Name} now has password {user.Password} and company {user.Company}");
        }

        public async Task<IActionResult> GetInfoAboutUsersAsync()
        {
            IEnumerable<User> users = await webRepository.GetUsersAsync();
            int sum = users.Count(); // LINQ agregate functions
            int ages = users.Sum(x => x.Age);                   // agregate functions

            int minAge = users.Min(x => x.Age);                   // agregate functions
            int maxAge = users.Max(x => x.Age);                   // agregate functions
            double avgAge = users.Average(x => x.Age);                  // agregate functions

            bool allMic = users.All(x => x.Company == "Microsoft");     // agregate functions
            bool anyMic = users.Any(x => x.Company == "Microsoft");     // agregate functions

            return Content($"Users count {sum} \nUsers ages sum {ages} \n" +
                $"Min age = {minAge} \nMax age = {maxAge} \nAverage age is {avgAge} \n" +
                $"All users from Microsoft?  {allMic}\nAny Users from Microsoft? {anyMic}");
        }

        // Connect to DB
        public WebContext connectToDB(){ // Make to DbContextOptions service in services
            var optionsBuilder = new DbContextOptionsBuilder<WebContext>();

            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());

            var config = builder.Build();

            string connectionString = config.GetConnectionString("DefaultConnection");

            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;

            WebContext context = new WebContext(options); // need keyword "using" to dispose
                                                            // not correct way to connect to DB

            return context;
        }

        // View Results


        //[FakeNotFoundResourceFilter]
        //[AsyncActionFilter]
        //[AsyncResultFilter]
        //[ExceptionFilter]
        //[Authorize]
        public IActionResult Index()
        {
            return View();
            //return Content(User.Identity.Name);
            //return Content("id = " + id);
        }

        [Authorize]
        public IActionResult SomePage()
        {
            return Content(User.Identity.Name);
        }

        public IActionResult Privacy()
        {
            return View();
            //return RedirectToAction("Area", "Home", new { width = 9, height = 5 });
        }

        [Authorize(Policy = "OnlyForMicrosoft")]
        public IActionResult About() 
        {
            return View();
            //return Redirect("~/Home/GetHtml");
        }

        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult AddOrder()
        {

            User user = new User()
            {
                Name = "Rick",
                Age = 25,
                Email = "rick@email.com"
            };

            _context.Users.Add(user); // CREATE

            Product product = new Product()
            {
                Name = "Shawerma",
                Price = 9
            };

            _context.Products.Add(product); // CREATE

            Order order = new Order()
            {
                User = user,
                Product = product
            };

            _context.Orders.Add(order); // CREATE

            _context.SaveChanges();

            return Content($"OK! Order № {order.Id} User {order.User.Name} added {order.Product.Name}. ");
        }

        public IActionResult GetOrders()
        {

            string orders = "";

            var joinUsers = _context.Orders.Join(_context.Users, // join users
                o => o.UserId,
                u => u.Id,
                (o, u) => new
                {
                    o.Id,
                    o.UserId,
                    o.ProductId,
                    UserName = u.Name,
                    UserAge = u.Age,
                    UserEmail = u.Email
                });

            var joinOrders = from order in _context.Orders // join users and products on orders
                             join users in _context.Users on order.UserId equals users.Id
                             join products in _context.Products on order.ProductId equals products.Id
                             select new
                             {
                                 order.Id,
                                 order.UserId,
                                 order.ProductId,
                                 UserName = users.Name,
                                 UserAge = users.Age,
                                 UserEmail = users.Email,
                                 ProductName = products.Name,
                                 ProductPrice = products.Price
                             };



            foreach (var item in joinOrders)
            {
                orders += item.Id;
                orders += " ";
                orders += item.UserId;
                orders += " ";
                orders += item.UserName;
                orders += " ";
                orders += item.ProductId;
                orders += " ";
                orders += item.ProductName;
                orders += "\n";
            }

            return Content(orders);
        }

       

        public IActionResult GetProducts()
        {
            var someProduct = _context.Products.Where(p => p.Name == "Burger").FirstOrDefault(); // READ

            if(someProduct is Product product)
            {
                product.Price = 11; // UPDATE
                //context.Update(product) -- // UPDATE

                //context.Remove(product); -- // REMOVE
            }

            _context.SaveChanges();

            string products = "";

            foreach (var item in _context.Products.Where(p => p.Price >= 10).OrderBy(p=>p.Name))
            {
                products += item.Id;
                products += " ";
                products += item.Name;
                products += " ";
                products += item.Price;
                products += "\n";
            }

            return Content(products);
        }

        //public IActionResult GetUsers()
        //{

        //    string users = "";

        //    foreach (var item in _context.Users)
        //    {
        //        users += item.Id;
        //        users += " ";
        //        users += item.Name;
        //        users += " ";
        //        users += item.Age;
        //        users += " ";
        //        users += item.Email;
        //        users += "\n";
        //    }

        //    return Content(users);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Error Handler
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        public string GetId(int id) // string result
        {
            return "Hi! Your ID is " + id;
        }

        public string GetSquareArea(int height, int width) // String result with params
        {
            return "Square area is " + width * height;
        }

        public HtmlResult GetHtml() // Get Result Html-page
        {
            return new HtmlResult("<h2>Hi, ASP.NET Core</h2>");
            
        }

        public IActionResult Area(int height, int width) // ContentResult
        {
            double area = width * height;
            return Content("Square area is " + area);
        }

        //public JsonResult GetUser() // JsonResult
        //{
        //    User mike = new User("Mike", "19");
        //    return Json(mike);
        //}

        public IActionResult GetStatusCode()
        {
            return StatusCode(403);
        }

        public IActionResult GetFile()
        {
            string path = Path.Combine(host.ContentRootPath, "Files/book.pdf");

            string type = "application/pdf";

            string name = "book.pdf";

            return File(path, type, name);
        }



        
    }
    
    //public class User // user class for testing JsonResult
    //{
    //    public string Name { get; set; }

    //    public string Age { get; set; }

    //    public User(string name, string age)
    //    {
    //        Name = name;
    //        Age = age;
    //    }
    //}
}
