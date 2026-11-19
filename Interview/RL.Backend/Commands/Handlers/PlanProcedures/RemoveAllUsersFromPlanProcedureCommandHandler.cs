using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedures;

public class RemoveAllUsersFromPlanProcedureCommandHandler : IRequestHandler<RemoveAllUsersFromPlanProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public RemoveAllUsersFromPlanProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(RemoveAllUsersFromPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

            var planProcedure = await _context.PlanProcedures
                .Include(pp => pp.PlanProcedureUsers)
                .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);

            if (planProcedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found in PlanId: {request.PlanId}"));

            //Only the assignments are removed, the procedure stays in the plan
            _context.PlanProcedureUsers.RemoveRange(planProcedure.PlanProcedureUsers);

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}
