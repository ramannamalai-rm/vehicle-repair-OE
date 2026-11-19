using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.PlanProcedures;
using RL.Backend.Exceptions;
using RL.Data;

namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveAllUsersFromPlanProcedureTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveAllUsersFromPlanProcedureTests_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemoveAllUsersFromPlanProcedureCommandHandler(context.Object);
        var request = new RemoveAllUsersFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = 1
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveAllUsersFromPlanProcedureTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemoveAllUsersFromPlanProcedureCommandHandler(context.Object);
        var request = new RemoveAllUsersFromPlanProcedureCommand()
        {
            PlanId = 1,
            ProcedureId = procedureId
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task RemoveAllUsersFromPlanProcedureTests_ProcedureNotInPlan_ReturnsNotFound(int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllUsersFromPlanProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromPlanProcedureCommand()
        {
            PlanId = 1,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = 1
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId + 1,
            ProcedureTitle = "Other Test Procedure"
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = 1,
            ProcedureId = procedureId + 1
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task RemoveAllUsersFromPlanProcedureTests_NoUsersAssigned_ReturnsSuccess(int planId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllUsersFromPlanProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 4)]
    [DataRow(35, 69, 27)]
    public async Task RemoveAllUsersFromPlanProcedureTests_UsersAssigned_RemovesAllUsers(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllUsersFromPlanProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId
        });
        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId,
            Name = "Test User"
        });
        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId + 1,
            Name = "Other Test User"
        });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId + 1
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbPlanProcedureUsers = await context.PlanProcedureUsers.ToListAsync();
        var dbPlanProcedure = await context.PlanProcedures
            .FirstOrDefaultAsync(pp => pp.PlanId == planId && pp.ProcedureId == procedureId);

        dbPlanProcedureUsers.Should().BeEmpty();
        dbPlanProcedure.Should().NotBeNull();

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 4)]
    [DataRow(35, 69, 27)]
    public async Task RemoveAllUsersFromPlanProcedureTests_OtherProcedureInPlan_KeepsItsUsers(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllUsersFromPlanProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId + 1,
            ProcedureTitle = "Other Test Procedure"
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId + 1
        });
        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId,
            Name = "Test User"
        });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId + 1,
            UserId = userId
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbPlanProcedureUsers = await context.PlanProcedureUsers.ToListAsync();

        dbPlanProcedureUsers.Should().ContainSingle();
        dbPlanProcedureUsers.Single().ProcedureId.Should().Be(procedureId + 1);

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 4)]
    [DataRow(35, 69, 27)]
    public async Task RemoveAllUsersFromPlanProcedureTests_SameProcedureInAnotherPlan_KeepsItsUsers(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllUsersFromPlanProcedureCommandHandler(context);
        var request = new RemoveAllUsersFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId
        });
        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId + 1
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = planId + 1,
            ProcedureId = procedureId
        });
        context.Users.Add(new Data.DataModels.User
        {
            UserId = userId,
            Name = "Test User"
        });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser
        {
            PlanId = planId + 1,
            ProcedureId = procedureId,
            UserId = userId
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbPlanProcedureUsers = await context.PlanProcedureUsers.ToListAsync();

        dbPlanProcedureUsers.Should().ContainSingle();
        dbPlanProcedureUsers.Single().PlanId.Should().Be(planId + 1);

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }
}
