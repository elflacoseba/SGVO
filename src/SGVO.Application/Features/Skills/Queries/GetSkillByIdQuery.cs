using SGVO.Application.Common;

namespace SGVO.Application.Features.Skills.Queries;

/// <summary>
/// Query to retrieve a single skill by ID.
/// </summary>
public sealed record GetSkillByIdQuery(long Id) : IQuery<SkillDetailDto?>;
