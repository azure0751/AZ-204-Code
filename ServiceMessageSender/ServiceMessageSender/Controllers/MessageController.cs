using Microsoft.AspNetCore.Mvc;

using ServiceMessageSender.Models;
using ServiceMessageSender.Services;
using System.Threading.Tasks;

namespace ServiceMessageSender.Controllers
{
    public class MessageController : Controller
    {
        private readonly ServiceBusService _serviceBusService;

        public MessageController()
        {
            _serviceBusService = new ServiceBusService();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new MessageModel();
            if (TempData.ContainsKey("ConnectionString"))
            {
                model.ConnectionString = TempData["ConnectionString"].ToString();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(MessageModel model)
        {
            if (ModelState.IsValid)
            {
                string queueName = "q1"; // Replace with your Azure Service Bus queue name
                await _serviceBusService.SendMessagesAsync(model.ConnectionString, queueName, model.MessageContent, model.MessageCount);

                TempData["ConnectionString"] = model.ConnectionString;
                ViewBag.StatusMessage = $"{model.MessageCount} messages have been sent successfully.";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ViewMessages()
        {
            var model = new MessageModel();
            if (TempData.ContainsKey("ConnectionString"))
            {
                model.ConnectionString = TempData["ConnectionString"].ToString();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ViewMessages(MessageModel model)
        {
            
                string queueName = "q1"; // Replace with your Azure Service Bus queue name
                var messages = await _serviceBusService.ReceiveMessagesAsync(model.ConnectionString, queueName, model.MessageCount);

                TempData["ConnectionString"] = model.ConnectionString;
                ViewBag.Messages = messages;           

            return View(model);
        }
    }
}