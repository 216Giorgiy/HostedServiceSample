using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HostedServiceSample.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HostingSample.Services;

namespace HostedServiceSample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Message Message { get; set; }

        public IList<Message> Messages { get; private set; }

        public IList<Word> Words { get; private set; }

        public async Task OnGetAsync()
        {
            Messages = await _db.Messages.AsNoTracking().ToListAsync();
            Words = await _db.Words.AsNoTracking().ToListAsync();
        }

        public async Task<IActionResult> OnPostAddMessageAsync()
        {
            _db.Messages.Add(Message);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAllMessagesAsync()
        {
            foreach (Message message in _db.Messages)
            {
                _db.Messages.Remove(message);
            }
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMessageAsync(int id)
        {
            var message = await _db.Messages.FindAsync(id);

            if (message != null)
            {
                _db.Messages.Remove(message);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostStopMessageProcessingServiceAsync()
        {
            var hostedServices = HttpContext.RequestServices.GetServices<IHostedService>();

            foreach (var service in hostedServices)
            {
                if (service.GetType() == typeof(MessageProcessingService))
                {
                    var castedService = (MessageProcessingService)service;
                    await service.StopAsync(castedService.StoppingToken);
                    break;
                }
            }

            return RedirectToPage();
        }
        
        public async Task<IActionResult> OnPostStopWordCountingServiceAsync()
        {
            var hostedServices = HttpContext.RequestServices.GetServices<IHostedService>();
            
            foreach (var service in hostedServices)
            {
                if (service.GetType() == typeof(WordCountingService))
                {
                    var castedService = (WordCountingService)service;
                    await service.StopAsync(castedService.StoppingToken);
                    break;
                }
            }

            return RedirectToPage();
        }

        public IActionResult OnPostRefreshAsync()
        {
            return RedirectToPage();
        }
    }
}
