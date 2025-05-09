using LendingService.Models;

namespace LendingService.Services
{
    public class LoanService
    {
        private readonly Dictionary<string, List<Loan>> _loans = new();
        private readonly Dictionary<int, Offer> _offers = new();

        public bool AddOrUpdateOffer(Offer offer)
        {
            _offers[offer.Id] = offer;
            return true;
        }

        public bool HasLoan(string msisdn) => _loans.ContainsKey(msisdn);

        public Offer? GetOffer(int offerId) =>
            _offers.TryGetValue(offerId, out var offer) ? offer : null;

        public Loan? GetLoan(string msisdn)
        {
            if (!_loans.TryGetValue(msisdn, out var loans))
                return null;

            return loans.FirstOrDefault(l => l.IsActive);
        }

        public bool UserExists(string msisdn) =>
            _loans.ContainsKey(msisdn);

        public Loan? CreateLoan(string msisdn, int offerId)
        {
            if (!_offers.TryGetValue(offerId, out var offer))
                return null;

            if (!_loans.TryGetValue(msisdn, out var loans))
            {
                loans = new List<Loan>();
                _loans[msisdn] = loans;
            }

            if (loans.Any(l => l.IsActive))
                return null;

            var taxesAmount = offer.Balance * offer.Taxes;
            var balanceLeft = offer.Balance + taxesAmount;

            var loan = new Loan
            {
                Id = new Random().Next(1, 10000),
                Offer = offer,
                BalanceLeft = balanceLeft,
                DueDate = DateTime.UtcNow.AddDays(30),
                IsActive = true
            };

            loans.Add(loan);
            return loan;
        }

        public decimal? RepayLoan(string msisdn, decimal topUp)
        {
            if (!_loans.TryGetValue(msisdn, out var loans))
                return null;

            var activeLoan = loans.FirstOrDefault(l => l.IsActive);
            if (activeLoan == null)
                return null;

            var repaid = Math.Min(topUp, activeLoan.BalanceLeft);
            activeLoan.BalanceLeft -= repaid;

            if (activeLoan.BalanceLeft == 0)
                activeLoan.IsActive = false;

            return repaid;
        }
    }
}
