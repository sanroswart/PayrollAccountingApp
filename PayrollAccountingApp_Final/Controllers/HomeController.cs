using Microsoft.AspNetCore.Mvc;
namespace PayrollAccountingApp.Controllers;
public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index","Transactions");
}
