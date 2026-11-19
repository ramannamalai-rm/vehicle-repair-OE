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
public class RemoveUserFromPlanProcedureTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveUserFromPlanProcedureTests_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemoveUserFromPlanProcedureCommandHandler(context.Object);
        var request = new RemoveUserFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = 1
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
    public async Task RemoveUserFromPlanProcedureTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemoveUserFromPlanProcedureCommandHandler(context.Object);
        var request = new RemoveUserFromPlanProcedureCommand()
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = 1
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
    public async Task RemoveUserFromPlanProcedureTests_InvalidUserId_ReturnsBadRequest(int userId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemoveUserFromPlanProcedureCommandHandler(context.Object);
        var request = new RemoveUserFromPlanProcedureCommand()
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = userId
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
    public async Task RemoveUserFromPlanProcedureTests_ProcedureNotInPlan_ReturnsNotFound(int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromPlanProcedureCommandHandler(context);
        var request = new RemoveUserFromPlanProcedureCommand()
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = 1
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
        context.Users.Add(new Data.DataModels.User
        {
            UserId = 1,
            Name = "Test User"
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 4)]
    [DataRow(35, 69, 27)]
    public async Task RemoveUserFromPlanProcedureTests_NotAssigned_ReturnsSuccess(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromPlanProcedureCommandHandler(context);
        var request = new RemoveUserFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
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
    public async Task RemoveUserFromPlanProcedureTests_Assigned_ReturnsSuccess(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromPlanProcedureCommandHandler(context);
        var request = new RemoveUserFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
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
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbPlanProcedureUser = await context.PlanProcedureUsers
            .FirstOrDefaultAsync(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId && ppu.UserId == userId);

        dbPlanProcedureUser.Should().BeNull();

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 4)]
    [DataRow(35, 69, 27)]
    public async Task RemoveUserFromPlanProcedureTests_Assigned_KeepsOtherUsers(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromPlanProcedureCommandHandler(context);
        var request = new RemoveUserFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
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
        var dbUserIds = await context.PlanProcedureUsers
            .Where(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId)
            .Select(ppu => ppu.UserId)
            .ToListAsync();

        dbUserIds.Should().BeEquivalentTo(new[] { userId + 1 });

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 4)]
    [DataRow(35, 69, 27)]
    public async Task RemoveUserFromPlanProcedureTests_SameProcedureInAnotherPlan_OnlyRemovesFromRequestedPlan(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveUserFromPlanProcedureCommandHandler(context);
        var request = new RemoveUserFromPlanProcedureCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
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
