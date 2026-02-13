using Microsoft.AspNetCore.Authorization;

namespace ASP_NET_17._TaskFlow_Ownership.Authorization;

public class ProjectMemberOrHigherRequirment
     : IAuthorizationRequirement
{
}
