using Devices.Application.DTOs;
using Devices.Domain.Common;
using Devices.Domain.Enums;
using Devices.Domain.Extensions;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Devices.UnitTests.Extensions
{
    public static class DeviceAssertionsExtensions
    {
        public static void MatchDevice(this DeviceDto deviceDto, int expectedId, string expectedName,
            string expectedBrand, DeviceState expectedState)
        {
            deviceDto.Id.Should().Be(expectedId);
            deviceDto.Name.Should().Be(expectedName);
            deviceDto.Brand.Should().Be(expectedBrand);
            deviceDto.State.Should().Be(expectedState.GetDescription());
        }

        public static AndConstraint<BooleanAssertions> BeSuccessful<T>(this Result<T> result)
        {
            return result.IsSuccess.Should().BeTrue("Expected result to be successful");
        }

        public static AndConstraint<BooleanAssertions> BeSuccessful(this Result result)
        {
            return result.IsSuccess.Should().BeTrue("Expected result to be successful");
        }

        public static AndConstraint<BooleanAssertions> BeFailure<T>(this Result<T> result)
        {
            return result.IsFailure.Should().BeTrue("Expected result to be a failure");
        }

        public static AndConstraint<BooleanAssertions> BeFailure(this Result result)
        {
            return result.IsFailure.Should().BeTrue("Expected result to be a failure");
        }

        public static AndConstraint<StringAssertions> WithErrorContaining<T>(
            this Result<T> result, string expectedError)
        {
            return result.Error.Should().Contain(expectedError);
        }

        public static AndConstraint<StringAssertions> WithErrorContaining(
            this Result result, string expectedError)
        {
            return result.Error.Should().Contain(expectedError);
        }
    }
}