using System;
using CS_webapp.Models;
using CS_webapp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Dynamic;


namespace CS_webapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        // ************* password MD5 hash coding method ***************************************
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes and create a string.
            var sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        // *************************************************************************************


        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Category;
            /*
            string beseda = "Hello World!";
            using (MD5 MD5Hash = MD5.Create())
            {
                string hash = GetHash(MD5Hash, beseda);

                Debug.WriteLine($"The SHA256 hash of {beseda} is: {hash}.");
            }
            */
            return View(objList);

        }

        // GET - CREATE
        public IActionResult Create()
        {
            Debug.WriteLine("Veljavnost modela pri GET-CREATE:  " + ModelState.IsValid);
            return View();
        }

        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {

                // Check if username is available

                string _ime = obj.Name;
                string user_id_query = "SELECT Id FROM dbo.Category WHERE Name=({0})";
                var user_id = _db.Category.FromSqlRaw(user_id_query, _ime).Select(abc => abc.Id).ToList();

                if (user_id.Count() != 0)
                {
                    Debug.WriteLine($"User {_ime} is already taken.. Please, choose other username!");
                    TempData["user_taken_error"] = "User " + _ime + " is already taken.. Please, choose other username!";
                    obj.Name = "";
                    return View(obj);
                }

                else
                {
                    // *** MD5 hashing the password (DisplayOrder SQL table column) on Registration of new user **********
                    Debug.WriteLine("HEY! POST - CREATE");
                    using (MD5 MD5Hash = MD5.Create())
                    {
                        //string DisOrd = obj.DisplayOrder.ToString();
                        //DisOrd = GetHash(MD5Hash, DisOrd);
                        //obj.DisplayOrder = int.DisOrd;
                        obj.DisplayOrder = GetHash(MD5Hash, obj.DisplayOrder);
                    }
                    // ***************************************************************************************************

                    //string RegDate = DateTime.Now.ToString("HH:mm:ss tt");
                    DateTime localDate = DateTime.Now;
                    obj.InitDate = localDate.ToString();
                    obj.ResetDate = "N/A";   //localDate.ToString();
                    obj.LastDate = "N/A";       //localDate.ToString();

                    _db.Category.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");

                }
            }

            else
            {
                Debug.WriteLine("Veljavnost modela pri POST-CREATE:  " + ModelState.IsValid);
                return View(obj);
            }
        }

        // GET - RESET
        public IActionResult Reset(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }

            var obj = _db.Category.Find(id);
            if (obj == null) { return NotFound(); }

            Debug.WriteLine("InitDate GET-RESET: " + obj.InitDate);
            return View(obj);
        }

        // POST - RESET
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset(Category obj, ResetirajGeslo mod)
        {
            Debug.WriteLine("InitDate: "+obj.InitDate);
            Debug.WriteLine("ResetDate: " + obj.ResetDate);
            Debug.WriteLine("LastDate: " + obj.LastDate);

            Debug.WriteLine("novo geslo:  "+mod.NewOrderFirst);
            Debug.WriteLine("ponovitev gesla:  "+mod.NewOrderSecond);

            if (mod.NewOrderFirst==mod.NewOrderSecond)
            {
                // *********** MD5 hashing the password (DisplayOrder SQL table column) on Reset by user *************
                using (MD5 MD5Hash = MD5.Create())
                {
                    obj.DisplayOrder = GetHash(MD5Hash, mod.NewOrderSecond);
                }
                // ***************************************************************************************************
                //DateTime localDate = DateTime.Now;
                obj.ResetDate = DateTime.Now.ToString();
                _db.Category.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            else
            {
                TempData["password_error"] = "Passwords doesn't match! Please, enter same passwords..";
                mod.NewOrderFirst = ""; mod.NewOrderSecond = "";
                return View(obj);
            }

            
        }


        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id==null || id == 0){return NotFound();}

            var obj = _db.Category.Find(id);
            if (obj == null){ return NotFound();}

            return View(obj);
        }

        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            _db.Category.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }

            var obj = _db.Category.Find(id);
            if (obj == null) { return NotFound(); }

            return View(obj);
        }

        // POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Category.Find(id);
            _db.Category.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET - LOGIN  
        public IActionResult Login()
        {
            // https://stackoverflow.com/questions/54252957/how-to-get-value-from-microsoft-entityframeworkcore-query-internal-entityqueryab
            //var tuple = new Tuple<Category, LoginModel>(new Category(), new LoginModel());
            /*
            string sql_poizvedba_all = "SELECT Id,Name,DisplayOrder FROM dbo.Category";
            var list_Id = _db.Category.FromSqlRaw(sql_poizvedba_all).Select(abc => abc.Id).ToList();
            var list_Name = _db.Category.FromSqlRaw(sql_poizvedba_all).Select(abc => abc.Name).ToList();
            var list_DisplayOrder = _db.Category.FromSqlRaw(sql_poizvedba_all).Select(abc => abc.DisplayOrder).ToList();
            Debug.WriteLine(list_Name[2]);
            */

            //return View(tuple);
            return View();
        }


        // POST - LOGIN (backup)
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult LoginPOST(Tuple<Category, LoginModel> model)       // for TUPLE of models
        public IActionResult LoginPOST(BigModel big)
        {
            if (ModelState.IsValid)
            {
                Debug.WriteLine("Model POST-LOGIN (backup) is valid");
                //var tuple = new Tuple<Category, LoginModel>(new Category(), new LoginModel());

                Debug.WriteLine("tuple.Item1.Id =  " + big.Category.Id);
                Debug.WriteLine("tuple.Item1.Name =  " + big.Category.Name);


                //return View("Login", tuple);
                return RedirectToAction("SecretData", big);
            }
            Debug.WriteLine("Model POST-LOGIN (backup) is NOT VALID!");
            return View("Input");
        }


        // POST - LOGIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(BigModel big)
        {

            //var tuple = new Tuple<Category, LoginModel>(new Category(), new LoginModel());
            
            if (ModelState.IsValid)
            {
                Debug.WriteLine("Model POST-LOGIN is valid");
                Debug.WriteLine($"Posredovano uporabniško ime:   {big.LoginModel.UserName}");
                Debug.WriteLine($"Posredovano geslo:   {big.LoginModel.Password}");

                // ***************** MD5 hashing the password on user Login ***********************
                using (MD5 MD5Hash = MD5.Create())
                {
                    big.LoginModel.Password = GetHash(MD5Hash, big.LoginModel.Password);
                }
                // ********************************************************************************

                string uporabnisko_ime = big.LoginModel.UserName;
                string user_id_query = "SELECT Id FROM dbo.Category WHERE Name=({0})";

                // First, check, if the username is registered in database with try & catch exception
                // If not, return "User {uporabnisko_ime} not registered"
                // Else, proceed with geslo verification given below
                var user_id = _db.Category.FromSqlRaw(user_id_query, uporabnisko_ime).Select(abc => abc.Id).ToList();
                if (user_id.Count() == 0)
                {
                    Debug.WriteLine("user_id.Count() = " + user_id.Count());
                    Debug.WriteLine($"User {uporabnisko_ime} is not registered in database! Please, enter correct username..");
                    TempData["user_error"] = "User " + uporabnisko_ime + " is not registered in database! Please, enter correct username..";
                    return View(big.LoginModel);
                }

                else
                {
                    string geslo = big.LoginModel.Password;

                    string password_query = "SELECT DisplayOrder FROM dbo.Category WHERE Name=({0})";
                    var password = _db.Category.FromSqlRaw(password_query, uporabnisko_ime).Select(abc => abc.DisplayOrder).ToList();

                    if (geslo == password[0])
                    {
                        Debug.WriteLine("Success!!! Correct PASSWORD!");
                        TempData["success"] = "Success!!! Correct PASSWORD!";
                        TempData["user"] = uporabnisko_ime;
                        TempData["pass"] = geslo;
                        TempData["userID"] = user_id[0]; Debug.WriteLine("user_id[0] = "+user_id[0]);



                        var logged_user = _db.Category.Find(user_id[0]); Debug.WriteLine("logged_user: "+logged_user);
                        Debug.WriteLine("logged_user.Id: " + logged_user.Id);
                        Debug.WriteLine("logged_user.Name: " + logged_user.Name);
                        Debug.WriteLine("logged_user.DisplayOrder: " + logged_user.DisplayOrder);
                        Debug.WriteLine("logged_user.InitDate: " + logged_user.InitDate);
                        logged_user.LastDate = DateTime.Now.ToString();
                        Debug.WriteLine("logged_user.LastDate: " + logged_user.LastDate);
                        
                        _db.Category.Update(logged_user);
                        _db.SaveChanges();
                        

                        return RedirectToAction("SecretData", big);
                        //return RedirectToAction("SecretData", tuple);
                    }

                    else
                    {
                        Debug.WriteLine("WRONG password! Try, again!");
                        TempData["password_error"] = "WRONG password! Try, again!";
                    }
                    return View(big);
                    //return View(tuple);

                }
            }
            Debug.WriteLine("Invalid POST-LOGIN model..");
            //return View(tuple);
            return View(big);
        }




        // GET - SECRET  
        public IActionResult SecretData()   //LoginModel obj
        {
            /*
            Debug.WriteLine($"GET-SECRET uporabniško ime:   {obj.UserName}");
            Debug.WriteLine($"GET-SECRET geslo:   {obj.Password}");

            TempData["user"] = obj.UserName;
            TempData["pass"] = obj.Password;
            */

            //Debug.WriteLine("Hashed password in GET-SECRET:  " + obj.DisplayOrder);
            //Debug.WriteLine(TempData["userID"]);
            
            /*
            var logged_user = _db.Category.Find(TempData["userID"]);
            logged_user.LastDate = DateTime.Now.ToString();
            _db.Category.Update(obj);
            _db.SaveChanges();
            */

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
  


}


