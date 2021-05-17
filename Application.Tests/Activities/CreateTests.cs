using Application.Activities;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Activities
{
    public class CreateTests : BaseTests, IDisposable
    {
        private readonly IUserAccessor _userAccessor;
        private Create.CommandValidator _commandValidator;
        private Create.Command _command;
        private Create.Handler _sut;
        private Activity _activity;
        private Result<Unit> _result;
        private ValidationResult _validationResult;
        private DataContext _context;

        public CreateTests()
        {
            _userAccessor = Substitute.For<IUserAccessor>();
        }

        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
        }

        [Fact]
        public async Task GivenValidActivity_WhenCreateActivity_ThenIsSuccessTrue()
        {
            GivenValidActivity();
            GivenValidCommand();
            GivenValidHandler();
            GivenValidUser();
            await WhenCreateHandlerInvoked();
            ThenIsSuccessIsTrue();
            ThenValidActivityIsCreated();
        }

        [Fact]
        public async Task GivenNotFoundUser_WhenCreateActivity_ThenIsSucccessFalse()
        {
            GivenValidActivity();
            GivenValidCommand();
            GivenValidHandler();
            GivenInvalidUser();
            await WhenCreateHandlerInvoked();
            ThenIsSuccessIsFalse();
            ThenIsSuccessErrorIsUserNotFound();
            ThenActivityWasNotCreated();
        }

        [Fact]
        public void GivenValidActivity_WhenValidateCommand_ThenValidatePass()
        {
            GivenValidActivity();
            GivenValidCommand();
            GivenValidCommandValidator();
            WhenCommandValidatorInvoked();
            ThenValidationResultIsSuccess();
        }

        [Fact]
        public void GivenInvalidActivity_WhenValidateCommand_ThenValidateFails()
        {
            GivenInvalidActivity();
            GivenValidCommand();
            GivenValidCommandValidator();
            WhenCommandValidatorInvoked();
            ThenValidationResultContainsErrors();
        }

        #region Givens

        private void GivenValidActivity()
        {
            _activity = new Activity
            {
                Title = "Test Title",
                Description = "Test Description",
                Category = "Category",
                Date = new DateTime(2021, 05, 07),
                City = "London",
                Venue = "London venue"
            };
        }

        private void GivenInvalidActivity()
        {
            _activity = new Activity();
        }

        private void GivenValidHandler()
        {
            _context = new DataContext(ContextOptions);
            _sut = new Create.Handler(_context, _userAccessor);
        }

        private void GivenValidUser() => _userAccessor.GetUsername().Returns("bob");

        private void GivenInvalidUser() => _userAccessor.GetUsername().Returns("invalid");

        private void GivenValidCommand() => _command = new Create.Command { Activity = _activity };

        private void GivenValidCommandValidator() => _commandValidator = new Create.CommandValidator();

        #endregion

        #region Whens

        private void WhenCommandValidatorInvoked() =>
            _validationResult = _commandValidator.Validate(_command);

        private async Task WhenCreateHandlerInvoked() =>
            _result = await _sut.Handle(_command, CancellationToken.None);

        #endregion

        #region Thens

        private void ThenIsSuccessIsTrue()
        {
            _result.IsSuccess.Should().BeTrue();
            _result.Value.Should().NotBeNull();
        }

        private void ThenIsSuccessIsFalse() => _result.IsSuccess.Should().BeFalse();

        private void ThenIsSuccessErrorIsUserNotFound() => _result.Error.Should().Be("User not found");

        private void ThenValidActivityIsCreated()
        {
            var activity = _context.Activities.FirstOrDefault(x => x.Title == _activity.Title);
            activity.Should().NotBeNull();
            activity.Should().BeEquivalentTo(_activity);
        }

        private void ThenActivityWasNotCreated()
        {
            var activity = _context.Activities.FirstOrDefault(x => x.Title == _activity.Title);
            activity.Should().BeNull();
        }

        private void ThenValidationResultIsSuccess() =>
            _validationResult.Errors.Should().BeEmpty();

        private void ThenValidationResultContainsErrors() =>
            _validationResult.Errors.Should().NotBeEmpty();

        #endregion
    }
}