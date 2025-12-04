using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using IAM_Service.Application.Common.Behavior;
using MediatR;
using Xunit;

namespace IAM_Service.Application.UnitTests.Validation_Behavior
{
    /// <summary>
    /// Unit tests for <see cref="ValidationBehavior{TRequest, TResponse}"/>.
    /// </summary>
    public class ValidationBehaviorTests
    {
        /// <summary>
        /// A fake request implementing <see cref="IRequest{TResponse}"/> for testing purposes.
        /// </summary>
        private class FakeRequest : IRequest<string> { }

        /// <summary>
        /// Returns a successful <see cref="RequestHandlerDelegate{TResponse}"/> that returns "Success".
        /// </summary>
        private static RequestHandlerDelegate<string> SuccessNext =>
            (ct) => Task.FromResult("Success");

        /// <summary>
        /// Returns a successful <see cref="RequestHandlerDelegate{TResponse}"/> that calls an action before returning "Success".
        /// </summary>
        /// <param name="onCalled">Action to execute when handler is called.</param>
        /// <returns>A delegate returning a Task with "Success".</returns>
        private static RequestHandlerDelegate<string> SuccessNextWithFlag(Action onCalled) =>
            (ct) =>
            {
                onCalled();
                return Task.FromResult("Success");
            };

        /// <summary>
        /// A fake validator that returns predefined validation failures.
        /// </summary>
        private sealed class FakeValidator : AbstractValidator<FakeRequest>
        {
            public FakeValidator(params ValidationFailure[] failures)
            {
                RuleFor(x => x).Custom((_, context) =>
                {
                    foreach (var f in failures)
                        context.AddFailure(f);
                });
            }
        }

        /// <summary>
        /// A validator that tracks the number of times it was called and last CancellationToken.
        /// </summary>
        private sealed class TrackableValidator : AbstractValidator<FakeRequest>
        {
            /// <summary>
            /// Number of times <see cref="ValidateAsync"/> was called.
            /// </summary>
            public int CallCount { get; private set; }

            /// <summary>
            /// Last cancellation token used in validation.
            /// </summary>
            public CancellationToken LastToken { get; private set; } = CancellationToken.None;

            public TrackableValidator(bool valid = true)
            {
                RuleFor(x => x).Custom((_, context) =>
                {
                    CallCount++;
                    if (!valid)
                        context.AddFailure("Test", "Invalid");
                });
            }

            public override Task<ValidationResult> ValidateAsync(
                ValidationContext<FakeRequest> context,
                CancellationToken ct = default)
            {
                LastToken = ct;
                return base.ValidateAsync(context, ct);
            }
        }

        /// <summary>
        /// Creates a <see cref="ValidationBehavior{FakeRequest, string}"/> with provided validators.
        /// </summary>
        /// <param name="validators">Validators to apply.</param>
        /// <returns>A validation behavior instance.</returns>
        private static ValidationBehavior<FakeRequest, string> Behavior(
            params IValidator<FakeRequest>[] validators) => new(validators);

        /// <summary>
        /// Ensures <see cref="ValidationBehavior{TRequest, TResponse}"/> throws a <see cref="ValidationException"/> when the request is invalid.
        /// </summary>
        [Fact]
        public async Task Throws_When_Invalid()
        {
            var behavior = Behavior(new FakeValidator(
                new ValidationFailure("Email", "Required"),
                new ValidationFailure("Pass", "Short")
            ));

            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => behavior.Handle(new FakeRequest(), SuccessNext, CancellationToken.None));

            Assert.Equal(2, ex.Errors.Count()); // dùng LINQ Count()
            Assert.Contains(ex.Errors, e => e.PropertyName == "Email");
        }

        /// <summary>
        /// Ensures validation passes and the next delegate is called when request is valid.
        /// </summary>
        [Fact]
        public async Task Passes_And_Calls_Next_When_Valid()
        {
            var validator = new TrackableValidator();
            var behavior = Behavior(validator);
            var called = false;

            var result = await behavior.Handle(
                new FakeRequest(),
                SuccessNextWithFlag(() => called = true),
                CancellationToken.None);

            Assert.Equal("Success", result);
            Assert.True(called);
            Assert.Equal(1, validator.CallCount);
        }

        /// <summary>
        /// Ensures validation is skipped if no validators are provided.
        /// </summary>
        [Fact]
        public async Task Skips_Validation_If_No_Validators()
        {
            var behavior = Behavior();
            var called = false;

            var result = await behavior.Handle(
                new FakeRequest(),
                SuccessNextWithFlag(() => called = true),
                CancellationToken.None);

            Assert.True(called);
            Assert.Equal("Success", result);
        }

        /// <summary>
        /// Ensures all validators are executed.
        /// </summary>
        [Fact]
        public async Task Calls_All_Validators()
        {
            var v1 = new TrackableValidator();
            var v2 = new TrackableValidator();
            var behavior = Behavior(v1, v2);

            await behavior.Handle(new FakeRequest(), SuccessNext, CancellationToken.None);

            Assert.Equal(1, v1.CallCount);
            Assert.Equal(1, v2.CallCount);
        }

        /// <summary>
        /// Ensures validation errors from multiple validators are aggregated.
        /// </summary>
        [Fact]
        public async Task Aggregates_Errors()
        {
            var behavior = Behavior(
                new FakeValidator(new ValidationFailure("A", "1")),
                new FakeValidator(new ValidationFailure("B", "2"))
            );

            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => behavior.Handle(new FakeRequest(), SuccessNext, CancellationToken.None));

            Assert.Equal(new[] { "A", "B" }, ex.Errors.Select(e => e.PropertyName).OrderBy(x => x));
        }

        /// <summary>
        /// Ensures the <see cref="CancellationToken"/> is forwarded to the validator.
        /// </summary>
        [Fact]
        public async Task Forwards_CancellationToken()
        {
            var cts = new CancellationTokenSource();
            var validator = new TrackableValidator();
            var behavior = Behavior(validator);

            await behavior.Handle(new FakeRequest(), SuccessNext, cts.Token);
            Assert.Equal(cts.Token, validator.LastToken);
        }

        /// <summary>
        /// Ensures the next delegate is not called when validation fails.
        /// </summary>
        [Fact]
        public async Task Does_Not_Call_Next_On_Failure()
        {
            var behavior = Behavior(new FakeValidator(new ValidationFailure("X", "Bad")));
            var called = false;

            await Assert.ThrowsAsync<ValidationException>(
                () => behavior.Handle(new FakeRequest(), SuccessNextWithFlag(() => called = true), CancellationToken.None));

            Assert.False(called);
        }

        /// <summary>
        /// Ensures that validation errors contain the correct error messages.
        /// </summary>
        [Fact]
        public async Task Includes_Correct_Messages()
        {
            var behavior = Behavior(new FakeValidator(
                new ValidationFailure("Email", "Email is required."),
                new ValidationFailure("Password", "Password must be at least 8 characters.")
            ));

            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => behavior.Handle(new FakeRequest(), SuccessNext, CancellationToken.None));

            Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("required"));
            Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("8 characters"));
        }
    }
}
