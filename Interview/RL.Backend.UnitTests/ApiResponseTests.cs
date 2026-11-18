using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RL.Backend.Exceptions;
using RL.Backend.Models;

namespace RL.Backend.UnitTests;

[TestClass]
public class ApiResponseTests
{
    [TestMethod]
    public void ApiResponseTests_NotFoundException_ReturnsNotFoundResult()
    {
        //Given
        var response = ApiResponse<Unit>.Fail(new NotFoundException("PlanId: 1 not found"));

        //When
        var result = response.ToActionResult();

        //Then
        result.Should().BeOfType(typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public void ApiResponseTests_BadRequestException_ReturnsBadRequestResult()
    {
        //Given
        var response = ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));

        //When
        var result = response.ToActionResult();

        //Then
        result.Should().BeOfType(typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void ApiResponseTests_OtherException_ReturnsBadRequestResult()
    {
        //Given
        var response = ApiResponse<Unit>.Fail(new Exception("Something went wrong"));

        //When
        var result = response.ToActionResult();

        //Then
        result.Should().BeOfType(typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void ApiResponseTests_Succeeded_ReturnsOkResult()
    {
        //Given
        var response = ApiResponse<Unit>.Succeed(new Unit());

        //When
        var result = response.ToActionResult();

        //Then
        result.Should().BeOfType(typeof(OkResult));
    }
}
