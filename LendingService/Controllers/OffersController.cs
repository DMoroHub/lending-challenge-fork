using LendingService.Models;
using LendingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LendingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly LoanService _loanService;

        public OffersController(LoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] List<Offer> offers)
        {
            if (offers == null) return BadRequest();

            foreach (var offer in offers)
                _loanService.AddOrUpdateOffer(offer);

            return Ok();
        }
    }
}
