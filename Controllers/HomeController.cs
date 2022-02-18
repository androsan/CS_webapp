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
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
        public IActionResult Create(BigModel big)
        {
            if (ModelState.IsValid)
            {
                // Check if username is available

                string _ime = big.Category.Name;
                string user_id_query = "SELECT Id FROM dbo.Category WHERE Name=({0})";
                var user_id = _db.Category.FromSqlRaw(user_id_query, _ime).Select(abc => abc.Id).ToList();
                Debug.WriteLine("User_ID old: "+user_id);

                if (user_id.Count() != 0)
                {
                    Debug.WriteLine($"User {_ime} is already taken.. Please, choose other username!");
                    TempData["user_taken_error"] = "User " + _ime + " is already taken.. Please, choose other username!";
                    big.Category.Name = "";
                    return View(big.Category);
                }

                else
                {
                    // *** MD5 hashing the password (DisplayOrder SQL table column) on Registration of new user **********
                    Debug.WriteLine("HEY! POST - CREATE");
                    using (MD5 MD5Hash = MD5.Create())
                    {
                        big.Category.DisplayOrder = GetHash(MD5Hash, big.Category.DisplayOrder);
                    }
                    // ***************************************************************************************************

                    //string RegDate = DateTime.Now.ToString("HH:mm:ss tt");
                    DateTime localDate = DateTime.Now;
                    big.Category.InitDate = localDate.ToString();
                    
                    big.Category.ResetDate = "N/A";   //localDate.ToString();
                    big.Category.LastDate = "N/A";       //localDate.ToString();
                    //----------------------------------------------------------------------------------------------------
                    _db.Category.Add(big.Category);   // Id of new row in Category table is generated HERE, not before!
                    //----------------------------------------------------------------------------------------------------
                    //~~~~~~~~~~~ Adding row in Podatki table ~~~~~~~~~~~~~~~~~

                    Debug.WriteLine("big.Category.Name:  "+ big.Category.Name);
                    
                    
                    _db.SaveChanges();


                    //................................................
                    // tukaj sta dve možnosti:

                    // 1. V tabelo big.Podatki dodaš Podatki.Name in Podatki.Secret, vendar bo potem tabela big.Podatki sama
                    //    generirala Id, ki pa se kasneje morda ne bo ujemal z Id tabele big.Category (brisanje vrstic,..)

                    // 2. Boljša varianta, kjer z SQL poizvedbo pridobiš Id big.Category glede na Name, potem tabeli
                    //    big.Podatki dodaš ta Id in pripneš pripadajoče ime (Name)

                    var user_id_new = _db.Category.FromSqlRaw(user_id_query, _ime).Select(abc => abc.Id).ToList();
                    Debug.WriteLine("User_ID new: " + user_id_new[0]);

                    //var pod_new = _db.Podatki.FromSqlRaw("INSERT INTO dbo.Podatki (Id) SELECT Id FROM dbo.Category WHERE Id NOT IN (SELECT Id FROM Podatki)").ToList();
                    //var pod_new = _db.Podatki.FromSqlRaw("INSERT INTO dbo.Podatki (Id) SELECT Id FROM dbo.Category WHERE Id NOT IN (SELECT Id FROM Podatki)").ToList();

                    //_db.Podatki.FromSqlRaw("INSERT INTO dbo.Podatki ({0})",user_id_new[0]);
                    //_db.Podatki.FromSqlRaw("INSERT INTO dbo.Podatki (Id) VALUES ({0})", user_id_new[0]);
                    //_db.SaveChanges();

                    //................................................

                    //var new_row = _db.Podatki.Find(user_id_new[0]);
                    //var new_row = _db.Podatki.Find(user_id_new[0]);
                    //new_row.Name = _ime;
                    //new_row.Secret = "Welcome, " + _ime;

                    //big.Podatki.Name = _ime;
                    big.Podatki.Name = _ime;
                    Debug.WriteLine("big.Podatki.Name : "+ big.Podatki.Name);
                    big.Podatki.Secret = "Welcome, " + _ime;
                    big.Podatki.Id = user_id_new[0];

                    _db.Podatki.Add(big.Podatki);

                    //big.Podatki.Id = user_id_new[0];
                    //big.Podatki.Name = _ime;
                    //big.Podatki.Secret = "Welcome, "+ _ime;
                    //_db.Podatki.Add(big.Podatki);   
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                    _db.SaveChanges();
                    return RedirectToAction("Index");

                }
            }

            else
            {
                Debug.WriteLine("Veljavnost modela pri POST-CREATE:  " + ModelState.IsValid);
                return View(big.Category);
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
                TempData["password_error"] = "Passwords don't match! Please, enter same passwords..";
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
        public IActionResult Delete(int? id, string ajax_variable)
        {
            Debug.WriteLine("I've found this guy wondering around GET-DELETE:  " + ajax_variable);
            TempData["ajax_variable"] = ajax_variable;

            if ( id == null || id == 0) { return NotFound(); }

            var obj = _db.Category.Find(id);
            if (obj == null) { return NotFound(); }

            return View(obj);
        }


        // POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            //Debug.WriteLine("ajax_variable as argument POST-DELETE:  "+ajax_variable);
            //Debug.WriteLine("ajax_variable as TempData POST-DELETE:  " + TempData["ajax_variable"]);

                var obj_cat = _db.Category.Find(id);
                var obj_pod = _db.Podatki.Find(id);


                _db.Category.Remove(obj_cat);
                _db.Podatki.Remove(obj_pod);
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
                    return View(big);
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
        public IActionResult SecretData() 
        {

            Debug.WriteLine($"GET-SECRET uporabniško ime:   {TempData["user"]}");

            string message_name = TempData["user"].ToString();

            string user_id_query = "SELECT Id FROM dbo.Podatki WHERE Name=({0})";
            var user_id = _db.Podatki.FromSqlRaw(user_id_query, message_name).Select(abc => abc.Id).ToList();
            var logged_user = _db.Podatki.Find(user_id[0]);
            var message = logged_user.Secret;

            //string message_query = "SELECT Secret FROM dbo.Podatki WHERE Name=({0})";
            //var message = _db.Podatki.FromSqlRaw(message_query, message_name).Select(abc => abc.Secret).ToList();

            Debug.WriteLine($"SECRET MESSAGE of user {message_name} is:  {message}");
            //IEnumerable<Podatki> objList = _db.Podatki;
            TempData["mes"] = message;

            return View(logged_user);
        }


        // POST - SECRET
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SecretData(Podatki logged_user)
        {
            if (ModelState.IsValid)
            {
           
                TempData["mes"] = logged_user.Secret; 
                Debug.WriteLine("SECRET-POST ----> logged_user.Id:  " + logged_user.Id);
                Debug.WriteLine("SECRET-POST ----> logged_user.Name:  " + logged_user.Name);
                Debug.WriteLine("SECRET-POST ----> logged_user.Secret:  " + logged_user.Secret);

                _db.Podatki.Update(logged_user);
                _db.SaveChanges();
                //logged_user.Secret = "";
                return View(logged_user);


            }
            Debug.WriteLine("Invalid POST-SECRET model.");
            return View(logged_user);
        }


            public IActionResult MyBackupService()
        {
            
            string password = "moje geslo";
            Debug.WriteLine("password: "+password);
            ViewData["vd0"] = "password: " + password;

            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            Debug.WriteLine("Salt: " + Convert.ToBase64String(salt));
            ViewData["vd1"] = "Salt: " + Convert.ToBase64String(salt);

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            Debug.WriteLine("Hashed: " + hashed);
            ViewData["vd2"] = "Hashed: " + hashed;
            

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


