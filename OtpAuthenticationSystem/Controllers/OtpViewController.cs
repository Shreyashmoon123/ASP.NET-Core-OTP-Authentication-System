using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using OtpAuthenticationSystem.Models;

namespace OtpAuthenticationSystem.Controllers
{
    public class OtpViewController : Controller
    {
        private readonly HttpClient _httpclient;
        public OtpViewController(HttpClient httpClient)
        {
            _httpclient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendOtp(SendOtpRequest request)
        {
            var response = await _httpclient.PostAsJsonAsync(
                "https://localhost:7251/api/Otp/Send",
                request);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

            if (result == null)
            {
                TempData["Error"] = "Something went wrong";
                return RedirectToAction("Index");
            }

            if (result.Success)
            {
                TempData["Success"] = result.Message;

                TempData["Email"] = request.Email;
                TempData["Purpose"] = request.Purpose;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
        {
            var response = await _httpclient.PostAsJsonAsync(
                "https://localhost:7251/api/Otp/Verify",
                request);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

            if (result == null)
            {
                TempData["Error"] = "Something Went Wrong";
                return RedirectToAction("Index");
            }

            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("Index");
        }

    }
    }
