using LendingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LendingService.Controllers
{
    [ApiController]
    [Route("customers/{msisdn}/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly LoanService _loanService;

        public LoansController(LoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost]
        public IActionResult RequestLoan(string msisdn, [FromForm] int ID)
        {
            if (_loanService.HasLoan(msisdn))
                return Conflict();

            var loan = _loanService.CreateLoan(msisdn, ID);
            if (loan == null) return BadRequest();

            return Ok(new
            {
                loan.Id,
                loan.BalanceLeft,
                DueDate = loan.DueDate.ToString("o")
            });
        }

        [HttpGet]
        public IActionResult GetLoan(string msisdn)
        {
            if(!_loanService.UserExists(msisdn))
                return NotFound();

            var loan = _loanService.GetLoan(msisdn);
            if (loan == null) return NoContent();

            return Ok(new
            {
                loan.BalanceLeft,
                DueDate = loan.DueDate.ToString("o"),
                Offer = new
                {
                    loan.Offer.Balance,
                    loan.Offer.Taxes
                }
            });
        }

        [HttpPut]
        public IActionResult RepayLoan(string msisdn, [FromForm] decimal TopUp)
        {
            if (!_loanService.UserExists(msisdn))
                return NotFound();

            var repaid = _loanService.RepayLoan(msisdn, TopUp);

            if (repaid == null) return NoContent();

            return Ok(new { Repaid = repaid });
        }
    }
}
