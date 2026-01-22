using IkinciElSatis.Data;
using IkinciElSatis.Models;
using IkinciElSatis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IkinciElSatis.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessageController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==============================
        // 1. TEKİL MESAJ GÖNDERME EKRANI 
        // ==============================
        [HttpGet]
        public async Task<IActionResult> Create(string receiverId, string? relatedProduct)
        {
            if (string.IsNullOrEmpty(receiverId)) return NotFound();

            var receiver = await _userManager.FindByIdAsync(receiverId);
            if (receiver == null) return NotFound();

            var model = new Message
            {
                ReceiverId = receiverId,
                RelatedProductName = relatedProduct
            };

            ViewBag.ReceiverName = receiver.FirstName + " " + receiver.LastName;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Message message)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            message.SenderId = senderId;
            message.SentDate = DateTime.Now;
            message.IsRead = false;

            if (!string.IsNullOrEmpty(message.Content) && !string.IsNullOrEmpty(message.ReceiverId))
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Mesajınız gönderildi!";

                return RedirectToAction("Index", new
                {
                    userId = message.ReceiverId,
                    relatedProduct = message.RelatedProductName
                });
            }

            return View(message);
        }

        // =========================
        // 2. GELİŞMİŞ SOHBET EKRANI
        // =========================
        [HttpGet]
        public async Task<IActionResult> Index(string? userId, string? relatedProduct)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var conversationIds = await _context.Messages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToListAsync();

            var conversations = await _context.Users
                .Where(u => conversationIds.Contains(u.Id))
                .ToListAsync();

            var model = new ChatViewModel
            {
                Conversations = conversations,
                CurrentReceiverId = userId,
                RelatedProductName = relatedProduct
            };

            if (!string.IsNullOrEmpty(userId))
            {
                var receiver = await _userManager.FindByIdAsync(userId);
                if (receiver != null)
                {
                    model.CurrentReceiver = receiver;

                    if (!conversations.Any(u => u.Id == receiver.Id))
                    {
                        model.Conversations.Add(receiver);
                    }

                    model.Messages = await _context.Messages
                        .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                                    (m.SenderId == userId && m.ReceiverId == currentUserId))
                        .OrderBy(m => m.SentDate)
                        .ToListAsync();

                    var unreadMessages = model.Messages
                        .Where(m => m.ReceiverId == currentUserId && !m.IsRead)
                        .ToList();

                    if (unreadMessages.Any())
                    {
                        unreadMessages.ForEach(m => m.IsRead = true);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return View(model);
        }

        // Sohbet ekranı içinden hızlı mesaj atma
        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string content, string? relatedProduct)
        {
            if (string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(content))
                return RedirectToAction("Index", new { userId = receiverId });

            var message = new Message
            {
                SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ReceiverId = receiverId,
                Content = content,
                RelatedProductName = relatedProduct,
                SentDate = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { userId = receiverId });
        }
    }
}