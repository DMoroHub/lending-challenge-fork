using System;
using System.Collections.Generic;
using LendingService.Services;
using LendingService.Models;
using Xunit;

namespace LendingService.Tests
{
    public class LoanServiceTest
    {
        private readonly LoanService _service = new();

        [Fact]
        public void AddOrUpdateOffer_ShouldAddOrUpdateOffer()
        {
            var offer = new Offer { Id = 1, Balance = 10, Taxes = 0.2m };
            var result = _service.AddOrUpdateOffer(offer);

            Assert.True(result);
            var fetched = _service.GetOffer(1);
            Assert.NotNull(fetched);
            Assert.Equal(10, fetched!.Balance);
            Assert.Equal(0.2m, fetched.Taxes);
        }

        [Fact]
        public void GetOffer_ShouldReturnNull_WhenOfferDoesNotExist()
        {
            var offer = _service.GetOffer(999);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateLoan_ShouldReturnNull_WhenOfferDoesNotExist()
        {
            var loan = _service.CreateLoan("123", 99);
            Assert.Null(loan);
        }

        [Fact]
        public void CreateLoan_ShouldSucceed_IfValid()
        {
            _service.AddOrUpdateOffer(new Offer { Id = 1, Balance = 10, Taxes = 0.2m });
            var loan = _service.CreateLoan("123", 1);

            Assert.NotNull(loan);
            Assert.Equal(12, loan!.BalanceLeft);
            Assert.True(loan.IsActive);
        }

        [Fact]
        public void CreateLoan_ShouldReturnNull_IfActiveLoanExists()
        {
            _service.AddOrUpdateOffer(new Offer { Id = 2, Balance = 5, Taxes = 0.1m });
            _service.CreateLoan("456", 2);

            var second = _service.CreateLoan("456", 2);
            Assert.Null(second);
        }

        [Fact]
        public void GetLoan_ShouldReturnActiveLoan()
        {
            _service.AddOrUpdateOffer(new Offer { Id = 3, Balance = 8, Taxes = 0.5m });
            _service.CreateLoan("789", 3);

            var loan = _service.GetLoan("789");

            Assert.NotNull(loan);
            Assert.True(loan!.IsActive);
        }

        [Fact]
        public void GetLoan_ShouldReturnNull_WhenUserHasNoLoans()
        {
            var loan = _service.GetLoan("000");
            Assert.Null(loan);
        }

        [Fact]
        public void RepayLoan_ShouldReturnNull_IfUserNotFound()
        {
            var result = _service.RepayLoan("notfound", 10);
            Assert.Null(result);
        }

        [Fact]
        public void RepayLoan_ShouldReturnNull_IfNoActiveLoan()
        {
            _service.AddOrUpdateOffer(new Offer { Id = 4, Balance = 10, Taxes = 0 });
            _service.CreateLoan("aaa", 4);
            _service.RepayLoan("aaa", 10); 

            var result = _service.RepayLoan("aaa", 5);
            Assert.Null(result);
        }

        [Fact]
        public void RepayLoan_ShouldPartiallyRepay()
        {
            _service.AddOrUpdateOffer(new Offer { Id = 5, Balance = 10, Taxes = 0.1m });
            _service.CreateLoan("bbb", 5);

            var repaid = _service.RepayLoan("bbb", 5);
            Assert.Equal(5, repaid);

            var loan = _service.GetLoan("bbb");
            Assert.NotNull(loan);
            Assert.True(loan!.BalanceLeft < 11);
        }

        [Fact]
        public void RepayLoan_ShouldFullyRepay_AndDeactivateLoan()
        {
            _service.AddOrUpdateOffer(new Offer { Id = 6, Balance = 10, Taxes = 0 });
            _service.CreateLoan("ccc", 6);

            var repaid = _service.RepayLoan("ccc", 10);
            Assert.Equal(10, repaid);

            var loan = _service.GetLoan("ccc");
            Assert.Null(loan); 
        }

        [Fact]
        public void UserExists_ShouldReturnTrue_IfUserHasLoanHistory()
        {
            _service.AddOrUpdateOffer(new Offer { Id = 7, Balance = 10, Taxes = 0 });
            _service.CreateLoan("ddd", 7);

            Assert.True(_service.UserExists("ddd"));
        }

        [Fact]
        public void UserExists_ShouldReturnFalse_IfUserNeverRequestedLoan()
        {
            Assert.False(_service.UserExists("nonexistent"));
        }
    }
}