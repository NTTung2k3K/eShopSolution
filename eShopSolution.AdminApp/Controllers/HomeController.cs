using eShopSolution.AdminApp.Models;
using eShopSolution.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Transaction = PayPal.Api.Transaction;

namespace eShopSolution.AdminApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
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

        public IActionResult AccessDenied()
        {
            return View();
        }



        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Query["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = $"{Request.Scheme}://{Request.Host}/Home/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    Payment createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    HttpContext.Session.SetString(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Query["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, HttpContext.Session.GetString(guid) as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            return View("SuccessView");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Item Name comes here",
                currency = "USD",
                price = "1",
                quantity = "1",
                sku = "sku"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = "1"
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = "3", // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        [HttpGet]
        public IActionResult SendSMS()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SendSMS(string phoneNumber)
        {
            string accountSid = "AC993180f82825c36acd4c9b1f2c1b55a4";
            string authToken = "0167af0e8685a137ce2a2db6f9e82f6b";
            TwilioClient.Init(accountSid, authToken);
            var message = await MessageResource.CreateAsync(
                body: "Hello" + DateTime.Now.ToString(),
                from: new Twilio.Types.PhoneNumber("+84332595856"),
                to: new Twilio.Types.PhoneNumber("+84904540945")


                );

            return View(message);
        }


        [HttpGet]
        public IActionResult Send()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Send(string a)
        {
            try
            {
                SendEmail();
                ViewBag.Message = "Email sent successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error sending email: " + ex.Message;
            }

            return View();
        }

        public void SendEmail()
        {
            /**/        /*
                         ShopSales.Models.ShoppingCart Cart = (Models.ShoppingCart)Session["Cart"];
                        ViewBag.Cart = Cart.shoppingCarts;

                        if (ModelState.IsValid)
                        {
                            ShopSales.Models.EF.Order ord = new Models.EF.Order();
                            ord.CustomerName = order.name;
                            ord.Address = order.address;
                            ord.Phone = order.phoneNumber;
                            ord.TotalAmount = Cart.shoppingCarts.Sum(x => x.TotalAmount);
                            ord.CreateDate = DateTime.Now;
                            ord.ModifiedDate = DateTime.Now;
                            ord.ModifierBy = order.phoneNumber;
                            ord.CreateBy = order.phoneNumber;

                            Random rd = new Random();
                            ord.Code = "OD" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                            ord.Email = order.email;
                            ord.Payment = order.payment;
                            ord.OrderState = "Unpaid";
                            Cart.shoppingCarts.ForEach(x => ord.OrderDetails.Add(new Models.EF.OrderDetail() {
                                ProductId = x.ProductId,
                                Price = x.Price,
                                Quantity = x.Quantity,
                            }));
                            dBContext.Ordes.Add(ord);
                            dBContext.SaveChanges();

                         */


            decimal total = 0;
            string strProduct = "";
            /*foreach (var i in Cart.shoppingCarts)
            {
                strProduct += "<tr>";
                strProduct += "<td>" + i.ProductName + "</td>";
                strProduct += "<td>" + i.Quantity + "</td>";
                strProduct += "<td>" + String.Format("{0:0,0} đ", i.TotalAmount).ToString() + "</td>";
                strProduct += "</tr>";
                total += i.TotalAmount;
            }*/

            strProduct += "<tr>";
            strProduct += "<td>" + "Kim cương"  + "</td>";
            strProduct += "<td>" + "1" + "</td>";
            strProduct += "<td>" + "299000" + "</td>";
            strProduct += "</tr>";
            string path = Path.Combine(_env.ContentRootPath, "Form-Send-Email", "CustomerForm.html");
            string contentCustomer = System.IO.File.ReadAllText(path);
            /*contentCustomer = contentCustomer.Replace("{{Code}}", ord.Code);
            contentCustomer = contentCustomer.Replace("{{CustomerName}}", ord.CustomerName);
            contentCustomer = contentCustomer.Replace("{{Date}}", ord.CreateDate.ToString("dd/MM/yyyy"));
            contentCustomer = contentCustomer.Replace("{{Product}}", strProduct);
            contentCustomer = contentCustomer.Replace("{{Total}}", String.Format("{0:0,0} đ", total).ToString());
            contentCustomer = contentCustomer.Replace("{{Payment}}", ord.Payment);
            contentCustomer = contentCustomer.Replace("{{Address}}", ord.Address);
            contentCustomer = contentCustomer.Replace("{{PhoneNumber}}", ord.Phone);
            contentCustomer = contentCustomer.Replace("{{Email}}", ord.Email);
            ShopSales.Models.Common.DoingMail.SendMail("ShopTK", "Order #" + ord.Code, contentCustomer, order.email);*/
            contentCustomer = contentCustomer.Replace("{{Product}}", strProduct);
            contentCustomer = contentCustomer.Replace("{{Code}}", "1322342334");
            DoingMail.SendMail("LuxuryDiamond", "Order #" + 1322342334, contentCustomer, "laogiado2003@gmail.com");
            /*// mail for Admin
            string contentCustomerForAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/Form_Send_Mail/send1.html"));
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{Code}}", ord.Code);
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{CustomerName}}", ord.CustomerName);
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{Date}}", ord.CreateDate.ToString("dd/MM/yyyy"));
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{Product}}", strProduct);
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{Total}}", String.Format("{0:0,0} đ", total).ToString());
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{Payment}}", ord.Payment);
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{Address}}", ord.Address);
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{PhoneNumber}}", ord.Phone);
            contentCustomerForAdmin = contentCustomerForAdmin.Replace("{{Email}}", ord.Email);
            DoingMail.SendMail("ShopTK", "Order #" + ord.Code, contentCustomerForAdmin, ConfigurationManager.AppSettings["Email"]);*/

            /*  foreach (var i in Cart.shoppingCarts)
              {
                  var product = dBContext.Products.Find(i.ProductId);
                  product.Quantity -= i.Quantity;
              }
              Cart.Clear();*/

            /*
                        dBContext.SaveChanges();
                        return RedirectToAction("OrderSuccess");*/
        }

    }
}
