using System.Threading;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;

namespace DamianTourBackend.Tests.UnitTests
{
    public class EnumAsStringAssertionRule : IAssertionRule
    {
        public bool AssertEquality(IEquivalencyValidationContext context)
        {
            var subject = context.Subject;
            var expectation = context.Expectation;

            if (!subject.GetType().IsEnum || !(expectation is string) || subject == null || expectation == null)
            {
                return false; //rule not applicable
            }

            Execute.Assertion
                .BecauseOf(context.Because, context.BecauseArgs)
                .ForCondition(subject.ToString().Equals(expectation.ToString()))
                .FailWith("Expected {context:string} to be {0}{reason}, but found {1}",
                    context.Subject.ToString(), context.Expectation.ToString());

            return true;
        }
    }
}